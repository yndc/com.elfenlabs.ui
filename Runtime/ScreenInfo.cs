using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct ScreenInfo : IComponentData
{
    /// <summary>
    /// Screen size in pixels
    /// </summary>
    public float2 Size;

    /// <summary>
    /// Screen ratio width/height
    /// </summary>
    public float Ratio;
}

/// <summary>
/// Provide screen related information into an entity such as screen size, screen ratio, etc.
/// </summary>
[UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
public partial struct ScreenInfoSystem : ISystem
{
    void OnCreate(ref SystemState state)
    {
        state.EntityManager.CreateSingleton<ScreenInfo>();
        UpdateScreenInfo(ref state);
    }

    void OnDestroy(ref SystemState state)
    {
        state.EntityManager.DestroyEntity(SystemAPI.GetSingletonEntity<ScreenInfo>());
    }

    void OnUpdate(ref SystemState state)
    {
        // TODO: screen info rarely changes, on mobile it is almost never, and on desktop it is only when the window is resized
        // so we can optimize this by only updating when the screen size changes
        // for now we will update every frame
        UpdateScreenInfo(ref state);
    }

    void UpdateScreenInfo(ref SystemState state)
    {
        var screenInfo = SystemAPI.GetSingleton<ScreenInfo>();
        var screenSize = new float2(Screen.width, Screen.height);
        if (screenInfo.Size.Equals(screenSize))
            return;
        screenInfo.Size = screenSize;
        screenInfo.Ratio = Screen.width / (float)Screen.height;
        SystemAPI.SetSingleton(screenInfo);
    }
}
