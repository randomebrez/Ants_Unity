using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axis : MonoBehaviour
{
    [SerializeField]
    Transform[] anchors;

    private LineRenderer _lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        _lineRenderer.positionCount = anchors.Length;
        for (int i = 0; i < anchors.Length; i++)
            _lineRenderer.SetPosition(i, anchors[i].position);
    }
}
