using System;
using UnityEngine;
using sm = UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public event EventHandler<EventArgs> SwitchScreen;

    protected virtual void OnSwitchScreen()
    {
        SwitchScreen?.Invoke(this, EventArgs.Empty);
    }

    public void OnStartClick()
    {
        sm.SceneManager.LoadSceneAsync(1);
    }

    public void OnParametersClick()
    {
        OnSwitchScreen();
    }

    public void OnExitToDesktopClick()
    {
        Application.Quit();
    }
}
