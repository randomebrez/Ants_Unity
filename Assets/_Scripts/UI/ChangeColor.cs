
using UnityEngine;

public class 
    ChangeColor : MonoBehaviour
{
    public GameObject go;
    public Material _material;
    [SerializeField] private Color _color;

    public void Awake()
    {
        go = this.gameObject;
        _material = go.GetComponent<Renderer>().material;
    }

    public void SetMaterialColor(Color color)
    {
        _material.color = color;
    }

    private void Update()
    {
        //_material.color = _color;
    }
}
