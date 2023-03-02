using System.Collections.Generic;
using System.Linq;
using Assets.Dtos;
using Unity.VisualScripting;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public GameObject BlockPrefab;
    public GameObject WallPrefab;
    public LayerMask Everything;
    public LayerMask UnwalkableMask;
    public GameObject BlockContainer;
    public Transform WallContainer;
    public float NodeRadius => EnvironmentManager.Instance.NodeRadius;
    public Dictionary<int, Block> Path = null;

    private Vector3 _gridWorldSize;
    private GroundBlock[,] _grid;
    private int _gridSizeX, _gridSizeY, _gridSizeZ;
    private float _nodeDiameter => 2 * NodeRadius;
    public int MaxSize => _gridSizeX * _gridSizeZ;

    public void SetupGrid(Vector3 gridWorldSize, float nodeRadius)
    {
        _gridWorldSize = gridWorldSize;

        _gridSizeX = Mathf.RoundToInt(_gridWorldSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(_gridWorldSize.y / _nodeDiameter);
        _gridSizeZ = Mathf.RoundToInt(_gridWorldSize.z / _nodeDiameter);
        _gridWorldSize = new Vector3(_gridSizeX * _nodeDiameter, 0, _gridSizeZ * _nodeDiameter);

        _grid = new GroundBlock[_gridSizeX, _gridSizeZ];
        var wolrdBottomLeft = transform.position - Vector3.right * _gridWorldSize.x / 2f - Vector3.forward * _gridWorldSize.z / 2f;
        var id = 0;
        for (int i = 0; i < _gridSizeX; i++)
        {
            for (int j = 0; j < _gridSizeZ; j++)
            {
                var worldPosition = wolrdBottomLeft + Vector3.right * (i * _nodeDiameter + nodeRadius) + Vector3.forward * (j * _nodeDiameter + nodeRadius) + 0.5f * Vector3.up;
                var walkable = !Physics.CheckSphere(worldPosition, nodeRadius, UnwalkableMask);
                var blockGo = Instantiate(BlockPrefab, worldPosition, Quaternion.identity, BlockContainer.transform);
                var component = blockGo.GetComponent<GroundBlock>();
                blockGo.transform.localScale = new Vector3(_nodeDiameter, 1f, _nodeDiameter);
                component.SetWalkable();
                component.Block = new Block(worldPosition, walkable, id);
                blockGo.name = $"({i},{j})";
                component.Block.XCoordinate = i;
                component.Block.ZCoordinate = j;
                id++;
                _grid[i, j] = component;
            }
        }

        SetNeighbours();
        SetupWalls();
    }

    public void SetupHexaGrid(Vector3 gridWorldSize, float nodeRadius)
    {
        _gridWorldSize = gridWorldSize;

        var apothem = Mathf.Sqrt(3) * 0.5f * nodeRadius;

        _gridSizeX = Mathf.RoundToInt(_gridWorldSize.x / apothem);
        _gridSizeY = Mathf.RoundToInt(_gridWorldSize.y / _nodeDiameter);
        _gridSizeZ = Mathf.RoundToInt(_gridWorldSize.z / (1.5f * nodeRadius));
        _gridWorldSize = new Vector3(_gridSizeX * apothem, 0, _gridSizeZ * (1.5f * nodeRadius));

        _grid = new GroundBlock[_gridSizeX, _gridSizeZ];
        var wolrdBottomLeft = transform.position - Vector3.right * _gridWorldSize.x / 2f - Vector3.forward * _gridWorldSize.z / 2f;
        var id = 0;
        for (int i = 0; i < _gridSizeX; i++)
        {
            for (int j = 0; j < _gridSizeZ; j++)
            {
                if ((i + j) % 2 == 0)
                {
                    var worldPosition = wolrdBottomLeft + (i * apothem) * Vector3.right + (1.5f * nodeRadius * j) * Vector3.forward;
                    var walkable = !Physics.CheckSphere(worldPosition, nodeRadius, UnwalkableMask);
                    var blockGo = Instantiate(BlockPrefab, worldPosition, Quaternion.identity, BlockContainer.transform);
                    var component = blockGo.GetComponent<GroundBlock>();
                    component.SetWalkable();
                    component.Block = new Block(worldPosition, walkable, id);
                    blockGo.name = $"({i},{j})";
                    component.Block.XCoordinate = i;
                    component.Block.ZCoordinate = j;
                    id++;
                    _grid[i, j] = component;
                }
            }
        }

        SetHexaNeighbours();
        SetupHexaWalls();
    }

    public void SetupHexaWalls()
    {
        var xPos = (_gridWorldSize.x) / 2;
        var zPos = (_gridWorldSize.z) / 2;
        var leftWallPos = new Vector3(0, 1f, -xPos);
        var righttWallPos = new Vector3(0, 1f, xPos);
        var topWallPos = new Vector3(0, 1f, zPos);
        var botWallPos = new Vector3(0, 1f, -zPos);

        var leftWall = Instantiate(WallPrefab, leftWallPos, Quaternion.identity, WallContainer).GetComponent<Wall>();
        leftWall.name = "LeftWall";
        leftWall.WallWidth = _gridWorldSize.z + _nodeDiameter;
        leftWall.BuildHexaWalls(0, 90, 0);
        var rightWall = Instantiate(WallPrefab, righttWallPos, Quaternion.identity, WallContainer).GetComponent<Wall>();
        rightWall.name = "RightWall";
        rightWall.WallWidth = _gridWorldSize.z + _nodeDiameter;
        rightWall.BuildHexaWalls(0, 90, 0);
        var topWall = Instantiate(WallPrefab, topWallPos, Quaternion.identity, WallContainer).GetComponent<Wall>();
        topWall.name = "TopWall";
        topWall.WallWidth = _gridWorldSize.x + _nodeDiameter;
        topWall.BuildHexaWalls();
        var botWall = Instantiate(WallPrefab, botWallPos, Quaternion.identity, WallContainer).GetComponent<Wall>();
        botWall.name = "BotWall";
        botWall.WallWidth = _gridWorldSize.x + _nodeDiameter;
        botWall.BuildHexaWalls();

        //BuildRandomWall();
    }

    public void SetupWalls()
    {
        var xPos = (_gridWorldSize.x + NodeRadius) / 2;
        var zPos = (_gridWorldSize.z + NodeRadius) / 2;
        var leftWallPos = new Vector3(0, 1f, - xPos);
        var righttWallPos = new Vector3(0, 1f, xPos);
        var topWallPos = new Vector3(0, 1f, zPos);
        var botWallPos = new Vector3(0, 1f, - zPos);

        var leftWall = Instantiate(WallPrefab, leftWallPos, Quaternion.identity, WallContainer).GetComponent<Wall>();
        leftWall.name = "LeftWall";
        leftWall.WallWidth = _gridWorldSize.z + _nodeDiameter;
        leftWall.BuildWall(0,90,0);
        var rightWall = Instantiate(WallPrefab, righttWallPos, Quaternion.identity, WallContainer).GetComponent<Wall>();
        rightWall.name = "RightWall";
        rightWall.WallWidth = _gridWorldSize.z + _nodeDiameter;
        rightWall.BuildWall(0,90,0);
        var topWall = Instantiate(WallPrefab, topWallPos, Quaternion.identity, WallContainer).GetComponent<Wall>();
        topWall.name = "TopWall";
        topWall.WallWidth = _gridWorldSize.x + _nodeDiameter;
        topWall.BuildWall();
        var botWall = Instantiate(WallPrefab, botWallPos, Quaternion.identity, WallContainer).GetComponent<Wall>();
        botWall.name = "BotWall";
        botWall.WallWidth = _gridWorldSize.x + _nodeDiameter;
        botWall.BuildWall();

        //BuildRandomWall();
    }

    public void BuildRandomWall()
    {
        var number = Mathf.RoundToInt(Random.value * 10);

        var HConstraint = _gridWorldSize.x / 2;
        var VConstraint = _gridWorldSize.z / 2;
        for (int i = 0; i < number; i++)
        {
            var xPos = Mathf.RoundToInt((Random.value - 0.5f) * HConstraint);
            var zPos = Mathf.RoundToInt((Random.value - 0.5f) * VConstraint);
            var randomPosition = new Vector3(xPos, 1f, zPos);
            // 0 if horizontal, 1 if vertical
            var isRotated = Mathf.RoundToInt(Random.value);
            var condition = (1 - isRotated) * (HConstraint - Mathf.Abs(randomPosition.x)) + isRotated * (VConstraint - Mathf.Abs(randomPosition.z));
            var randomWidth = Random.value * condition;
            var wall = Instantiate(WallPrefab, randomPosition, Quaternion.identity, WallContainer).GetComponent<Wall>();
            wall.name = $"RandomWall_{i}";
            wall.WallWidth = randomWidth;
            wall.BuildWall(0, isRotated * 90, 0);
        }
    }

    private void SetNeighbours()
    {
        for (int i = 0; i < _gridSizeX; i++)
        {
            for (int j = 0; j < _gridSizeZ; j++)
            {
                var block = _grid[i, j].Block;
                block.Neighbours = GetNeighbours(block);
            }
        }
    }

    private void SetHexaNeighbours()
    {
        for (int i = 0; i < _gridSizeX; i++)
        {
            for (int j = 0; j < _gridSizeZ; j++)
            {
                try
                {
                    var blockGo = _grid[i, j];
                    if (blockGo != null)
                        blockGo.Block.Neighbours = GetHexaNeighbours(blockGo.Block);
                }
                catch(System.IndexOutOfRangeException e)
                {
                    continue;
                }
            }
        }
    }


    private List<Block> GetNeighbours(Block block)
    {
        var neighbours = new List<Block>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                var checkX = block.XCoordinate + i;
                var checkY = block.ZCoordinate + j;

                if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeZ)
                    neighbours.Add(_grid[checkX, checkY].Block);
            }
        }
        return neighbours;
    }

    private List<Block> GetHexaNeighbours(Block block)
    {
        var neighbours = new List<Block>();
        var xPos = block.XCoordinate;
        var zPos = block.ZCoordinate;
        var xCoordinates = new int[] { xPos - 2, xPos - 1, xPos + 1, xPos + 2, xPos + 1, xPos - 1 };
        var zCoordinates = new int[] { zPos, zPos + 1, zPos + 1, zPos, zPos - 1, zPos - 1 };

        for (int i = 0; i < 6; i++)
        {
            try
            {
                var neighbour = _grid[xCoordinates[i], zCoordinates[i]];
                neighbours.Add(neighbour.Block);
            }
            catch(System.IndexOutOfRangeException e)
            {
                continue;
            }
        }
        return neighbours;
    }

    public Block BlockFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + _gridWorldSize.x / 2) / _gridWorldSize.x;
        float percentY = (worldPosition.z + _gridWorldSize.z / 2) / _gridWorldSize.z;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        var i = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
        var j = Mathf.RoundToInt((_gridSizeZ - 1) * percentY);

        return _grid[i, j].Block;
    }

    public Block GetBlockFromWorldPosition(Vector3 worldPosition)
    {
        var layer = LayerMask.GetMask(Layer.Walkable.ToString());
        if (Physics.Raycast(worldPosition + Vector3.up, Vector3.down, out RaycastHit hit, 10f, layer))
            return hit.collider.GetComponentInParent<GroundBlock>().Block;

        return null;
    }

    public List<Block> GetAllBlocksInCircle(Vector3 center, float radius)
    {
        var blocks = new List<Block>();

        var colliders = Physics.OverlapSphere(center, radius);
        foreach (var col in colliders)
        {
            var blockGo = col.GetComponent<GroundBlock>();
            if (blockGo != null)
                blocks.Add(blockGo.Block);
        }

        return blocks;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(_gridWorldSize.x, 1, _gridWorldSize.z));
        if (_grid == null)
            return;

        if (Path != null)
        {
            var cubeSize = new Vector3(_nodeDiameter - 0.05f, 0.1f, _nodeDiameter - 0.05f);
            foreach (var go in _grid)
            {
                if (!Path.ContainsKey(go.Block.Id))
                    continue;

                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(go.Block.WorldPosition, cubeSize);
            }
        }
    }
}