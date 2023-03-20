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

    // Base scanner parameters
    private float _scanInterval;
    private List<float> _subdivisions = new List<float>();

    protected bool _initialyzed = false;
    protected int _scannerSubdivision;
    protected abstract float _scannerAngle { get; }
    protected abstract float _scannerRadius { get; }
    protected abstract bool _checkObtruction { get; }

    protected int _scannerSurface { get; private set; }

    protected float _apothem => (float)Math.Sqrt(3) * GlobalParameters.NodeRadius / 2;

    // Colliders container
    private List<GameObject> _objects = new List<GameObject>();
    private Collider[] _colliders = new Collider[150];


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

    public void Initialyze(BaseAnt ant, int scannerSubdivision, int scanFrequency)
    {
        _ant = ant;
        _scannerSubdivision = scannerSubdivision;
        _scanInterval = 1.0f / scanFrequency;

        var colliders = new Collider[70];
        Physics.OverlapSphereNonAlloc(transform.position, _scannerRadius, colliders, LayerMask.GetMask(Layer.Walkable.ToString()));
        var blockCount = colliders.Where(t => t != null).Count();
        var temp = (blockCount - 1)/ _scannerSubdivision;

        //var scannerSurface = Mathf.PI * _scannerRadius * _scannerRadius;
        //var nodeSurface = 2 * Mathf.Sqrt(3) * GlobalParameters.NodeRadius * GlobalParameters.NodeRadius;
        //var temp = scannerSurface / nodeSurface;


        _scannerSurface = (int)temp;

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
        var count = Physics.OverlapSphereNonAlloc(transform.position, _scannerRadius, _colliders, ScanningMainLayer, QueryTriggerInteraction.Collide);

        _objects.Clear();
        for (int i = 0; i < count; i++)
        {
            var obj = _colliders[i].gameObject;
            if (IsInSight(obj))
                _objects.Add(obj);
        }
    }

    private Dictionary<int, List<GameObject>> ToDictionnary(List<GameObject> objects)
    {
        var result = new Dictionary<int, List<GameObject>>();

        for(int i = 0; i < _scannerSubdivision; i ++)
            result.Add(i, new List<GameObject>());

        var delta = 360f / _scannerSubdivision;
        foreach (var obj in objects)
        {
            var objDirection = obj.transform.position - transform.position;
            var angle = Vector3.SignedAngle(_ant.BodyHeadAxis, objDirection, Vector3.up);

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
        var position = transform.position;
        var objPosition = obj.transform.position;
        var direction = objPosition - position;

        // Check on angle
        if (2 * Mathf.Abs(Vector3.SignedAngle(_ant.BodyHeadAxis, direction, Vector3.up)) > _scannerAngle)
            return false;

        if (_checkObtruction == false)
            return true;

        // Check if no obstruction
        position.y += objPosition.y;
        direction.y = 0;
        if (Physics.Linecast(position, direction, OcclusionLayer))
            return false;

        return true;
    }
}
