using UnityEngine;
using sm = UnityEngine.SceneManagement;

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
        sm.SceneManager.LoadSceneAsync(0);
    }

    public void OnExitToDesktopClick()
    {
        Application.Quit();
    }
}
