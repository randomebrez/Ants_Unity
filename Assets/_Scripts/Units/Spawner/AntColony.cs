using Assets._Scripts.Units.Ants;
using Assets._Scripts.Utilities;
using Assets.Dtos;
using Assets.Gateways;
using mew;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static mew.ScriptablePheromoneBase;

public class AntColony : MonoBehaviour
{
    private NeuralNetworkGateway _neuralNetworkGateway;

    private SpawnerAnt _spawner;
    private BlockBase _block;
    private List<BaseAnt> _population;
    private List<(BaseAnt ant, float score)> _currentSelection = new List<(BaseAnt ant, float score)>();
    private List<(BaseAnt ant, float score)> _bestBrains = new List<(BaseAnt ant, float score)>();

    private int _generationId = 0;
    private bool _initialyzed = false;
    private int _numberMaxToSelect = 0;

    // Unity methods
    void Start()
    {
        _spawner = GetComponentInChildren<SpawnerAnt>();
        _block = GetComponentInChildren<BlockBase>();

        _block.transform.localScale =  GlobalParameters.NodeRadius * (2 * Vector3.one - Vector3.up);
        _numberMaxToSelect = (int)(GlobalParameters.PercentToSelectAmongstBest * 2 * GlobalParameters.ColonyMaxPopulation);
    }

    public void Update()
    {
        if (_generationId == 0 && _initialyzed && _population.Count <= 0)
            GenerateNewGeneration(GlobalParameters.FirstBrainsFilePath);
    }


    // Public methods
    public void Initialyze(string name)
    {
        transform.name = name;

        _neuralNetworkGateway = new NeuralNetworkGateway();
        _population = new List<BaseAnt>();

        StatisticsManager.Instance.InitializeViewAsync(new List<StatisticEnum>
        {
            //StatisticEnum.Score,
            StatisticEnum.Age,
            StatisticEnum.ComeBackMean,
            StatisticEnum.FoodCollected,
            StatisticEnum.FoodGrabbed
        }).GetAwaiter().GetResult();

        _initialyzed = true;
    }

    public void MoveAllAnts()
    {
        if (_initialyzed == false)
            return;

        for (int i = 0; i < _population.Count; i++)
            _population[i].Move();
    }

    public void RenewPopulation()
    {
        SelectBestUnits();
        GetStatistics();
        DestroyPreviousGeneration();

        GenerateNewGeneration();
    }

    public Dictionary<PheromoneTypeEnum, List<Block>> GetAllAntPositions()
    {
        if (_initialyzed == false)
            return new Dictionary<PheromoneTypeEnum, List<Block>>();
        var result = new Dictionary<PheromoneTypeEnum, List<Block>>();
        result.Add(PheromoneTypeEnum.Wander, new List<Block>());
        result.Add(PheromoneTypeEnum.CarryFood, new List<Block>());

        for (int i = 0; i < _population.Count; i++)
        {
            var ant = _population[i];
            result[ant.GetPheroType()].Add(ant.CurrentPos);
        }

        return result;
    }


    // Private methods
    private void GenerateNewGeneration(string filePath = null)
    {
        AntSceneManager.Instance.SetNewGenerationId(_generationId);

        List<AntBrains> brainsToGive;
        // Get as many brain as ants we want to pop
        if (string.IsNullOrEmpty(filePath) == false)
            brainsToGive = GetBrainsFromFile(filePath);
        else
            brainsToGive = GenerateBrainsAndUnits();

        _population = _spawner.InstantiateUnits(brainsToGive, ScriptableAntBase.AntTypeEnum.Worker);
        _generationId++;
    }

    private List<AntBrains> GenerateBrainsAndUnits()
    {
        var antBrains = _bestBrains.Select(t => t.ant.GetBrain());
        var mainBrains = _neuralNetworkGateway.GenerateNextGeneration(GlobalParameters.ColonyMaxPopulation, antBrains.Select(t => t.MainBrain).ToList());
        var childBrains = mainBrains.Where(t => t.ParentA != new System.Guid());
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

    private List<AntBrains> GetBrainsFromFile(string fileName)
    {
        var lines = File.ReadAllLines(fileName);
        var brains = _neuralNetworkGateway.GetBrainsFromString(lines.ToList());
        var brainNumberToGenerate = GlobalParameters.ColonyMaxPopulation - brains.Count;
        var otherBrains = _neuralNetworkGateway.GenerateNextGeneration(brainNumberToGenerate, brains);
        brains.AddRange(otherBrains);

        var result = new List<AntBrains>();
        for (int i = 0; i < GlobalParameters.ColonyMaxPopulation; i++)
        {
            result.Add(new AntBrains
            {
                MainBrain = brains[i]
            });
        }

        return result;
    }

    private void SelectBestUnits()
    {
        if (_population.Count == 0)
            return;

        _currentSelection.Clear();
        // Get all scores
        foreach (var ant in _population)
            _currentSelection.Add((ant, ant.GetUnitScore()));

        // Filter best ones
        _currentSelection = _currentSelection.Where(t => t.score > 0).OrderByDescending(t => t.score).Take(_numberMaxToSelect).ToList();

        var selectedNumber = Mathf.Min(_numberMaxToSelect, _currentSelection.Count);
        var index1 = selectedNumber / 3;
        for (int i = 0; i < selectedNumber; i++)
        {
            if (i < index1)
                _currentSelection[i].ant.GetBrain().MainBrain.MaxChildNumber = GlobalParameters.MeanChildNumberByBrains + 1;
            else if (i < _numberMaxToSelect - index1)
                _currentSelection[i].ant.GetBrain().MainBrain.MaxChildNumber = GlobalParameters.MeanChildNumberByBrains;
            else
                _currentSelection[i].ant.GetBrain().MainBrain.MaxChildNumber = GlobalParameters.MeanChildNumberByBrains - 1;
        }
        _bestBrains = _currentSelection;
    }

    private void GetStatistics()
    {
        StatisticsManager.Instance.GetStatisticsAsync(_generationId - 1, _population).GetAwaiter().GetResult();
    }

    private void DestroyPreviousGeneration()
    {
        foreach (var ant in _population)
            Destroy(ant.gameObject);

        _population.Clear();
    }
}
