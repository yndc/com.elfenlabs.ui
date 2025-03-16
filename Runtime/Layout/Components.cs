using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// The position relative to the top-left corner of the element calculated by the layout engine
/// This position is not the final position to be rendered, transforms styling and other factors may affect the final position
/// </summary>
public struct UILayoutPosition : IComponentData
{
    public float2 Value;
}

/// <summary>
/// Size of the element
/// </summary>
public struct UILayoutSize : IComponentData
{
    public float2 Minimum;
    public float2 Maximum;
    public float2 Final;
    public void SetWidthAll(float value)
    {
        Minimum.x = value;
        Maximum.x = value;
        Final.x = value;
    }
    public void SetHeightAll(float value)
    {
        Minimum.y = value;
        Maximum.y = value;
        Final.y = value;
    }
}

public struct UILayoutDirty : IComponentData, IEnableableComponent { }