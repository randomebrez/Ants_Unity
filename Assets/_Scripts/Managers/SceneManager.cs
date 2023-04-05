using Assets._Scripts.Utilities;
using System;
using System.IO;

internal class SceneManager : BaseManager<SceneManager>
{
    public enum SceneState
    {
        Initialyze,
        CreateFolders,
        SpawnContext,
        SpawnContextObjects,
        Running,
        Restart
    }

    private SceneState _sceneState = SceneState.Initialyze;

    public Action<SceneState> BeforeStateChanged;
    public Action<SceneState> AfterStateChanged;

    private int _frameCount = 0;
    public bool SimulationPaused = false;

    public void Start()
    {
        ChangeState();
    }

    private void Update()
    {
        if (SimulationPaused)
            return;

        switch (_sceneState)
        {
            case SceneState.CreateFolders:
                CreateFolders();
                ChangeState();
                break;
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
            case SceneState.Restart:
                RestartSimulation();
                ChangeState();
                break;
        }
    }

    private void ChangeState()
    {
        BeforeStateChanged?.Invoke(_sceneState);

        switch (_sceneState)
        {
            case SceneState.Initialyze:
                _sceneState = SceneState.CreateFolders;
                break;
            case SceneState.CreateFolders:
                _sceneState = SceneState.SpawnContext;
                break;
            case SceneState.SpawnContext:
                _sceneState = SceneState.SpawnContextObjects;
                break;
            case SceneState.SpawnContextObjects:
                _sceneState = SceneState.Running;
                break;
            case SceneState.Restart:
                _sceneState = SceneState.Running;
                break;
        }

        AfterStateChanged?.Invoke(_sceneState);
    }


    private void CreateFolders()
    {
        if (Directory.Exists(GlobalParameters.FirstBrainsFolderPath) == false)
            Directory.CreateDirectory(GlobalParameters.FirstBrainsFolderPath);
    }

    private void InstantiateContext()
    {
        EnvironmentManager.Instance.SpawnGround();
    }

    private void InstantiateContextObjects()
    {
        // Spawn food
        EnvironmentManager.Instance.SpawnFood();

        // Spawn ants
        UnitManager.Instance.CreateNewColony();
    }

    private void RunLife()
    {
        if (_frameCount < GlobalParameters.GenerationFrameCount)
        {
            UnitManager.Instance.MoveAllUnits();
            EnvironmentManager.Instance.DropPheromones();
            EnvironmentManager.Instance.ApplyTimeEffect();
        }
        else
        {
            EnvironmentManager.Instance.RenewEnvironment();
            UnitManager.Instance.RenewColonies();
            _frameCount = -1;
        }

        _frameCount++;
    }

    public void Restart()
    {
        _sceneState = SceneState.Restart;
    }

    private void RestartSimulation()
    {
        UnitManager.Instance.ClearColonies();
        EnvironmentManager.Instance.RenewEnvironment();
        UnitManager.Instance.CreateNewColony();
        _frameCount = 0;
        SimulationPaused = false;
    }
}
