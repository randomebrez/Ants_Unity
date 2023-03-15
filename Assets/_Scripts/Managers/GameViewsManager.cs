using UnityEngine;

public class GameViewsManager : MonoBehaviour
{
    private SidePanels _sidePanels;
    public Transform MainView { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        MainView = transform.GetChild(0);
        _sidePanels = transform.GetChild(1).GetComponent<SidePanels>();
    }

    public void SetCamera(Camera camera)
    {
        var cameraGo = GetComponent<Canvas>();
        cameraGo.worldCamera = camera;
        cameraGo.planeDistance = 160;
    }

    public void Initialyze(int sideZoneNumber)
    {
        _sidePanels.SplitZone(sideZoneNumber);
    }

    public void AddCurveValue(int zoneIndex, float xValue, float yValue)
    {
        _sidePanels.AddCurveValue(zoneIndex, xValue, yValue);
    }
}
