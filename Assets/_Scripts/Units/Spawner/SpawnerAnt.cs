using mew;
using UnityEngine;

public class SpawnerAnt : Singleton<SpawnerAnt>
{
    public BaseAnt InstantiateUnit(BaseAnt prefab, Vector3 worldPosition, Quaternion rotation, Transform parent = null)
    {
        if (parent == null)
            parent = transform;

        return Instantiate(prefab, worldPosition, rotation, parent);
    }
}
