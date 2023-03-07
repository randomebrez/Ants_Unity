using Assets.Gateways;
using mew;
using NeuralNetwork.Interfaces.Model;
using NeuralNetwork.Managers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AntColony : MonoBehaviour
{
    private NeuralNetworkGateway _neuralNetworkGateway;
    private NetworkCaracteristics _unitBrainCaracteristics;

    private List<(Brain brain, float score)> _bestBrains = new List<(Brain brain, float score)>();

    private int _maxPopulation;
    private List<BaseAnt> _population;
    private int _generationId = 0;

    private SpawnerAnt _spawner;

    private bool _initialyzed = false;

    private bool StartCooldown = false;
    protected float _scanInterval = 60;
    protected float _scanTimer;

    public void Initialyze(string name, NetworkCaracteristics networkCaracteristics, int maxPopulation)
    {
        transform.name = name;

        _maxPopulation = maxPopulation;
        _unitBrainCaracteristics = networkCaracteristics;

        _neuralNetworkGateway = new NeuralNetworkGateway(new PopulationManager(networkCaracteristics));
        _population = new List<BaseAnt>();

        _scanTimer = _scanInterval;

        _initialyzed = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        _spawner = GetComponentInChildren<SpawnerAnt>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_initialyzed == false)
            return;
        if (!StartCooldown)
        {
            GenerateNewGeneration();
            StartCooldown = true;
        }
        else
        {
            _scanTimer -= Time.deltaTime;
            if (_scanTimer < 0)
            {
                _scanTimer += _scanInterval;
                GenerateNewGeneration();
            }
        }
    }

    private void GenerateNewGeneration()
    {
        SelectBestUnits();
        DestroyPreviousGeneration();
        RepopFood();

        // Get as many brain as ants we want to pop
        var brains = _neuralNetworkGateway.GenerateNextGeneration(_maxPopulation, _bestBrains.Select(t => t.brain).ToList());
        _population = _spawner.InstantiateUnits(brains.ToList(), ScriptableAntBase.AntTypeEnum.Worker);
        _generationId++;
    }

    private void SelectBestUnits()
    {
        if (_population.Count == 0)
            return;

        foreach(var ant in _population)
            _bestBrains.Add((ant.BrainManager.GetBrain(), ant.GetUnitScore()));

        _bestBrains = _bestBrains.OrderByDescending(t => t.score).Take(_maxPopulation / 2).ToList();
        var sum = 0f;
        if (_bestBrains.Where(t => t.score > 0).Count() > 1)
        {
            _bestBrains = _bestBrains.Where(t => t.score > 0).ToList();
            foreach (var pair in _bestBrains)
                sum += pair.score;
        }
        var highScore = _bestBrains.First().score;
        sum = sum == 0 ? highScore : sum;
        Debug.Log($"Generation : {_generationId}\nHighest score : {highScore} - Total food grabbed : {sum}");
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
        var deltaTheta = 360f / (foodNumber / 15);
        for (int i = 0; i < foodNumber; i++)
            EnvironmentManager.Instance.SpawnFood(i * deltaTheta);
    }
}
