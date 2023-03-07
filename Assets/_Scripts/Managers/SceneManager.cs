using System;

internal class SceneManager : BaseManager<SceneManager>
{
    public static event Action<SceneState> OnBeforeStateChanged;
    public static event Action<SceneState> OnAfterStateChanged;

    private int _foodNumber = 100;

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
        var deltaTheta = 360f / (_foodNumber / 10);
        EnvironmentManager.Instance.SpawnGround();
        for (int i = 0; i < _foodNumber; i++)
            EnvironmentManager.Instance.SpawnFood(i * deltaTheta);
        ChangeState(SceneState.SpawningAntNest);
    }

    public void HandleSpawningAnts()
    {
        UnitManager.Instance.CreateNewColony();
    }

    [Serializable]
    public enum SceneState
    {
        Starting,
        SpawningAntNest
    }
}
