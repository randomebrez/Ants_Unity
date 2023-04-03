using Assets._Scripts.Utilities;
using Assets.Dtos;
using UnityEngine;

public class FoodToken : MonoBehaviour
{
    private Block _blockPos;

    public void Start()
    {
        _blockPos = EnvironmentManager.Instance.BlockFromWorldPoint(transform.position);
        transform.position = _blockPos.WorldPosition + 2 * GlobalParameters.NodeRadius * Vector3.up;
    }
}