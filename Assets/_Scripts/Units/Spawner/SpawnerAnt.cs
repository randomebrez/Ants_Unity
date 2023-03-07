using mew;
using NeuralNetwork.Interfaces.Model;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerAnt : MonoBehaviour
{
    public BlockBase BlockPosition;

    // boolean value to know if it is able to spawnAnt
    private bool _activated = false;
    private float _radius => EnvironmentManager.Instance.NodeRadius;

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
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
            var startPosition = transform.position + 0.5f * Vector3.up + _radius * randomVector;
            var startAngle = Vector3.SignedAngle(Vector3.right, randomVector, Vector3.up);

            // Ant game object
            var spawnedAnt = Instantiate(scriptableObject.AntPrefab, startPosition, Quaternion.Euler(0, startAngle, 0));
            spawnedAnt.transform.parent = transform;
            spawnedAnt.name = $"{antType}_{i}";

            //Set statistics according to scriptable object
            spawnedAnt.Initialyze(scriptableObject.BaseStats, brains[i]);
            spawnedAnt.Clicked += UnitManager.Instance.AntClick;
            result.Add(spawnedAnt);
        }

        return result;
    }
}
