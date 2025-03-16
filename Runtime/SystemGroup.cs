using Unity.Entities;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class UISystemGroup : ComponentSystemGroup { }

[UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
[CreateAfter(typeof(ScreenInfoSystem))]
public partial class UIInitializationSystemGroup : ComponentSystemGroup { }