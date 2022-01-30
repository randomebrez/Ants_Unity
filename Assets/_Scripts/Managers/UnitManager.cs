using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mew;

public class UnitManager : Singleton<UnitManager>
{
    public GameObject UnitsContainer;
    private Vector2 _v;
    
    public void SpawnAnt(ScriptableAntBase.AntTypeEnum type, Vector3 worldPosition)
    {
        var scriptableObject = ResourceSystem.Instance.AntOfTypeGet(type);

        var spawnedAnt = Instantiate(scriptableObject.AntPrefab, worldPosition, Quaternion.identity, UnitsContainer.transform);

        spawnedAnt.SetStats(scriptableObject.BaseStats);
    }

    // Start is called before the first frame update
    void Start()
    {
        //SpawnAnt(ScriptableAntBase.AntTypeEnum.Worker, new Vector3 (0,1.25f,0));
    }

    // Update is called once per frame
    void Update()
    {
        var random = Random.insideUnitCircle;
        _v = Quaternion.Euler(0, 0, 0) * new Vector3(random.x, 0, Mathf.Abs(random.y)) * 4;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Vector3.zero, new Vector3(_v.x, 0, _v.y));
    }
}
