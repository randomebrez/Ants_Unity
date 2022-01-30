using System.Collections.Generic;
using Assets.Dtos;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public GameObject BlockPrefab;
    public LayerMask Everything;
    public LayerMask UnwalkableMask;
    public Vector3 GridWorldSize;
    public GameObject BlockContainer;
    public float NodeRadius { get; set; }
    public Dictionary<int, Block> Path = null;

    private GroundBlock[,] _grid;
    private int _gridSizeX, _gridSizeY, _gridSizeZ;
    private float _nodeDiameter => 2 * NodeRadius;
    public int MaxSize => _gridSizeX * _gridSizeZ;

    public void SetupGrid(float nodeRadius)
    {
        NodeRadius = nodeRadius;

        _gridSizeX = Mathf.RoundToInt(GridWorldSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(GridWorldSize.y / _nodeDiameter);
        _gridSizeZ = Mathf.RoundToInt(GridWorldSize.z / _nodeDiameter);

        _grid = new GroundBlock[_gridSizeX, _gridSizeZ];
        var wolrdBottomLeft = transform.position - Vector3.right * GridWorldSize.x / 2f - Vector3.forward * GridWorldSize.z / 2f;
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
                component.Setup(walkable);
                component.Block = new Block(worldPosition, walkable, id);
                blockGo.name = $"({i},{j})";
                component.Block.XCoordinate = i;
                component.Block.ZCoordinate = j;
                id++;
                _grid[i, j] = component;
            }
        }

        SetNeighbours();
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

    public Block BlockFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + GridWorldSize.x / 2) / GridWorldSize.x;
        float percentY = (worldPosition.z + GridWorldSize.z / 2) / GridWorldSize.z;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        var i = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
        var j = Mathf.RoundToInt((_gridSizeZ - 1) * percentY);

        return _grid[i, j].Block;
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
        Gizmos.DrawWireCube(transform.position, new Vector3(GridWorldSize.x, 1, GridWorldSize.z));
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