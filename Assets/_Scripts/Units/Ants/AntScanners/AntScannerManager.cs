using Assets._Scripts.Utilities;
using mew;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static mew.ScriptablePheromoneBase;
using Assets.Dtos;
using NeuralNetwork.Interfaces.Model;
using Assets._Scripts.Units.Ants.AntScanners;

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

    private AntScannerBlock _scannerBlock;
    private AntScannerObstacles _obstacleScanner;
    public string[] PortionInfos => _portionInfos;

    bool initialyzed = false;
    
    private void Awake()
    {
        _ant = GetComponentInParent<BaseAnt>();

        _obstacleScanner = GetComponentInChildren<AntScannerObstacles>();
        _scannerBlock = GetComponentInChildren<AntScannerBlock>();
    }

    public List<float> GetInputs()
    {
        return _inputs.GetAllInputs();
    }

    public void InitialyzeScanners(Dictionary<int, Brain> brains)
    {
        var subdiv = _ant.Stats.ScannerSubdivisions;
        _obstacleScanner.Initialyze(_ant, subdiv);
        _scannerBlock.Initialyze(_ant, subdiv);
        _inputs = new NeuralNetworkInputs(subdiv, brains);
        _portionInfos = new string[subdiv];
        initialyzed = true;
    }

    public void ScanAll(bool carryFood)
    {
        if (!initialyzed)
            return;

        ScanWithAllScanners();
        PortionInputsUpdate();
        _inputs.CarryFood = carryFood;
    }

    private void ScanWithAllScanners()
    {
        _obstacleScanner.Scan();
        _scannerBlock.Scan();
        _updateinputs = true;
    }

    private void PortionInputsUpdate()
    {
        for (int i = 0; i < _ant.Stats.ScannerSubdivisions; i++)
            _inputs.UpdatePortion(i, GetPortionInputs(i));
        _updateinputs = false;
    }

    private PortionInputs GetPortionInputs(int portionIndex)
    {
        var pheroW = _scannerBlock.GetPheromonesOfType(portionIndex, PheromoneTypeEnum.Wander);
        var pheroC = _scannerBlock.GetPheromonesOfType(portionIndex, PheromoneTypeEnum.CarryFood);

        var portioninputs = new PortionInputs
        {
            PheroW = pheroW.averageDensity,
            PheroC = pheroC.averageDensity,
            WallDist = _obstacleScanner.GetPortionValue(portionIndex),
            FoodToken = _scannerBlock.IsThereFood(portionIndex),
            IsNestInSight = _scannerBlock.IsNestInSight(portionIndex)
        };
        portioninputs.ActivateTriggerObject = _scannerBlock.IsPortionInVisionField(portionIndex);

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
