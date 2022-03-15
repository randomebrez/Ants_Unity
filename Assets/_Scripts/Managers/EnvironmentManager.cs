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

    public void SpawnFood(float angle)
    {
        //var randomX = (Random.value - 0.5f) * GroundSize.x;
        //var randomZ = (Random.value - 0.5f) * GroundSize.z;

        var radius = GroundSize.z / 3f;
        var spawnPosition = Quaternion.Euler(0, angle, 0) * (radius * Vector3.forward) + new Vector3(0,1.1f,0);
        InstantiateObject(FoodPrefab, spawnPosition, Quaternion.identity, EnvironmentContainer.transform, 3 * NodeRadius);
    }

    private void Update()
    {
        // Get mouse position to spawn food on click        
    }
}
