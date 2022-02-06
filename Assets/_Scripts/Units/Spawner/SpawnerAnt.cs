using mew;
using UnityEngine;

public class SpawnerAnt : MonoBehaviour
{
    private float _radius => transform.localScale.x;

    // boolean value to know if it is able to spawnAnt
    private bool _activated = true;

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, 1.1f, transform.position.z);
    }

    private void Update()
    {
        if (!_activated)
            return;

        if (Input.GetKeyDown(KeyCode.Space) == false)
            return;

        InstantiateUnit(ScriptableAntBase.AntTypeEnum.Worker);
    }

    public void SetRadius(float newRadius)
    {
        transform.localScale = new Vector3(newRadius, transform.localScale.y, newRadius);
    }

    private void InstantiateUnit(ScriptableAntBase.AntTypeEnum antType, Transform parent = null)
    {
        var scriptableObject = ResourceSystem.Instance.AntOfTypeGet(ScriptableAntBase.AntTypeEnum.Worker);

        if (parent == null)
            parent = transform;

        var randomPoint = Random.value * 2 * Mathf.PI;

        var randomVector = new Vector3(Mathf.Cos(randomPoint), 0, Mathf.Sin(randomPoint));
        var startPosition = transform.position + _radius * randomVector;
        var startAngle = Vector3.SignedAngle(Vector3.right, randomVector, Vector3.up);

        var spawnedAnt = Instantiate(scriptableObject.AntPrefab, startPosition, Quaternion.Euler(0, startAngle, 0));
        spawnedAnt.transform.parent = parent;
        spawnedAnt.SetStats(scriptableObject.BaseStats);
    }
}
