using Assets._Scripts.Units.Ants;
using Assets._Scripts.Utilities;
using Assets.Dtos;
using Assets.Gateways;
using mew;
using NeuralNetwork.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AntColony : MonoBehaviour
{
    private NeuralNetworkGateway _neuralNetworkGateway;

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

        _population = new List<BaseAnt>();

        _currentGenerationLifeTime = GlobalParameters.GenerationLifeTime;

        _initialyzed = true;

        StatisticsManager.Instance.InitializeView(new List<StatisticEnum>
        {
            //StatisticEnum.Score,
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
        var brainsToGive = new List<AntBrains>();
        for (int i = 0; i < GlobalParameters.ColonyMaxPopulation; i++)
        {
            brainsToGive.Add(new AntBrains
            {
                MainBrain = mainBrains[i]
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
        var numberToTake = (int)(2 * GlobalParameters.ColonyMaxPopulation / 3f);
        _currentSelection = _currentSelection.Where(t => t.score > 0).OrderByDescending(t => t.score).Take(numberToTake).ToList();

        var index1 = (int)(numberToTake / 3);
        for (int i = 0; i < Mathf.Min(numberToTake, _currentSelection.Count); i++)
        {
            if (i < index1)
                _currentSelection[i].ant.GetBrain().MainBrain.MaxChildNumber = 5;
            else if (i < numberToTake - index1)
                _currentSelection[i].ant.GetBrain().MainBrain.MaxChildNumber = 3;
            else
                _currentSelection[i].ant.GetBrain().MainBrain.MaxChildNumber = 1;
        }
        _bestBrains = _currentSelection;
    }

    private void GetStatistics()
    {
        var interestingStats = _bestBrains.Where(t => t.score > 0);
        StatisticsManager.Instance.GetStatistics(_generationId - 1, _bestBrains.Select(t => t.ant).ToList());
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
        for (int i = foodContainer.childCount; i > 0; i--)
            Destroy(foodContainer.GetChild(i - 1).gameObject);

        if (_generationId % 15 > 10)
        {
            EnvironmentManager.Instance.SpawnFoodPaquet(GlobalParameters.InitialFoodTokenNumber / 2);
            EnvironmentManager.Instance.SpawnFoodPaquet(GlobalParameters.InitialFoodTokenNumber / 2);
        }
        else
        {
            var deltaTheta = 360f / (GlobalParameters.InitialFoodTokenNumber / 10);
            for (int i = 0; i < GlobalParameters.InitialFoodTokenNumber; i++)
                EnvironmentManager.Instance.SpawnFood(i * deltaTheta);
        }
    }

    private void CleanPheromoneContainer()
    {
        _spawner.CleanPheromones();
    }
}
