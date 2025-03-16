using Unity.Entities;

public struct UIPixelToWorld : IComponentData
{
    public float Value;
}

public struct UIWorldToPixel : IComponentData
{
    public float Value;
}

[UpdateInGroup(typeof(UIInitializationSystemGroup))]
[CreateAfter(typeof(UIInitializationSystem))]
public partial struct ScalingSystem : ISystem
{
    void OnCreate(ref SystemState state)
    {
        var screenSize = SystemAPI.GetSingleton<ScreenInfo>().Size;
        var orthoSize = SystemAPI.GetSingleton<UICameraOrthogonalSize>().Value;
        var scale = 1f; // TODO: get this from settings later

        state.EntityManager.CreateSingleton(new UIPixelToWorld { Value = orthoSize * 2 / screenSize.y * scale });
        state.EntityManager.CreateSingleton(new UIWorldToPixel { Value = screenSize.y / (orthoSize * 2) / scale });
    }
}