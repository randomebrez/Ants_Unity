using Assets._Scripts.Utilities;
using Assets.Dtos;
using mew;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static mew.ScriptablePheromoneBase;

public class GroundBlock : MonoBehaviour
{
    public Block Block { get; set; }

    public Material WalkableMaterial;
    public Material UnwalkableMaterial;

    private Dictionary<PheromoneTypeEnum, Vector3> _pheromoneTokenPositions;
    private Dictionary<PheromoneTypeEnum, BasePheromone> _pheromoneTokens;

    private Vector3 _foodTokenPosition;
    private FoodToken _foodToken;

    public bool HasAnyActivePheromoneToken = false;
    public bool HasAnyFood => _foodToken != null && _foodToken.StackNumber > 0;

    private MeshRenderer _renderer;
    [Range(0,31)] public int OcclusionLayer;

    private void Awake()
    {
        _renderer = GetComponentInChildren<MeshRenderer>();
        SetPheromoneTokenPositions();
    }    

    public float GetPheromoneDensity(PheromoneTypeEnum pheroType)
    {
        if (_pheromoneTokens[pheroType] != null)
            return _pheromoneTokens[pheroType].Pheromone.Density;

        return 0;
    }

    public void AddOrCreatePheromoneOnBlock(PheromoneTypeEnum pheroType)
    {
        var scriptablePheromone = ResourceSystem.Instance.PheromoneOfTypeGet(pheroType);
        var pheromoneToHandle = _pheromoneTokens[pheroType];
        
        if (pheromoneToHandle != null)
            pheromoneToHandle.MergePheromones(scriptablePheromone.BaseCaracteristics.Duration);
        else
        {
            // Spawn pheromone
            var pheromone = Instantiate(scriptablePheromone.PheromonePrefab, transform);
            pheromone.transform.localPosition = _pheromoneTokenPositions[pheroType];
            pheromone.Initialyze(scriptablePheromone.BaseCaracteristics);

            _pheromoneTokens[pheroType] = pheromone;

            HasAnyActivePheromoneToken = true;
        }
    }

    public void CleanPheromones()
    {
        foreach (var pheromone in _pheromoneTokens.Values.Where(t => t!= null))
            Destroy(pheromone.gameObject);
    }

    public void AddOrCreateFoodTookenOnBlock(FoodToken foodToken)
    {
        if (_foodToken == null)
        {
            var spawned = Instantiate(foodToken, transform);
            foodToken.transform.localPosition = _foodTokenPosition;
            _foodToken = spawned;
        }
        _foodToken.StackNumber++;
        _foodToken.gameObject.SetActive(true);
    }

    public void RemoveFoodToken(bool allToken = false)
    {
        if (_foodToken == null)
            return;

        if (allToken)
            _foodToken.StackNumber = 0;

        else if (_foodToken.StackNumber > 0)
            _foodToken.StackNumber--;

        if (_foodToken != null && _foodToken.StackNumber == 0)
            _foodToken.gameObject.SetActive(false);
    }

    public void ApplyTimeEffect()
    {
        var activePheromones = _pheromoneTokens.Values.Where(t => t != null);

        if (activePheromones.Count() == 0)
            return;

        foreach (var pheromoneToken in activePheromones)
            pheromoneToken.ApplyTimeEffect();

        HasAnyActivePheromoneToken = activePheromones.Count() > 0;
    }

    public void SetWalkable()
    {
        _renderer.material = WalkableMaterial;
        transform.GetChild(0).gameObject.layer = (int) Layer.Walkable;
    }

    public void SetUnwalkable()
    {
        _renderer.material = UnwalkableMaterial;
        gameObject.layer = (int) Layer.Unwalkable;
    }


    private void SetPheromoneTokenPositions()
    {
        _pheromoneTokenPositions = new Dictionary<PheromoneTypeEnum, Vector3>
        {
            { PheromoneTypeEnum.Wander, 0.5f * GlobalParameters.NodeRadius * Vector3.right },
            { PheromoneTypeEnum.CarryFood, - 0.5f * GlobalParameters.NodeRadius * Vector3.right }
        };

        _pheromoneTokens = new Dictionary<PheromoneTypeEnum, BasePheromone>
        {
            { PheromoneTypeEnum.Wander, null },
            { PheromoneTypeEnum.CarryFood, null }
        };

        _foodTokenPosition = 0.5f * GlobalParameters.NodeRadius * Vector3.up;
    }
    /*
    public void OnDrawGizmos()
    {
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireCube(transform.position, Vector3.one);
    }*/
}
