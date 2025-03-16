using Unity.Collections;
using Unity.Entities;

public partial struct ElementIndexingSystem : ISystem
{
    NativeHashMap<FixedString32Bytes, int> idIndex;
    NativeHashMap<FixedString32Bytes, int> classIndex;

    void OnCreate(ref SystemState state)
    {
        idIndex = new NativeHashMap<FixedString32Bytes, int>(1024, Allocator.Persistent);
        classIndex = new NativeHashMap<FixedString32Bytes, int>(1024, Allocator.Persistent);
    }

    void OnDestroy(ref SystemState state)
    {
        idIndex.Dispose();
        classIndex.Dispose();
    }

    public bool TryGetIDIndex(FixedString32Bytes id, out int index)
    {
        if (idIndex.TryGetValue(id, out index))
            return true;
        return false;
    }

    public bool TryGetClassIndex(FixedString32Bytes c, out int index)
    {
        if (classIndex.TryGetValue(c, out index))
            return true;
        return false;
    }
}