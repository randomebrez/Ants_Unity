using UnityEngine;

public class AntScannerPheromones : AntScannerBase
{
    protected override float ScannerAngle => 175;
    protected override bool CheckObtruction => false;

    protected override float ScannerRadius => _ant.Stats.VisionRadius;

    public override float GetPortionValue(int index)
    {
        var sum = 0f;
        foreach (var obj in Objects[index])
        {
            var pheromone = obj.GetComponent<BasePheromone>().Pheromone;
            sum += pheromone.Density;
        }
        return sum;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var obstacle in ObjectsFlattenList)
        {
            Gizmos.DrawWireCube(obstacle.transform.position, Vector3.one);
        }
    }
}
