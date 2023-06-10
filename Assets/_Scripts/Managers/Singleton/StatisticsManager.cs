using Assets._Scripts.Gateways;
using Assets._Scripts.Utilities;
using Assets.Abstractions;
using Assets.Dtos;
using mew;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class StatisticsManager : BaseManager<StatisticsManager>
{
    private GameViewsManager _gameViewManager;
    private IStorage _dbGateway;
    private Dictionary<UnitStatististicsEnum, int> _statisticsDisplayZoneIndexes;
    private Dictionary<UnitStatististicsEnum, Vector2> _globalHighScore = new Dictionary<UnitStatististicsEnum, Vector2>();

    public void SetGameViewManager(GameViewsManager gameViewManager)
    {
        _gameViewManager = gameViewManager;
    }

    public void InitializeView(List<UnitStatististicsEnum> statisticsToDisplay)
    {
        _dbGateway = new FileStorageGateway(GlobalParameters.SqlFolderPath);
        _gameViewManager.Initialyze(statisticsToDisplay.Select(t => t.ToString()).ToList());

        _statisticsDisplayZoneIndexes = new Dictionary<UnitStatististicsEnum, int>();
        for (int i = 0; i < statisticsToDisplay.Count; i++)
            _statisticsDisplayZoneIndexes.Add(statisticsToDisplay[i], i);
    }

    public async Task GetStatisticsAsync(int generationId, List<BaseAnt> ants)
    {
        var sumFoodCollected = 0f;
        var sumFoodGrabbed = 0f;
        var comeBackMean = 0f;
        var ageMean = 0f;
        var bestScore = Mathf.NegativeInfinity;
        var count2 = 0f;
        foreach (var ant in ants)
        {
            var antStatistics = ant.GetStatistics();

            if (antStatistics[UnitStatististicsEnum.Score] > bestScore)
                bestScore = antStatistics[UnitStatististicsEnum.Score];

            if (antStatistics[UnitStatististicsEnum.ComeBackMean] < int.MaxValue)
            {
                comeBackMean += 1f / antStatistics[UnitStatististicsEnum.ComeBackMean];
                count2++;
            }
            sumFoodCollected += antStatistics[UnitStatististicsEnum.FoodCollected];
            sumFoodGrabbed += antStatistics[UnitStatististicsEnum.FoodGrabbed];
            ageMean += antStatistics[UnitStatististicsEnum.Age];
        }

        comeBackMean = count2 > 0 ? comeBackMean / count2 : 0;
        ageMean /= ants.Count;

        var xPoint = generationId * Vector2.right;
        var currentHighScore = new Dictionary<UnitStatististicsEnum, Vector2>
        {
            { UnitStatististicsEnum.Score, xPoint + (float)Math.Round(bestScore, 2) * Vector2.up },            
            { UnitStatististicsEnum.ComeBackMean, xPoint + (float)Math.Round(comeBackMean, 2) * Vector2.up },
            { UnitStatististicsEnum.FoodGrabbed, xPoint + sumFoodGrabbed * Vector2.up },
            { UnitStatististicsEnum.FoodCollected, xPoint + sumFoodCollected * Vector2.up },
            { UnitStatististicsEnum.Age, xPoint + ageMean * Vector2.up }
        };

        if (_globalHighScore.Count == 0)
        {
            _globalHighScore = new Dictionary<UnitStatististicsEnum, Vector2>
            {
                { UnitStatististicsEnum.Score, xPoint + (float)Math.Round(bestScore, 2) * Vector2.up },                
                { UnitStatististicsEnum.FoodGrabbed, xPoint + sumFoodGrabbed * Vector2.up },
                { UnitStatististicsEnum.FoodCollected, xPoint + sumFoodCollected * Vector2.up }
            };
        }
        else
        {
            if (bestScore > _globalHighScore[UnitStatististicsEnum.Score].y)
                _globalHighScore[UnitStatististicsEnum.Score] = new Vector2(generationId, bestScore);

            if (sumFoodCollected > _globalHighScore[UnitStatististicsEnum.FoodCollected].y)
                _globalHighScore[UnitStatististicsEnum.FoodCollected] = new Vector2(generationId, sumFoodCollected);

            if (sumFoodGrabbed > _globalHighScore[UnitStatististicsEnum.FoodGrabbed].y)
                _globalHighScore[UnitStatististicsEnum.FoodGrabbed] = new Vector2(generationId, sumFoodGrabbed);
        }

       

        AddStatisticsToCurve(currentHighScore);
        UpdateStatisticsText(currentHighScore, _globalHighScore);

        //if (generationId % GlobalParameters.StoreFrequency == 0)
        //    await SaveWinnersAsync(generationId, ants, sumFoodCollected).ConfigureAwait(false);
        //else if (sumFoodCollected >= 99)
        //    await SaveWinnersAsync(generationId, ants, sumFoodCollected).ConfigureAwait(false);

        Debug.Log($"Generation : {generationId}\nHighest score : {bestScore} - Food Grabbed : {sumFoodGrabbed} - Food Gathered : {sumFoodCollected}");
    }

    public void AddStatisticsToCurve(Dictionary<UnitStatististicsEnum, Vector2> values)
    {
        foreach(var value in values)
            if (_statisticsDisplayZoneIndexes.ContainsKey(value.Key))
                _gameViewManager.AddCurveValue(_statisticsDisplayZoneIndexes[value.Key], value.Value);
    }

    public void UpdateStatisticsText(Dictionary<UnitStatististicsEnum, Vector2> currentGenerationScore, Dictionary<UnitStatististicsEnum, Vector2> globalHighScores)
    {
        var text = new StringBuilder("HighScore :\n");
        foreach (var pair in globalHighScores)
            text.AppendLine($"{pair.Key} : {pair.Value.y} - ({pair.Value.x})");
        text.AppendLine("\nCurrent score :");
        foreach (var pair in currentGenerationScore)
            text.AppendLine($"{pair.Key} : {pair.Value.y}");
        _gameViewManager.UpdateHighScores(text.ToString());
    }
}
