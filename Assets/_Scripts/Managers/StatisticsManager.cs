using Assets._Scripts.Units.Ants;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

internal class StatisticsManager : BaseManager<StatisticsManager>
{
    private GameViewsManager _gameViewManager;
    private Dictionary<StatisticEnum, int> _statisticsDisplayZoneIndexes = new Dictionary<StatisticEnum, int>();

    public void SetGameViewManager(GameViewsManager gameViewManager)
    {
        _gameViewManager = gameViewManager;
    }

    public void InitializeView(List<StatisticEnum> statisticsToDisplay)
    {
        _gameViewManager.Initialyze(statisticsToDisplay.Select(t => t.ToString()).ToList());
        for(int i = 0; i < statisticsToDisplay.Count; i++)
            _statisticsDisplayZoneIndexes.Add(statisticsToDisplay[i], i);
    }

    public void AddValues(Dictionary<StatisticEnum, Vector2> values)
    {
        foreach(var value in values)
            _gameViewManager.AddCurveValue(_statisticsDisplayZoneIndexes[value.Key], value.Value);
    }

    public void UpdateHighScores(Dictionary<StatisticEnum, Vector2> scores)
    {
        var text = new StringBuilder();
        foreach (var pair in scores)
            text.AppendLine($"{pair.Key} : {pair.Value.y} - ({pair.Value.x})");
        _gameViewManager.UpdateHighScores(text.ToString());
    }
}
