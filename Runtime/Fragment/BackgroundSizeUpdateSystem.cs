using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(UISystemGroup))]
[UpdateAfter(typeof(LayoutUpdateSystem))]
public partial struct BackgroundSizeUpdateSystem : ISystem
{
    void OnUpdate(ref SystemState state)
    {
        var query = SystemAPI.QueryBuilder()
            .WithAll<UILayoutSize, UIBackgroundEntity>()
            .Build();

        var job = new BackgroundSizeUpdateJob
        {
            PostTransformMatrixLookup = SystemAPI.GetComponentLookup<PostTransformMatrix>(),
            PixelToWorld = SystemAPI.GetSingleton<UIPixelToWorld>().Value
        };

        state.Dependency = job.ScheduleParallel(query, state.Dependency);
    }

    partial struct BackgroundSizeUpdateJob : IJobEntity
    {
        [NativeDisableParallelForRestriction] public ComponentLookup<PostTransformMatrix> PostTransformMatrixLookup;
        public float PixelToWorld;

        public void Execute(in UIBackgroundEntity backgroundEntity, in UILayoutSize layoutSize)
        {
            var postTransform = PostTransformMatrixLookup.GetRefRW(backgroundEntity.Value);

            // Position the quad to cover the entire element
            var position = new float2(layoutSize.Final.x / 2f * PixelToWorld, -layoutSize.Final.y / 2f * PixelToWorld);
            var scale = layoutSize.Final * PixelToWorld;
            postTransform.ValueRW.Value = float4x4.TRS(new float3(position, 0f), quaternion.identity, new float3(scale, 1f));
        }
    }
}