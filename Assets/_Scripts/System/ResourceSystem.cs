using mew;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceSystem : Singleton<ResourceSystem>
{
    public List<ScriptableAntBase> Ants { get; private set; }
    public List<ScriptablePheromoneBase> Pheromones { get; private set; }
    private Dictionary<ScriptableAntBase.AntTypeEnum, ScriptableAntBase> _antsDict;
    private Dictionary<ScriptablePheromoneBase.PheromoneTypeEnum, ScriptablePheromoneBase> _pheromonesDict;

    protected override void Awake()
    {
        base.Awake();
        AssembleRessources();
    }

    private void AssembleRessources()
    {
        Ants = Resources.LoadAll<ScriptableAntBase>("Units/Ants").ToList();
        _antsDict = Ants.ToDictionary(t => t.AntType, t => t);

        Pheromones = Resources.LoadAll<ScriptablePheromoneBase>("Units/Pheromones").ToList();
        _pheromonesDict = Pheromones.ToDictionary(t => t.BaseCaracteristics.PheromoneType, t => t);
    }

    public ScriptableAntBase AntOfTypeGet(ScriptableAntBase.AntTypeEnum type) => _antsDict[type];

    public ScriptablePheromoneBase PheromoneOfTypeGet(ScriptablePheromoneBase.PheromoneTypeEnum type) => _pheromonesDict[type];

    public void ModifyVisionRadius(ScriptableAntBase.AntTypeEnum antType, int newValue)
    {
        _antsDict[antType].BaseStats.VisionRadius = newValue;
    }

    public void ModifyVisionAngle(ScriptableAntBase.AntTypeEnum antType, int newValue)
    {
        _antsDict[antType].BaseStats.VisionAngle = newValue;
    }

    public void ModifyPheromoneDuration(ScriptablePheromoneBase.PheromoneTypeEnum pheroType,int newValue)
    {
        _pheromonesDict[pheroType].BaseCaracteristics.Duration = newValue;
    }
}
