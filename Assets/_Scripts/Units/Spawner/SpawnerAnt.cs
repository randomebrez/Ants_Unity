using Assets._Scripts.Utilities;
using mew;
using NeuralNetwork.Interfaces.Model;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerAnt : MonoBehaviour
{
    private PheromoneManager _pheromoneManager;
    private Transform _unitContainer;

    private void Start()
    {
        _pheromoneManager = GetComponentInChildren<PheromoneManager>();
        _unitContainer = transform.GetChild(1);
    }

    public List<BaseAnt> InstantiateUnits(List<Brain> brains, ScriptableAntBase.AntTypeEnum antType)
    {
        var result = new List<BaseAnt>();
        var scriptableObject = ResourceSystem.Instance.AntOfTypeGet(antType);

        for (int i = 0; i < brains.Count; i++)
        {
            // Calculate random position and rotation to get out of the nest
            var randomPoint = Random.value * 2 * Mathf.PI;
            var randomVector = new Vector3(Mathf.Cos(randomPoint), 0, Mathf.Sin(randomPoint));
            var startPosition = transform.position + (GlobalParameters.NodeRadius / 2) * Vector3.up + GlobalParameters.NodeRadius * randomVector;
            var startAngle = Vector3.SignedAngle(Vector3.right, randomVector, Vector3.up);

            // Ant game object
            var spawnedAnt = Instantiate(scriptableObject.AntPrefab, startPosition, Quaternion.Euler(0, startAngle, 0));
            spawnedAnt.transform.parent = _unitContainer;
            spawnedAnt.name = $"{antType}_{i}";

            //Set statistics according to scriptable object
            spawnedAnt.Initialyze(scriptableObject.BaseStats, brains[i], _pheromoneManager.transform);
            spawnedAnt.Clicked += UnitManager.Instance.AntClick;
            result.Add(spawnedAnt);
        }

        return result;
    }

    public void CleanPheromones()
    {
        _pheromoneManager.CleanAllPheromones();
    }
}
