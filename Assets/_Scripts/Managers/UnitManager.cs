using System.Collections.Generic;
using UnityEngine;
using mew;
using Assets._Scripts.Utilities;

internal class UnitManager : BaseManager<UnitManager>
{
    public AntColony AntColonyPrefab;
    public GameObject UnitsContainer;
    //public GameObject NeuronManager;

    private Vector3 GetGroundSize => GlobalParameters.GroundSize;
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
            var spawnposition = GlobalParameters.NodeRadius * Vector3.up;
            var spawnBlock = EnvironmentManager.Instance.BlockFromWorldPoint(spawnposition);
            if (spawnBlock == null)
                Debug.Log("spawn null");
            spawned = InstantiateObject(AntColonyPrefab.gameObject, spawnposition, Quaternion.identity, UnitsContainer.transform, spawnerMaxRange * 1.5f);
        }

        var id = _colonies.Count + 1;
        spawned.transform.parent = EnvironmentManager.Instance.GetUnitContainer();
        var colony = spawned.GetComponent<AntColony>();
        colony.Initialyze($"Colony_{id}");
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

        //var neuronManager = Instantiate(NeuronManager, transform.position + 2 * Vector3.up, Quaternion.identity, transform);
        //neuronManager.GetComponent<NeuronManager>().Initialyze(ant);

        //var probaScreen = CanvasContainer.GetComponentInChildren<ProbaScreenManager>();
        ant.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.yellow;
        //probaScreen.SetAnt(ant);
        _lastClicked = ant;
    }
}
