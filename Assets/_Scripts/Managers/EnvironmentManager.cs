using Assets._Scripts.Utilities;
using Assets.Dtos;
using UnityEngine;

internal class EnvironmentManager : BaseManager<EnvironmentManager>
{
    public Ground GroundPrefab;
    public GameObject FoodPrefab;
    public GameObject PheromonePrefab;
    public Transform EnvironmentContainer;

    private Ground _ground;

    public void SpawnGround()
    {
        if (_ground != null)
            return;

        _ground = Instantiate(GroundPrefab, EnvironmentContainer.position, Quaternion.identity, EnvironmentContainer);
        _ground.SetupHexaGrid();
    }

    public void DropPheromones()
    {
        // Positions of ants given by unitmanager
        var unitPositions = UnitManager.Instance.GetUnitPositions();

        foreach(var pair in unitPositions)
        {
            for(int i = 0; i < pair.Value.Count; i++)
            {
                // Spawn pheromone
                var scriptablePheromone = ResourceSystem.Instance.PheromoneOfTypeGet(pair.Key);
                var newPheromone = Instantiate(scriptablePheromone.PheromonePrefab, GetPheromoneContainer());
                newPheromone.Initialyze(scriptablePheromone.BaseCaracteristics, pair.Value[i]);

                //Add pheromone on block
                _ground.AddorCreatePheromoneOnBlock(newPheromone, pair.Value[i]);
            }
        }
    }

    public void ApplyTimeEffect()
    {
        _ground.ApplyTimeEffect();
    }

    public void CleanAllPheromones()
    {
        _ground.CleanAllPheromones();
    }

    public void SpawnFood(float angle)
    {
        var radius = (GlobalParameters.GroundSize.z / 3f);
        var randomXShift = Random.Range(-2, 3);
        var randomZShift = Random.Range(-2, 3);
        var spawnPosition = Quaternion.Euler(0, angle, 0) * ((radius + randomZShift) * Vector3.forward + randomXShift * Vector3.right) + GlobalParameters.NodeRadius * Vector3.up;
        var foodContainer = Instance.GetFoodContainer();
        var spawned = InstantiateObject(FoodPrefab, spawnPosition, Quaternion.identity, foodContainer, 3 * GlobalParameters.NodeRadius);
    }

    public void SpawnFoodPaquet(int tokenNumber)
    {
        var randomXpos = Random.Range(GlobalParameters.GroundSize.x / 6f, GlobalParameters.GroundSize.x /3f);
        if (Random.Range(0, 2) == 1)
            randomXpos = -randomXpos;
        var randomZpos = Random.Range(GlobalParameters.GroundSize.z / 6f, GlobalParameters.GroundSize.z / 3f);
        if (Random.Range(0, 2) == 1)
            randomZpos = -randomZpos;
        for (int i = 0; i < tokenNumber; i++)
        {
            var randomXShift = Random.Range(-GlobalParameters.GroundSize.x / 7f, GlobalParameters.GroundSize.x / 7f);
            var randomZShift = Random.Range(-GlobalParameters.GroundSize.z / 7f, GlobalParameters.GroundSize.z / 7f);
            var spawnPosition = (randomZpos + randomZShift) * Vector3.forward + (randomXpos + randomXShift) * Vector3.right + GlobalParameters.NodeRadius * Vector3.up;
            var foodContainer = Instance.GetFoodContainer();
            var spawned = InstantiateObject(FoodPrefab, spawnPosition, Quaternion.identity, foodContainer, 3 * GlobalParameters.NodeRadius);
        }
    }

    public Block BlockFromWorldPoint(Vector3 worldPosition)
    {
        return _ground.GetBlockFromWorldPosition(worldPosition);
    }

    public Transform GetPheromoneContainer()
    {
        return EnvironmentContainer.GetChild(1);
    }

    public Transform GetFoodContainer()
    {
        return EnvironmentContainer.GetChild(1);
    }

    public Transform GetUnitContainer()
    {
        return EnvironmentContainer.GetChild(0);
    }
}
