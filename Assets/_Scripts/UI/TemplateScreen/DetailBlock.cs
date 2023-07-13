using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DetailBlock : MonoBehaviour
{
    public RectTransform FieldPanelTemplate;
    private Dictionary<string, string> _fields;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Initialyze(Dictionary<string, string> fields)
    {
        _fields = fields;
        DisplayFields();
    }

    private void DisplayFields()
    {
        var xAnchorMin = 0f;
        var deltaAnchor = 1f / _fields.Count;
        foreach(var field in _fields)
        {
            var fieldGo = Instantiate(FieldPanelTemplate, transform);
            fieldGo.name = $"{field.Key}";
            fieldGo.GetComponent<RectTransform>().anchorMin = new Vector2(xAnchorMin, 0.2f);
            fieldGo.GetComponent<RectTransform>().anchorMax = new Vector2(xAnchorMin + deltaAnchor, 0.8f);
            var name = fieldGo.GetChild(0);
            var value = fieldGo.GetChild(1);
            fieldGo.GetChild(0).GetComponent<TextMeshProUGUI>().text = field.Key;
            fieldGo.GetChild(1).GetComponent<TextMeshProUGUI>().text = field.Value;
            fieldGo.gameObject.SetActive(true);

            xAnchorMin += deltaAnchor;
        }
    }
}
