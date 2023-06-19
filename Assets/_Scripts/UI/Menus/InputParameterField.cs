using Assets.Dtos;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class InputParameterField : MonoBehaviour
{
    private string _description;
    private TextMeshProUGUI _inputDescription;
    private string _initialValue;
    private TMP_InputField _inputValue;
    private TMP_Dropdown _inputDropdownValue;
    private SimulationParameterConstraint _valueConstraint;

    private bool _useDropDown;

    public bool Edited = false;

    // Strange thing Initialyze method is called before Start() (even before Awake())
    void Start()
    {
        _inputDescription = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        _inputDescription.text = _description;
        _inputValue = transform.GetChild(0).GetChild(0).GetComponent<TMP_InputField>();
        _inputDropdownValue = transform.GetChild(0).GetChild(2).GetComponent<TMP_Dropdown>();
        

        // That is why we do that here (cf top comment)
        _useDropDown = _valueConstraint.PossibleValues.Count > 0;
        _inputValue.gameObject.SetActive(!_useDropDown);
        _inputDropdownValue.gameObject.SetActive(_useDropDown);

        if (_useDropDown)
        {
            _inputDropdownValue.AddOptions(_valueConstraint.PossibleValues.ToList());
            _inputDropdownValue.SetValueWithoutNotify(_inputDropdownValue.options.FindIndex(t => t.text == _initialValue));
        }
        else
            _inputValue.text = _initialValue;
    }

    public void Initialyze(string description, string initialValue, SimulationParameterConstraint constraint)
    {
        _description = description;
        _initialValue = initialValue;
        _valueConstraint = constraint;
    }

    public string GetParameterValue()
    {
        if (_useDropDown)
            return _inputDropdownValue.captionText.text;
        else
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
        var valid = string.IsNullOrEmpty(GetParameterValue()) == false;
        
        if (valid && string.IsNullOrEmpty(_valueConstraint.RegExp) ==  false)
            valid = Regex.Match(GetParameterValue(), _valueConstraint.RegExp).Success;
        else if (valid && int.TryParse(GetParameterValue(), out var intValue))
            valid = intValue >= _valueConstraint.MinValue && intValue <= _valueConstraint.MaxValue;
        else if (valid && _valueConstraint.PossibleValues.Count > 0)
            valid = _valueConstraint.PossibleValues.Contains(GetParameterValue());

        if (valid == false && _useDropDown == false)
            _inputValue.text = _initialValue;
    }
}
