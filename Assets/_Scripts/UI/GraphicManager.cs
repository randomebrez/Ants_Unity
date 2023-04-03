using System.Collections.Generic;
using UnityEngine;

public class GraphicManager : MonoBehaviour
{
    private Vector2 _xMinMax = new Vector2(1, 9);
    private Vector2 _yMinMax = new Vector2(1, 0.2f);
    private int _axisIndexNumber = 5;

    private int _xShift = 2;
    private float _yScale = 1.1f;
    private int _numberToAverage;

    private Axis _xAxis;
    private Axis _yAxis;
    private Curve _curve;
    private List<Vector2> _coordinates = new List<Vector2>();

    void Awake()
    {
        _curve = transform.GetChild(2).GetComponent<Curve>();
        _xAxis = transform.GetChild(0).GetComponent<Axis>();
        _yAxis = transform.GetChild(1).GetComponent<Axis>();
    }

    void Start()
    {
        _xAxis.Initialyze(AxisEnum.Horizontal, _axisIndexNumber, _xShift);
        _yAxis.Initialyze(AxisEnum.Vertical, _axisIndexNumber, _yMinMax.y / _axisIndexNumber);
        _numberToAverage = 1 + (_axisIndexNumber - 1) * _xShift;
        _curve.Initialyze(_numberToAverage, new Vector2(_xMinMax.y, _yMinMax.y), _axisIndexNumber);
    }

    public void AddCurveValue(Vector2 point)
    {
        _coordinates.Add(point);
        // Handle xAxis
        if (_curve.GetQueueLength == _numberToAverage)
        {
            _curve.ClearQueue();
            _xMinMax.x = _xMinMax.y + 1;
            _xMinMax.y = _xMinMax.x + (_axisIndexNumber - 1) * _xShift;
            UpdateHorizontalIndexes();
        }
        // Handle yAxis
        if (point.y > 0.9 * _yMinMax.y)
        {
            _yMinMax.y = _yScale * point.y;
            UpdateVerticalIndexes();
            _curve.SetTopRightPoint(new Vector2(_xMinMax.y, _yMinMax.y));
        }
        _curve.AddValue(point);
    }

    private void UpdateHorizontalIndexes()
    {
        var dict = new Dictionary<int, float>();

        for (int i = 0; i < _axisIndexNumber; i++)
            dict.Add(i, _xMinMax.x + i * _xShift);
        _xAxis.UpdateIndexesValue(dict);
    }

    private void UpdateVerticalIndexes()
    {
        var dict = new Dictionary<int, float>();
        for (int i = 0; i < _axisIndexNumber; i++)
            dict.Add(i, (i + 1) * _yMinMax.y / _axisIndexNumber);
        _yAxis.UpdateIndexesValue(dict);
    }
}
