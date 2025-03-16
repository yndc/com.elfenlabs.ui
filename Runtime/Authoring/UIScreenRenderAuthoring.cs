using Elfenlabs.Companion;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class UIScreenRenderAuthoring : MonoBehaviour
{
    public Camera Camera;

    public UIDocumentAuthoring Document;

    Entity rootDocument;

    void Awake()
    {
        // Create UIRootElement for this screen UI document
        var world = World.DefaultGameObjectInjectionWorld;
        var manager = world.EntityManager;
        rootDocument = UIUtility.CreateElement(world);

        manager.SetName(rootDocument, "UIScreenRoot");
        manager.AddComponent<UIDocumentRoot>(rootDocument);

        // Set the document root size as the screen size
        manager.SetComponentData(rootDocument, new UIStyleSize
        {
            Value = new UIUnitValue2(UIUnitValue.ViewportWidth(100f), UIUnitValue.ViewportHeight(100f))
        });

        // Make the root element follow the UI camera
        manager.SetComponentData(rootDocument, new LocalTransform { Scale = 1f, Rotation = quaternion.identity });
        manager.AddComponentData(rootDocument, new EntityFollowTransform(Camera.transform));

        // TODO: TEST STUFF
        manager.SetComponentData(rootDocument, new UIStyleGap
        {
            Value = new UIUnitValue2(UIUnitValue.Pixels(20f), UIUnitValue.Pixels(10f))
        });
        manager.AddComponentData(rootDocument, new UIStyleBackgroundColor
        {
            Value = new float4(1f, 1f, 1f, 0.1f)
        });
    }

    void Start()
    {
        TestRun();
    }

    void OnDestroy()
    {
        // World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(rootDocument);
    }

    void TestRun()
    {
        AddElement(new float2(50f, 50f), new float4(0f, 1f, 0f, 1f)); 
        AddElement(new float2(50f, 50f), new float4(0f, 1f, 0f, 1f)); 
        AddElement(new float2(50f, 50f), new float4(0f, 1f, 0f, 1f));
        AddElement(new float2(200f, 100f), new float4(1f, 0f, 0f, 1f));
        for (int i = 0; i < 20; i++)
        {
            AddElement(new float2(50f, 50f), new float4(0f, 1f, 0f, 1f));
        }
    }

    void AddElement(float2 size, float4 col)
    {
        var world = World.DefaultGameObjectInjectionWorld;
        var manager = world.EntityManager;
        var element = UIUtility.CreateElement(world, rootDocument);
        manager.AddComponentData(element, new UIStylePosition
        {
            Position = UIPosition.Absolute,
            Value = new UIUnitValue4
            (
                UIUnitValue.Pixels(0f),
                UIUnitValue.Auto,
                UIUnitValue.Auto,
                UIUnitValue.Auto
            )
        });
        
        manager.AddComponentData(element, new UIStyleBackgroundColor
        {
            Value = col
        });
        manager.SetComponentData(element, new UIStyleSize
        {
            Value = new UIUnitValue2(
            UIUnitValue.Pixels(size.x),
            UIUnitValue.Pixels(size.y))
        });
    }
}