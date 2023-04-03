using UnityEngine;

public class AntScannerObstacles : AntScannerBase
{
    private float _avoidDistance => _ant.PhysicalLength;
    protected override float _scannerRadius => _ant.Stats.VisionRadius * 4 * _apothem + 0.1f;    

    public override float GetPortionValue(int index)
    {
        var currentMin = _scannerRadius;
        foreach(var obj in Objects[index])
        {
            var distance = Vector3.Distance(_positionAtScanTime, obj.transform.position);

            if (distance < currentMin)
                currentMin = distance;
        }
        return currentMin / _scannerRadius;
    }

    protected override bool IsInSight(GameObject obj)
    {
        var objPosition = obj.transform.position;
        var direction = objPosition - _positionAtScanTime;

        if (direction.magnitude < _ant.PhysicalLength * 4)
            return true;

        return base.IsInSight(obj);
    }

    #region Unused tools

    public bool IsMoveValid(Vector3 from, Vector3 to)
    {
        return !Physics.Linecast(from, to, OcclusionLayer);
    }

    public bool IsHeadingForCollision()
    {
        RaycastHit hit;
        if (Physics.SphereCast(_positionAtScanTime, _ant.Stats.BoundRadius, _directionAtScanTime, out hit, _avoidDistance, OcclusionLayer))
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
        for (int i = 0; i < _scannerSubdivision; i++)
        {
            Vector3 dir = Quaternion.Euler(0, currentAngle, 0) * _directionAtScanTime;
            Ray ray = new Ray(_positionAtScanTime, dir);
            if (!Physics.SphereCast(ray, _ant.Stats.BoundRadius, _avoidDistance, OcclusionLayer))
            {
                return dir;
            }
        }

        return _directionAtScanTime;
    }

    #endregion

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
