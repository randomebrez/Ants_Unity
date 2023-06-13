using Assets._Scripts.Utilities;
using Assets.Dtos;
using mew;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static mew.ScriptablePheromoneBase;

public class AntColony : MonoBehaviour
{
    private SpawnerAnt _spawner;
    private BlockBase _block;
    private List<BaseAnt> _population;
    private List<(BaseAnt ant, float score)> _currentSelection = new List<(BaseAnt ant, float score)>();
    private List<(BaseAnt ant, float score)> _bestBrains = new List<(BaseAnt ant, float score)>();

    private int _generationId = 0;
    private bool _initialyzed = false;


    // Public methods
    public void Initialyze(string name)
    {
        _population = new List<BaseAnt>();
        _spawner = GetComponentInChildren<SpawnerAnt>();
        _block = GetComponentInChildren<BlockBase>();
        _block.transform.localScale = GlobalParameters.NodeRadius * (2 * Vector3.one - Vector3.up);

        transform.name = name;

        var blocksUnderneath = EnvironmentManager.Instance.GroundBlockWithinCircle(transform.position, 2 * GlobalParameters.NodeRadius);
        foreach (var block in blocksUnderneath)
            block.IsUnderNest = true;

        StatisticsManager.Instance.InitializeView(new List<UnitStatististicsEnum>
        {
            //StatisticEnum.Score,
            UnitStatististicsEnum.Age,
            UnitStatististicsEnum.ComeBackMean,
            UnitStatististicsEnum.FoodCollected,
            UnitStatististicsEnum.FoodGrabbed
        });

        _initialyzed = true;
    }

    public void MoveAllAnts()
    {
        if (_initialyzed == false)
            return;

        for (int i = 0; i < _population.Count; i++)
            _population[i].Move();
    }

    public int GetBestBrainCount()
    {
        return _bestBrains.Count;
    }

    public Dictionary<PheromoneTypeEnum, List<Block>> GetAllAntPositions()
    {
        if (_initialyzed == false)
            return new Dictionary<PheromoneTypeEnum, List<Block>>();

        var result = new Dictionary<PheromoneTypeEnum, List<Block>>
        {
            { PheromoneTypeEnum.Wander, new List<Block>() },
            { PheromoneTypeEnum.CarryFood, new List<Block>() }
        };

        for (int i = 0; i < _population.Count; i++)
        {
            var ant = _population[i];
            result[ant.GetPheroType()].Add(ant.CurrentPos.Block);
        }

        return result;
    }

    public void InstantiateUnits(UnitWrapper[] units)
    {
        _population = _spawner.InstantiateUnits(units, ScriptableAntBase.AntTypeEnum.Worker);
        _generationId++;
        AntSceneManager.Instance.SetNewGenerationId(_generationId);
    }

    public List<BaseAnt> SelectBestUnits()
    {
        if (_population.Count == 0)
            return new List<BaseAnt>();

        _currentSelection.Clear();
        // Get all scores
        foreach (var ant in _population)
            _currentSelection.Add((ant, ant.GetUnitScore()));

        // Filter best ones
        _currentSelection = _currentSelection.Where(t => t.score > 0).OrderByDescending(t => t.score).Take(GlobalParameters.UnitNumberToSelect).ToList();

        var selectedNumber = Mathf.Min(GlobalParameters.UnitNumberToSelect, _currentSelection.Count);
        var index1 = selectedNumber / 3;
        var meanChildnumber = GlobalParameters.ReproductionCaracteristics.MeanChildNumberByUnit;
        for (int i = 0; i < selectedNumber; i++)
        {
            if (i < index1)
                _currentSelection[i].ant.GetNugetUnit.MaxChildNumber = meanChildnumber + meanChildnumber / 2;
            else if (i < GlobalParameters.UnitNumberToSelect - index1)
                _currentSelection[i].ant.GetNugetUnit.MaxChildNumber = meanChildnumber;
            else
                _currentSelection[i].ant.GetNugetUnit.MaxChildNumber = meanChildnumber - meanChildnumber / 2;
        }
        _bestBrains = _currentSelection;
        return _bestBrains.Select(t => t.ant).ToList();
    }

    public void SetStatistics()
    {
        StatisticsManager.Instance.GetStatisticsAsync(_generationId - 1, _population).GetAwaiter().GetResult();
    }

    public void DestroyAllUnits()
    {
        foreach (var ant in _population)
            Destroy(ant.gameObject);

        _population.Clear();
    }
}
