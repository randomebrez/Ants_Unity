using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using mew;

public class AntScannerPheromones : AntScannerBase
{
    protected override float ScannerAngle => 175;
    protected override bool CheckObtruction => false;

    protected override float ScannerRadius => 3 * _ant.Stats.VisionRadius;

    public override float GetPortionValue(int index)
    {
        var sum = 0f;
        foreach (var obj in Objects[index])
        {
            var pheromone = obj.GetComponent<BasePheromone>().Pheromone;
            sum += pheromone.Density;
        }
        return Objects[index].Count == 0 ? sum : sum / Objects[index].Count;
    }

    public (int number, float averageDensity) GetPheromonesOfType(int index, ScriptablePheromoneBase.PheromoneTypeEnum type)
    {
        var sum = 0f;
        var pheroDensities = Objects[index]
            .Where(t => t.GetComponent<BasePheromone>().Caracteristics.PheromoneType == type)
            .Select(t => t.GetComponent<BasePheromone>().Pheromone.Density);

        foreach (var density in pheroDensities)
            sum += density;
        var number = pheroDensities.Count();
        if (number == 0)
            return (0, 0);

        return  (number, sum / number);
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var obstacle in ObjectsFlattenList)
        {
            Gizmos.DrawWireCube(obstacle.transform.position, Vector3.one);
        }
    }*/
}