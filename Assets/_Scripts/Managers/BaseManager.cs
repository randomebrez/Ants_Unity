using UnityEngine;

internal abstract class BaseManager<T> : Singleton<T>, IManager where T : BaseManager<T>
{
    public LayerMask ObstructionLayer;

    public GameObject CanvasContainer;

    public GameObject InstantiateObject(GameObject gameObject, Vector3 position, Quaternion rotation, Transform parent, float checkSphereRadius)
    {
        var obstacles = Physics.OverlapSphere(position, checkSphereRadius, ObstructionLayer);
        return obstacles.Length <= 0 ? Instantiate(gameObject, position, rotation, parent) : null;
    }
}
