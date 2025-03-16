using System;
using Unity.Entities;

public struct UIDepth : ISharedComponentData, IEquatable<UIDepth>
{
    public int Value;
    public bool Equals(UIDepth other) => Value == other.Value;
    public override int GetHashCode() => Value;
}

/// <summary>
/// Similar to Unity.Transforms.Child but maintains order and is exclusive to UI elements
/// </summary>
public struct UIChildren : IBufferElementData
{
    public Entity Value;
}