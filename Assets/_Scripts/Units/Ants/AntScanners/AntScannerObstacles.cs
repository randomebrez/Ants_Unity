using UnityEngine;

public class AntScannerObstacles : AntScannerBase
{
    private float _avoidDistance => _ant.PhysicalLength;
    protected override float _scannerAngle => _ant.Stats.VisionAngle;
    protected override bool _checkObtruction => true;
    protected override float _scannerRadius => _ant.Stats.VisionRadius * 2 * _apothem;

    public bool IsMoveValid(Vector3 from, Vector3 to)
    {
        return !Physics.Linecast(from, to, OcclusionLayer);
    }

    public bool IsHeadingForCollision()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, _ant.Stats.BoundRadius, _ant.BodyHeadAxis, out hit, _avoidDistance, OcclusionLayer))
        {
            return true;
        }
        else { }
        return false;
    }

    public Vector3 ObstacleRays()
    {
        var deltaTheta = 360f / _scannerSubdivision;
        var currentAngle = -180f + deltaTheta / 2f;
        var antAxe = _ant.BodyHeadAxis;
        for (int i = 0; i < _scannerSubdivision; i++)
        {
            Vector3 dir = Quaternion.Euler(0, currentAngle, 0) * antAxe;
            Ray ray = new Ray(_ant.transform.position, dir);
            if (!Physics.SphereCast(ray, _ant.Stats.BoundRadius, _avoidDistance, OcclusionLayer))
            {
                return dir;
            }
        }

        return antAxe;
    }

    public override float GetPortionValue(int index)
    {
        var currentMin = _scannerRadius;
        foreach(var obj in Objects[index])
        {
            var distance = Vector3.Distance(_ant.transform.position, obj.transform.position);

            if (distance < currentMin)
                currentMin = distance;
        }
        return currentMin / _scannerRadius;
    }

    protected override bool IsInSight(GameObject obj)
    {
        var position = transform.position;
        var objPosition = obj.transform.position;
        var direction = objPosition - position;

        if (direction.magnitude < _ant.PhysicalLength * 4)
            return true;

        return base.IsInSight(obj);
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        foreach (var obstacle in ObjectsFlattenList)
        {
            Gizmos.DrawWireCube(obstacle.transform.position, Vector3.one);
        }

        if (_mesh != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawMesh(_mesh, transform.position, transform.rotation);
        }
    }*/
}
