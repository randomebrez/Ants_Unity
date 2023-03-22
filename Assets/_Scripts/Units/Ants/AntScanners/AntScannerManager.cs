using Assets._Scripts.Utilities;
using mew;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static mew.ScriptablePheromoneBase;
using Assets.Dtos;
using NeuralNetwork.Interfaces.Model;

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
        return _inputs.GetAllInputs();
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
        portioninputs.ActivateTriggerObject = _collectableScanner.IsPortionActive(portionIndex);

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
}
