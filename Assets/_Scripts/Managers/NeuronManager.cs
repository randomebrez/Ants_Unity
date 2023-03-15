using mew;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NeuronManager : MonoBehaviour
{
    public GameObject NeuronPrefab;
    private BaseAnt _ant;
    private bool _initialyzed = false;


    public void Initialyze(BaseAnt ant)
    {
        _ant = ant;
        var spawned = Instantiate(NeuronPrefab, transform.position, Quaternion.identity);
        spawned.GetComponent<NeuronUI>().Initialyze(_ant.BrainManager.GetBrain().Neurons.Inputs.First());
        _initialyzed = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
