using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Elfenlabs.Mesh;
using UnityEngine;
using UnityEngine.Rendering;

public struct UIQuadPrototype : IComponentData
{
    public Entity Value;
}

public struct UICameraOrthogonalSize : IComponentData
{
    public float Value;
}

public struct UICameraTransform : IComponentData
{
    public float3 Position;
    public quaternion Rotation;
}

[UpdateInGroup(typeof(UIInitializationSystemGroup))]
[CreateAfter(typeof(ScreenInfoSystem))]
public partial struct UIInitializationSystem : ISystem
{
    void OnCreate(ref SystemState state)
    {
        InitializeQuadPrototype(ref state);
        state.EntityManager.CreateSingleton(new UICameraOrthogonalSize { Value = 5f });
        state.EntityManager.CreateSingleton(new UICameraTransform { Position = new float3(0f, 10f, -10f), Rotation = quaternion.EulerXYZ(0f, 0f, 0f) });
    }

    void OnDestroy(ref SystemState state)
    {
        state.EntityManager.DestroyEntity(SystemAPI.GetSingletonEntity<UIQuadPrototype>());
    }

    /// <summary>
    /// Initializes the prototype entity for UI elements
    /// </summary>
    /// <param name="state"></param>
    void InitializeQuadPrototype(ref SystemState state)
    {
        // Create the prototype entity for UI elements
        var mesh = MeshUtility.CreateQuad(new float2(1f, 1f));
        var shader = Shader.Find("Elfenlabs/Sprite");
        var material = new Material(shader);
        var desc = new RenderMeshDescription(
            shadowCastingMode: ShadowCastingMode.Off,
            receiveShadows: false,
            motionVectorGenerationMode: MotionVectorGenerationMode.ForceNoMotion,
            layer: LayerMask.NameToLayer("UI"),
            renderingLayerMask: 4294967295,
            lightProbeUsage: LightProbeUsage.Off,
            staticShadowCaster: false
        );

        // Create an array of mesh and material required for runtime rendering.
        var renderMeshArray = new RenderMeshArray(new Material[] { material }, new Mesh[] { mesh });

        // Create empty base entity
        var quadPrototype = state.EntityManager.CreateEntity();
        state.EntityManager.SetName(quadPrototype, "UIQuad");
        state.EntityManager.AddComponent<Prefab>(quadPrototype);
        state.EntityManager.AddComponent<LocalToWorld>(quadPrototype);
        state.EntityManager.AddComponent<Parent>(quadPrototype);
        state.EntityManager.AddComponentData(quadPrototype, new LocalTransform { Scale = 1f });
        state.EntityManager.AddComponentData(quadPrototype, new PostTransformMatrix { Value = float4x4.identity });

        // Populate the prototype entity with the required components
        RenderMeshUtility.AddComponents(
            quadPrototype,
            state.EntityManager,
            desc,
            renderMeshArray,
            MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));

        state.EntityManager.AddComponentData(quadPrototype, new SpriteBackgroundColor
        {
            Value = new float4(1f, 1f, 0f, 1f),
        });

        state.EntityManager.CreateSingleton(new UIQuadPrototype { Value = quadPrototype });
    }
}
