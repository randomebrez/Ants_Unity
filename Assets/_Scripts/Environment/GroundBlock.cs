using Assets.Dtos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundBlock : MonoBehaviour
{
    public Block Block { get; set; }

    public Material WalkableMaterial;
    public Material UnwalkableMaterial;

    private MeshRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {

    }

    public void Setup(bool walkable)
    {
        if (walkable)
            _renderer.material = WalkableMaterial;
        else
        {
            _renderer.material = UnwalkableMaterial;
        }
    }
}
