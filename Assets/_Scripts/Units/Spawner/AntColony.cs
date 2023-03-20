using Assets._Scripts.Units.Ants;
using Assets._Scripts.Utilities;
using Assets.Dtos;
using Assets.Gateways;
using mew;
using NeuralNetwork.Interfaces.Model;
using NeuralNetwork.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AntColony : MonoBehaviour
{
    private NeuralNetworkGateway _neuralNetworkGateway;
    private NeuralNetworkGateway _scannerNetworkGateway;

    private SpawnerAnt _spawner;
    private BlockBase _block;
    private List<BaseAnt> _population;
    private List<(BaseAnt ant, float score)> _currentSelection = new List<(BaseAnt ant, float score)>();
    private List<(BaseAnt ant, float score)> _bestBrains = new List<(BaseAnt ant, float score)>();

    private int _generationId = 0;
    private bool _startCooldown = false;
    private float _currentGenerationLifeTime;
    private bool _initialyzed = false;

    // Unity methods
    void Start()
    {
        _spawner = GetComponentInChildren<SpawnerAnt>();
        _block = GetComponentInChildren<BlockBase>();

        _block.transform.localScale =  GlobalParameters.NodeRadius * (2 * Vector3.one - Vector3.up);
    }

    void Update()
    {
        if (_initialyzed == false)
            return;
        if (!_startCooldown)
        {
            GenerateNewGeneration();
            _startCooldown = true;
        }
        else
        {
            _currentGenerationLifeTime -= Time.deltaTime;
            if (_currentGenerationLifeTime < 0)
            {
                _currentGenerationLifeTime = GlobalParameters.GenerationLifeTime;
                GenerateNewGeneration();
            }
        }
    }


    // Public methods
    public void Initialyze(string name)
    {
        transform.name = name;

        _neuralNetworkGateway = new NeuralNetworkGateway(new PopulationManager(GlobalParameters.NetworkCaracteristics));
        _scannerNetworkGateway = new NeuralNetworkGateway(new PopulationManager(GlobalParameters.PortionNetworkCaracteristics));

        _population = new List<BaseAnt>();

        _currentGenerationLifeTime = GlobalParameters.GenerationLifeTime;

        _initialyzed = true;

        StatisticsManager.Instance.InitializeView(new List<StatisticEnum>
        {
            StatisticEnum.Score,
            //StatisticEnum.BestFoodReachStepNumber,
            StatisticEnum.ComeBackMean,
            StatisticEnum.FoodCollected,
            StatisticEnum.FoodGrabbed
        });
    }


    // Private methods
    private void GenerateNewGeneration()
    {
        AntSceneManager.Instance.SetNewGenerationId(_generationId);

        if (_population.Count != 0)
        {
            SelectBestUnits();
            GetStatistics();
            CleanPheromoneContainer();
            RepopFood();
            DestroyPreviousGeneration();
            
        }

        // Get as many brain as ants we want to pop
        var brainsToGive = GenerateBrainsAndUnits();
        _population = _spawner.InstantiateUnits(brainsToGive, ScriptableAntBase.AntTypeEnum.Worker);
        _generationId++;
    }

    private List<AntBrains> GenerateBrainsAndUnits()
    {
        var antBrains = _bestBrains.Select(t => t.ant.GetBrain());
        var mainBrains = _neuralNetworkGateway.GenerateNextGeneration(GlobalParameters.ColonyMaxPopulation, antBrains.Select(t => t.MainBrain).ToList());
        var portionBrains = new Dictionary<int, List<Brain>>();
        for (int i = 0; i < GlobalParameters.ScannerSubdivision; i++)
        {
            var bestPortionBrains = antBrains.Select(t => t.ScannerBrains[i]).ToList();
            portionBrains.Add(i, _scannerNetworkGateway.GenerateNextGeneration(GlobalParameters.ColonyMaxPopulation, bestPortionBrains).ToList());
        }
        var brainsToGive = new List<AntBrains>();
        for (int i = 0; i < GlobalParameters.ColonyMaxPopulation; i++)
        {
            var scannerBrains = new Dictionary<int, Brain>();
            for (int j = 0; j < GlobalParameters.ScannerSubdivision; j++)
                scannerBrains.Add(j, portionBrains[j][i]);

            brainsToGive.Add(new AntBrains
            {
                MainBrain = mainBrains[i],
                ScannerBrains = scannerBrains
            });
        }
        return brainsToGive;
    }

    private void SelectBestUnits()
    {
        if (_population.Count == 0)
            return;

        _currentSelection.Clear();
        foreach (var ant in _population)
            _currentSelection.Add((ant, ant.GetUnitScore()));

        _bestBrains = _currentSelection.OrderByDescending(t => t.score).Take((int)(2 * GlobalParameters.ColonyMaxPopulation / 3f)).ToList();        
    }

    private void GetStatistics()
    {
        var interestingStats = _bestBrains.Where(t => t.score > 0);
        StatisticsManager.Instance.GetStatistics(_generationId, _bestBrains.Select(t => t.ant).ToList());
    }

    private void DestroyPreviousGeneration()
    {
        foreach (var ant in _population)
            Destroy(ant.gameObject);

        _population.Clear();
    }

    private void RepopFood()
    {
        var foodContainer = EnvironmentManager.Instance.GetFoodContainer();

        for (int i = foodContainer.childCount; i > 0; i --)
        {
            Destroy(foodContainer.GetChild(i - 1).gameObject);
        }
        var foodNumber = 100;
        var deltaTheta = 360f / (foodNumber / 10);
        for (int i = 0; i < foodNumber; i++)
            EnvironmentManager.Instance.SpawnFood(i * deltaTheta);
    }

    private void CleanPheromoneContainer()
    {
        _spawner.CleanPheromones();
    }
}
