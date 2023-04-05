using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Curve : MonoBehaviour
{
    private Transform _curvePointPrefab;
    private LineRenderer _lineRenderer;
    private Color _color = Color.white;

    private List<Vector2> _realMeanCoordinates = new List<Vector2>();
    private List<Vector2> _realCoordinates = new List<Vector2>();

    private Transform _x0;
    private List<Transform> _meanPoints;
    private List<Transform> _queuePoints;
    private List<Transform> _pointsToDestroy;

    // Points used by LineRenderer
    private List<Transform> _curvePoints;

    private Vector2 _topRightPoint;
    private int _indexNumber;
    private int _maxQueueLength;
    private bool _initialyzed = false;
    private bool _rescaled;


    // Unity methods
    void Awake()
    {
        _curvePointPrefab = transform.GetChild(0);
        _lineRenderer = GetComponent<LineRenderer>();
        _curvePoints = new List<Transform>();
        _pointsToDestroy = new List<Transform>();
        _meanPoints = new List<Transform>();
        _queuePoints = new List<Transform>();
    }

    void Update()
    {
        if (_initialyzed == false)
            return;

        DrawCurve();
        DestroyUselessPoints();
    }


    public int GetQueueLength => _queuePoints.Count;

    public void Initialyze(int maxQueueLength, Vector2 topRightPoint, int indexNumber)
    {
        _maxQueueLength = maxQueueLength;
        _topRightPoint = topRightPoint;
        _indexNumber = indexNumber;
    }

    public void AddValue(Vector2 point)
    {
        if (_realCoordinates.Count == 0)
        {
            AddFirstPoint(point);
            return;
        }

        var newPoint = Instantiate(_curvePointPrefab, transform);
        var xPoint = (1f / _indexNumber) + 0.8f * GetQueueLength / (_maxQueueLength - 1);
        newPoint.GetComponent<RectTransform>().anchorMin = new Vector2(xPoint, point.y / _topRightPoint.y);
        newPoint.GetComponent<RectTransform>().anchorMax = new Vector2(xPoint, point.y / _topRightPoint.y);
        newPoint.name = $"x = {GetQueueLength + 1}";

        _realCoordinates.Add(point);
        _queuePoints.Add(newPoint);
    }

    public void ClearQueue()
    {
        var mean = 0f;
        for (int i = 0; i < _queuePoints.Count; i++)
        {
            var realCoordinateIndex = 1 + _meanPoints.Count * _maxQueueLength + i;
            _pointsToDestroy.Add(_queuePoints[i].transform);
            mean += _realCoordinates[realCoordinateIndex].y;
        }
        var meanCoordinates = new Vector2(_meanPoints.Count, mean / _queuePoints.Count);
        _realMeanCoordinates.Add(meanCoordinates);

        var meanPoint = Instantiate(_curvePointPrefab, transform);
        meanPoint.name = $"mean = {_meanPoints.Count}";
        _meanPoints.Add(meanPoint);
        _queuePoints.Clear();
        UpdateCurvePointsTransform();
    }

    public void SetTopRightPoint(Vector2 topRightPoint)
    {
        _topRightPoint = topRightPoint;
        UpdateCurvePointsTransform();
    }


    private void AddFirstPoint(Vector2 point)
    {
        var newPoint = Instantiate(_curvePointPrefab, transform);
        newPoint.GetComponent<RectTransform>().anchorMin = new Vector2(0, point.y / _topRightPoint.y);
        newPoint.GetComponent<RectTransform>().anchorMax = new Vector2(0, point.y / _topRightPoint.y);
        newPoint.name = $"Origin";

        _realCoordinates.Add(point);
        _x0 = newPoint;
        _curvePoints.Add(_x0);
        _initialyzed = true;
    }

    private void UpdateCurvePointsTransform()
    {
        if (_x0 != null)
        {
            _x0.GetComponent<RectTransform>().anchorMin = new Vector2(0, _realCoordinates[0].y / _topRightPoint.y);
            _x0.GetComponent<RectTransform>().anchorMax = new Vector2(0, _realCoordinates[0].y / _topRightPoint.y);
        }
        for (int i = 0; i < _meanPoints.Count; i++)
        {
            _meanPoints[i].GetComponent<RectTransform>().anchorMin = new Vector2((1f / _indexNumber) * (i + 1) / (_meanPoints.Count + 1), _realMeanCoordinates[i].y / _topRightPoint.y);
            _meanPoints[i].GetComponent<RectTransform>().anchorMax = new Vector2((1f / _indexNumber) * (i + 1) / (_meanPoints.Count + 1), _realMeanCoordinates[i].y / _topRightPoint.y);
        }
        for (int i = 0; i < GetQueueLength; i++)
        {
            var index = 1 + i + _maxQueueLength * _meanPoints.Count;
            var xPoint = (1f / _indexNumber) + 0.8f * i / (_maxQueueLength - 1);
            _queuePoints[i].GetComponent<RectTransform>().anchorMin = new Vector2(xPoint, _realCoordinates[index].y / _topRightPoint.y);
            _queuePoints[i].GetComponent<RectTransform>().anchorMax = new Vector2(xPoint, _realCoordinates[index].y / _topRightPoint.y);
        }
        _rescaled = true;
    }

    private void DrawCurve()
    {
        if (_rescaled)
        {
            _curvePoints.Clear();
            _curvePoints.Add(_x0);
            for (int i = 0; i < _meanPoints.Count; i++)
                _curvePoints.Add(_meanPoints[i]);
            for (int i = 0; i < GetQueueLength; i++)
                _curvePoints.Add(_queuePoints[i]);

            _rescaled = false;
        }
        else 
        {
            var pointNumber = 1 + _meanPoints.Count + GetQueueLength;
            if (_curvePoints.Count != pointNumber)
            {
                var minIndexNotYetAdded = GetQueueLength - (pointNumber - _curvePoints.Count);
                for (int i = minIndexNotYetAdded; i < GetQueueLength ; i++)
                    _curvePoints.Add(_queuePoints[i]);
            }
        }
        _lineRenderer.positionCount = _curvePoints.Count;
        for (int i = 0; i < _curvePoints.Count; i++)
            _lineRenderer.SetPosition(i, _curvePoints[i].localPosition);
    }

    private void DestroyUselessPoints()
    {
        if (_pointsToDestroy.Any() == false)
            return;
        for (int i = 0; i < _pointsToDestroy.Count; i++)
            Destroy(_pointsToDestroy[i].gameObject);

        _pointsToDestroy.Clear();
    }
}
