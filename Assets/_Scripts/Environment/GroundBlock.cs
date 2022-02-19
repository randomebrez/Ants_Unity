using Assets.Dtos;
using UnityEngine;

public class GroundBlock : MonoBehaviour
{
    public Block Block { get; set; }

    public Material WalkableMaterial;
    public Material UnwalkableMaterial;

    private MeshRenderer _renderer;
    [Range(0,31)] public int OcclusionLayer;

    private void Awake()
    {
        _renderer = GetComponentInChildren<MeshRenderer>();
    }

    public void SetWalkable()
    {
        _renderer.material = WalkableMaterial;
    }

    public void SetUnwalkable()
    {
        _renderer.material = UnwalkableMaterial;
    }
}
