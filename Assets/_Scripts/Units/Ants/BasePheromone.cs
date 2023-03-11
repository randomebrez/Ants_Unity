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
    private float _livingTime = 0;
    private bool _expired = false;
    private float _cleaningTime = 0f;
    private bool _hasAlreadyClean = false;
    private bool _initialyzed = false;

    public float Remainingtime => Pheromone.LifeTimeSeconds - _livingTime;

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

        _livingTime += Time.deltaTime;
        _expired = _livingTime > Pheromone.LifeTimeSeconds;

        if (_expired)
            Destroy(gameObject);

        var percentDone = _livingTime / Pheromone.LifeTimeSeconds;
        Pheromone.Density = 1 - (float)percentDone;
        var color = _renderer.material.color;
        color.a = Mathf.Max(0, Pheromone.Density);
        _renderer.material.color = color;
    }

    void FixedUpdate()
    {
        if (_initialyzed == false)
            return;
    
        if (_hasAlreadyClean == false && _livingTime > _cleaningTime)
            CleanOtherPheromones();
    }



    public virtual void Initialyze(ScriptablePheromoneBase.Caracteristics caracteristics, Block position)
    {
        BlockPos = position;
        Caracteristics = caracteristics;
        Pheromone = new Pheromone
        {
            CreationDate = DateTime.UtcNow,
            LifeTimeSeconds = Caracteristics.Duration,
            Density = 1
        };
        _cleaningTime = Remainingtime / 10f;
        _initialyzed = true;
    }

    private void CleanOtherPheromones()
    {
        Collider[] colliders = new Collider[10];
        Physics.OverlapSphereNonAlloc(transform.position, GlobalParameters.NodeRadius / 2f, colliders, LayerMask.GetMask(Layer.Pheromone.ToString()));
        var temp = colliders.Where(t => t != null && t.GetComponent<BasePheromone>().Caracteristics.PheromoneType == Caracteristics.PheromoneType).ToList();
        foreach (var t in temp)
        {
            if (t.transform == transform)
                continue;
            try
            {
                Pheromone.LifeTimeSeconds += t.GetComponent<BasePheromone>().Remainingtime;
                Destroy(t.gameObject);
            }
            catch
            {
                continue;
            }
        }
        _hasAlreadyClean = true;
    }
}
