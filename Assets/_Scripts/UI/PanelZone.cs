using UnityEngine;

public class PanelZone : MonoBehaviour
{
    public GraphicManager GraphicPrefab;

    private GraphicManager _graphicInstance;
    private RectTransform _canvasTransform;
    private RectTransform _panelTransform;

    // Unity methods
    void Awake()
    {
        _canvasTransform = GetComponent<RectTransform>();
        _panelTransform = _canvasTransform.GetChild(0).GetComponent<RectTransform>();
        _graphicInstance = _canvasTransform.GetChild(0).GetComponent<GraphicManager>();
    }

    public void Start()
    {
        _graphicInstance = Instantiate(GraphicPrefab, _canvasTransform);//.position, Quaternion.identity, _panelTransform);
        _graphicInstance.Initialyze(6);
    }


    public void AddCurveValue(float xValue, float yValue)
    {
        _graphicInstance.AddCurveValue(xValue, yValue);
    }

    public void SetAnchors(float yMin, float yMax)
    {
        _canvasTransform.anchorMin = new Vector2(0, yMin);
        _canvasTransform.anchorMax = new Vector2(1, yMax);
    }
}
