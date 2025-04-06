using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial class UISystemGroup : ComponentSystemGroup { }

[UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
[CreateAfter(typeof(ScreenInfoSystem))]
public partial class UIInitializationSystemGroup : ComponentSystemGroup { }