using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            OnResumeClick();
    }

    public void OnResumeClick()
    {
        // Tell to release pause and set this inactive
        SceneManager.Instance.SimulationPaused = false;
        gameObject.SetActive(false);
    }

    public void OnRestartClick()
    {
        // Renew Colony & environment
        // Call Create new generation with no selected brains
        SceneManager.Instance.Restart();
        SceneManager.Instance.SimulationPaused = false;
        gameObject.SetActive(false);
    }

    public void OnExitToMenuClick()
    {
        //Close the scene and launch MainMenu scene
    }

    public void OnExitToDesktopClick()
    {
        Application.Quit();
    }
}
