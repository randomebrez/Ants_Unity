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
    
    private const float NormalMu = 0f;
    private const float NormalSigma = 270f;

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

    public void CreateMesh()
    {
        _obstacleScanner.CreateWedgeMesh(0.1f);
    }

    public void InitialyzeScanners(int subdivisions)
    {
        _subdivisons = subdivisions;
        _obstacleScanner.Initialyze(_ant, subdivisions);
        _pheromoneScanner.Initialyze(_ant, subdivisions);
        _collectableScanner.Initialyze(_ant, subdivisions);
    }

    public List<float> GetProbabilities(bool carryFood)
    {
        var scores = new List<float>();
        for (int i = 0; i < _subdivisons; i++)
        {
            scores.Add(GetPortionScore(i, carryFood));
        }

        return GetInhibitedDirections(scores);
    }

    private float GetPortionScore(int portionIndex, bool carryFood)
    {
        var malus = GetPortionMalus(portionIndex);
        var bonus = GetPortionBonus(portionIndex, carryFood);
        return bonus - malus;
    }

    private float GetPortionBonus(int portionIndex, bool carryFood)
    {
        var carryPheroValue = _pheromoneScanner.GetPheromonesOfType(portionIndex, ScriptablePheromoneBase.PheromoneTypeEnum.CarryFood);
        var carryPheroValueExp = StaticHelper.ComputeExponentialProbability(carryPheroValue, 0, 0.5f);

        switch (carryFood)
        {
            case true:
                return carryPheroValueExp;
            case false:
                var wanderPheroValue = _pheromoneScanner.GetPheromonesOfType(portionIndex, ScriptablePheromoneBase.PheromoneTypeEnum.Wander);
                var wanderPheroValueExp = StaticHelper.ComputeExponentialProbability(wanderPheroValue, 0, 0.5f);
                return (carryPheroValueExp + wanderPheroValueExp) / 2;
        }
    }

    private float GetPortionMalus(int portionIndex)
    {
        var obstacleValue = _obstacleScanner.GetPortionValue(portionIndex);
        var obstacleValueExp = StaticHelper.ComputeExponentialProbability(obstacleValue, _ant.PhysicalLength / _ant.Stats.VisionRadius, 1f);

        return 1 - obstacleValueExp;
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
        var deltaAngle = 360f / _subdivisons;
        var startingPoint = -180f + deltaAngle / 2;

        for (int i = 0; i < portionScores.Count; i++)
        {
            var basePortionProbability = StaticHelper.ComputeNormalLaw(startingPoint + i * deltaAngle, NormalMu, NormalSigma);
            var scoreResult = portionScores[i] * (portionNumber + 1) - scoreSum;
            var probability = basePortionProbability * (1 + scoreResult / portionNumber);
            probaSum += probability;
            unnormedProbabilities.Add(probability);
        }

        var normedProbabilities = new List<float>();
        foreach (var probability in unnormedProbabilities)
        {
            normedProbabilities.Add(probability / probaSum);
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
        
        return count == 0 ? 1 : distance / count;
    }

    public bool IsMoveValid(Vector3 from, Vector3 to)
    {
        return _obstacleScanner.IsMoveValid(from, to);
    }

    public (bool isIt, Vector3 avoidDir) IsHeadingForCollision()
    {
        if (_obstacleScanner.IsHeadingForCollision() == false)
            return (false, Vector3.zero);

        return (true, _obstacleScanner.ObstacleRays());
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
