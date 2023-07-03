using System.Collections.Generic;
using UnityEngine;
using mew;
using Assets._Scripts.Utilities;
using static mew.ScriptablePheromoneBase;
using Assets.Dtos;
using System.Linq;
using Assets.Gateways;
using NeuralNetwork.Abstraction.Model;
using Assets._Scripts.Managers;

internal class UnitManager : BaseManager<UnitManager>
{
    public AntColony AntColonyPrefab;

    private NeuralNetworkGateway _neuralNetworkGateway;
    private Vector3 GetGroundSize => GlobalParameters.GroundSize;
    private List<AntColony> _colonies = new List<AntColony>();
    private BaseAnt _lastClicked;
    private BrainBuilder _brainGraphBuilder;

    private InstanceGraph _simulationCaracteristicGraph;
    private List<BrainTemplate> _distinctTemplates;
    private Dictionary<string, BrainCaracteristics> _templateBrainCaracteristics;

    protected override void Awake()
    {
        _neuralNetworkGateway = new NeuralNetworkGateway();
        base.Awake();

        // Needed to set scriptable parameters in case we don't get through parameter screen
        AntScriptableStatisticsSet();
    }

    protected void Start()
    {
        _brainGraphBuilder = new BrainBuilder();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            CreateNewColony();

        if (Input.GetKeyDown(KeyCode.A))
            CreateNewColony();
    }

    // Colonies management
    public void CreateNewColony()
    {
        GameObject spawned = null;
        while (spawned == null)
        {
            var randomX = (Random.value - 0.5f) * GetGroundSize.x - 1;
            var randomZ = (Random.value - 0.5f) * GetGroundSize.z - 1;

            var spawnerMaxRange = Mathf.Max(AntColonyPrefab.transform.lossyScale.x, AntColonyPrefab.transform.lossyScale.y, AntColonyPrefab.transform.lossyScale.z);
            var spawnposition = 2 * GlobalParameters.NodeRadius * Vector3.up;
            var spawnBlock = EnvironmentManager.Instance.GroundBlockFromWorldPoint(spawnposition);
            if (spawnBlock == null)
                Debug.Log("spawn null");
            spawned = InstantiateObject(AntColonyPrefab.gameObject, spawnBlock.Block.WorldPosition + 2 * GlobalParameters.NodeRadius * Vector3.up, Quaternion.identity, EnvironmentManager.Instance.GetUnitContainer(), spawnerMaxRange * 1.5f);
        }

        var id = _colonies.Count + 1;
        var colony = spawned.GetComponent<AntColony>();
        colony.Initialyze($"Colony_{id}");

        var units = GetRandomUnits(GlobalParameters.ColonyMaxPopulation);
        colony.InstantiateUnits(units, GlobalParameters.ColonyMaxPopulation);

        _colonies.Add(colony);
    }
    public void RenewColonies()
    {
        for (int i = 0; i < _colonies.Count; i++)
        {
            var bestUnits = _colonies[i].SelectBestUnits();
            _colonies[i].SetStatistics();
            var units = GenerateNewUnits(bestUnits);
            _colonies[i].DestroyAllUnits();
            _colonies[i].InstantiateUnits(units.units, units.randomUnitNumber);
        }
    }
    public void ClearColonies()
    {
        for (int i = _colonies.Count; i > 0; i--)
            Destroy(_colonies[i - 1].gameObject);

        _colonies.Clear();
    }
    public int GetColoniesSurvivorNumber(int index)
    {
        return _colonies[index].GetBestBrainCount();
    }


    // Units management
    public void MoveAllUnits()
    {
        for (int i = 0; i < _colonies.Count; i++)
            _colonies[i].MoveAllAnts();
    }
    public Dictionary<PheromoneTypeEnum, List<Block>> GetUnitPositions()
    {
        if (_colonies.Any() == false)
            return new Dictionary<PheromoneTypeEnum, List<Block>>();

        var result = _colonies[0].GetAllAntPositions();
        for (int i = 1; i < _colonies.Count; i++)
        {
            var temp = _colonies[i].GetAllAntPositions();
            foreach (var pair in temp)
                result[pair.Key].AddRange(pair.Value);
        }

        return result;
    }
    public void AntClick(BaseAnt ant)
    {
        if (_lastClicked != null)
            _lastClicked.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.black;

        ant.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.yellow;
        _lastClicked = ant;
    }




