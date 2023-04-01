using System;
using Assets.Dtos;
using UnityEngine;
using mew;

public class BasePheromone : MonoBehaviour
{
    public ScriptablePheromoneBase.Caracteristics Caracteristics { get; private set; }
    public Pheromone Pheromone;
    public Block BlockPos;

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

    public virtual void Initialyze(ScriptablePheromoneBase.Caracteristics caracteristics, Block position)
    {
        BlockPos = position;
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

    public void ApplyTimeEffect()
    {
        Pheromone.RemainingTime --;
        Pheromone.Density = (float)Pheromone.RemainingTime / Pheromone.Lifetime;

        if (Pheromone.RemainingTime <= 0)
            Destroy(gameObject);
    }

    public void MergePheromones(BasePheromone other)
    {
        Pheromone.Lifetime += other.Pheromone.RemainingTime;
        Pheromone.RemainingTime += other.Pheromone.RemainingTime;
        Pheromone.Density = (float)Pheromone.RemainingTime / Pheromone.Lifetime;
    }
}
