using Assets._Scripts.Units.Ants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class StatisticsManager : BaseManager<StatisticsManager>
{
    private GameViewsManager _gameViewManager;
    private Dictionary<StatisticEnum, int> _statisticsDisplayZoneIndexes = new Dictionary<StatisticEnum, int>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetGameViewManager(GameViewsManager gameViewManager)
    {
        _gameViewManager = gameViewManager;
    }

    public void InitializeView(List<StatisticEnum> statisticsToDisplay)
    {
        _gameViewManager.Initialyze(statisticsToDisplay.Count);
        for(int i = 0; i < statisticsToDisplay.Count; i++)
            _statisticsDisplayZoneIndexes.Add(statisticsToDisplay[i], i);
    }

    public void AddValues(Dictionary<StatisticEnum, (float x, float y)> values)
    {
        foreach(var value in values)
            _gameViewManager.AddCurveValue(_statisticsDisplayZoneIndexes[value.Key], value.Value.x, value.Value.y);
    }
}