    // Set a brain graph as the one to use to generate units
    public void SimulationInstanceGraphSet(string graphName)
    {
        // Get the graph with all template instanciated, and portions attributed
        _simulationCaracteristicGraph = _brainGraphBuilder.UserSelectionInstanceGraphGet(graphName);

        // For practicity : save distinct template used by the graph
        _distinctTemplates = new List<BrainTemplate>();
        var distinctTemplates = _simulationCaracteristicGraph.CaracteristicNodes.GroupBy(t => t.Value.Template.Name);
        foreach (var templateCount in distinctTemplates)
            _distinctTemplates.Add(templateCount.First().Value.Template);

        // Map once the brain caracteristics of the distinct template. (Object that is understood by the nuget)
        _templateBrainCaracteristics = new Dictionary<string, BrainCaracteristics>();
        foreach (var template in _distinctTemplates)
            _templateBrainCaracteristics.Add(template.Name, ToBrainCarac(template));
    }

    //Given genomes grouped by template & following instance graph structure, Build a genome graph for each unit (i-th position in all dictionary's list correspond to the i-th unit)
    private List<GenomeGraph> GenomeGraphListBuild(InstanceGraph instanceGraph, Dictionary<string, List<Genome>> genomes)
    {
        var graphs = new List<GenomeGraph>();
        var number = genomes.First().Value.Count();

        for (int i = 0; i < number; i++)
        {
            var graph = new GenomeGraph();
            foreach (var node in instanceGraph.CaracteristicNodes)
            {
                graph.GenomeNodes.Add(new BrainGenomePair
                {
                    Caracteristics = ToBrainCarac(node.Value),
                    Genome = genomes[node.Value.Template.Name][i]
                });
            }
            foreach (var edge in instanceGraph.CaracteristicEdges)
            {
                var result = new HashSet<string>();
                foreach (var origin in edge.Value)
                    result.Add(instanceGraph.CaracteristicNodes[origin].UniqueName);

                graph.GenomeEdges.Add(edge.Key, result.ToList());
            }

            graphs.Add(graph);
        }

        return graphs;
    }


    // Unit generation
    private UnitWrapper[] GetRandomUnits(int unitNumber)
    {
        if (unitNumber <= 0)
            return new UnitWrapper[0];

        var randomGenomes = GenerateRandomGenomes(unitNumber, _distinctTemplates);
        var genomeGraphs = GenomeGraphListBuild(_simulationCaracteristicGraph, randomGenomes);

        var nugetUnits = _neuralNetworkGateway.GetUnits(genomeGraphs);

        var result = new UnitWrapper[unitNumber];
        for(int i = 0; i < unitNumber; i++)
        {
            result[i] = new UnitWrapper
            {
                NugetUnit = nugetUnits[i],
                InstanceGraph = _simulationCaracteristicGraph
            };
        }

        return result;
    }
    private (UnitWrapper[] units, int randomUnitNumber) GenerateNewUnits(List<BaseAnt> bestUnits)
    {
        var result = new UnitWrapper[GlobalParameters.ColonyMaxPopulation];

        // Generate as many children as possible from best units
        var genomes = GenerateMixedGenomes(_distinctTemplates, bestUnits);
        var childrenGenomeGraphs = GenomeGraphListBuild(_simulationCaracteristicGraph, genomes);
        if (bestUnits.Count == 1)
            childrenGenomeGraphs.Add(bestUnits[0].GetNugetUnit.GenomeGraph);
        var children = _neuralNetworkGateway.GetUnits(childrenGenomeGraphs);

        for (int i = 0; i < children.Length; i++)
        {
            result[i] = new UnitWrapper
            {
                NugetUnit = children[i],
                InstanceGraph = _simulationCaracteristicGraph
            };
        }
            
        // Generate random units to reach ColonyMaxPopulation treshold
        var randomUnitToGenerate = GlobalParameters.ColonyMaxPopulation - children.Count();
        var randomUnits = GetRandomUnits(randomUnitToGenerate);

        for (int i = children.Length; i < GlobalParameters.ColonyMaxPopulation; i++)
            result[i] = randomUnits[i - children.Length];

        return (result, randomUnitToGenerate);
    }
    private void AntScriptableStatisticsSet()
    {
        ResourceSystem.Instance.ModifyVisionRadius(ScriptableAntBase.AntTypeEnum.Worker, GlobalParameters.VisionRange);
        ResourceSystem.Instance.ModifyVisionAngle(ScriptableAntBase.AntTypeEnum.Worker, GlobalParameters.VisionAngle);
    }


