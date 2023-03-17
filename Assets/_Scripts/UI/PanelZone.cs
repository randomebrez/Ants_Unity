using TMPro;
using UnityEngine;

public class PanelZone : MonoBehaviour
{
    public GraphicManager GraphicPrefab;

    private GraphicManager _graphicInstance;
    private RectTransform _canvasTransform;
    private Transform _panelTransform;

    public Transform MainView;

    // Unity methods
    void Awake()
    {
        _canvasTransform = GetComponent<RectTransform>();
        _panelTransform = transform.GetChild(0);
        MainView = _panelTransform.GetChild(0);
    }


    public void InstantiateInMainView(string title)
    {
        var titleText = _panelTransform.GetChild(1).GetChild(0);
        titleText.GetComponent<TextMeshProUGUI>().text = title;
        _graphicInstance = Instantiate(GraphicPrefab, MainView);
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
