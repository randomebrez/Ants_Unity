using System.Linq;
using mew;
using UnityEngine;

public class AntScannerPheromones : AntScannerBase
{
    protected override float _scannerAngle => 360;
    protected override bool _checkObtruction => false;
    protected override bool _checkVisionField => false;
    public (int number, float averageDensity) GetPheromonesOfType(int index, ScriptablePheromoneBase.PheromoneTypeEnum type)
    {
        try
        {
            var sum = 0f;
            var pheroDensities = Objects[index]
                .Where(t => t.GetComponent<BasePheromone>().Caracteristics.PheromoneType == type && t.GetComponent<BasePheromone>().Pheromone != null)
                .Select(t => t.GetComponent<BasePheromone>().Pheromone.Density).OrderByDescending(t => t).Take(_scannerSurface).ToList();

            var number = pheroDensities.Count();
            if (number == 0)
                return (0, 0);

            foreach (var density in pheroDensities)
                sum += density;
            
            return (number, sum / _scannerSurface);
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
            return (0, 0);
        }
    }

    public override float GetPortionValue(int index)
    {
        throw new System.NotImplementedException();
    }

    /*
    public void OnDrawGizmos()
    {
        // Draw scanner with portions
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, _scannerRadius);
        float deltaTheta = 360 / _scannerSubdivision;
        // start at the back of the ant for the 'ToDictionary' method that uses the fact that indexes are increasing
        var current = -180 - deltaTheta / 2f;
        for (int i = 0; i < _scannerSubdivision; i++)
        {
            Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, current, 0) * _directionAtScanTime * _scannerRadius);
            current += deltaTheta;
        }
    
        // Print scanned obj
        var portionColors = new Color[] { Color.yellow, Color.red, Color.blue, Color.green, Color.cyan, Color.white };
        for (int i = 0; i < _scannerSubdivision; i++)
        {
            Gizmos.color = portionColors[i];
            foreach (var obstacle in Objects[i])
            {
                Gizmos.DrawWireCube(obstacle.transform.position, Vector3.one);
            }
        }
    
        // Print ground detected
        var colliders = new Collider[70];
        Physics.OverlapSphereNonAlloc(transform.position, _scannerRadius, colliders, LayerMask.GetMask(Layer.Walkable.ToString()));
        Gizmos.color = Color.magenta;
        var temp = colliders.Where(t => t != null).ToList();
        Debug.Log(temp.Count);
        foreach (var collider in temp)
            Gizmos.DrawWireCube(collider.transform.position, 0.5f * Vector3.one);
    
        // Vision Field
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, -_scannerAngle / 2f, 0) * _directionAtScanTime * _scannerRadius);
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, _scannerAngle / 2f, 0) * _directionAtScanTime * _scannerRadius);
    }*/
}