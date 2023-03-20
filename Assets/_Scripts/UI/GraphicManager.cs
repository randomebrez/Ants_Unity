using System.Collections.Generic;
using UnityEngine;

public class GraphicManager : MonoBehaviour
{
    private Vector2 _maxAxisValue = 0.05f * Vector2.one;

    private float _xScale = 2f;
    private float _yScale = 1.1f;

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
        _xAxis.Initialyze(AxisEnum.Horizontal);
        _yAxis.Initialyze(AxisEnum.Vertical);
    }

    public void AddCurveValue(Vector2 point)
    {
        if (point.x > 0.8 * _maxAxisValue.x)
            _maxAxisValue.x = _xScale * point.x;
        if (point.y > 0.9 * _maxAxisValue.y)
            _maxAxisValue.y = _yScale * point.y;

        _curve.SetDataMax(_maxAxisValue);
        UpdateAxisIndexes(AxisEnum.Horizontal);
        UpdateAxisIndexes(AxisEnum.Vertical);

        _coordinates.Add(point);
        _curve.AddValue(point);
    }

    private void UpdateAxisIndexes(AxisEnum axisType)
    {
        var (axis, maxValue) = axisType == AxisEnum.Horizontal ? (_xAxis, _maxAxisValue.x) : (_yAxis, _maxAxisValue.y);
        var dict = new Dictionary<int, float>();
        for (int i = 0; i < 5; i++)
            dict.Add(i, (i +1) * maxValue / 5f);
        axis.UpdateIndexesValue(dict);
    }
}
