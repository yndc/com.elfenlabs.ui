using Unity.Mathematics;

public enum UIUnit
{
    Percent,
    Pixel,
    ViewportHeight,
    ViewportWidth,
}

public struct UIUnitValue
{
    public float Value;
    public UIUnit Unit;
    public bool IsAuto => Unit == UIUnit.Percent && Value == 0;

    public static UIUnitValue Auto => new UIUnitValue { Value = 0, Unit = UIUnit.Percent };
    public UIUnitValue(float value, UIUnit unit)
    {
        Value = value;
        Unit = unit;
    }

    public static UIUnitValue Pixels(float value) => new UIUnitValue { Value = value, Unit = UIUnit.Pixel };
    public static UIUnitValue Percent(float value) => new UIUnitValue { Value = value, Unit = UIUnit.Percent };
    public static UIUnitValue ViewportHeight(float value) => new UIUnitValue { Value = value, Unit = UIUnit.ViewportHeight };
    public static UIUnitValue ViewportWidth(float value) => new UIUnitValue { Value = value, Unit = UIUnit.ViewportWidth };

    public static implicit operator UIUnitValue(float value) => Pixels(value);
    public static implicit operator UIUnitValue(int value) => Pixels(value);
}

public struct UIUnitValue2
{
    public UIUnitValue X;
    public UIUnitValue Y;
    public UIUnitValue2(UIUnitValue x, UIUnitValue y)
    {
        X = x;
        Y = y;
    }
    public readonly float2 Resolve(float2 parentSize)
    {
        return new float2(
            X.Unit == UIUnit.Percent ? parentSize.x * X.Value / 100f : X.Value,
            Y.Unit == UIUnit.Percent ? parentSize.y * Y.Value / 100f : Y.Value
        );
    }
}

public struct UIUnitValue4
{
    public UIUnitValue X;
    public UIUnitValue Y;
    public UIUnitValue Z;
    public UIUnitValue W;
    public UIUnitValue4(UIUnitValue x, UIUnitValue y, UIUnitValue z, UIUnitValue w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }
}