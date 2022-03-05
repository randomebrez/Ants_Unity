using UnityEngine;

internal class EnvironmentManager : BaseManager<EnvironmentManager>
{
    public Vector3 GroundSize = new Vector3(75, 0, 50);
    public float NodeRadius = 1f;

    public GameObject GroundPrefab;
    public GameObject EnvironmentContainer;
    public GameObject FoodPrefab;

    private Ground _ground;

    public void SpawnGround()
    {
        if (_ground != null)
            return;

        _ground = Instantiate(GroundPrefab, EnvironmentContainer.transform).GetComponent<Ground>();
        _ground.SetupGrid(GroundSize, NodeRadius);
    }

    public void SpawnFood()
    {
        var randomX = (Random.value - 0.5f) * GroundSize.x;
        var randomZ = (Random.value - 0.5f) * GroundSize.z;
        var spawnPosition = new Vector3(randomX, 1.1f, randomZ);
        InstantiateObject(FoodPrefab, spawnPosition, Quaternion.identity, EnvironmentContainer.transform, 3 * NodeRadius);
    }

    private void Update()
    {
        // Get mouse position to spawn food on click        
    }
}
