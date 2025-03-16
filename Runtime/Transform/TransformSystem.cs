using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct TransformSystem : ISystem
{
    void OnUpdate(ref SystemState state)
    {
        var query = SystemAPI.QueryBuilder()
            .WithAllRW<LocalTransform, UILayoutPosition>()
            .Build();

        if (!query.IsEmpty)
        {
            var job = new LayoutToTransformJob
            {
                PixelToWorld = SystemAPI.GetSingleton<UIPixelToWorld>().Value
            };
            state.Dependency = job.ScheduleParallel(query, state.Dependency);
        }
    }

    partial struct LayoutToTransformJob : IJobEntity
    {
        public float PixelToWorld;

        public void Execute(ref LocalTransform localTransform, in UILayoutPosition layoutPosition)
        {
            localTransform.Position = new float3(layoutPosition.Value * PixelToWorld * new float2(1f, -1f), 0f);
        }
    }
}