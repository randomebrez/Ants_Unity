using System.Collections.Generic;
using UnityEngine;

public class SidePanels : MonoBehaviour
{
    // Unity Editor fields
    public PanelZone PanelZonePrefab;


    private RectTransform _rightPanel;
    private RectTransform _leftPanel;
    private Dictionary<int, PanelZone> _canvasZones = new Dictionary<int, PanelZone>();

    private float _highScoreZoneHeight;


    // Unity methods
    void Start()
    {
        _leftPanel = transform.GetChild(0).GetComponent<RectTransform>();
        _rightPanel = transform.GetChild(1).GetComponent<RectTransform>();
    }

    public void SplitZone(int zoneNumber)
    {
        _highScoreZoneHeight = (int)_rightPanel.rect.height / 1.5f;
        var zoneHeight = (int)((2 * _rightPanel.rect.height - _highScoreZoneHeight)/ zoneNumber);

        var numberOfZoneInRightPanel = (_rightPanel.rect.height - _highScoreZoneHeight) / zoneHeight;
        // If we're close to put 2 zones (for instance) in right panel. Reduce a bit the zone height to fill correctly the right panel
        if (numberOfZoneInRightPanel - Mathf.Floor(numberOfZoneInRightPanel) > 0.8f)
            numberOfZoneInRightPanel = Mathf.Ceil(numberOfZoneInRightPanel);
        else
            numberOfZoneInRightPanel = Mathf.Floor(numberOfZoneInRightPanel);

        var rightPanelZoneHeight = (int)Mathf.Min(zoneHeight, (int)((_rightPanel.rect.height - _highScoreZoneHeight) / numberOfZoneInRightPanel));
        var leftPanelZoneHeight = (int)(_leftPanel.rect.height / (zoneNumber - numberOfZoneInRightPanel));

        var resetOffSet = true;
        var offSet = _highScoreZoneHeight;
        for (int i = 0; i < zoneNumber; i++)
        {
            RectTransform parent = _leftPanel;
            zoneHeight = leftPanelZoneHeight;
            if (i < (int)numberOfZoneInRightPanel)
            {
                zoneHeight = rightPanelZoneHeight;
                parent = _rightPanel;
            }
            else if (resetOffSet)
            {
                resetOffSet = false;
                offSet = 0;
            }

            var newPanel = Instantiate(PanelZonePrefab, parent.transform);
            newPanel.SetAnchors(offSet / parent.rect.height, (zoneHeight + offSet) / parent.rect.height);

            _canvasZones.Add(i, newPanel);

            offSet += zoneHeight;
        }
    }

    public void AddCurveValue(int graphicIndex, Vector2  point)
    {
        _canvasZones[graphicIndex].AddCurveValue(point);
    }
}