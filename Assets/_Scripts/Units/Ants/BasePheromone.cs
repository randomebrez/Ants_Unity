using System;
using Assets.Dtos;
using UnityEngine;
using mew;
using Assets._Scripts.Utilities;
using System.Linq;

public class BasePheromone : MonoBehaviour
{
    public ScriptablePheromoneBase.Caracteristics Caracteristics { get; private set; }
    public Pheromone Pheromone;
    public Block BlockPos;

    private MeshRenderer _renderer;
    private float _timeSinceLastTick = 0f;
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

        _timeSinceLastTick += Time.deltaTime;
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
        Pheromone.RemainingTime -= _timeSinceLastTick;
        Pheromone.Density = Pheromone.RemainingTime / Pheromone.Lifetime;

        var color = _renderer.material.color;
        color.a = Mathf.Max(0, Pheromone.Density);
        _renderer.material.color = color;

        _timeSinceLastTick = 0;

        if (Pheromone.RemainingTime <= 0)
            Destroy(gameObject);
    }

    public void MergePheromones(BasePheromone other)
    {
        Pheromone.Lifetime += other.Pheromone.RemainingTime;
        Pheromone.RemainingTime += other.Pheromone.RemainingTime;
        Pheromone.Density = Pheromone.RemainingTime / Pheromone.Lifetime;
    }
}
