using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mew;

public class UnitManager : Singleton<UnitManager>
{
    public GameObject UnitsContainer;

    void Start()
    {
        for(int i = 0; i < 5; i ++)
        {
            SpawnAnt(ScriptableAntBase.AntTypeEnum.Worker, new Vector3(0, 1.25f, 0));
        }
    }

    public void SpawnAnt(ScriptableAntBase.AntTypeEnum type, Vector3 worldPosition)
    {
        var scriptableObject = ResourceSystem.Instance.AntOfTypeGet(type);

        var spawnedAnt = Instantiate(scriptableObject.AntPrefab, worldPosition, Quaternion.identity, UnitsContainer.transform);

        spawnedAnt.SetStats(scriptableObject.BaseStats);
    }
}
