using Assets._Scripts.Utilities;
using mew;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;

public class ProbaScreenManager : MonoBehaviour
{
    private BaseAnt _ant;
    private TextMeshProUGUI _scannerText;
    private RectTransform _rectTransform;
    public GameObject ProbabilityDiagramUnitPrefab;
    public GameObject ProbabilityDiagramContainer;
    public Material ProbabilityDiagramMaterial;

    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
    private GameObject[] _probabilityDiagramPortions;


    public int ScreenWidth = 100;
    public int ScreenHeight = 100;

    public float ScreenPositionX = 0;
    public float ScreenPositionY = 0;

    private bool Initialyzed => _ant != null;

    private string InfoHeader => _ant != null ? $"Ant name {_ant.name}\n" : string.Empty;


    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _scannerText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Initialyzed == false)
            return;


        UpdatePortionInfos();
        UpdateProbabilityDiagram();
    }

    public void SetAnt(BaseAnt ant)
    {
        _semaphore.Wait();
        try
        {
            if (ant != null)
            {
                _ant = ant;
                if (_probabilityDiagramPortions != null)
                {
                    foreach (var go in _probabilityDiagramPortions)
                        Destroy(go);
                }
                _probabilityDiagramPortions = new GameObject[_ant.Stats.ScannerSubdivisions];
                DrawMesh();
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void DrawMesh()
    {
        var alpha = 360 / _probabilityDiagramPortions.Length;
        for (int i = 0; i < _probabilityDiagramPortions.Length; i++)
        {
            var diagramUnit = Instantiate(ProbabilityDiagramUnitPrefab, ProbabilityDiagramContainer.transform, false);
            diagramUnit.name = $"DiagramUnit_{i * alpha}°";
            diagramUnit.transform.position += new Vector3(0, 0.5f, 0);
            diagramUnit.transform.rotation = Quaternion.Euler(90, 0, i * alpha);
            var mesh = StaticHelper.CreateDiagramMesh(0.5f, alpha / 2, ScreenWidth / 5); ;
            diagramUnit.GetComponent<MeshFilter>().mesh = mesh;
            _probabilityDiagramPortions[i] = diagramUnit;
        }        
    }

    private void UpdateProbabilityDiagram()
    {
        var probabilities = _ant.Probabilities;
        var maxProba = 0f;
        foreach(var proba in probabilities)
        {
            if (proba > maxProba)
                maxProba = proba;
        }
        for(int i = 0; i < probabilities.Length; i++)
        {
            float gValue = probabilities[i]/ maxProba;
            
            var newColor = new Color(0, probabilities[i], 0, 1);
            _probabilityDiagramPortions[i].GetComponent<ChangeColor>().SetMaterialColor(newColor);    
        }
    }

    private void UpdatePortionInfos()
    {
        var text = new StringBuilder(InfoHeader);
        for (int i = 0; i < _probabilityDiagramPortions.Length; i++)
            text.AppendLine(_ant.PortionInfos[i]);

        _scannerText.text = text.ToString();
    }
}
