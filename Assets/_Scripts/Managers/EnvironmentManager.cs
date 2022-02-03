using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : Singleton<EnvironmentManager>
{
    public Vector3 GroundSize = new Vector3(50, 0, 50);
    public float NodeRadius = 1f;

    public GameObject GroundPrefab;
    public GameObject EnvironmentContainer;

    private Ground _ground;

    public void SpawnGround()
    {
        if (_ground != null)
            return;

        _ground = Instantiate(GroundPrefab, EnvironmentContainer.transform).GetComponent<Ground>();
        _ground.SetupGrid(GroundSize, NodeRadius);
    }
}
