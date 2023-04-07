using Assets.Dtos;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ParametersMenu : MonoBehaviour
{
    private Dictionary<SimulationParameterTypeEnum, InputParameterField> _simulationParameters = new Dictionary<SimulationParameterTypeEnum, InputParameterField>();
    private Dictionary<NeuralNetworkParameterTypeEnum, InputParameterField> _neuralNetworkParameters = new Dictionary<NeuralNetworkParameterTypeEnum, InputParameterField>();
    private Dictionary<AntCaracteristicsParameterTypeEnum, InputParameterField> _antCaracteristicsParameters = new Dictionary<AntCaracteristicsParameterTypeEnum, InputParameterField>();

    public Transform SimulationPanel;
    public Transform NeuralNetworkPanel;
    public Transform AntCaracteristicsPanel;

    public InputParameterField InputParameterPrefab;


    public event EventHandler<EventArgs> SwitchScreen;

    protected virtual void OnSwitchScreen()
    {
        SwitchScreen?.Invoke(this, EventArgs.Empty);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Initialyze()
    {
        SimulationParametersInitialyze();
        NeuralNetworkParametersInitialyze();
        AntCaracteristicsParametersInitialyze();
    }


    public void OnMenuClick()
    {
        OnSwitchScreen();
    }

    public void OnSaveClick()
    {
        SimulationParametersSave();
        NeuralNetworkParametersSave();
        AntCaracteristicsParametersSave();
    }


    private void SimulationParametersInitialyze()
    {
        var parameters = ParametersManager.Instance.SimulationParametersGet();
        var itemSize = Mathf.Min(0.15f, 1f / parameters.Count);
        var yAnchor = 1f;
        foreach (var pair in parameters)
        {
            var item = Instantiate(InputParameterPrefab, SimulationPanel);            
            item.GetComponent<RectTransform>().anchorMin = new Vector2(0, yAnchor - itemSize);
            item.GetComponent<RectTransform>().anchorMax = new Vector2(1, yAnchor);
            item.Initialyze(pair.Key.ToString(), pair.Value.value, pair.Value.valueConstraint);
            yAnchor -= itemSize;
            _simulationParameters.Add(pair.Key, item);
        }
    }

    private void SimulationParametersSave()
    {
        var valueDict = new Dictionary<SimulationParameterTypeEnum, string>();
        foreach (var pair in _simulationParameters)
            valueDict.Add(pair.Key, pair.Value.GetParameterValue());
        ParametersManager.Instance.SimulationParametersSave(valueDict);
    }

    private void NeuralNetworkParametersInitialyze()
    {
        var parameters = ParametersManager.Instance.NeuralNetworkParametersGet();
        var itemSize = Mathf.Min(0.15f, 1f / parameters.Count);
        var yAnchor = 1f;
        foreach (var pair in parameters)
        {
            var item = Instantiate(InputParameterPrefab, NeuralNetworkPanel);            
            item.GetComponent<RectTransform>().anchorMin = new Vector2(0, yAnchor - itemSize);
            item.GetComponent<RectTransform>().anchorMax = new Vector2(1, yAnchor);
            item.Initialyze(pair.Key.ToString(), pair.Value.value, pair.Value.valueConstraint);
            yAnchor -= itemSize;
            _neuralNetworkParameters.Add(pair.Key, item);
        }
    }

    private void NeuralNetworkParametersSave()
    {
        var valueDict = new Dictionary<NeuralNetworkParameterTypeEnum, string>();
        foreach (var pair in _neuralNetworkParameters)
            valueDict.Add(pair.Key, pair.Value.GetParameterValue());
        ParametersManager.Instance.NeuralNetworkParametersSave(valueDict);
    }

    private void AntCaracteristicsParametersInitialyze()
    {
        var parameters = ParametersManager.Instance.AntCaracteristicsParametersGet();
        var itemSize = Mathf.Min(0.15f, 1f / parameters.Count);
        var yAnchor = 1f;
        foreach (var pair in parameters)
        {
            var item = Instantiate(InputParameterPrefab, AntCaracteristicsPanel);
            item.GetComponent<RectTransform>().anchorMin = new Vector2(0, yAnchor - itemSize);
            item.GetComponent<RectTransform>().anchorMax = new Vector2(1, yAnchor);
            item.Initialyze(pair.Key.ToString(), pair.Value.value, pair.Value.valueConstraint);
            yAnchor -= itemSize;
            _antCaracteristicsParameters.Add(pair.Key, item);
        }
    }

    private void AntCaracteristicsParametersSave()
    {
        var valueDict = new Dictionary<AntCaracteristicsParameterTypeEnum, string>();
        foreach (var pair in _antCaracteristicsParameters)
            valueDict.Add(pair.Key, pair.Value.GetParameterValue());
        ParametersManager.Instance.AntCaracteristicsParametersSave(valueDict);
    }
}
