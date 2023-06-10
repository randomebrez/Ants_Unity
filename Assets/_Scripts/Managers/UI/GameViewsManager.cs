using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameViewsManager : MonoBehaviour
{
    private SidePanels _sidePanels;
    public Transform MainView { get; private set; }
    private TextMeshProUGUI _headerText;
    private Transform _pauseMenu;

    private int _generationId;
    private TimeSpan _genereationElapsedTime;

    private bool _simulationStarted = false;

    // Start is called before the first frame update
    void Awake()
    {
        MainView = transform.GetChild(0);
        _sidePanels = transform.GetChild(1).GetComponent<SidePanels>();
        _headerText = MainView.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        _pauseMenu = transform.GetChild(2);
    }

    private void Update()
    {
        if (_simulationStarted == false)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
            HandlePauseMenu();

        if (_pauseMenu.gameObject.activeInHierarchy)
            return;

        var temp = Math.Round(Time.deltaTime, 3).ToString().Split(',');
        _genereationElapsedTime += new TimeSpan(0, 0, 0, int.Parse(temp[0]), int.Parse(temp[1]));
        var text = $"Generation n° {_generationId}\nElapsed time : {_genereationElapsedTime.Minutes}:{_genereationElapsedTime.Seconds}:{_genereationElapsedTime.Milliseconds}";
        _headerText.text = text;
    }

    public void SetNewGenerationId(int generationId)
    {
        _generationId = generationId;
        _genereationElapsedTime = TimeSpan.Zero;
        _simulationStarted = true;
    }

    public void SetCamera(Camera camera)
    {
        var cameraGo = GetComponent<Canvas>();
        cameraGo.worldCamera = camera;
    }

    public void Initialyze(List<string> titles)
    {
        _sidePanels.SplitZone(titles);
    }

    public void AddCurveValue(int zoneIndex, Vector2 point)
    {
        _sidePanels.AddCurveValue(zoneIndex, point);
    }

    public void UpdateHighScores(string text)
    {
        _sidePanels.UpdateHighScore(text);
    }


    private void HandlePauseMenu()
    {
        if (_pauseMenu.gameObject.activeInHierarchy == false)
        {
            _pauseMenu.gameObject.SetActive(true);
            SceneManager.Instance.SimulationPaused = true;
        }
    }
}
