using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mew;

internal class UnitManager : BaseManager<UnitManager>
{
    public SpawnerAnt SpawnerAntPrefab;
    public GameObject UnitsContainer;

    private Vector3 GetGroundSize => EnvironmentManager.Instance.GroundSize;

    private Dictionary<int, SpawnerAnt> _spawners = new Dictionary<int, SpawnerAnt>();

    public void SpawnAntNest()
    {
        GameObject spawned = null;
        while (spawned == null)
        {
            var randomX = (Random.value - 0.5f) * GetGroundSize.x - 1;
            var randomZ = (Random.value - 0.5f) * GetGroundSize.z - 1;

            var spawnerMaxRange = Mathf.Max(SpawnerAntPrefab.transform.lossyScale.x, SpawnerAntPrefab.transform.lossyScale.y, SpawnerAntPrefab.transform.lossyScale.z);
            spawned = InstantiateObject(SpawnerAntPrefab.gameObject, new Vector3(randomX, 0, randomZ), Quaternion.identity, UnitsContainer.transform, spawnerMaxRange * 1.5f);
        }

        var id = _spawners.Count + 1;
        _spawners.Add(id, spawned.GetComponent<SpawnerAnt>());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            SpawnAntNest();

        if (Input.GetKeyDown(KeyCode.A))
            SpawnAntNest();
    }
}
