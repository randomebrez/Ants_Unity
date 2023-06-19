using Assets._Scripts.Utilities;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private Dictionary<string, Transform> _bricks = new Dictionary<string, Transform>();

    public GroundBlock BrickPrefab;

    public void BuildWall(float wallWidth, float height, int thickness, float angleX = 0f, float angleY = 0f, float angleZ = 0f)
    {
        // Closest brick width to asked one that ensure to have a finite number on each line
        var hBrickNumber = Mathf.RoundToInt(wallWidth / GlobalParameters.NodeRadius);
        var vBrickNumber = Mathf.RoundToInt(height / GlobalParameters.NodeRadius);
        var brickWidth = wallWidth / hBrickNumber;

        var rotation = Quaternion.Euler(new Vector3(angleX, angleY, angleZ));
        var bottomLeft = transform.position - new Vector3((hBrickNumber * brickWidth - 1) / 2, 0, 0);

        for (int k = 0; k < thickness; k++)
        {
            for (int i = 0; i < hBrickNumber; i++)
            {
                for (int j = 0; j < vBrickNumber; j++)
                {
                    var brick = Instantiate(BrickPrefab, rotation * (bottomLeft + i * Vector3.right + j * Vector3.up), Quaternion.identity, transform);
                    brick.SetUnwalkable();
                    _bricks.Add($"({i},{j},{k})", brick.transform);
                }
            }
            bottomLeft += Vector3.forward;
        }

        StaticHelper.SetRecursiveLayer(new List<GameObject> { transform.gameObject }, gameObject.layer);
    }

    public void BuildHexaWalls(float wallWidth, float height, int thickness, float yrotation = 0f)
    {
        var wallRotation = Quaternion.Euler(yrotation * Vector3.up);
        var xShift = 0.5f * Mathf.Sqrt(3) * GlobalParameters.NodeRadius;
        var zShift = 1.5f * GlobalParameters.NodeRadius;

        var horizontalSize = Mathf.RoundToInt(wallWidth / xShift);
        var verticalSize = Mathf.RoundToInt(height);
        var brickWidth = wallWidth / horizontalSize;

        var bottomLeft = transform.position - new Vector3(horizontalSize * brickWidth / 2, 0, 0);

        for (int k = 0; k < thickness; k++)
        {
            var switchVar = 1;
            if (k%2 == 1)
                    switchVar = -1;

            for (int i = 0; i < horizontalSize; i++)
            {
                for (int j = 0; j < verticalSize; j++)
                {
                    if ((i+j)%2 == 0)
                    {
                        var spawnPosition = bottomLeft + i * xShift * Vector3.right + j * GlobalParameters.NodeRadius * Vector3.up;
                        var brick = Instantiate(BrickPrefab, spawnPosition, Quaternion.identity, transform);
                        brick.transform.localScale *= GlobalParameters.NodeRadius;
                        brick.SetUnwalkable();
                        _bricks.Add($"({i},{j},{k})", brick.transform);
                    }
                }
            }
            bottomLeft += zShift * Vector3.forward + switchVar * xShift * Vector3.right;
        }

        transform.rotation = wallRotation;

        StaticHelper.SetRecursiveLayer(new List<GameObject> { transform.gameObject }, gameObject.layer);
    }
}
