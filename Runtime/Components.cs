using Unity.Entities;

public struct UIDocumentRoot : IComponentData { }

public struct UIDocumentGroup : ISharedComponentData
{
    public int Value;
}

public struct UIElement : IComponentData { }

public struct UIFragments : IBufferElementData
{
    public Entity Value;
}

/// <summary>
/// Marks UI component as rendered
/// </summary>
public struct UIRendered : IComponentData { }