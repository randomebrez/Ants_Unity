using TMPro;
using UnityEngine;

public class InputParameterField : MonoBehaviour
{
    private TextMeshProUGUI _inputDescription;
    private TMP_InputField _inputValue;
    private string _initialValue;

    public bool Edited = false;

    // Start is called before the first frame update
    void Awake()
    {
        _inputDescription = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        _inputValue = transform.GetChild(0).GetChild(0).GetComponent<TMP_InputField>();
    }

    public void Initialyze(string description, string initialValue)
    {
        _inputDescription.text = description;
        _initialValue = initialValue;
    }

    public string GetParameterValue()
    {
        return _inputValue.text;
    }

    public void OnEndEditing()
    {
        // Save temp value
        Edited = _inputValue.text != _initialValue;
    }
}
