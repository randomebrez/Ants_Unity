using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GraphicManager : MonoBehaviour
{
    // X axis
    private Transform _xAxis;
    private float _xDimension;
    private float _xShift;

    // Y Axis
    private Transform _yAxis;
    private float _yDimension;
    private float _yShift;

    // Data
    private LineRenderer _curve;
    private List<float> _xCoordinates = new List<float>();
    private List<float> _yCoordinates = new List<float>();

    void Awake()
    {
        _xAxis = transform.GetChild(0);
        _yAxis = transform.GetChild(1);
        _curve = transform.GetChild(2).GetComponent<LineRenderer>();
    }

    public void Initialyze(float marging)
    {
        var rectTransform = transform.GetComponent<RectTransform>();
        _xDimension = rectTransform.rect.width;
        _xShift = rectTransform.rect.width / 5f;

        _yDimension = rectTransform.rect.height;
        _yShift = rectTransform.rect.height / 5f;

        SetupAxis();
    }

    private void SetupAxis()
    {
        // Setup x axis
        var xAxis = _xAxis.GetComponent<LineRenderer>();
        xAxis.positionCount = 2;
        xAxis.SetPosition(0, Vector3.zero);
        xAxis.SetPosition(1, 5 * _xShift * Vector3.right);

        // Setup y axis
        var yAxis = _yAxis.GetComponent<LineRenderer>();
        yAxis.positionCount = 2;
        yAxis.SetPosition(0, Vector3.zero);
        yAxis.SetPosition(1, 5 * _yShift * Vector3.up);
    }

    public void AddCurveValue(float xValue, float yValue)
    {
        if (_xCoordinates.Count == 0)
        {
            _xCoordinates.Add(0);
            _yCoordinates.Add(0);
        }    

        _xCoordinates.Add(xValue);
        _yCoordinates.Add(yValue);
        var updateShifts = UpdateShifts(xValue, yValue);
        if (updateShifts)
            DrawCurve();
        else
            AddLastValuesToCurve();
    }

    private bool UpdateShifts(float xValue, float yValue)
    {
        var updateShifts = false;
        if (xValue * _xShift > 0.8 * _xDimension)
        {
            _xShift = (2 * _xDimension) / (5 * xValue);
            updateShifts = true;
        }
        if (yValue * _yShift > 0.8 * _yDimension)
        {
            _yShift = (2 * _yDimension) / (5 * yValue);
            updateShifts = true;
        }

        return updateShifts;
    }

    private void DrawCurve()
    {
        _curve.positionCount = _xCoordinates.Count;
        for (int i = 0; i < _xCoordinates.Count; i++)
        {
            var point = _xCoordinates[i] * _xShift * Vector3.right + _yCoordinates[i] * _yShift * Vector3.up;
            _curve.SetPosition(i, point);
        }        
    }

    private void AddLastValuesToCurve()
    {
        var point = _xCoordinates[_xCoordinates.Count - 1] * _xShift * Vector3.right + _yCoordinates[_yCoordinates.Count - 1] * _yShift * Vector3.up;
        _curve.positionCount++;
        _curve.SetPosition(_curve.positionCount - 1, point);
    }
}
