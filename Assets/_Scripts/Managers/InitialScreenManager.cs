using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialScreenManager : MonoBehaviour
{
    private MainMenu _mainMenu;
    private ParametersMenu _parametersMenu;


    // Start is called before the first frame update
    void Start()
    {
        _mainMenu = transform.GetChild(0).GetComponent<MainMenu>();
        _parametersMenu = transform.GetChild(1).GetComponent<ParametersMenu>();

        _mainMenu.SwitchScreen += SwitchScreen;
        _parametersMenu.SwitchScreen += SwitchScreen;
    }

    private void SwitchScreen(object sender, EventArgs e)
    {
        if (_mainMenu.gameObject.activeInHierarchy)
        {
            _mainMenu.gameObject.SetActive(false);
            _parametersMenu.gameObject.SetActive(true);
        }
        else
        {
            _mainMenu.gameObject.SetActive(true);
            _parametersMenu.gameObject.SetActive(false);
        }
    }
}
