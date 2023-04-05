using System;
using UnityEngine;

public class ParametersMenu : MonoBehaviour
{
    public event EventHandler<EventArgs> SwitchScreen;

    protected virtual void OnSwitchScreen()
    {
        SwitchScreen?.Invoke(this, EventArgs.Empty);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnMenuClick()
    {
        OnSwitchScreen();
    }

    public void OnSaveClick()
    {
        
    }
}
