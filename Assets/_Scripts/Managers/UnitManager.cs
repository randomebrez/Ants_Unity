using System.Collections.Generic;
using UnityEngine;
using mew;
using Assets._Scripts.Utilities;
using static mew.ScriptablePheromoneBase;
using Assets.Dtos;
using System.Linq;

internal class UnitManager : BaseManager<UnitManager>
{
    public AntColony AntColonyPrefab;

    private Vector3 GetGroundSize => GlobalParameters.GroundSize;
    private List<AntColony> _colonies = new List<AntColony>();
    private BaseAnt _lastClicked;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            CreateNewColony();

        if (Input.GetKeyDown(KeyCode.A))
            CreateNewColony();
    }

    public void CreateNewColony()
    {
        GameObject spawned = null;
        while (spawned == null)
        {
            var randomX = (Random.value - 0.5f) * GetGroundSize.x - 1;
            var randomZ = (Random.value - 0.5f) * GetGroundSize.z - 1;

            var spawnerMaxRange = Mathf.Max(AntColonyPrefab.transform.lossyScale.x, AntColonyPrefab.transform.lossyScale.y, AntColonyPrefab.transform.lossyScale.z);
            var spawnposition = 2 * GlobalParameters.NodeRadius * Vector3.up;
            var spawnBlock = EnvironmentManager.Instance.GroundBlockFromWorldPoint(spawnposition);
            if (spawnBlock == null)
                Debug.Log("spawn null");
            spawned = InstantiateObject(AntColonyPrefab.gameObject, spawnBlock.Block.WorldPosition + 2 * GlobalParameters.NodeRadius * Vector3.up, Quaternion.identity, EnvironmentManager.Instance.GetUnitContainer(), spawnerMaxRange * 1.5f);
        }

        var id = _colonies.Count + 1;
        var colony = spawned.GetComponent<AntColony>();
        colony.Initialyze($"Colony_{id}");
        _colonies.Add(colony);
    }

    public Dictionary<PheromoneTypeEnum, List<Block>> GetUnitPositions()
    {
        if (_colonies.Any() == false)
            return new Dictionary<PheromoneTypeEnum, List<Block>>();

        var result = _colonies[0].GetAllAntPositions();
        for (int i = 1; i < _colonies.Count; i++)
        {
            var temp = _colonies[i].GetAllAntPositions();
            foreach(var pair in temp)
                result[pair.Key].AddRange(pair.Value);
        }

        return result;
    }

    public void MoveAllUnits()
    {
        for (int i = 0; i < _colonies.Count; i++)
            _colonies[i].MoveAllAnts();
    }

    public void RenewColonies()
    {
        for (int i = 0; i < _colonies.Count; i++)
            _colonies[i].RenewPopulation();

    }

    public void AntClick(BaseAnt ant)
    {
        if (_lastClicked != null)
            _lastClicked.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.black;

        ant.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.yellow;
        _lastClicked = ant;
    }
}
