using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curve : MonoBehaviour
{
    private int _scale;
    private List<float> _xCoordinates = new List<float>();
    private List<float> _yCoordinates = new List<float>();
    private Color _color = Color.white;
    private LineRenderer _lineRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateStats(float xValue, float yValue)
    {
        _xCoordinates.Add(xValue);
        _yCoordinates.Add(yValue);
        _lineRenderer.positionCount++;
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, new Vector3(xValue, yValue, 0));
    }
}
