using System.Collections.Generic;
using UnityEngine;

public enum AxisEnum
{
    Horizontal,
    Vertical
}

public class Axis : MonoBehaviour
{
    [SerializeField]
    Transform[] anchors;

    private LineRenderer _lineRenderer;
    private AxisIndexes _indexes;

    // Unity methods
    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _indexes = transform.GetChild(2).GetComponent<AxisIndexes>();
    }

    void Update()
    {
        _lineRenderer.positionCount = anchors.Length;
        for (int i = 0; i < anchors.Length; i++)
            _lineRenderer.SetPosition(i, anchors[i].position);
    }

    public void Initialyze(AxisEnum axisType)
    {
        _indexes.Initialyze(axisType);
    }

    public void UpdateIndexesValue(Dictionary<int, float> newValues)
    {
        foreach (var pair in newValues)
            _indexes.UpdateIndexValue(pair.Key, pair.Value);
    }
}
