using Assets.Dtos;
using UnityEngine;

public class Food : MonoBehaviour
{
    private Block _blockPos;

    public void Start()
    {
        _blockPos = EnvironmentManager.Instance.BlockFromWorldPoint(transform.position);
        transform.SetPositionAndRotation(_blockPos.WorldPosition + (EnvironmentManager.Instance.NodeRadius / 2f) * Vector3.up, Quaternion.identity);
    }
}
