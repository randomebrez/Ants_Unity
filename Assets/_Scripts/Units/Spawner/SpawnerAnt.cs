using Assets._Scripts.Utilities;
using Assets.Dtos;
using mew;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerAnt : MonoBehaviour
{
    private Transform _unitContainer;

    private void Start()
    {
        _unitContainer = transform.GetChild(0);
    }

    public List<BaseAnt> InstantiateUnits(UnitWrapper[] units, ScriptableAntBase.AntTypeEnum antType)
    {
        var result = new List<BaseAnt>();
        var scriptableObject = ResourceSystem.Instance.AntOfTypeGet(antType);

        for (int i = 0; i < units.Length; i++)
        {
            // Calculate random position and rotation to get out of the nest
            var randomPoint = Random.value * 2 * Mathf.PI;
            var randomVector = new Vector3(Mathf.Cos(randomPoint), 0, Mathf.Sin(randomPoint));
            var startPosition = GlobalParameters.NodeRadius * (1 + GlobalParameters.NodeRadius) * Vector3.up + GlobalParameters.NodeRadius * randomVector;
            var startingBlock = EnvironmentManager.Instance.GroundBlockFromWorldPoint(startPosition);
            var startAngle = Vector3.SignedAngle(Vector3.right, randomVector, Vector3.up);

            // Ant game object
            var spawnedAnt = Instantiate(scriptableObject.AntPrefab, startPosition, Quaternion.Euler(0, startAngle, 0));
            spawnedAnt.transform.parent = _unitContainer;
            spawnedAnt.transform.localScale = GlobalParameters.NodeRadius * Vector3.one;
            spawnedAnt.name = $"{antType}_{i}";

            //Set statistics according to scriptable object
            spawnedAnt.Initialyze(scriptableObject.BaseStats, units[i], startingBlock);
            spawnedAnt.Clicked += UnitManager.Instance.AntClick;
            result.Add(spawnedAnt);
        }

        return result;
    }
}
