using Assets._Scripts.Utilities;
using mew;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static mew.ScriptablePheromoneBase;
using Assets.Dtos;
using NeuralNetwork.Interfaces.Model;
using NeuralNetwork.Managers;
using System.Text;

public class AntScannerManager : MonoBehaviour
{
    public enum ScannerTypeEnum
    {
        Obstacles,
        Food,
        Pheromones
    }

    private NeuralNetworkInputs _inputs;
    private string[] _portionInfos;

    protected float _scanInterval;
    protected float _scanTimer;
    protected bool _updateinputs = true;

    private BaseAnt _ant;

    private AntScannerObstacles _obstacleScanner;
    private AntScannerPheromones _pheromoneScanner;
    private AntScannerCollectables _collectableScanner;
    public string[] PortionInfos => _portionInfos;

    bool initialyzed = false;
    
    private void Awake()
    {
        _ant = GetComponentInParent<BaseAnt>();

        _obstacleScanner = GetComponentInChildren<AntScannerObstacles>();
        _pheromoneScanner = GetComponentInChildren<AntScannerPheromones>();
        _collectableScanner = GetComponentInChildren<AntScannerCollectables>();
    }

    private void Update()
    {
        if (!initialyzed)
            return;

        _scanTimer -= Time.deltaTime;
        if (_scanTimer < 0)
        {
            _scanTimer += _scanInterval;
            Scan();
        }
        if (_updateinputs)
            InputsUpdate();
    }


    public void InitialyzeScanners(Dictionary<int, Brain> brains)
    {
        var subdiv = _ant.Stats.ScannerSubdivisions;
        _obstacleScanner.Initialyze(_ant, subdiv, GlobalParameters.BaseScannerRate);
        _pheromoneScanner.Initialyze(_ant, subdiv, GlobalParameters.BaseScannerRate);
        _collectableScanner.Initialyze(_ant, subdiv, GlobalParameters.BaseScannerRate);
        _inputs = new NeuralNetworkInputs(subdiv, brains);
        _portionInfos = new string[subdiv];
        initialyzed = true;
    }

    public void UpdateAntInputs(bool carryFood)
    {
        _inputs.CarryFood = carryFood;
    }

    public List<float> GetInputs()
    {
        //return _inputs.GetPortionOutputs();
        return _inputs.GetAllInputs();
    }

    public Dictionary<int, Brain> GetBrains()
    {
        return _inputs.GetBrains();
    }

    private void Scan()
    {
        _obstacleScanner.Scan();
        _pheromoneScanner.Scan();
        _collectableScanner.Scan();
        _updateinputs = true;
    }

    private void InputsUpdate()
    {
        for (int i = 0; i < _ant.Stats.ScannerSubdivisions; i++)
            _inputs.UpdatePortion(i, GetPortionInputs(i));
        _updateinputs = false;
    }


    #region Portions


    private PortionInputs GetPortionInputs(int portionIndex)
    {
        var pheroW = _pheromoneScanner.GetPheromonesOfType(portionIndex, PheromoneTypeEnum.Wander);
        var pheroC = _pheromoneScanner.GetPheromonesOfType(portionIndex, PheromoneTypeEnum.CarryFood);

        var portioninputs = new PortionInputs
        {
            PheroW = pheroW.averageDensity,
            PheroC = pheroC.averageDensity,
            WallDist = _obstacleScanner.GetPortionValue(portionIndex),
            FoodToken = _collectableScanner.GetFoodToken(portionIndex),
            IsNestInSight = _collectableScanner.IsNestInSight(portionIndex, _ant.NestName).isIt
        };

        //if (_ant.name == "Worker_0")
        //{
        //    var outputValue = new StringBuilder($"{portionIndex} : ");
        //    var inputs = portioninputs.ToList();
        //    for (int i = 0; i < inputs.Count; i++)
        //        outputValue.Append($"{inputs[i]} ; ");
        //
        //    Debug.Log(outputValue.ToString());
        //}

        return portioninputs;
    }

    private void UpdateportionInfos()

    {
        for(int i = 0; i < _portionInfos.Length; i++)
            _portionInfos[i] = GetPortionInfos(i);
    }

    private string GetPortionInfos(int portionIndex)
    {
        //var pheromonesW = _pheromoneScanner.GetPheromonesOfType(portionIndex, PheromoneTypeEnum.Wander);
        //var pheromonesC = _pheromoneScanner.GetPheromonesOfType(portionIndex, PheromoneTypeEnum.CarryFood);
        //var obstacleValue = _obstacleScanner.GetPortionValue(portionIndex);
        //
        //return $"P{portionIndex} | B:{Math.Round(bonus,3)} | M:{Math.Round(malus, 3)} | pm:{Math.Round(probaModif, 3)} | oV:{obstacleValue} | pW:{pheromonesW.number}-{Math.Round(pheromonesW.averageDensity, 3)} | pC:{pheromonesC.number}-{Math.Round(pheromonesC.averageDensity, 3)}";
        return string.Empty;
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

    public List<GameObject> CollectablesListByTag(string tag)
    {
        var typedCollectables = _collectableScanner.ObjectsFlattenList.Where(t => t.tag == tag);

        return typedCollectables.ToList();
    }

    #endregion
}
