using UnityEngine;

public class PanelZone : MonoBehaviour
{
    public GraphicManager GraphicPrefab;

    private GraphicManager _graphicInstance;
    private RectTransform _canvasTransform;

    // Unity methods
    void Awake()
    {
        _canvasTransform = GetComponent<RectTransform>();
        _graphicInstance = _canvasTransform.GetChild(0).GetComponent<GraphicManager>();
    }

    public void Start()
    {
        _graphicInstance = Instantiate(GraphicPrefab, _canvasTransform);
    }


    public void AddCurveValue(Vector2 point)
    {
        _graphicInstance.AddCurveValue(point);
    }

    public void SetAnchors(float yMin, float yMax)
    {
        _canvasTransform.anchorMin = new Vector2(0, yMin);
        _canvasTransform.anchorMax = new Vector2(1, yMax);
    }
}
