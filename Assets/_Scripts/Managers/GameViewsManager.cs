using System.Collections.Generic;
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
    }

    public void Initialyze(List<string> titles)
    {
        _sidePanels.SplitZone(titles);
    }

    public void AddCurveValue(int zoneIndex, Vector2 point)
    {
        _sidePanels.AddCurveValue(zoneIndex, point);
    }
}
