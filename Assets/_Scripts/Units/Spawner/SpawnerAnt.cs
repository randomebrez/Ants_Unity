using mew;
using UnityEngine;

public class SpawnerAnt : MonoBehaviour
{
    private float _radius => EnvironmentManager.Instance.NodeRadius;
    private int antCounter = 0;
    public BlockBase BlockPosition;

    // boolean value to know if it is able to spawnAnt
    private bool _activated = true;

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    private void Update()
    {
        if (!_activated)
            return;

        if (Input.GetKeyDown(KeyCode.Space) == false)
            return;

        for (int i = 0; i < 5; i++)
            InstantiateUnit(ScriptableAntBase.AntTypeEnum.Worker);
    }

    public void SetRadius(float newRadius)
    {
        transform.localScale = new Vector3(newRadius, transform.localScale.y, newRadius);
    }

    private void InstantiateUnit(ScriptableAntBase.AntTypeEnum antType, Transform parent = null)
    {
        antCounter++;

        var scriptableObject = ResourceSystem.Instance.AntOfTypeGet(antType);

        if (parent == null)
            parent = transform;

        // Calculate random position and rotation to get out of the nest
        var randomPoint = Random.value * 2 * Mathf.PI;
        var randomVector = new Vector3(Mathf.Cos(randomPoint), 0, Mathf.Sin(randomPoint));
        var startPosition = transform.position + 0.5f * Vector3.up + _radius * randomVector;
        var startAngle = Vector3.SignedAngle(Vector3.right, randomVector, Vector3.up);

        // Ant game object
        var spawnedAnt = Instantiate(scriptableObject.AntPrefab, startPosition, Quaternion.Euler(0, startAngle, 0));
        spawnedAnt.transform.parent = parent;
        spawnedAnt.name = $"{antType}_{antCounter}";

        //Set statistics according to scriptable object
        spawnedAnt.Initialyze(scriptableObject.BaseStats);
        spawnedAnt.Clicked += UnitManager.Instance.AntClick;
    }
}
