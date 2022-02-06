using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mew;

public class UnitManager : Singleton<UnitManager>
{
    public GameObject UnitsContainer;

    private Vector3 GetGroundSize => EnvironmentManager.Instance.GroundSize;

    public void SpawnAnt(ScriptableAntBase.AntTypeEnum type, Vector3 worldPosition, Quaternion rotation)
    {
        var scriptableObject = ResourceSystem.Instance.AntOfTypeGet(type);

        var spawnedAnt = SpawnerAnt.Instance.InstantiateUnit(scriptableObject.AntPrefab, worldPosition, rotation, UnitsContainer.transform);
        spawnedAnt.transform.SetPositionAndRotation(worldPosition, rotation);
        Debug.Log($"{spawnedAnt.transform.position.x}, {spawnedAnt.transform.position.y}, {spawnedAnt.transform.position.z}");

        spawnedAnt.SetStats(scriptableObject.BaseStats);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) == false)
            return;

        var randomX = (Random.value - 0.5f) * GetGroundSize.x;
        var randomZ = (Random.value - 0.5f) * GetGroundSize.z;

        var randomRotation = (Random.value - 0.5f) * 360;

        SpawnAnt(ScriptableAntBase.AntTypeEnum.Worker, new Vector3(randomX, 0, randomZ), Quaternion.Euler(0,randomRotation,0));
    }
}
