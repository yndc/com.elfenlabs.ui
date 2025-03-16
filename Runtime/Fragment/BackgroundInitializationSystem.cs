using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct UIBackgroundEntity : IComponentData
{
    public Entity Value;
}

[UpdateInGroup(typeof(UIInitializationSystemGroup))]
[CreateAfter(typeof(UIInitializationSystem))]
public partial struct BackgroundInitializationSystem : ISystem
{
    void OnUpdate(ref SystemState state)
    {
        var query = SystemAPI.QueryBuilder()
            .WithAll<UIStyleBackgroundColor, UILayoutSize>()
            .WithNone<UIBackgroundEntity>()
            .Build();

        if (query.IsEmpty)
            return;

        var ecb = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        var job = new BackgroundInitializationJob
        {
            ECB = ecb.AsParallelWriter(),
            QuadPrototype = SystemAPI.GetSingleton<UIQuadPrototype>().Value,
            PixelToWorld = SystemAPI.GetSingleton<UIPixelToWorld>().Value
        };

        state.Dependency = job.ScheduleParallel(query, state.Dependency);
    }

    partial struct BackgroundInitializationJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;
        public Entity QuadPrototype;
        public float PixelToWorld;

        public void Execute(Entity entity, [ChunkIndexInQuery] int chunkIndexInQuery, in UIStyleBackgroundColor backgroundColor, in UILayoutSize layoutSize)
        {
            ECB.AddBuffer<UIFragments>(chunkIndexInQuery, entity);

            var quad = ECB.Instantiate(chunkIndexInQuery, QuadPrototype);
            ECB.AppendToBuffer(chunkIndexInQuery, entity, new UIFragments { Value = quad });
            ECB.AddComponent(chunkIndexInQuery, entity, new UIBackgroundEntity { Value = quad });
            ECB.SetComponent(chunkIndexInQuery, quad, new Parent { Value = entity });
            ECB.SetComponent(chunkIndexInQuery, quad, new SpriteBackgroundColor { Value = backgroundColor.Value });
        }
    }
}