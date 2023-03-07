using System.Collections.Generic;
using UnityEngine;
using mew;
using NeuralNetwork.Interfaces.Model;

internal class UnitManager : BaseManager<UnitManager>
{
    public AntColony AntColonyPrefab;
    public GameObject UnitsContainer;

    public int MaxPopulationByColony;
    public NetworkCaracteristics BrainCaracteristics = new NetworkCaracteristics
        {
            GeneNumber = 200,
            InputNumber = 31,
            OutputNumber = 6,
            NeutralNumber = 10,
            WeighBytesNumber = 4
        };

    private Vector3 GetGroundSize => EnvironmentManager.Instance.GroundSize;
    private Dictionary<int, AntColony> _colonies = new Dictionary<int, AntColony>();
    private BaseAnt _lastClicked;

    public void CreateNewColony()
    {
        GameObject spawned = null;
        while (spawned == null)
        {
            var randomX = (Random.value - 0.5f) * GetGroundSize.x - 1;
            var randomZ = (Random.value - 0.5f) * GetGroundSize.z - 1;

            var spawnerMaxRange = Mathf.Max(AntColonyPrefab.transform.lossyScale.x, AntColonyPrefab.transform.lossyScale.y, AntColonyPrefab.transform.lossyScale.z);
            spawned = InstantiateObject(AntColonyPrefab.gameObject, new Vector3(0, 1, 0), Quaternion.identity, UnitsContainer.transform, spawnerMaxRange * 1.5f);
        }

        var id = _colonies.Count + 1;
        spawned.transform.parent = EnvironmentManager.Instance.GetUnitContainer();
        var colony = spawned.GetComponent<AntColony>();
        colony.Initialyze($"Colony_{id}",BrainCaracteristics, MaxPopulationByColony);
        _colonies.Add(id, colony);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            CreateNewColony();

        if (Input.GetKeyDown(KeyCode.A))
            CreateNewColony();
    }

    public void AntClick(BaseAnt ant)
    {
        if (_lastClicked != null)
            _lastClicked.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.black;

        var probaScreen = CanvasContainer.GetComponentInChildren<ProbaScreenManager>();
        ant.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.yellow;
        probaScreen.SetAnt(ant);
        _lastClicked = ant;
    }
}
