using Assets._Scripts.Utilities;
using System;

internal class SceneManager : BaseManager<SceneManager>
{
    public enum SceneState
    {
        Initialyze,
        SpawnContext,
        SpawnContextObjects,
        Running
    }

    private SceneState _sceneState = SceneState.Initialyze;

    public Action<SceneState> BeforeStateChanged;
    public Action<SceneState> AfterStateChanged;

    private bool _stateChanged = false;
    private int _frameCount = 0;

    public void Start()
    {
        ChangeState();
    }

    private void Update()
    {
        _frameCount++;

        switch (_sceneState)
        {
            case SceneState.SpawnContext:
                InstantiateContext();
                ChangeState();
                break;
            case SceneState.SpawnContextObjects:
                InstantiateContextObjects();
                ChangeState();
                break;
            case SceneState.Running:
                RunLife();
                break;
        }
    }

    private void ChangeState()
    {
        BeforeStateChanged?.Invoke(_sceneState);

        switch (_sceneState)
        {
            case SceneState.Initialyze:
                _sceneState = SceneState.SpawnContext;
                break;
            case SceneState.SpawnContext:
                _sceneState = SceneState.SpawnContextObjects;
                break;
            case SceneState.SpawnContextObjects:
                _sceneState = SceneState.Running;
                break;
        }

        AfterStateChanged?.Invoke(_sceneState);
        _stateChanged = true;
    }

    private void InstantiateContext()
    {
        EnvironmentManager.Instance.SpawnGround();
    }

    private void InstantiateContextObjects()
    {
        // Spawn food
        var deltaTheta = 360f / 6;
        for (int i = 0; i < GlobalParameters.InitialFoodTokenNumber; i++)
            EnvironmentManager.Instance.SpawnFood(i * deltaTheta);

        // Spawn ants
        UnitManager.Instance.CreateNewColony();
    }

    private void RunLife()
    {
        if (_frameCount < GlobalParameters.GenerationFrameCount)
        {
            switch(_frameCount%3)
            {
                case 0:
                    EnvironmentManager.Instance.ApplyTimeEffect();
                    break;
                case 1:
                    UnitManager.Instance.MoveAllUnits();
                    break;
                case 2:
                    EnvironmentManager.Instance.DropPheromones();
                    break;
            }
        }
        else
        {
            UnitManager.Instance.RenewColonies();
            _frameCount = -1;
        }

        _frameCount++;
    }
}
