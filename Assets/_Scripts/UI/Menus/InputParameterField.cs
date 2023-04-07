using Assets.Dtos;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class InputParameterField : MonoBehaviour
{
    private string _description;
    private TextMeshProUGUI _inputDescription;
    private string _initialValue;
    private TMP_InputField _inputValue;
    private InputParameterConstraint _valueConstraint;

    public bool Edited = false;

    // Start is called before the first frame update
    void Start()
    {
        _inputDescription = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        _inputDescription.text = _description;
        _inputValue = transform.GetChild(0).GetChild(0).GetComponent<TMP_InputField>();
        _inputValue.text = _initialValue;
    }

    public void Initialyze(string description, string initialValue, InputParameterConstraint constraint)
    {
        _description = description;
        _initialValue = initialValue;
        _valueConstraint = constraint;
    }

    public string GetParameterValue()
    {
        return _inputValue.text;
    }

    public void OnEndEditing()
    {
        // Save temp value
        CheckConstraint();
        Edited = _inputValue.text != _initialValue;
    }

    private void CheckConstraint()
    {
        var valid = string.IsNullOrEmpty(_inputValue.text) == false;
        
        if (valid && string.IsNullOrEmpty(_valueConstraint.RegExp) ==  false)
            valid = Regex.Match(_inputValue.text, _valueConstraint.RegExp).Success;
        else if (valid && int.TryParse(_inputValue.text, out var intValue))
            valid = intValue >= _valueConstraint.MinValue && intValue <= _valueConstraint.MaxValue;

        if (valid == false)
            _inputValue.text = _initialValue;
    }
}
