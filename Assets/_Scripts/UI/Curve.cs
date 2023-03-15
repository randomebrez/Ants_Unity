using System.Collections.Generic;
using UnityEngine;

public class Curve : MonoBehaviour
{
    private Transform _curvePointPrefab;
    private LineRenderer _lineRenderer;
    private Color _color = Color.white;

    private List<Vector2> _realCoordinates = new List<Vector2>();
    private List<Transform> _curvePoints;

    private Vector2 _topRightPoint = 5 * Vector2.one;
    private bool _dataMaxChanged;


    // Unity methods
    void Awake()
    {
        _curvePointPrefab = transform.GetChild(0);
        _lineRenderer = GetComponent<LineRenderer>();
        _curvePoints = new List<Transform>();
    }

    void Update()
    {
        if (_dataMaxChanged)
            DrawCurve();
        
        _lineRenderer.positionCount = _curvePoints.Count;
        for (int i = 0; i < _curvePoints.Count; i++)
            _lineRenderer.SetPosition(i, _curvePoints[i].localPosition);
    }


    public void SetDataMax(Vector2 dataMax)
    {
        _topRightPoint = dataMax;
        _dataMaxChanged = true;
    }

    public void AddValue(Vector2 point)
    {
        var newPoint = Instantiate(_curvePointPrefab, transform);
        newPoint.GetComponent<RectTransform>().anchorMin = new Vector2(point.x / _topRightPoint.x, point.y / _topRightPoint.y);
        newPoint.GetComponent<RectTransform>().anchorMax = new Vector2(point.x / _topRightPoint.x, point.y / _topRightPoint.y);
        newPoint.name = $"x = {_curvePoints.Count}";

        _realCoordinates.Add(point);
        _curvePoints.Add(newPoint);
    }


    private void DrawCurve()
    {
        for (int i = 0; i < _curvePoints.Count; i++)
        {
            _curvePoints[i].GetComponent<RectTransform>().anchorMin = new Vector2(_realCoordinates[i].x / _topRightPoint.x, _realCoordinates[i].y / _topRightPoint.y);
            _curvePoints[i].GetComponent<RectTransform>().anchorMax = new Vector2(_realCoordinates[i].x / _topRightPoint.x, _realCoordinates[i].y / _topRightPoint.y);
        }
        _dataMaxChanged = false;
    }
}
