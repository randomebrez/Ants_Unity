using UnityEngine;

public class Axis : MonoBehaviour
{
    [SerializeField]
    Transform[] anchors;

    private LineRenderer _lineRenderer;

    // Unity methods
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }
    void Update()
    {
        _lineRenderer.positionCount = anchors.Length;
        for (int i = 0; i < anchors.Length; i++)
            _lineRenderer.SetPosition(i, anchors[i].position);
    }
}
