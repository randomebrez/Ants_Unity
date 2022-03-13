using System.Linq;
using System.Collections.Generic;
using mew;
using UnityEngine;

public abstract class AntScannerBase : MonoBehaviour
{
    private bool _initialyzed = false;

    [SerializeField]
    protected LayerMask ScanningMainLayer;
    [SerializeField]
    protected LayerMask OcclusionLayer;
    public Color color;

    protected int _scannerSubdivision;
    private List<float> _subdivisions = new List<float>();

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
    private List<GameObject> _objects = new List<GameObject>();
    private Collider[] _colliders = new Collider[150];

    private int _count;

    protected BaseAnt _ant;
    protected abstract float ScannerAngle { get; }

    protected int ScanFrequency = 100;
    protected float _scanInterval;
    protected float _scanTimer;
    protected abstract bool CheckObtruction { get; }
    protected float _height = 0.1f;
    protected abstract float ScannerRadius { get; }

    public int GetCount => _count;

    public abstract float GetPortionValue(int index);

    public void Initialyze(BaseAnt ant, int scannerSubdivision)
    {
        _ant = ant;
        _scannerSubdivision = scannerSubdivision;

        float deltaTheta = 360 / _scannerSubdivision;
        var current = -180f;
        for (int i = 0; i < _scannerSubdivision; i++)
        {
            _subdivisions.Add(current);
            current += deltaTheta;
        }

        _initialyzed = true;
    }

    private void Start()
    {
        _scanInterval = 1.0f / ScanFrequency;
    }

    private void Update()
    {
        if (_initialyzed == false)
            return;

        _scanTimer -= Time.deltaTime;
        if (_scanTimer < 0)
        {
            _scanTimer += _scanInterval;
            Scan();
        }
    }



    protected virtual void Scan()
    {
        var count = Physics.OverlapSphereNonAlloc(transform.position, ScannerRadius, _colliders, ScanningMainLayer, QueryTriggerInteraction.Collide);

        _count = 0;
        _objects.Clear();
        for (int i = 0; i < count; i++)
        {
            var obj = _colliders[i].gameObject;
            if (IsInSight(obj))
            {
                _count++;
                _objects.Add(obj);
            }
        }
    }

    private Dictionary<int, List<GameObject>> ToDictionnary(List<GameObject> objects)
    {
        var result = new Dictionary<int, List<GameObject>>();

        for(int i = 0; i < _scannerSubdivision; i ++)
            result.Add(i, new List<GameObject>());


        var delta = 360f / _scannerSubdivision;
        var position = transform.position;
        foreach (var obj in objects)
        {
            var objPosition = obj.transform.position;
            var direction = objPosition - position;
            var angle = Vector3.SignedAngle(_ant.BodyHeadAxis, direction, Vector3.up);

            var portionIndex = _subdivisions.Any(t => t > angle) ? _subdivisions.FindIndex(t => t > angle): _subdivisions.Count;
            result[portionIndex - 1].Add(obj);
        }

        return result;
    }

    protected virtual bool IsInSight(GameObject obj)
    {
        var position = transform.position;
        var objPosition = obj.transform.position;
        var direction = objPosition - position;

        // Check on angle
        if (Mathf.Abs(Vector3.SignedAngle(_ant.BodyHeadAxis, direction, Vector3.up)) > ScannerAngle)
            return false;

        if (CheckObtruction == false)
            return true;

        // Check if no obstruction
        position.y += _height / 2;
        direction.y = position.y;
        if (Physics.Linecast(position, direction, OcclusionLayer))
            return false;

        return true;
    }
}
