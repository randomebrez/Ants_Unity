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
        transform.GetChild(0).gameObject.layer = (int) Layer.Walkable;
    }

    public void SetUnwalkable()
    {
        _renderer.material = UnwalkableMaterial;
        gameObject.layer = (int) Layer.Unwalkable;
    }

    public void OnDrawGizmos()
    {
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}
