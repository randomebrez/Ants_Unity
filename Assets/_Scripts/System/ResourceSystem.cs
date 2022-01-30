using mew;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceSystem : Singleton<ResourceSystem>
{
    public List<ScriptableAntBase> Ants { get; private set; }
    private Dictionary<ScriptableAntBase.AntTypeEnum, ScriptableAntBase> _AntsDict;

    protected override void Awake()
    {
        base.Awake();
        AssembleRessources();
    }

    private void AssembleRessources()
    {
        Ants = Resources.LoadAll<ScriptableAntBase>("Units/Ants").ToList();
        _AntsDict = Ants.ToDictionary(t => t.AntType, t => t);
    }

    public ScriptableAntBase AntOfTypeGet(ScriptableAntBase.AntTypeEnum type) => _AntsDict[type];
}
