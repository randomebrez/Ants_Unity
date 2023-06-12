using UnityEngine;

internal interface IManager
{
    GameObject InstantiateObject(GameObject gameObject, Vector3 position, Quaternion rotation, Transform parent, float checkSphereRadius);
}