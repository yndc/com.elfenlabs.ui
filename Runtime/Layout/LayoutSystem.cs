using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Elfenlabs.Mathematics;

[UpdateInGroup(typeof(UISystemGroup))]
public partial struct LayoutUpdateSystem : ISystem
{
    void OnUpdate(ref SystemState state)
    {
        var screenSize = SystemAPI.GetSingleton<ScreenInfo>().Size;
        var pixelToWorld = SystemAPI.GetSingleton<UIPixelToWorld>().Value;
        foreach (var (_, size, transform) in SystemAPI.Query<RefRO<UIDocumentRoot>, RefRW<UILayoutSize>, RefRW<LocalTransform>>())
        {
            size.ValueRW.Final = screenSize;
            size.ValueRW.Minimum = screenSize;
            size.ValueRW.Maximum = screenSize;
            transform.ValueRW.Position = new float3(screenSize / 2f * pixelToWorld * new float2(-1f, 1f), 10f);
        }

        var firstPassQuery = SystemAPI.QueryBuilder()
            .WithAllRW<UILayoutSize, LocalTransform>()
            .WithAll<UIStyleSize>()
            .WithAll<UIStyleDisplay>()
            .WithAll<UIDepth>()
            .WithAll<Parent>()
            .Build();

        var depthsCount = GetDepthsCount(ref state);
        for (var depth = 1; depth < depthsCount; depth++)
        {
            firstPassQuery.SetSharedComponentFilter(new UIDepth { Value = depth });

            if (firstPassQuery.IsEmpty)
                continue;

            var job = new ResolvePercentagePassJob
            {
                LayoutSizeLookup = SystemAPI.GetComponentLookup<UILayoutSize>(),
            };

            state.Dependency = job.ScheduleParallel(firstPassQuery, state.Dependency);
        }

        var secondPassQuery = SystemAPI.QueryBuilder()
            .WithAllRW<UILayoutSize, LocalTransform>()
            .WithAll<UIStyleLayout>()
            .WithAll<UIStyleGap>()
            .WithAll<UIDepth>()
            .WithAll<UIChildren>()
            .Build();

        for (var depth = depthsCount - 1; depth >= 0; depth--)
        {
            secondPassQuery.SetSharedComponentFilter(new UIDepth { Value = depth });

            if (secondPassQuery.IsEmpty)
                continue;

            var job = new ResolveWrappingPassJob
            {
                ElementLookup = SystemAPI.GetComponentLookup<UIElement>(),
                LayoutSizeLookup = SystemAPI.GetComponentLookup<UILayoutSize>(),
                LayoutPositionLookup = SystemAPI.GetComponentLookup<UILayoutPosition>(),
            };

            state.Dependency = job.ScheduleParallel(secondPassQuery, state.Dependency);
        }
    }

    readonly int GetDepthsCount(ref SystemState state)
    {
        state.EntityManager.GetAllUniqueSharedComponents<UIDepth>(out var depths, Allocator.Temp);
        var maxDepth = depths.Length;
        depths.Dispose();
        return maxDepth;
    }

    /// <summary>
    /// Top-down pass to resolve percentage values
    /// </summary>
    partial struct ResolvePercentagePassJob : IJobEntity
    {
        [NativeDisableContainerSafetyRestriction] public ComponentLookup<UILayoutSize> LayoutSizeLookup;
        public void Execute(ref UILayoutSize layoutSize, in UIStyleSize styleSize, in UIStyleDisplay display, in Parent parent)
        {
            // Calculate width
            if (styleSize.Value.X.IsAuto)
            {
                if (display.Value == UIDisplay.Block)
                {
                    layoutSize.SetWidthAll(LayoutSizeLookup[parent.Value].Final.x);
                }
                else
                {
                    layoutSize.SetWidthAll(0f);
                }
            }
            else if (styleSize.Value.X.Unit == UIUnit.Percent)
            {
                layoutSize.SetWidthAll(LayoutSizeLookup[parent.Value].Final.x * styleSize.Value.X.Value / 100f);
            }
            else
            {
                layoutSize.SetWidthAll(styleSize.Value.X.Value);
            }

            // Calculate height
            if (styleSize.Value.Y.IsAuto)
            {
                layoutSize.SetHeightAll(0f);
            }
            else if (styleSize.Value.Y.Unit == UIUnit.Percent)
            {
                layoutSize.SetHeightAll(LayoutSizeLookup[parent.Value].Final.y * styleSize.Value.Y.Value / 100f);
            }
            else
            {
                layoutSize.SetHeightAll(styleSize.Value.Y.Value);
            }
        }
    }

    /// <summary>
    /// Bottom-up pass to resolve wrapping and shrink sizes
    /// </summary>
    partial struct ResolveWrappingPassJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<UIElement> ElementLookup;
        [NativeDisableContainerSafetyRestriction] public ComponentLookup<UILayoutSize> LayoutSizeLookup;
        [NativeDisableContainerSafetyRestriction] public ComponentLookup<UILayoutPosition> LayoutPositionLookup;

        public void Execute(ref UILayoutSize layoutSize, in UIStyleLayout layout, in UIStyleGap gap, in DynamicBuffer<UIChildren> children)
        {
            // TODO: calculate text

            // Calculate size from children
            var flexCalculator = new FlexCalculator(layout.Value == UILayout.FlexRow ? layoutSize.Final.x : layoutSize.Final.y, gap.Value.Resolve(layoutSize.Final));
            var perpendicularTotal = 0f;
            foreach (var childComponent in children)
            {
                var child = childComponent.Value;
                if (child == Entity.Null || !ElementLookup.HasComponent(child))
                    continue;

                var childSize = LayoutSizeLookup[child];
                var childPosition = new float2();
                switch (layout.Value)
                {
                    case UILayout.FlexColumn:
                        childPosition = flexCalculator.Add(childSize.Final.Swap()).Swap();
                        break;
                    case UILayout.FlexRow:
                        childPosition = flexCalculator.Add(childSize.Final);
                        break;
                    case UILayout.Grid:
                        throw new System.NotImplementedException();
                }
                LayoutPositionLookup[child] = new UILayoutPosition { Value = childPosition };
            }

            // Set the final size based on content
            switch (layout.Value)
            {
                case UILayout.FlexColumn:
                    layoutSize.Final.y = math.max(layoutSize.Final.y, flexCalculator.GetLongestLineLength());
                    layoutSize.Final.x = math.max(layoutSize.Final.x, perpendicularTotal);
                    break;
                case UILayout.FlexRow:
                    layoutSize.Final.x = math.max(layoutSize.Final.x, flexCalculator.GetLongestLineLength());
                    layoutSize.Final.y = math.max(layoutSize.Final.y, perpendicularTotal);
                    break;
                case UILayout.Grid:
                    throw new System.NotImplementedException();
            }
        }
    }
}