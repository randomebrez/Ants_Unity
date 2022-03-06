using Assets._Scripts.Utilities;
using mew;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AntScannerManager : MonoBehaviour
{
    public enum ScannerTypeEnum
    {
        Obstacles,
        Food,
        Pheromones
    }
    private const float Delta = 0.00001f;
    private AntScannerObstacles _obstacleScanner;
    private AntScannerPheromones _pheromoneScanner;
    private AntScannerCollectables _collectableScanner;

    private BaseAnt _ant;

    private int _subdivisons;
    
    private void Awake()
    {
        _ant = GetComponentInParent<BaseAnt>();

        _obstacleScanner = GetComponentInChildren<AntScannerObstacles>();
        _pheromoneScanner = GetComponentInChildren<AntScannerPheromones>();
        _collectableScanner = GetComponentInChildren<AntScannerCollectables>();
    }


    public void ScannerSubdivisionSet(int subdivisions)
    {
        _subdivisons = subdivisions;
        _obstacleScanner.ScannerSubdivisionSet(subdivisions);
        _pheromoneScanner.ScannerSubdivisionSet(subdivisions);
        _collectableScanner.ScannerSubdivisionSet(subdivisions);
    }

    public List<float> GetProbabilities()
    {
        var scores = new List<float>();
        for (int i = 0; i < _subdivisons; i++)
        {
            scores.Add(GetPortionScore(i));
        }

        return GetInhibitedDirections(scores);
    }

    private float GetPortionScore(int portionIndex)
    {
        var obstacleValue = _obstacleScanner.GetPortionValue(portionIndex);
        var pheromoneValue = _pheromoneScanner.GetPortionValue(portionIndex);

        var obstacleValueExp = StaticHelper.ComputeExponentialProbability(obstacleValue, _ant.PhysicalLength, 100, Delta);
        var pheromoneValueExp = StaticHelper.ComputeExponentialProbability(pheromoneValue, 0, 100, Delta);

        return 1 + (pheromoneValueExp - (1 - obstacleValueExp)) / 2;
    }

    private List<float> GetInhibitedDirections(List<float> portionScores)
    {
        var scoreSum = 0f;
        var probaSum = 0f;
        foreach (var score in portionScores)
        {
            scoreSum += score;
        }

        var unnormedProbabilities = new List<float>();

        var portionNumber = portionScores.Count;
        var basePortionProbability = 360 / portionNumber;

        foreach (var score in portionScores)
        {
            var scoreResult = score * (portionNumber + 1) - scoreSum;
            var probability = basePortionProbability * (1 + scoreResult / portionNumber);
            probaSum += probability;
            unnormedProbabilities.Add(probability);
        }

        var normedProbabilities = new List<float>();
        foreach (var score in unnormedProbabilities)
        {
            normedProbabilities.Add(score / probaSum);
        }

        return normedProbabilities;
    }

    #region Obstacles
    public float GetObstacleInRangeNormalizedDistance(Vector3 position, float visionRadius)
    {
        var distance = 0f;
        var count = 0;
        var obstacles = _obstacleScanner.ObjectsFlattenList;
        foreach(var obstacle in obstacles)
        {
            count++;
            distance += Vector3.Distance(position, obstacle.transform.position) / visionRadius;
        }
        
        return distance / count;
    }

    public int ObstaclesInRangeCount()
    {
        return _obstacleScanner.GetCount;
    }

    public bool IsMoveValid(Vector3 from, Vector3 to)
    {
        return _obstacleScanner.IsMoveValid(from, to);
    }

    #endregion

    #region Collectables

    public (bool answer, Transform tranform) IsNestInSight(string nestName)
    {
        var nestTransform = _collectableScanner.ObjectsFlattenList.FirstOrDefault(t => t.name == nestName);

        if (nestTransform != null)
            return (true, nestTransform.transform);

        return (false, null);
    }

    public List<GameObject> CollectablesListByTag(string type)
    {
        var typedCollectables = _collectableScanner.ObjectsFlattenList.Where(t => t.tag == type);

        return typedCollectables.ToList();
    }

    #endregion
}
