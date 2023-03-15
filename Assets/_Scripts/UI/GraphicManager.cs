using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GraphicManager : MonoBehaviour
{
    private Vector2 _maxAxisValue = 5 * Vector2.one;

    // Data
    private Curve _curve;
    private List<Vector2> _coordinates = new List<Vector2>();

    void Awake()
    {
        _curve = transform.GetChild(2).GetComponent<Curve>();
    }

    public void AddCurveValue(Vector2 point)
    {
        if (point.x > 0.8 * _maxAxisValue.x)
            _maxAxisValue.x = 2.5f * point.x;
        if (point.y > 0.8 * _maxAxisValue.y)
            _maxAxisValue.y = 2.5f * point.y;

        _curve.SetDataMax(_maxAxisValue);

        _coordinates.Add(point);
        _curve.AddValue(point);
    }
}
