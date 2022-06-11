using Assets._Scripts.Utilities;
using mew;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static mew.ScriptablePheromoneBase;

public class AntScannerManager : MonoBehaviour
{
    public enum ScannerTypeEnum
    {
        Obstacles,
        Food,
        Pheromones
    }

    float[] _bonuses;
    float[] _maluses;
    private float[] _probabilities;

    protected int ScanFrequency = 10;
    protected float _scanInterval;
    protected float _scanTimer;
    protected bool _scanMode = true;

    private BaseAnt _ant;
    // Todo : move subdivision to the ant caracteristics
    private int _subdivisions;
    private AntScannerObstacles _obstacleScanner;
    private AntScannerPheromones _pheromoneScanner;
    private AntScannerCollectables _collectableScanner;

    public float[] ProbabilitiesGet => _probabilities;
    public bool CarryingFood;
    
    private void Awake()
    {
        _ant = GetComponentInParent<BaseAnt>();

        _obstacleScanner = GetComponentInChildren<AntScannerObstacles>();
        _pheromoneScanner = GetComponentInChildren<AntScannerPheromones>();
        _collectableScanner = GetComponentInChildren<AntScannerCollectables>();
    }

    private void Update()
    {
        _scanTimer -= Time.deltaTime;
        if (_scanTimer < 0)
        {
            _scanTimer += _scanInterval;
            if (_scanMode)
                Scan();
            else
                ProbabilitiesUpdate();
            _scanMode = !_scanMode;
        }
    }

    public void CreateMesh()
    {
        _obstacleScanner.CreateWedgeMesh(0.1f);
    }

    public void InitialyzeScanners(int subdivisions)
    {
        _subdivisions = subdivisions;
        _obstacleScanner.Initialyze(_ant, subdivisions, ScanFrequency);
        _pheromoneScanner.Initialyze(_ant, subdivisions, ScanFrequency);
        _collectableScanner.Initialyze(_ant, subdivisions, ScanFrequency);

        _probabilities = new float[subdivisions];
        _bonuses = new float[subdivisions];
        _maluses = new float[subdivisions];
    }

    public void Scan()
    {
        _obstacleScanner.Scan();
        _pheromoneScanner.Scan();
        _collectableScanner.Scan();
    }

    private void ProbabilitiesUpdate()
    {
        for (int i = 0; i < _subdivisions; i++)
        {
            var (bonus, malus) = GetPortionScore(i);
            _bonuses[i] = bonus;
            _maluses[i] = malus;
        }

        var temp = StaticCompute.GetPortionProbabilities(_bonuses, _maluses);
        if (Mathf.Abs(temp.Item1 - 1) > 0.1)
            Debug.Log("Proba above 1");
        _probabilities = temp.Item2;
    }


    #region Portions

    private (float bonus, float malus) GetPortionScore(int portionIndex)
    {
        var malus = GetPortionMalus(portionIndex);
        var bonus = GetPortionBonus(portionIndex, CarryingFood);
        return (bonus, malus);
    }

    private float GetPortionBonus(int portionIndex, bool carryFood)
    {
        var wanderPheromonesBonus = ComputeBonus(portionIndex, PheromoneTypeEnum.Wander);
        var carryPheromonesBonus = ComputeBonus(portionIndex, PheromoneTypeEnum.CarryFood);


        switch (carryFood)
        { 
            case true:
                return (0.9f * wanderPheromonesBonus + 0.1f * carryPheromonesBonus); 
            case false:
                return 0.9f * carryPheromonesBonus + 0.1f * wanderPheromonesBonus;
        }
    }

    private float ComputeBonus(int portionIndex, PheromoneTypeEnum pheroType)
    {
        var pheromones = _pheromoneScanner.GetPheromonesOfType(portionIndex, pheroType);

        var numberTanh = StaticCompute.ComputeTanh(pheromones.number);
        var densityTanh = StaticCompute.ComputeTanh(pheromones.averageDensity);

        // maximum number of collider of a scanner is 150
        var modificator = Mathf.Pow(pheromones.number / 100f, 0.1f);

        return modificator * numberTanh * densityTanh;
    }

    private float GetPortionMalus(int portionIndex)
    {
        var obstacleValue = _obstacleScanner.GetPortionValue(portionIndex);
        var obstacleValueExp = StaticCompute.ComputeExponentialProbability(obstacleValue, _ant.PhysicalLength / _ant.Stats.VisionRadius, 1f);

        return 1 - obstacleValueExp;
    }

    #endregion

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
        return _collectableScanner.IsNestInSight(nestName);
    }

    public List<GameObject> CollectablesListByTag(string type)
    {
        var typedCollectables = _collectableScanner.ObjectsFlattenList.Where(t => t.tag == type);

        return typedCollectables.ToList();
    }

    #endregion
}
