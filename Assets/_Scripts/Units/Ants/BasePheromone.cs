using System;
using Assets.Dtos;
using UnityEngine;
using System.Timers;
using mew;

public class BasePheromone : MonoBehaviour
{
    private MeshRenderer _renderer;
    public Pheromone Pheromone;
    private Timer _timer;
    private bool _expired = false;

    //public ScriptablePheromoneBase.Caracteristics Caracteristics { get => new ScriptablePheromoneBase.Caracteristics { Color = Color.blue, Duration = 5 }; }

    public ScriptablePheromoneBase.Caracteristics Caracteristics { get; private set; }
    public virtual void SetCaracteristics(ScriptablePheromoneBase.Caracteristics caracteristics) => Caracteristics = caracteristics;

        // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        _renderer.material.color = Caracteristics.Color;
        Pheromone = new Pheromone
        {
            CreationDate = DateTime.UtcNow,
            Lifetime = new TimeSpan(0, 0, Caracteristics.Duration)
        };
        TimerInitialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (_expired)
            Destroy(gameObject);

        var percentDone = (DateTime.UtcNow - Pheromone.CreationDate).TotalSeconds / Caracteristics.Duration;
        Pheromone.Density *= (1 - (float)percentDone);
        var color = _renderer.material.color;
        color.a = Mathf.Max(0, 1 - (float)percentDone);
        _renderer.material.color = color;
    }

    private void TimerInitialize()
    {
        if (_timer != null)
            return;

        _timer = new Timer(Caracteristics.Duration * 1000);
        _timer.Enabled = true;
        _timer.AutoReset = false;
        _timer.Elapsed += OnTick;
        _timer.Start();
    }

    private void OnTick(object sender, ElapsedEventArgs e)
    {
        _timer.Stop();
        _expired = true;
    }
}
