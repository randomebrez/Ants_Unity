using System.Diagnostics;
using System.Linq;
using mew;
using UnityEngine;

public class AntScannerPheromones : AntScannerBase
{
    protected override float _scannerAngle => 360;
    protected override bool _checkObtruction => false;
    protected override float _scannerRadius => _ant.Stats.VisionRadius * 3 * _apothem;

    public (int number, float averageDensity) GetPheromonesOfType(int index, ScriptablePheromoneBase.PheromoneTypeEnum type)
    {
        try
        {
            var sum = 0f;
            var pheroDensities = Objects[index]
                .Where(t => t.GetComponent<BasePheromone>().Caracteristics.PheromoneType == type && t.GetComponent<BasePheromone>().Pheromone != null)
                .Select(t => t.GetComponent<BasePheromone>().Pheromone.Density);

            foreach (var density in pheroDensities)
                sum += density;
            var number = pheroDensities.Count();
            if (number == 0)
                return (0, 0);

            return (number, sum / number);
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogException(e);
            return (0, 0);
        }
        
    }

    public override float GetPortionValue(int index)
    {
        throw new System.NotImplementedException();
    }

    //public void OnDrawGizmos()
    //{
    //    // Print scanned obj
    //    var portionColors = new Color[] { Color.yellow, Color.red, Color.blue, Color.green, Color.cyan, Color.white };
    //    for (int i = 0; i < _scannerSubdivision; i++)
    //    {
    //        Gizmos.color = portionColors[i];
    //        foreach (var obstacle in Objects[i])
    //        {
    //            Gizmos.DrawWireCube(obstacle.transform.position, Vector3.one);
    //        }
    //    }
    //}
}