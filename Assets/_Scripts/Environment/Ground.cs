using System.Collections.Generic;
using Assets._Scripts.Utilities;
using Assets.Dtos;
using UnityEngine;

public class Ground : MonoBehaviour
{
    private Transform _blockContainer;
    private Transform _wallContainer;

    public GroundBlock BlockPrefab;
    public Wall WallPrefab;
    public LayerMask Everything;
    public LayerMask UnwalkableMask;
    
    public float NodeRadius => GlobalParameters.NodeRadius;
    public Dictionary<int, Block> Path = null;

    private Vector3 _gridWorldSize;
    private GroundBlock[,] _grid;
    private int _gridSizeX, _gridSizeY, _gridSizeZ;
    private Vector3 _worldBottomleft;
    private float _xShift => Mathf.Sqrt(3) * 0.5f * NodeRadius;
    private float _zShift => 1.5f * NodeRadius;
    private float _nodeDiameter => 2 * NodeRadius;
    private List<Vector3> _positions = new List<Vector3>();

    public void Awake()
    {
        _blockContainer = transform.GetChild(0);
        _wallContainer = transform.GetChild(1);
    }

    public void AddorCreatePheromoneOnBlock(BasePheromone pheromone, Block block)
    {
        var blockGo = _grid[block.XCoordinate, block.ZCoordinate];

        blockGo.AddOrCreatePheromoneOnBlock(pheromone);
    }

    public void ApplyTimeEffect()
    {
        PheromoneTimeEffect();
    }

    private void PheromoneTimeEffect()
    {
        for (int i = 0; i < _gridSizeX; i++)
        {
            for(int j = 0; j < _gridSizeY; i++)
            {
                if (_grid[i,j].HasAnyActivePheromoneToken)
                    _grid[i,j].ApplyTimeEffect();
            }
        }
    }

    public void CleanAllPheromones()
    {
        for (int i = 0; i < _gridSizeX; i++)
        {
            for (int j = 0; j < _gridSizeY; i++)
            {
                if (_grid[i, j].HasAnyActivePheromoneToken)
                    _grid[i, j].CleanPheromones();
            }
        }
    }

    public void SetupHexaGrid()
    {
        _gridWorldSize = GlobalParameters.GroundSize;

        _gridSizeX = Mathf.RoundToInt(_gridWorldSize.x / _xShift);
        _gridSizeY = Mathf.RoundToInt(_gridWorldSize.y / _nodeDiameter);
        _gridSizeZ = Mathf.RoundToInt(_gridWorldSize.z / _zShift);
        _gridWorldSize = new Vector3(_gridSizeX * _xShift, 0, _gridSizeZ * _zShift);

        _grid = new GroundBlock[_gridSizeX, _gridSizeZ];
        _worldBottomleft = transform.position - Vector3.right * _gridWorldSize.x / 2f - Vector3.forward * _gridWorldSize.z / 2f;
        var id = 0;
        for (int i = 0; i < _gridSizeX; i++)
        {
            for (int j = 0; j < _gridSizeZ; j++)
            {
                if ((i + j) % 2 == 0)
                {
                    var worldPosition = _worldBottomleft + (i * _xShift) * Vector3.right + (1.5f * NodeRadius * j) * Vector3.forward;
                    _positions.Add(worldPosition);
                    var walkable = !Physics.CheckSphere(worldPosition, NodeRadius, UnwalkableMask);
                    var blockGo = Instantiate(BlockPrefab, worldPosition, Quaternion.identity, _blockContainer.transform);
                    blockGo.SetWalkable();
                    blockGo.Block = new Block(worldPosition, walkable, id);
                    blockGo.name = $"({i},{j})";
                    blockGo.Block.XCoordinate = i;
                    blockGo.Block.ZCoordinate = j;
                    _grid[i, j] = blockGo;

                    id++;
                }
            }
        }

        SetHexaNeighbours();
        SetupHexaWalls();
        transform.position += Vector3.up * NodeRadius;
    }

    public void SetupHexaWalls()
    {
        var xPos =  _worldBottomleft + Vector3.forward * _gridWorldSize.z / 2f ;
        var zPos = _worldBottomleft + Vector3.right * _gridWorldSize.x / 2f;
        var leftWallPos = xPos;
        var righttWallPos = - xPos + 2 * xPos.y * Vector3.up;
        var botWallPos = zPos;
        var topWallPos = - zPos + 2 * zPos.y * Vector3.up;

        var leftWall = Instantiate(WallPrefab, leftWallPos, Quaternion.identity, _wallContainer);
        leftWall.name = "LeftWall";
        leftWall.BuildHexaWalls(_gridWorldSize.z + _nodeDiameter, 5, 3, -90);
        var rightWall = Instantiate(WallPrefab, righttWallPos, Quaternion.identity, _wallContainer);
        rightWall.name = "RightWall";
        rightWall.BuildHexaWalls(_gridWorldSize.z + _nodeDiameter, 5, 3, 90);
        var topWall = Instantiate(WallPrefab, topWallPos, Quaternion.identity, _wallContainer);
        topWall.name = "TopWall";
        topWall.BuildHexaWalls(_gridWorldSize.x + _nodeDiameter, 5, 3);
        var botWall = Instantiate(WallPrefab, botWallPos, Quaternion.identity, _wallContainer);
        botWall.name = "BotWall";
        botWall.BuildHexaWalls(_gridWorldSize.x + _nodeDiameter, 5, 3, 180);

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
            var randomHeight = Random.Range(1, 8);
            var randomThickness = Random.Range(1, 4);

            var wall = Instantiate(WallPrefab, randomPosition, Quaternion.identity, _wallContainer).GetComponent<Wall>();
            wall.name = $"RandomWall_{i}";
            wall.BuildWall(randomWidth, randomHeight, randomThickness, 0, isRotated * 90, 0);
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
        if (Physics.Raycast(transform.position + worldPosition + Vector3.up, Vector3.down, out RaycastHit hit, 10f, layer))
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

    /*
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(_gridWorldSize.x, 1, _gridWorldSize.z));
        //Gizmos.color = Color.red;
        //foreach(var position in _positions)
        //    Gizmos.DrawWireCube(position, new Vector3(1, 1, 1));
    }*/
}