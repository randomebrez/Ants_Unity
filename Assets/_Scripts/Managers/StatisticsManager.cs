using Assets._Scripts.Gateways;
using Assets._Scripts.Units.Ants;
using Assets._Scripts.Utilities;
using Assets.Abstractions;
using mew;
using NeuralNetwork.Interfaces.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class StatisticsManager : BaseManager<StatisticsManager>
{
    private GameViewsManager _gameViewManager;
    private IStorage _dbGateway;
    private Dictionary<StatisticEnum, int> _statisticsDisplayZoneIndexes = new Dictionary<StatisticEnum, int>();
    private Dictionary<StatisticEnum, Vector2> _globalHighScore = new Dictionary<StatisticEnum, Vector2>();

    public void SetGameViewManager(GameViewsManager gameViewManager)
    {
        _gameViewManager = gameViewManager;
    }

    public async Task InitializeViewAsync(List<StatisticEnum> statisticsToDisplay)
    {
        _dbGateway = new FileStorageGateway(GlobalParameters.SqlFolderPath);
        _gameViewManager.Initialyze(statisticsToDisplay.Select(t => t.ToString()).ToList());
        for(int i = 0; i < statisticsToDisplay.Count; i++)
            _statisticsDisplayZoneIndexes.Add(statisticsToDisplay[i], i);
    }

    public async Task GetStatisticsAsync(int generationId, List<BaseAnt> ants)
    {
        var sumFoodCollected = 0f;
        var sumFoodGrabbed = 0f;
        var comeBackMean = 0f;
        var bestScore = Mathf.NegativeInfinity;
        var count2 = 0f;
        foreach (var ant in ants)
        {
            var antStatistics = ant.GetStatistics();

            if (antStatistics[StatisticEnum.Score] > bestScore)
                bestScore = antStatistics[StatisticEnum.Score];

            if (antStatistics[StatisticEnum.ComeBackMean] < int.MaxValue)
            {
                comeBackMean += antStatistics[StatisticEnum.ComeBackMean];
                count2++;
            }
            sumFoodCollected += antStatistics[StatisticEnum.FoodCollected];
            sumFoodGrabbed += antStatistics[StatisticEnum.FoodGrabbed];
        }

        comeBackMean = count2 > 0 ? comeBackMean / count2 : 0;

        var xPoint = generationId * Vector2.right;
        var currentHighScore = new Dictionary<StatisticEnum, Vector2>
        {
            { StatisticEnum.Score, xPoint + (float)Math.Round(bestScore, 2) * Vector2.up },            
            { StatisticEnum.ComeBackMean, xPoint + (float)Math.Round(comeBackMean, 2) * Vector2.up },
            { StatisticEnum.FoodGrabbed, xPoint + sumFoodGrabbed * Vector2.up },
            { StatisticEnum.FoodCollected, xPoint + sumFoodCollected * Vector2.up }
        };

        if (_globalHighScore.Count == 0)
        {
            _globalHighScore = new Dictionary<StatisticEnum, Vector2>
            {
                { StatisticEnum.Score, xPoint + (float)Math.Round(bestScore, 2) * Vector2.up },                
                { StatisticEnum.FoodGrabbed, xPoint + sumFoodGrabbed * Vector2.up },
                { StatisticEnum.FoodCollected, xPoint + sumFoodCollected * Vector2.up }
            };
        }
        else
        {
            if (bestScore > _globalHighScore[StatisticEnum.Score].y)
                _globalHighScore[StatisticEnum.Score] = new Vector2(generationId, bestScore);

            if (sumFoodCollected > _globalHighScore[StatisticEnum.FoodCollected].y)
                _globalHighScore[StatisticEnum.FoodCollected] = new Vector2(generationId, sumFoodCollected);

            if (sumFoodGrabbed > _globalHighScore[StatisticEnum.FoodGrabbed].y)
                _globalHighScore[StatisticEnum.FoodGrabbed] = new Vector2(generationId, sumFoodGrabbed);
        }

       

        AddStatisticsToCurve(currentHighScore);
        UpdateStatisticsText(currentHighScore, _globalHighScore);

        if (generationId % GlobalParameters.StoreFrequency == 0)
            await SaveWinnersAsync(generationId, ants, sumFoodCollected).ConfigureAwait(false);
        else if (sumFoodCollected >= 99)
            await SaveWinnersAsync(generationId, ants, sumFoodCollected).ConfigureAwait(false);

        Debug.Log($"Generation : {generationId}\nHighest score : {bestScore} - Food Grabbed : {sumFoodGrabbed} - Food Gathered : {sumFoodCollected}");
    }

    public void AddStatisticsToCurve(Dictionary<StatisticEnum, Vector2> values)
    {
        foreach(var value in values)
            if (_statisticsDisplayZoneIndexes.ContainsKey(value.Key))
                _gameViewManager.AddCurveValue(_statisticsDisplayZoneIndexes[value.Key], value.Value);
    }

    public void UpdateStatisticsText(Dictionary<StatisticEnum, Vector2> currentGenerationScore, Dictionary<StatisticEnum, Vector2> globalHighScores)
    {
        var text = new StringBuilder("HighScore :\n");
        foreach (var pair in globalHighScores)
            text.AppendLine($"{pair.Key} : {pair.Value.y} - ({pair.Value.x})");
        text.AppendLine("\nCurrent score :");
        foreach (var pair in currentGenerationScore)
            text.AppendLine($"{pair.Key} : {pair.Value.y}");
        _gameViewManager.UpdateHighScores(text.ToString());
    }

    private async Task SaveWinnersAsync(int generationId, List<BaseAnt> ants, float foodCollected)
    {
        await _dbGateway.StoreBrainsAsync(generationId, ants.Select(t => t.GetBrain().MainBrain).ToList()).ConfigureAwait(false);
    }
}
