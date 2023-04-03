using System.Linq;
using System.Collections.Generic;
using mew;
using UnityEngine;
using System;
using Assets._Scripts.Utilities;
using Assets.Dtos;

public abstract class AntScannerBase : MonoBehaviour
{
    [SerializeField]
    protected LayerMask ScanningMainLayer;
    [SerializeField]
    protected LayerMask OcclusionLayer;

    // Ant reference
    protected BaseAnt _ant;
    protected Vector3 _positionAtScanTime;
    protected Vector3 _directionAtScanTime;

    // Base scanner parameters
    protected List<float> _subdivisions = new List<float>();

    protected bool _initialyzed = false;
    protected int _scannerSubdivision;
    protected virtual float _scannerAngle => _ant.Stats.VisionAngle;
    protected virtual float _scannerRadius => _ant.Stats.VisionRadius * 2 * _apothem + 0.1f;
    protected virtual bool _checkObtruction => true;
    protected virtual bool _checkVisionField => true;

    protected int _scannerSurface { get; private set; }

    protected float _apothem => (float)Math.Sqrt(3) * GlobalParameters.NodeRadius / 2;

    // Colliders container
    protected List<GameObject> _objects = new List<GameObject>();
    protected Collider[] _colliders = new Collider[150];


    // Public methods to get colliders
    public Dictionary<int, List<GameObject>> Objects
    {
        get
        {
            _objects.RemoveAll(t => !t);
            return ToDictionnary(_objects);
        }
    }
    public List<GameObject> ObjectsFlattenList
    {
        get
        {
            _objects.RemoveAll(t => !t);
            return _objects;
        }
    }

    public abstract float GetPortionValue(int index);

    public virtual void Initialyze(BaseAnt ant, int scannerSubdivision)
    {
        _ant = ant;
        _scannerSubdivision = scannerSubdivision;
        _positionAtScanTime = transform.position;
        _directionAtScanTime = _ant.BodyHeadAxis;

        // Compute number of tile within scanner range
        var colliders = new Collider[70];
        Physics.OverlapSphereNonAlloc(transform.position, _scannerRadius, colliders, LayerMask.GetMask(Layer.Walkable.ToString()));
        var blockCount = colliders.Where(t => t != null).Count();
        _scannerSurface = (blockCount - 1)/ _scannerSubdivision;

        float deltaTheta = 360 / _scannerSubdivision;
        // start at the back of the ant for the 'ToDictionary' method that uses the fact that indexes are increasing
        var current = -180 - deltaTheta / 2f;
        for (int i = 0; i < _scannerSubdivision; i++)
        {
            _subdivisions.Add(current);
            current += deltaTheta;
        }
        _initialyzed = true;
    }

    public virtual void Scan()
    {
        _positionAtScanTime = _ant.CurrentPos.WorldPosition;
        _directionAtScanTime = _ant.BodyHeadAxis;
        var count = Physics.OverlapSphereNonAlloc(_positionAtScanTime, _scannerRadius, _colliders, ScanningMainLayer, QueryTriggerInteraction.Collide);

        _objects.Clear();
        for (int i = 0; i < count; i++)
        {
            var obj = _colliders[i].gameObject;
            if (IsInSight(obj))
                _objects.Add(obj);
        }
    }

    protected virtual Dictionary<int, List<GameObject>> ToDictionnary(List<GameObject> objects)
    {
        var result = new Dictionary<int, List<GameObject>>();

        for(int i = 0; i < _scannerSubdivision; i ++)
            result.Add(i, new List<GameObject>());

        var delta = 360f / _scannerSubdivision;
        foreach (var obj in objects)
        {
            var objDirection = obj.transform.position - _positionAtScanTime;
            var angle = Vector3.SignedAngle(_directionAtScanTime, objDirection, Vector3.up);
            if (_subdivisions.Any(t => t > angle))
                result[_subdivisions.FindIndex(t => t > angle) - 1].Add(obj);
            else if (360 + _subdivisions[0] > angle)
                result[_scannerSubdivision - 1].Add(obj);
            else
                result[0].Add(obj);
        }

        return result;
    }

    protected virtual bool IsInSight(GameObject obj)
    {
        var objPosition = obj.transform.position;
        var direction = objPosition - _positionAtScanTime;
        var floatDist = direction.x * direction.x + direction.y * direction.y + direction.z * direction.z;

        // Check on distance
        if (floatDist <= _apothem * _apothem || floatDist > _scannerRadius * _scannerRadius)
            return false;

        // Check on angle
        if (_checkVisionField && 2 * Mathf.Abs(Vector3.SignedAngle(_directionAtScanTime, direction, Vector3.up)) > _scannerAngle)
            return false;

        if (_checkObtruction == false)
            return true;

        // Check if no obstruction
        var fakePos = new Vector3(_positionAtScanTime.x, objPosition.y, _positionAtScanTime.z);
        direction.y = 0;
        if (Physics.Linecast(fakePos, direction, OcclusionLayer))
            return false;

        return true;
    }
}
