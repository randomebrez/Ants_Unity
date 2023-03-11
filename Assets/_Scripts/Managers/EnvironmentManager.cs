using Assets._Scripts.Utilities;
using Assets.Dtos;
using UnityEngine;

internal class EnvironmentManager : BaseManager<EnvironmentManager>
{
    public GameObject GroundPrefab;
    public GameObject EnvironmentContainer;
    public GameObject FoodPrefab;

    private Ground _ground;

    public void SpawnGround()
    {
        if (_ground != null)
            return;

        _ground = Instantiate(GroundPrefab, EnvironmentContainer.transform).GetComponent<Ground>();
        _ground.SetupHexaGrid();
    }

    public void SpawnFood(float angle)
    {
        var radius = (GlobalParameters.GroundSize.z / 3f);
        var randomXShift = Random.Range(-2, 3);
        var randomZShift = Random.Range(-2, 3);
        var spawnPosition = Quaternion.Euler(0, angle, 0) * ((radius + randomZShift) * Vector3.forward + randomXShift * Vector3.right) + (GlobalParameters.NodeRadius / 2) * Vector3.up;
        var spawned = InstantiateObject(FoodPrefab, spawnPosition, Quaternion.identity, EnvironmentContainer.transform, 3 * GlobalParameters.NodeRadius);
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
        return EnvironmentContainer.transform.GetChild(1);
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
