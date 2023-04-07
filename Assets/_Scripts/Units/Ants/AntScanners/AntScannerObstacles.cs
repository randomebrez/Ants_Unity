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

    private void OnDrawGizmos()
    {
        if (_ant.name != "Worker_0")
            return;
    
        // Draw scanner with portions
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(_positionAtScanTime, _scannerRadius);
        float deltaTheta = 360 / _scannerSubdivision;
        // start at the back of the ant for the 'ToDictionary' method that uses the fact that indexes are increasing
        var current = -180 - deltaTheta / 2f;
        for (int i = 0; i < _scannerSubdivision; i++)
        {
            Gizmos.DrawLine(_positionAtScanTime, _positionAtScanTime + Quaternion.Euler(0, current, 0) * _directionAtScanTime * _scannerRadius);
            current += deltaTheta;
        }
    
        // Print scanned obj
        var portionColors = new Color[] { Color.yellow, Color.red, Color.blue, Color.green, Color.cyan,  Color.white };
        for (int i = 0; i < _scannerSubdivision; i++)
        {
            Gizmos.color = portionColors[i];
            foreach (var obstacle in Objects[i])
            {
                Gizmos.DrawWireCube(obstacle.transform.position, Vector3.one);
            }
        }
    
        // Print ground detected
        //var colliders = new Collider[70];
        //Physics.OverlapSphereNonAlloc(_positionAtScanTime, _scannerRadius, colliders, LayerMask.GetMask(Layer.Walkable.ToString()));
        //Gizmos.color = Color.magenta;
        //foreach (var collider in colliders)
        //    Gizmos.DrawWireCube(collider.transform.position, Vector3.one);
    
        // Vision Field
        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(_positionAtScanTime, _positionAtScanTime + Quaternion.Euler(0, -_scannerAngle / 2f, 0) * _directionAtScanTime * _scannerRadius);
        //Gizmos.DrawLine(_positionAtScanTime, _positionAtScanTime + Quaternion.Euler(0, _scannerAngle / 2f, 0) * _directionAtScanTime * _scannerRadius);
    }
}
