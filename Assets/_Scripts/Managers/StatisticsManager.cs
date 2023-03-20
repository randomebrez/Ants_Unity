using Assets._Scripts.Units.Ants;
using Assets._Scripts.Utilities;
using mew;
using NeuralNetwork.Interfaces.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

internal class StatisticsManager : BaseManager<StatisticsManager>
{
    private GameViewsManager _gameViewManager;
    private Dictionary<StatisticEnum, int> _statisticsDisplayZoneIndexes = new Dictionary<StatisticEnum, int>();
    private Dictionary<StatisticEnum, Vector2> _globalHighScore = new Dictionary<StatisticEnum, Vector2>();

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

    public void GetStatistics(int generationId, List<BaseAnt> ants)
    {
        var sumFoodCollected = 0f;
        var sumFoodGrabbed = 0f;
        //var bestFoodReach = Mathf.Infinity;
        //var bestComeBack = Mathf.Infinity;
        //var foodReachMean = 0f;
        var comeBackMean = 0f;
        var bestScore = 0f;
        //var count1 = 0f;
        var count2 = 0f;
        foreach (var ant in ants)
        {
            var antStatistics = ant.GetStatistics();

            //if (antStatistics[StatisticEnum.ComeBackMean] < bestComeBack)
            //    bestComeBack = antStatistics[StatisticEnum.ComeBackMean];
            //if (antStatistics[StatisticEnum.BestFoodReachStepNumber] < bestFoodReach)
            //    bestFoodReach = antStatistics[StatisticEnum.BestFoodReachStepNumber];
            if (antStatistics[StatisticEnum.Score] > bestScore)
                bestScore = antStatistics[StatisticEnum.Score];

            //if (antStatistics[StatisticEnum.BestFoodReachStepNumber] < int.MaxValue)
            //{
            //    foodReachMean += antStatistics[StatisticEnum.BestFoodReachStepNumber];
            //    count1++;
            //}
            if (antStatistics[StatisticEnum.ComeBackMean] < int.MaxValue)
            {
                comeBackMean += antStatistics[StatisticEnum.ComeBackMean];
                count2++;
            }
            sumFoodCollected += antStatistics[StatisticEnum.FoodCollected];
            sumFoodGrabbed += antStatistics[StatisticEnum.FoodGrabbed];
        }

        //foodReachMean = foodReachMean / count1;
        comeBackMean = count2 > 0 ? comeBackMean / count2 : 0;

        var xPoint = (generationId - 1) * Vector2.right;
        //bestComeBack = bestComeBack == Mathf.Infinity ? 0 : bestComeBack;
        var currentHighScore = new Dictionary<StatisticEnum, Vector2>
        {
            { StatisticEnum.Score, xPoint + (float)Math.Round(bestScore, 2) * Vector2.up },
            //{ StatisticEnum.BestFoodReachStepNumber, xPoint + (float)Math.Round(1f / bestFoodReach, 2) * Vector2.up },
            { StatisticEnum.ComeBackMean, xPoint + (float)Math.Round(comeBackMean, 2) * Vector2.up },
            { StatisticEnum.FoodCollected, xPoint + sumFoodCollected * Vector2.up },
            { StatisticEnum.FoodGrabbed, xPoint + sumFoodGrabbed * Vector2.up }
        };
        //currentHighScore[StatisticEnum.BestFoodReachStepNumber] = xPoint + (float)Math.Round(foodReachMean, 2) * Vector2.up;

        if (_globalHighScore.Count == 0)
        {
            _globalHighScore = new Dictionary<StatisticEnum, Vector2>
            {
                { StatisticEnum.Score, xPoint + (float)Math.Round(bestScore, 2) * Vector2.up },
                //{ StatisticEnum.BestFoodReachStepNumber, xPoint + (float)Math.Round(bestFoodReach, 2) * Vector2.up },
                //{ StatisticEnum.ComeBackMean, xPoint + (float)Math.Round(bestComeBack, 2) * Vector2.up },
                { StatisticEnum.FoodCollected, xPoint + sumFoodCollected * Vector2.up },
                { StatisticEnum.FoodGrabbed, xPoint + sumFoodGrabbed * Vector2.up }
            };
        }
        else
        {
            if (bestScore > _globalHighScore[StatisticEnum.Score].y)
                _globalHighScore[StatisticEnum.Score] = new Vector2(generationId, bestScore);

            //if (bestFoodReach < _globalHighScore[StatisticEnum.BestFoodReachStepNumber].y)
            //    _globalHighScore[StatisticEnum.BestFoodReachStepNumber] = new Vector2(generationId, bestFoodReach);

            //if (bestComeBack < _globalHighScore[StatisticEnum.ComeBackMean].y)
            //    _globalHighScore[StatisticEnum.ComeBackMean] = new Vector2(generationId, bestComeBack);

            if (sumFoodCollected > _globalHighScore[StatisticEnum.FoodCollected].y)
                _globalHighScore[StatisticEnum.FoodCollected] = new Vector2(generationId, sumFoodCollected);

            if (sumFoodGrabbed > _globalHighScore[StatisticEnum.FoodGrabbed].y)
                _globalHighScore[StatisticEnum.FoodGrabbed] = new Vector2(generationId, sumFoodGrabbed);
        }

       

        AddValues(currentHighScore);
        UpdateHighScores(_globalHighScore);

        if (generationId % 20 == 0)
            SaveWinners(generationId, ants, sumFoodCollected);
        else if (sumFoodCollected >= 99)
            SaveWinners(generationId, ants, sumFoodCollected);

        Debug.Log($"Generation : {generationId}\nHighest score : {bestScore} - Food Grabbed : {sumFoodGrabbed} - Food Gathered : {sumFoodCollected}");
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

    private void SaveWinners(int generationId, List<BaseAnt> ants, float foodCollected)
    {
        var text = new StringBuilder($"{foodCollected}\n");
        for(int i = 0; i < ants.Count(); i++)
        {
            var antBrains = ants[i].GetBrain();
            text.AppendLine(ToSaveFormat(antBrains.MainBrain.Vertices));
            for (int j = 0; j < antBrains.ScannerBrains.Count(); j++)
                text.AppendLine($"{j}{ToSaveFormat(antBrains.ScannerBrains[j].Vertices)}");
        }

        using (var stream = File.Create($"{GlobalParameters.LogFileBase}\\{generationId}.txt"))
        {
            var bytes = new UTF8Encoding().GetBytes(text.ToString());
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
        }
    }

    private string ToSaveFormat(List<Vertex> edges)
    {
        var result = new StringBuilder();
        foreach(var edge in edges)
            result.Append($"{edge.Identifier}?{Math.Round(edge.Weight, 2)}!");
        return result.ToString();
    }
}
