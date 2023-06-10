using mew;
using System.Collections.Generic;
using UnityEngine;
using static mew.ScriptablePheromoneBase;
using Assets.Dtos;
using Assets._Scripts.Units.Ants.AntScanners;

public class AntScannerManager : MonoBehaviour
{
    public enum ScannerTypeEnum
    {
        Obstacles,
        Food,
        Pheromones
    }

    private UnitScanningResult _inputs;
    private string[] _portionInfos;

    private BaseAnt _ant;

    private AntScannerBlock _scannerBlock;
    private AntScannerObstacles _obstacleScanner;
    public string[] PortionInfos => _portionInfos;
    public UnitScanningResult GetInputs => _inputs;
    bool initialyzed = false;
    
    private void Awake()
    {
        _ant = GetComponentInParent<BaseAnt>();

        _obstacleScanner = GetComponentInChildren<AntScannerObstacles>();
        _scannerBlock = GetComponentInChildren<AntScannerBlock>();
    }

    public Dictionary<int, UnitPortionInputValues> GetPortionInputs(List<int> portionIndexes)
    {
        var result = new Dictionary<int, UnitPortionInputValues>();
        foreach (var index in portionIndexes)
            result.Add(index, _inputs.PortionInputValues[index]);
        return result;
    }

    public void InitialyzeScanners()
    {
        var subdiv = _ant.Stats.ScannerSubdivisions;
        _obstacleScanner.Initialyze(_ant, subdiv);
        _scannerBlock.Initialyze(_ant, subdiv);
        _inputs = new UnitScanningResult(subdiv);
        _portionInfos = new string[subdiv];

        initialyzed = true;
    }

    public void ScanAll(bool carryFood)
    {
        if (!initialyzed)
            return;

        // Update scanner objects lists
        ScanWithAllScanners();

        // Update PortionInput values
        for (int i = 0; i < _ant.Stats.ScannerSubdivisions; i++)
            SinglePortionInputUpdate(i);

        // Othe UnityInputs
        _inputs.CarryFood = carryFood;
    }

    private void ScanWithAllScanners()
    {
        _obstacleScanner.Scan();
        _scannerBlock.Scan();
    }

    private void SinglePortionInputUpdate(int portionIndex)
    {
        _inputs.PortionInputValues[portionIndex].PheroW = _scannerBlock.GetPheromonesOfType(portionIndex, PheromoneTypeEnum.Wander).averageDensity;
        _inputs.PortionInputValues[portionIndex].PheroC = _scannerBlock.GetPheromonesOfType(portionIndex, PheromoneTypeEnum.CarryFood).averageDensity;
        _inputs.PortionInputValues[portionIndex].WallDist = _obstacleScanner.GetPortionValue(portionIndex);
        _inputs.PortionInputValues[portionIndex].FoodToken = _scannerBlock.IsThereFood(portionIndex);
        _inputs.PortionInputValues[portionIndex].IsNestInSight = _scannerBlock.IsNestInSight(portionIndex);

        //if (_ant.name == "Worker_0")
        //{
        //    var outputValue = new StringBuilder($"{portionIndex} : ");
        //    var inputs = portioninputs.ToList();
        //    for (int i = 0; i < inputs.Count; i++)
        //        outputValue.Append($"{inputs[i]} ; ");
        //
        //    Debug.Log(outputValue.ToString());
        //}
    }
}
