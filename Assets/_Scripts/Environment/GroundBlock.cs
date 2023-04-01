using Assets._Scripts.Utilities;
using Assets.Dtos;
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

    public bool HasAnyActivePheromoneToken = false;

    private MeshRenderer _renderer;
    [Range(0,31)] public int OcclusionLayer;

    private void Awake()
    {
        _renderer = GetComponentInChildren<MeshRenderer>();
        SetPheromoneTokenPositions();
    }

    private void SetPheromoneTokenPositions()
    {
        _pheromoneTokenPositions.Add(PheromoneTypeEnum.Wander, Vector3.up + 0.5f * GlobalParameters.NodeRadius * Vector3.right);
        _pheromoneTokenPositions.Add(PheromoneTypeEnum.CarryFood, Vector3.up - 0.5f * GlobalParameters.NodeRadius * Vector3.right);
        _pheromoneTokens.Add(PheromoneTypeEnum.Wander, null);
        _pheromoneTokens.Add(PheromoneTypeEnum.CarryFood, null);
    }

    public void AddOrCreatePheromoneOnBlock(BasePheromone pheromone)
    {
        var pheromoneToHandle = _pheromoneTokens[pheromone.Caracteristics.PheromoneType];

        if (pheromoneToHandle == null)
        {
            pheromone.transform.parent = transform;
            pheromone.transform.position = _pheromoneTokenPositions[pheromone.Caracteristics.PheromoneType];
            _pheromoneTokens[pheromone.Caracteristics.PheromoneType] = pheromone;
        }
        else
        {
            pheromoneToHandle.MergePheromones(pheromone);
            Destroy(pheromone.gameObject);
        }
    }

    public void CleanPheromones()
    {
        foreach (var pheromone in _pheromoneTokens.Values)
            Destroy(pheromone.gameObject);
    }

    public void ApplyTimeEffect()
    {
        foreach (var pheromoneToken in _pheromoneTokens.Values)
            pheromoneToken.ApplyTimeEffect();

        HasAnyActivePheromoneToken = _pheromoneTokens.Values.Any(t => t.Pheromone.RemainingTime > 0);
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

    /*
    public void OnDrawGizmos()
    {
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireCube(transform.position, Vector3.one);
    }*/
}
