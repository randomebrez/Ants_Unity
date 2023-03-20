using Assets.Dtos;
using System.Linq;
using UnityEngine;

public class AntScannerCollectables : AntScannerBase
{
    protected override float _scannerAngle => _ant.Stats.VisionAngle;
    protected override bool _checkObtruction => false;
    protected override float _scannerRadius => _ant.Stats.VisionRadius * 2 * _apothem;

    public override float GetPortionValue(int index)
    {
        throw new System.NotImplementedException();
    }

    public bool GetFoodToken(int portionIndex)
    {
        return Objects[portionIndex].Where(t => t.tag == "Food").Any();
    }

    public (bool isIt, Transform nest) IsNestInSight(int portionindex, string nestName)
    {
        var nest = Objects[portionindex].FirstOrDefault(t => t.name == nestName);
        if (nest == null)
            return (false, null);

        if (Physics.Linecast(transform.position, nest.transform.position - transform.position, OcclusionLayer))
            return (false, null);

        return (true, nest.transform);
    }

    //private void OnDrawGizmos()
    //{
    //    // Draw scanner with portions
    //    Gizmos.color = Color.black;
    //    Gizmos.DrawWireSphere(transform.position, _scannerRadius);
    //    float deltaTheta = 360 / _scannerSubdivision;
    //    // start at the back of the ant for the 'ToDictionary' method that uses the fact that indexes are increasing
    //    var current = -180 - deltaTheta / 2f;
    //    for (int i = 0; i < _scannerSubdivision; i++)
    //    {
    //        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, current, 0) * _ant.BodyHeadAxis * _scannerRadius);
    //        current += deltaTheta;
    //    }
    //
    //    // Print scanned obj
    //    var portionColors = new Color[] { Color.yellow, Color.red, Color.blue, Color.green, Color.cyan,  Color.white };
    //    for (int i = 0; i < _scannerSubdivision; i++)
    //    {
    //        Gizmos.color = portionColors[i];
    //        foreach (var obstacle in Objects[i])
    //        {
    //            Gizmos.DrawWireCube(obstacle.transform.position, Vector3.one);
    //        }
    //    }
    //
    //    // Print ground detected
    //    var colliders = new Collider[70];
    //    Physics.OverlapSphereNonAlloc(transform.position, _scannerRadius, colliders, LayerMask.GetMask(Layer.Walkable.ToString()));
    //    Gizmos.color = Color.magenta;
    //    foreach (var collider in colliders)
    //        Gizmos.DrawWireCube(collider.transform.position, Vector3.one);
    //
    //    // Vision Field
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, -_scannerAngle / 2f, 0) * _ant.BodyHeadAxis * _scannerRadius);
    //    Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, _scannerAngle / 2f, 0) * _ant.BodyHeadAxis * _scannerRadius);
    //}
}    