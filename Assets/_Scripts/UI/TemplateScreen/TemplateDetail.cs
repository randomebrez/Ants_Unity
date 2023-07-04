using UnityEngine;
using UnityEngine.EventSystems;

public class TemplateDetail : MonoBehaviour, IPointerDownHandler
{
    public GameObject InputBlock;
    public ScrollBar InputScrollBar;

    public GameObject NeutralBlock;
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
        InputScrollBar.AddItem(InputBlock);
        Debug.Log($"Mouse was clicked on InputAddBtn");
    }
    public void OnNeutralLayerAddBtnClick()
    {
        NeutralScrollBar.AddItem(NeutralBlock);
        Debug.Log($"Mouse was clicked on NeutralLayerAddBtn");
    }
}
