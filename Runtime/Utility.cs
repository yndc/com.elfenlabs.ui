using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public static class UIUtility
{
    public static Entity GetElement(ref SystemState state, FixedString32Bytes runes)
    {
        throw new System.NotImplementedException();
    }

    public static float2 PixelToWorld(float2 pixels, float2 screenSize, float orthoSize)
    {
        float screenHeight = screenSize.y;
        float screenWidth = screenSize.x;
        float wupVertical = 2 * orthoSize / screenHeight;
        float aspectRatio = screenWidth / screenHeight;
        float wupHorizontal = wupVertical / aspectRatio;
        return new float2(pixels.x * wupHorizontal, pixels.y * wupVertical);
    }

    public static float2 WorldToPixel(float2 units, float2 screenSize, float orthoSize)
    {
        float screenHeight = screenSize.y;
        float screenWidth = screenSize.x;
        float ppuVertical = screenHeight / (2 * orthoSize);
        float aspectRatio = (float)screenWidth / screenHeight;
        float ppuHorizontal = ppuVertical * aspectRatio;
        return new float2(units.x * ppuHorizontal, units.y * ppuVertical);
    }

    public static Entity CreateElement(World world, Entity parent = default)
    {
        var manager = world.EntityManager;
        var entity = manager.CreateEntity();
        manager.SetName(entity, "UIElement");

        // Transform components
        manager.AddComponentData(entity, new LocalToWorld { Value = float4x4.identity });
        manager.AddComponentData(entity, new LocalTransform { Scale = 1f });
        manager.AddComponentData(entity, new PostTransformMatrix { Value = float4x4.identity });
        manager.AddBuffer<UIChildren>(entity);
        if (parent != default)
        {
            manager.AddComponentData(entity, new Parent { Value = parent });
            manager.GetBuffer<UIChildren>(parent).Add(new UIChildren { Value = entity });
        }

        // Layout components
        manager.AddComponentData(entity, new UIStyleLayout { Value = UILayout.FlexColumn });
        manager.AddComponentData(entity, new UIStyleDisplay { Value = UIDisplay.Block });
        manager.AddComponentData(entity, new UIStyleGap { Value = new UIUnitValue2(0f, 0f) });
        manager.AddComponentData(entity, new UILayoutPosition { });
        manager.AddComponentData(entity, new UILayoutSize { });
        manager.AddComponentData(entity, new UIStyleSize { Value = new UIUnitValue2(UIUnitValue.Auto, UIUnitValue.Auto) });

        var depth = parent == default ? 0 : manager.GetSharedComponent<UIDepth>(parent).Value + 1;
        manager.AddSharedComponent(entity, new UIDepth { Value = depth });
        manager.AddComponent<UILayoutDirty>(entity);
        manager.AddComponent<UIElement>(entity);
        manager.AddBuffer<UIFragments>(entity);
        return entity;
    }

    /// <summary>
    /// Attaches an entity to a parent entity and recursively updates the depth of the entity
    /// </summary>
    /// <param name="world"></param>
    /// <param name="entity"></param>
    /// <param name="parent"></param>
    public static void Attach(World world, Entity entity, Entity parent, int order = -1)
    {
        var manager = world.EntityManager;

        if (manager.GetComponentData<Parent>(entity).Value == parent)
            return;

        manager.SetComponentData(entity, new Parent { Value = parent });

        // Update child order
        var children = manager.GetBuffer<UIChildren>(parent);
        if (order == -1 || order >= children.Length)
            children.Add(new UIChildren { Value = entity });
        else
            children.Insert(order, new UIChildren { Value = entity });

        // Update depth
        var currentDepth = manager.GetSharedComponent<UIDepth>(entity).Value;
        var parentDepth = manager.GetSharedComponent<UIDepth>(parent).Value;

        void UpdateDepth(Entity entity, int parentDepth)
        {
            manager.SetSharedComponent(entity, new UIDepth { Value = parentDepth + 1 });
            var children = manager.GetBuffer<UIChildren>(entity);
            for (int i = 0; i < children.Length; i++)
            {
                UpdateDepth(children[i].Value, parentDepth + 1);
            }
        }

        if (currentDepth != parentDepth + 1)
        {
            UpdateDepth(entity, parentDepth);
        }
    }

    public static void CreateRootElement(World world, float2 size)
    {
        var entity = world.EntityManager.CreateEntity();
        world.EntityManager.SetName(entity, "UIRootContainer");
        world.EntityManager.AddComponentData(entity, new LocalTransform
        {
            Position = new float3(0f, 10f, 10f),
            Rotation = quaternion.EulerXYZ(0f, 0f, 0f),
            Scale = 1f
        });
        world.EntityManager.AddComponent<UIDocumentRoot>(entity);
        world.EntityManager.AddSharedComponent(entity, new UIDepth { Value = 0 });
        world.EntityManager.AddComponentData(entity, new LocalToWorld { Value = float4x4.identity });
        world.EntityManager.AddComponentData(entity, new PostTransformMatrix { Value = float4x4.identity });
        world.EntityManager.AddComponentData(entity, new LocalTransform { Position = new float3(0f, 10f, 0f), Scale = 1f }); // TODO: Standarize UI position
        world.EntityManager.AddComponentData(entity, new UILayoutSize { Final = new(size.x, size.y) });
    }
}