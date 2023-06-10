using System;
using Assets.Dtos;
using UnityEngine;
using mew;

public class PheromoneToken : MonoBehaviour
{
    public ScriptablePheromoneBase.Caracteristics Caracteristics { get; private set; }
    public Pheromone Pheromone;

    private MeshRenderer _renderer;
    private bool _initialyzed = false;

    // Unity methods
    void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        _renderer.material.color = Caracteristics.Color;
    }

    private void Update()
    {
        if (_initialyzed == false)
            return;

        var color = _renderer.material.color;
        color.a = Mathf.Max(0, Pheromone.Density);
        _renderer.material.color = color;
    }


    public virtual void Initialyze(ScriptablePheromoneBase.Caracteristics caracteristics)
    {
        Caracteristics = caracteristics;
        Pheromone = new Pheromone
        {
            CreationDate = DateTime.UtcNow,
            Lifetime = Caracteristics.Duration,
            RemainingTime = Caracteristics.Duration,
            Density = 1
        };
        _initialyzed = true;
    }


    // Method that is called at each iteration to apply time effect on block
    public void ApplyTimeEffect()
    {
        Pheromone.RemainingTime --;
        Pheromone.Density = (float)Pheromone.RemainingTime / Pheromone.Lifetime;

        if (Pheromone.RemainingTime <= 0)
            Destroy(gameObject);
    }

    public void MergePheromones(int otherRemainingTime)
    {
        Pheromone.Lifetime += otherRemainingTime;
        Pheromone.RemainingTime += otherRemainingTime;
        Pheromone.Density = (float)Pheromone.RemainingTime / Pheromone.Lifetime;
    }
}
