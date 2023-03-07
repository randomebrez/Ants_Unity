using System.Linq;
using UnityEngine;

public class AntScannerCollectables : AntScannerBase
{
    protected override float ScannerAngle => _ant.Stats.VisionAngle;
    protected override bool CheckObtruction => false;

    protected override float ScannerRadius => _ant.Stats.VisionRadius;

    public override float GetPortionValue(int index)
    {
        throw new System.NotImplementedException();
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

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var obstacle in ObjectsFlattenList)
        {
            Gizmos.DrawWireCube(obstacle.transform.position, Vector3.one);
        }
    }*/
}
