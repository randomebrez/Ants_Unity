using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TemplateDetail : MonoBehaviour, IPointerDownHandler
{
    public ScrollBar InputScrollBar;

    public ScrollBar NeutralScrollBar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnInputLayerClick()
    {
        Debug.Log($"Mouse was clicked on InputLayer");
    }

    private void OnOutputLayerClick()
    {
        Debug.Log($"Mouse was clicked on OutputLayer");
    }

    private void OnNeutralLayersClick()
    {
        Debug.Log($"Mouse was clicked on NeutralLayers");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        switch(eventData.pointerCurrentRaycast.gameObject.name)
        {
            case "InputLayer":
                OnInputLayerClick();
                break;
            case "InputAddBtn":
                OnInputAddBtnClick();
                break;
            case "NeutralLayers":
                OnNeutralLayersClick();
                break;
            case "OutputLayer":
                OnOutputLayerClick();
                break;
        }
        
    }


    // Button clicked
    public void OnInputAddBtnClick()
    {
        var testDict = new Dictionary<string, string>
        {
            { "Oui", "Non" },
            { "Vrai", "Faux" },
            { "Carrement", "153" },
        };
        InputScrollBar.AddItem(testDict);
        Debug.Log($"Mouse was clicked on InputAddBtn");
    }
    public void OnNeutralLayerAddBtnClick()
    {
        var testDict = new Dictionary<string, string>
        {

        };
        NeutralScrollBar.AddItem(testDict);
        Debug.Log($"Mouse was clicked on NeutralLayerAddBtn");
    }
}
