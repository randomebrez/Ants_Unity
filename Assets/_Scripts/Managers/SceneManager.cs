using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class SceneManager : BaseManager<SceneManager>
{
    public static event Action<SceneState> OnBeforeStateChanged;
    public static event Action<SceneState> OnAfterStateChanged;

    public SceneState State { get; private set; }

    public void Start() => ChangeState(SceneState.Starting);


    public void ChangeState(SceneState newState)
    {
        State = newState;

        OnBeforeStateChanged?.Invoke(State);

        switch (State)
        {
            case SceneState.Starting:
                HandleStartingState();
                break;
            case SceneState.SpawningAntNest:
                HandleSpawningAnts();
                break;
        }

        OnAfterStateChanged?.Invoke(State);
    }
    

    public void HandleStartingState()
    {
        EnvironmentManager.Instance.SpawnGround();
        for (int i = 0; i < 50; i++)
            EnvironmentManager.Instance.SpawnFood();
        ChangeState(SceneState.SpawningAntNest);
    }

    public void HandleSpawningAnts()
    {
        UnitManager.Instance.SpawnAntNest();
    }

    [Serializable]
    public enum SceneState
    {
        Starting,
        SpawningAntNest
    }
}
