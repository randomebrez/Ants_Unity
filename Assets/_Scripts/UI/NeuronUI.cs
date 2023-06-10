using NeuralNetwork.Abstraction.Model;
using UnityEngine;

public class NeuronUI : MonoBehaviour
{
    private Neuron _neuron;
    private bool _initialyzed = false;
    private Material _material;
    public float Value;

    private void Awake()
    {
        _material = GetComponentInChildren<MeshRenderer>().material;
        Debug.Log("Awake");
    }

    // Update is called once per frame
    void Update()
    {
        if (_initialyzed == false)
            return;

        var color = Color.white;
        Value = _neuron.Value;
        color.a = Value;
        _material.color = color;
    }

    public void Initialyze(Neuron neuron)
    {
        _neuron = neuron;
        _initialyzed = true;
    }
}
