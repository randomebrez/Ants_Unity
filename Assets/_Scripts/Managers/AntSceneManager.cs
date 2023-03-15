using UnityEngine;

internal class AntSceneManager : BaseManager<AntSceneManager>
{
    // Role :
    //  - Initialyze GameBoard
    //  - Put Ground in main view => put environment in mainView

    public GameObject SceneMainPrefab;
    public GameViewsManager GameViewsManagerPrefab;
    public Camera MainCamera;

    private GameViewsManager _gameViewsManager;

    private bool _initialyzed = false;

    // Start is called before the first frame update
    void Start()
    {
        _gameViewsManager = Instantiate(GameViewsManagerPrefab);
    }

    private void Update()
    {
        if (_initialyzed)
            return;
        Initialyze();
        _initialyzed = true;
    }

    private void Initialyze()
    {
        _gameViewsManager.SetCamera(MainCamera);
        var scene = Instantiate(SceneMainPrefab);
        StatisticsManager.Instance.SetGameViewManager(_gameViewsManager);
    }
}
