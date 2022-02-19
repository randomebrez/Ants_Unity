using Assets._Scripts.Utilities;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public float WallWidth = 10f;
    public float WallHeight = 4f;

    public float BrickWidth = 1f;
    public float BrickHeight = 0.5f;

    public GroundBlock BrickPrefab;

    private int _HBrickNumber => Mathf.RoundToInt(WallWidth/BrickWidth);
    private int _VBrickNumber => Mathf.RoundToInt(WallHeight/BrickHeight);
    private Dictionary<string, Transform> _bricks = new Dictionary<string, Transform>();

    public void BuildWall(float angleX = 0f, float angleY = 0f, float angleZ = 0f)
    {
        // Closest brick width to asked one that ensure to have a finite number on each line
        BrickWidth = WallWidth / _HBrickNumber;
        BrickHeight = WallHeight / _VBrickNumber;

        var rotation = Quaternion.Euler(new Vector3(angleX, angleY, angleZ));
        var bottomLeft = transform.position - new Vector3((_HBrickNumber * BrickWidth - 1) / 2, 0, 0);

        for (int i = 0; i < _HBrickNumber ; i++)
        {
            for (int j = 0; j < _VBrickNumber; j++)
            {
                var brick = Instantiate(BrickPrefab, rotation * (bottomLeft + i * Vector3.right + j * Vector3.up), Quaternion.identity, transform);
                brick.SetUnwalkable();
                _bricks.Add($"({i},{j})", brick.transform);
            }
        }

        StaticHelper.SetRecursiveLayer(new List<GameObject> { transform.gameObject }, gameObject.layer);
    }
}
