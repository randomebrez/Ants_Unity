using Assets._Scripts.Utilities;
using Assets.Dtos;
using System.Collections.Generic;
using UnityEngine;

internal class EnvironmentManager : BaseManager<EnvironmentManager>
{
    public Ground GroundPrefab;
    public FoodToken FoodPrefab;
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
        // Positions of ants
        var unitPositions = UnitManager.Instance.GetUnitPositions();

        foreach(var pair in unitPositions)
        {
            for(int i = 0; i < pair.Value.Count; i++)
                _ground.AddOrCreatePheromoneOnBlock(pair.Key, pair.Value[i]);
        }
    }

    public void ApplyTimeEffect()
    {
        _ground.ApplyTimeEffect();
    }


    public void SpawnFood()
    {
        var deltaTheta = 360f / 6;
        for (int i = 0; i < GlobalParameters.InitialFoodTokenNumber; i++)
        {
            //Maybe take random value with step NodeRadius or apothem
            var radius = (GlobalParameters.GroundSize.z / 3f);
            var randomXShift = Random.Range(-2, 3);
            var randomZShift = Random.Range(-2, 3);

            var spawnPosition = Quaternion.Euler(0, i * deltaTheta, 0) * ((radius + randomZShift) * Vector3.forward + randomXShift * Vector3.right) + GlobalParameters.NodeRadius * Vector3.up;
            var blockPosition = GroundBlockFromWorldPoint(spawnPosition);

            if (blockPosition == null)
                i--;
            _ground.AddOrCreateFoodTookenOnBlock(FoodPrefab, blockPosition.Block);
        }
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
            var blockPosition = GroundBlockFromWorldPoint(spawnPosition);

            _ground.AddOrCreateFoodTookenOnBlock(FoodPrefab, blockPosition.Block);
        }
    }

    public GroundBlock GroundBlockFromWorldPoint(Vector3 worldPosition)
    {
        return _ground.GroundBlockFromWorldPosition(worldPosition);
    }

    public GroundBlock GroundBlockFromBlock(Block block)
    {
        return _ground.GroundBlockFromBlock(block);
    }

    public List<GroundBlock> GroundBlockWithinCircle(Vector3 center, float radius)
    {
        return _ground.GroundBlockWithinCircle(center, radius);
    }


    public void RenewEnvironment()
    {
        _ground.CleanAllPheromones();
        _ground.CleanAllFoodToken();

        SpawnFood();
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
