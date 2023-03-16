using System;
using TMPro;
using UnityEngine;

public class AxisIndexes : MonoBehaviour
{

    public Transform IndexPrefab;


    public void Initialyze(AxisEnum axisType, int indexNumber = 5)
    {
        var xShift = 1f / indexNumber;
        var yShift = 1f / indexNumber;
        switch(axisType)
        {
            case AxisEnum.Horizontal:
                yShift = 0;
                break;
            case AxisEnum.Vertical:
                xShift = 0;
                break;
        }
        for (int i = 0; i < indexNumber; i++)
        {
            var newIndex = Instantiate(IndexPrefab, transform);
            newIndex.GetComponent<RectTransform>().anchorMin += new Vector2((i + 1) * xShift, (i + 1) * yShift);
            newIndex.GetComponent<RectTransform>().anchorMax += new Vector2((i + 1) * xShift, (i + 1) * yShift);
            newIndex.GetChild(0).GetComponent<TextMeshPro>().text = $"{i + 1}";
            newIndex.name = $"Index_{i}";
        }
    }

    public void UpdateIndexValue(int index, float value)
    {
        var indexTransform = transform.GetChild(index + 1).GetChild(0);
        var textComponent = indexTransform.GetComponent<TextMeshPro>();
        textComponent.text = Math.Round(value, 2).ToString();
    }
}