    // Genome Generation
    private Dictionary<string, List<Genome>> GenerateRandomGenomes(int genomeNumber, List<BrainTemplate> distinctTemplates)
    {
        var result = new Dictionary<string, List<Genome>>();
        foreach (var template in distinctTemplates)
        {
            var genomes = _neuralNetworkGateway.GetGenomes(genomeNumber, _templateBrainCaracteristics[template.Name]);
            result.Add(template.Name, genomes);
        }

        return result;
    }
    private Dictionary<string, List<Genome>> GenerateMixedGenomes(List<BrainTemplate> distinctTemplates, List<BaseAnt> bestUnits)
    {
        var result = new Dictionary<string, List<Genome>>();
        var couples = CreateCouples(bestUnits);

        foreach (var template in distinctTemplates)
            result.Add(template.Name, new List<Genome>());

        foreach(var couple in couples)
        {
            // Since each unit needs same template as everyone else, this work.
            // i-th position in all list of the dictionary, are genomes of the i-th couple
            foreach(var template in distinctTemplates)
            {
                // Get BrainName from UnitWrapper InstanceGraph
                // Then fetch the genome from the NugetUnit BrainGraph
                var genomeABrainName = couple.parentA.Unit.InstanceGraph.InstanceByTemplate[template.Name].First().UniqueName;
                var genomeA = couple.parentA.GetNugetUnit.BrainGraph.BrainNodes[genomeABrainName].Genome;

                var genomeBBrainName = couple.parentB.Unit.InstanceGraph.InstanceByTemplate[template.Name].First().UniqueName;
                var genomeB = couple.parentA.GetNugetUnit.BrainGraph.BrainNodes[genomeBBrainName].Genome;

                var mixedGenome = _neuralNetworkGateway.GetMixedGenome(genomeA, genomeB, _templateBrainCaracteristics[template.Name], 1, 0.01f);
                result[template.Name].Add(mixedGenome);
            }            
        }
        return result;
    }


    // Mapping & tooling
    private (BaseAnt parentA, BaseAnt parentB)[] CreateCouples(List<BaseAnt> bestUnits)
    {
        var result = new List<(BaseAnt parentA, BaseAnt parentB)>();
        while (bestUnits.Count > 1 && result.Count < GlobalParameters.ColonyMaxPopulation)
        {
            var firstindex = Random.Range(0, bestUnits.Count);
            var parentA = bestUnits[firstindex];
            var secondIndex = firstindex;
            while (secondIndex == firstindex)
                secondIndex = Random.Range(0, bestUnits.Count);
            var parentB = bestUnits[secondIndex];
            result.Add((parentA, parentB));
            parentA.GetNugetUnit.ChildrenNumber++;
            parentB.GetNugetUnit.ChildrenNumber++;
            bestUnits = bestUnits.Where(t => t.GetNugetUnit.ChildrenNumber < t.GetNugetUnit.MaxChildNumber).ToList();
        }

        return result.ToArray();
    }
    private BrainCaracteristics ToBrainCarac(BrainTemplate template)
    {
        return new BrainCaracteristics()
        {
            InputLayer = ToLayerCarac(template.InputLayer),
            NeutralLayers = template.NeutralLayers.Select(t => ToLayerCarac(t)).ToList(),
            OutputLayer = ToLayerCarac(template.OutputLayer),
            IsDecisionBrain = template.IsDecisionBrain,
            GenomeCaracteristics = ToGenomeCarac(template, template.GenomeCaracteristics)
        };
    }
    private BrainCaracteristics ToBrainCarac(BrainInstance instance)
    {
        var result = ToBrainCarac(instance.Template);
        result.BrainName = instance.UniqueName;
        return result;
    }
    private GenomeCaracteristics ToGenomeCarac(BrainTemplate template, GenomeParameters parameters)
    {
        var maxEdgeNumber = template.MaxEdgeNumberGet();
        var geneNumber = (int)(parameters.NetworkCoverage * maxEdgeNumber / 100f);

        return new GenomeCaracteristics
        {
            GeneNumber = geneNumber,
            WeighBytesNumber = parameters.WeightBitNumber
        };
    }

    private LayerCaracteristics ToLayerCarac(NeuronLayerCaracteristics carac)
    {
        return new LayerCaracteristics(carac.LayerId, carac.LayerType)
        {
            NeuronNumber = carac.NeuronNumber,
            ActivationFunction = carac.ActivationFunction,
            ActivationFunction90PercentTreshold = carac.ActivationFunction90PercentTreshold,
            NeuronTreshold = carac.NeuronTreshold
        };
    }
}
