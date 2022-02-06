using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mew;

public class UnitManager : Singleton<UnitManager>
{
    public SpawnerAnt SpawnerAntPrefab;
    public GameObject UnitsContainer;

    private Vector3 GetGroundSize => EnvironmentManager.Instance.GroundSize;

    private Dictionary<int, SpawnerAnt> _spawners = new Dictionary<int, SpawnerAnt>();

    public void SpawnAntNest()
    {
        var randomX = (Random.value - 0.5f) * GetGroundSize.x;
        var randomZ = (Random.value - 0.5f) * GetGroundSize.z;

        var spawned = Instantiate(SpawnerAntPrefab, new Vector3(randomX, 0, randomZ), Quaternion.identity, UnitsContainer.transform);
        var id = _spawners.Count + 1;
        _spawners.Add(id, spawned);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            SpawnAntNest();

        if (Input.GetKeyDown(KeyCode.A))
            SpawnAntNest();
    }
}
