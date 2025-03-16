using Unity.Entities;
using Unity.Mathematics;

public enum UIPosition
{
    Relative,
    Absolute,
}

public enum UILayout
{
    FlexColumn,
    FlexRow,
    Grid
}

public enum UIDisplay
{
    Block,
    Inline,
}

public enum UIAlignment
{
    Start,
    Center,
    End,
    Stretch,
}

public struct UIStyleAlign : IComponentData
{
    public UIAlignment Value;
    public UIAlignment CrossValue;
}

public struct UIStyleDisplay : IComponentData
{
    public UIDisplay Value;
}

public struct UIStylePosition : IComponentData
{
    public UIPosition Position;
    public UIUnitValue4 Value;
}

public struct UIStyleLayout : IComponentData
{
    public UILayout Value;
}

public struct UIStyleGap : IComponentData
{
    public UIUnitValue2 Value;
}

/// <summary>
/// Sets the size of a UI element, zero means auto size
/// </summary>
public struct UIStyleSize : IComponentData
{
    public UIUnitValue2 Value;
}

public struct UIStyleBackgroundColor : IComponentData
{
    public float4 Value;
}

public struct UIStyleMargin : IComponentData
{
    public UIUnitValue4 Value;
}

public struct UIStylePadding : IComponentData
{
    public UIUnitValue4 Value;
}

/// <summary>
/// Width / Height ratio
/// </summary>
public struct UIStyleAspectRatio : IComponentData
{
    public float Value;
}