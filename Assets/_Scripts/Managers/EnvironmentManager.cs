using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : Singleton<EnvironmentManager>
{
    public GameObject GroundPrefab;
    public GameObject EnvironmentContainer;

    private Ground _ground;

    public void SpawnGround(float nodeRadius)
    {
        if (_ground != null)
            return;

        _ground = Instantiate(GroundPrefab, EnvironmentContainer.transform).GetComponent<Ground>();
        _ground.SetupGrid(nodeRadius);
    }
}
