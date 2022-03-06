using mew;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var obstacle in ObjectsFlattenList)
        {
            Gizmos.DrawWireCube(obstacle.transform.position, Vector3.one);
        }
    }
}
