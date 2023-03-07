using Assets.Dtos;
using UnityEngine;

internal class EnvironmentManager : BaseManager<EnvironmentManager>
{
    public Vector3 GroundSize = new Vector3(10, 0, 10);
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
        _ground.SetupHexaGrid(GroundSize, NodeRadius);
    }

    public void SpawnFood(float angle)
    {
        var radius = (GroundSize.z / 3f);
        var spawnPosition = Quaternion.Euler(0, angle, 0) * (radius * Vector3.forward);
        var spawned = InstantiateObject(FoodPrefab, spawnPosition, Quaternion.identity, EnvironmentContainer.transform, 3 * NodeRadius);
        spawned.transform.parent = Instance.GetFoodContainer();
    }

    public Block BlockFromWorldPoint(Vector3 worldPosition)
    {
        return _ground.GetBlockFromWorldPosition(worldPosition);
    }

    public Transform GetPheromoneContainer()
    {
        return EnvironmentContainer.transform.GetChild(1);
    }

    public Transform GetFoodContainer()
    {
        return EnvironmentContainer.transform.GetChild(2);
    }

    public Transform GetUnitContainer()
    {
        return EnvironmentContainer.transform.GetChild(0);
    }

    private void Update()
    {
        // Get mouse position to spawn food on click        
    }
}
