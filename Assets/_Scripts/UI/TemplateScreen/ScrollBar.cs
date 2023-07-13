using System.Collections.Generic;
using UnityEngine;

public class ScrollBar : MonoBehaviour
{
    public RectTransform Content;
    public DetailBlock ItemTemplate;
    public int DisplayRowNumber;

    private List<RectTransform> _items;
    // Start is called before the first frame update
    void Start()
    {
        _items = new List<RectTransform>();
        var windowSize = Content.parent.GetComponent<RectTransform>().rect;
        Content.sizeDelta = new Vector2(0, windowSize.height);
    }

    public void AddItem(Dictionary<string, string> fields)
    {
        var item = Instantiate(ItemTemplate, Content);
        item.Initialyze(fields);
        _items.Add(item.GetComponent<RectTransform>());
        UpdateContentHeight();
        UpdateItemsAnchors();
    }

    private void UpdateContentHeight()
    {
        var windowSize = Content.parent.GetComponent<RectTransform>().rect;
        var alpha = _items.Count / (float)DisplayRowNumber;
        var currentSize = Content.sizeDelta;
        if (alpha > 1)
            Content.sizeDelta = new Vector2(0, alpha * windowSize.height);
    }

    private void UpdateItemsAnchors()
    {
        float itemCount = _items.Count;

        for(int i = 0; i < _items.Count; i++)
        {
            var item = _items[i];
            item.anchorMin = new Vector2(0, 1 - (i + 1) / Mathf.Max(DisplayRowNumber, itemCount));
            item.anchorMax = new Vector2(1, 1 - i / Mathf.Max(DisplayRowNumber, itemCount));
            item.gameObject.SetActive(true);
        }
    }
}
