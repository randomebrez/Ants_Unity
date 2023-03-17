using Assets._Scripts.Units.Ants;
using Assets._Scripts.Utilities;
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

    private SpawnerAnt _spawner;
    private BlockBase _block;
    private List<BaseAnt> _population;
    private List<(BaseAnt ant, Brain brain, float score)> _currentSelection = new List<(BaseAnt ant, Brain brain, float score)>();
    private List<(BaseAnt ant, Brain brain, float score)> _bestBrains = new List<(BaseAnt ant, Brain brain, float score)>();

    private int _generationId = 0;
    private bool _startCooldown = false;
    private float _currentGenerationLifeTime;
    private bool _initialyzed = false;

    private Dictionary<StatisticEnum, Vector2> _currentHighScore = new Dictionary<StatisticEnum, Vector2>();
    private Dictionary<StatisticEnum, Vector2> _globalHighScore = new Dictionary<StatisticEnum, Vector2>();

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

        _population = new List<BaseAnt>();

        _currentGenerationLifeTime = GlobalParameters.GenerationLifeTime;

        _initialyzed = true;

        StatisticsManager.Instance.InitializeView(new List<StatisticEnum>
        {
            StatisticEnum.Score,
            StatisticEnum.BestFoodReach,
            StatisticEnum.BestComeBack,
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
            DestroyPreviousGeneration();
            RepopFood();
            CleanPheromoneContainer();
        }

        // Get as many brain as ants we want to pop
        var brains = _neuralNetworkGateway.GenerateNextGeneration(GlobalParameters.ColonyMaxPopulation, _bestBrains.Select(t => t.brain).ToList());
        _population = _spawner.InstantiateUnits(brains.ToList(), ScriptableAntBase.AntTypeEnum.Worker);
        _generationId++;
    }

    private void SelectBestUnits()
    {
        if (_population.Count == 0)
            return;

        _currentSelection.Clear();
        foreach (var ant in _population)
            _currentSelection.Add((ant, ant.BrainManager.GetBrain(), ant.GetUnitScore()));

        _bestBrains = _currentSelection.OrderByDescending(t => t.score).Take((int)(2 * GlobalParameters.ColonyMaxPopulation / 3f)).ToList();        
    }

    private void GetStatistics()
    {
        var sumFoodCollected = 0f;
        var sumFoodGrabbed = 0f;
        var bestFoodReach = Mathf.Infinity;
        var bestComeBack = Mathf.Infinity;
        var highScore = _bestBrains.First();
        foreach (var pair in _bestBrains.Where(t => t.score > 0))
        {
            var antStatistics = pair.ant.GetStatistics();

            if (antStatistics[StatisticEnum.BestComeBack] < bestComeBack)
                bestComeBack = antStatistics[StatisticEnum.BestComeBack];
            if (antStatistics[StatisticEnum.BestFoodReach] < bestFoodReach)
                bestFoodReach = antStatistics[StatisticEnum.BestFoodReach];

            sumFoodCollected += antStatistics[StatisticEnum.FoodCollected];
            sumFoodGrabbed += antStatistics[StatisticEnum.FoodGrabbed];
        }

        var xPoint = (_generationId - 1 ) * Vector2.right;

        _currentHighScore = new Dictionary<StatisticEnum, Vector2>
        {
            { StatisticEnum.Score, xPoint + (float)Math.Round(highScore.score, 2) * Vector2.up },
            { StatisticEnum.BestFoodReach, xPoint + (float)Math.Round(bestFoodReach, 2) * Vector2.up },
            { StatisticEnum.BestComeBack, xPoint + (float)Math.Round(bestComeBack, 2) * Vector2.up },
            { StatisticEnum.FoodCollected, xPoint + sumFoodCollected * Vector2.up },
            { StatisticEnum.FoodGrabbed, xPoint + sumFoodGrabbed * Vector2.up }
        };
        if (_globalHighScore.Count == 0)
        {
            _globalHighScore = new Dictionary<StatisticEnum, Vector2>
            {
                { StatisticEnum.Score, xPoint + (float)Math.Round(highScore.score, 2) * Vector2.up },
                { StatisticEnum.BestFoodReach, xPoint + (float)Math.Round(bestFoodReach, 2) * Vector2.up },
                { StatisticEnum.BestComeBack, xPoint + (float)Math.Round(bestComeBack, 2) * Vector2.up },
                { StatisticEnum.FoodCollected, xPoint + sumFoodCollected * Vector2.up },
                { StatisticEnum.FoodGrabbed, xPoint + sumFoodGrabbed * Vector2.up }
            };
        }

        if (_currentHighScore[StatisticEnum.Score].y > _globalHighScore[StatisticEnum.Score].y)
            _globalHighScore[StatisticEnum.Score] = _currentHighScore[StatisticEnum.Score];

        if (_currentHighScore[StatisticEnum.BestFoodReach].y < _globalHighScore[StatisticEnum.BestFoodReach].y)
            _globalHighScore[StatisticEnum.BestFoodReach] = _currentHighScore[StatisticEnum.BestFoodReach];

        if (_currentHighScore[StatisticEnum.BestComeBack].y < _globalHighScore[StatisticEnum.BestComeBack].y)
            _globalHighScore[StatisticEnum.BestComeBack] = _currentHighScore[StatisticEnum.BestComeBack];

        if (_currentHighScore[StatisticEnum.FoodCollected].y > _globalHighScore[StatisticEnum.FoodCollected].y)
            _globalHighScore[StatisticEnum.FoodCollected] = _currentHighScore[StatisticEnum.FoodCollected];

        if (_currentHighScore[StatisticEnum.FoodGrabbed].y > _globalHighScore[StatisticEnum.FoodGrabbed].y)
            _globalHighScore[StatisticEnum.FoodGrabbed] = _currentHighScore[StatisticEnum.FoodGrabbed];

        bestFoodReach = bestFoodReach < Mathf.Infinity ? bestFoodReach : 0;
        bestComeBack = bestComeBack < Mathf.Infinity ? bestComeBack : 0;

        StatisticsManager.Instance.AddValues(_currentHighScore);
        StatisticsManager.Instance.UpdateHighScores(_globalHighScore);

        Debug.Log($"Generation : {_generationId}\nHighest score : {highScore.score} - Food Grabbed : {sumFoodGrabbed} - Food Gathered : {sumFoodCollected}");
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
        if (foodContainer.childCount <= 0)
            return;

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
