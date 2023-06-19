using Assets._Scripts.Gateways;
using Assets._Scripts.Utilities;
using Assets.Abstractions;
using Assets.Dtos;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NeuralNetwork.Abstraction.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Scripts.Managers
{
    public class BrainBuilder
    {
        private List<UnitPortionCaracteristics> _portions = new List<UnitPortionCaracteristics>();
        private IStorage _fileStorageGateway;

        public BrainBuilder() 
        {
            _fileStorageGateway = new FileStorageGateway(GlobalParameters.SavedBrainsFolder, GlobalParameters.TemplateFileName, GlobalParameters.GraphTemplatesFileName);
        }

        // Ask to retrieve a TemplateGraph by its name
        public GraphInstance UserSelectionInstanceGraphGet(string templateGraphName)
        {
            // Retrieve the TemplateGraph from DataSource
            var templateGraph = TemplateGraphGetFromDataSource(templateGraphName);
            // Map it to an InstanceGraph
            var result = InstanceGraphGet(templateGraph);
            return result;
        }

        // Get the TemplateGraph from the dataSource by its name
        private GraphTemplate TemplateGraphGetFromDataSource(string templateGraphName)
        {
            //var graph = _fileStorageGateway.GraphTemplateFetchAsync(templateGraphName).GetAwaiter().GetResult();
            //return graph;
            if (templateGraphName == "Splitted")
            {
                return GenerateSplittedGraph();
            }
            else if (templateGraphName == "BigBrain")
                return GenerateBigBrainGraph();
            
            return new GraphTemplate();
        }


        // Transform a TemplateGraph in a InstanceGraph using simulation parameters
        private GraphInstance InstanceGraphGet(GraphTemplate templateGraph)
        {
            // Calculate Unit ScannerPortion using GlobalParameters
            _portions = TypePortionIndexesGet(GlobalParameters.ScannerSubdivision, GlobalParameters.VisionAngle);
            // Create all nodes of the InstanceGraph 
            var nodes = BrainGraphNodesRecBuild(templateGraph, templateGraph.DecisionBrain);
            // Create all edges of the InstanceGraph
            var edges = BrainCaracteristicsEdgesBuild(templateGraph, nodes);

            var unityInputUsingTemplates = templateGraph.Nodes.Where(t => t.Value.NeedUnityInpus).ToDictionary(t => t.Key, t => t.Value);
            var result = new GraphInstance
            {
                CaracteristicNodes = ToDictionary(nodes),
                CaracteristicEdges = edges,
                UnityInputsUsingTemplates = templateGraph.Nodes.Where(t => t.Value.NeedUnityInpus).ToDictionary(t => t.Key, t => t.Value),
                InstanceByTemplate = nodes.GroupBy(t => t.Template.Name).ToDictionary(t => t.Key, t => t.ToList())
            };

            var templateUseCounter = new Dictionary<string, int>();
            var templateInputNumber = new Dictionary<string, int>();
            var distinctTemplateList = new HashSet<string>();

            foreach (var carac in result.CaracteristicNodes.Values)
            {
                // if we never encounter yet that template
                if (distinctTemplateList.Contains(carac.Template.Name) == false)
                {
                    // Count it's input neuron number within the graph
                    var inputNumber = GetInputNumber(result, carac);
                    // Save the input number for that template
                    templateInputNumber.Add(carac.Template.Name, inputNumber);

                    distinctTemplateList.Add(carac.Template.Name);
                    templateUseCounter.Add(carac.Template.Name, 0);
                }


                // Fill in it's input neuron number in InputLayer
                carac.Template.InputLayer.NeuronNumber = templateInputNumber[carac.Template.Name];
                // Give the instance a name
                carac.PrettyName = $"{carac.Template.Name}_{templateUseCounter[carac.Template.Name]}";
                // increment use of that template
                templateUseCounter[carac.Template.Name] += 1;
            }

            return result;
        }



        // Objective here is to build all CaracteristicInstance, given a link in a TemplateGraph.
        // There can be many to build for a single link only if portion are required
        // Link contains information about ScannerPortion that generated brains should use.
        // Ex :
        // 1. If we have a link with SingleVisionPortion info => create one CaracteristicInstance for each VisionPortion
        // 2. If we have a link with VisionPortion info => create one CaracteristicInstance using inputs from all VisionPortions
        private List<BrainCaracteristicsInstance> EdgeOriginCaracteristicInstancesBuild(GraphLink link)
        {
            var result = new List<BrainCaracteristicsInstance>();

            var visionPortions = _portions.Where(t => t.IntersectVisionRange);
            var noVisionPortions = _portions.Where(t => t.IntersectVisionRange == false);

            switch (link.LinkTypeEnum)
            {
                case GraphLinkTypeEnum.Default:
                    result.Add(BrainCaracteristicBuild(link.Origin, null));
                    break;
                case GraphLinkTypeEnum.AllPortions:
                    result.Add(BrainCaracteristicBuild(link.Origin, _portions.Select(t => t.Id).ToList()));
                    break;
                case GraphLinkTypeEnum.VisionPortions:
                    result.Add(BrainCaracteristicBuild(link.Origin, visionPortions.Select(t => t.Id).ToList()));
                    break;
                case GraphLinkTypeEnum.NoVisionPortions:
                    result.Add(BrainCaracteristicBuild(link.Origin, noVisionPortions.Select(t => t.Id).ToList()));
                    break;
                case GraphLinkTypeEnum.SingleAllPortions:
                    foreach (var portion in _portions)
                        result.Add(BrainCaracteristicBuild(link.Origin, new List<int> { portion.Id }));
                    break;
                case GraphLinkTypeEnum.SingleVisionPortions:
                    foreach (var portion in visionPortions)
                        result.Add(BrainCaracteristicBuild(link.Origin, new List<int> { portion.Id }));
                    break;
                case GraphLinkTypeEnum.SingleNoVisionPortions:
                    foreach (var portion in noVisionPortions)
                        result.Add(BrainCaracteristicBuild(link.Origin, new List<int> { portion.Id }));
                    break;
            }

            return result;
        }

        
        // Return the list of portions of a unit
        // Portion is a couple (minAngle, MaxAngle) with Id and a boolean that is equal to 'true' if the portion intersects the unit VisionField
        private List<UnitPortionCaracteristics> TypePortionIndexesGet(int portionNumber, float visionAngle)
        {
            var result = new List<UnitPortionCaracteristics>();

            var deltaTheta = 360f / portionNumber;
            var currentMin = -180 - deltaTheta / 2f;
            var minVisionAngle = -visionAngle / 2f;
            var maxVisionAngle = visionAngle / 2f;

            for(int i = 0; i < portionNumber; i++)
            {
                var currentMax = currentMin + deltaTheta;
                var inVisionField = (currentMax > minVisionAngle) && (currentMin < maxVisionAngle);

                var portion = new UnitPortionCaracteristics
                {
                    MinAngle = currentMin,
                    MaxAngle = currentMax,
                    PrettyName = $"{i}",
                    Id = i,
                    IntersectVisionRange = inVisionField
                };
                result.Add(portion);
                currentMin = currentMax;
            }

            return result;
        }


        // Reading the TemplateGraph from it's DecisionBrain recursively build all CaracteristicInstance (Nodes) of the graph. (WITHOUT EDGES)
        private List<BrainCaracteristicsInstance> BrainGraphNodesRecBuild(GraphTemplate graph, BrainCaracteristicsTemplate targetTemplate)
        {
            var result = new List<BrainCaracteristicsInstance>();
            // Only add decisionBrain "manually"
            if (targetTemplate.IsDecisionBrain)
            {
                // ToDo : Can't this brain have portions too ? 
                // What if our graph is a unique brain like first brain implemented ?
                var decisionBrain = BrainCaracteristicBuild(targetTemplate, null);
                result.Add(decisionBrain);
            }
            
            if (graph.Edges.TryGetValue(targetTemplate.Name, out var currentLinks) == false)
                return result;
            else
            {
                //Build origin instances
                // May be several for each link (for example one by portion)
                var newOrigins = new List<BrainCaracteristicsInstance>();
                var originTemplates = new Dictionary<string, BrainCaracteristicsTemplate>();
                foreach(var currentLink in currentLinks)
                {
                    newOrigins.AddRange(EdgeOriginCaracteristicInstancesBuild(currentLink));
                    if (originTemplates.ContainsKey(currentLink.Origin.Name) == false)
                        originTemplates.Add(currentLink.Origin.Name, currentLink.Origin);
                }
                result.AddRange(newOrigins);

                //Rec call on origin template
                foreach (var originTemplate in originTemplates)
                    result.AddRange(BrainGraphNodesRecBuild(graph, originTemplate.Value));
            }
            return result;
        }

        // Reading the TemplateGraph edges, build edges for the InstanceGraph
        private Dictionary<string, List<string>> BrainCaracteristicsEdgesBuild(GraphTemplate tpGraph, List<BrainCaracteristicsInstance> instanceNodes)
        {
            var result = new Dictionary<string, List<string>>();
            foreach(var targetLinks in tpGraph.Edges)
            {
                var originTemplateNames = targetLinks.Value.Select(t => t.Origin.Name).ToHashSet();
                var targetTemplateName = targetLinks.Key;

                // Search for instances of corresponding templates
                var targetInstances = instanceNodes.Where(t => t.Template.Name == targetTemplateName).ToList();
                var originInstances = instanceNodes.Where(t => originTemplateNames.Contains(t.Template.Name)).ToList();

                // Build origin brain names list
                var tempOriginsList = new List<string>();
                for (int j = 0; j < originInstances.Count(); j++)
                    tempOriginsList.Add(originInstances[j].UniqueName);

                // Add it for all instance target
                for (int i = 0; i < targetInstances.Count(); i++)
                    result.Add(targetInstances[i].UniqueName, tempOriginsList);
            }

            return result;
        }

        
        // Count CaracteristicInstance input number
        // Requires the InstanceGraph to be set for 'BrainOutput' input type
        private int GetInputNumber(GraphInstance graph, BrainCaracteristicsInstance caracteristics)
        {
            var result = 0;
            foreach (var inputType in caracteristics.InputPortions)
                result += inputType.UnityInputTypes.Count() * inputType.PortionIndexes.Count;
            if (caracteristics.Template.InputsTypes.Any(t => t.InputType == InputTypeEnum.CarryFood))
                result++;
            
            // Ici a voir si on peut pas imaginer des schémas ou le template n'aurait pas le même nombre d'input que toutes ses instances.
            if (graph.CaracteristicEdges.TryGetValue(caracteristics.UniqueName, out var feeders))
            {
                foreach (var feeder in feeders)
                    result += graph.CaracteristicNodes[feeder].Template.OutputLayer.NeuronNumber;
            }
            return result;
        }

        private UnityInput InputPortionCopyAndSetIndexes(UnityInput portion, List<int> indexes = null)
        {
            var visionPortions = _portions.Where(t => t.IntersectVisionRange);
            var noVisionPortions = _portions.Where(t => t.IntersectVisionRange == false);

            var result = new UnityInput(InputTypeEnum.Portion, portion.MaxPortionIndexes)
            {
                PortionTypeToApplyOn = portion.PortionTypeToApplyOn,
                UnityInputTypes = portion.UnityInputTypes
            };
            
            if (indexes == null)
            {
                indexes = new List<int>();
                switch (portion.PortionTypeToApplyOn)
                {
                    case PortionTypeEnum.WithinSightField:
                        indexes = _portions.Where(t => t.IntersectVisionRange).Select(t => t.Id).ToList();
                        break;
                    case PortionTypeEnum.OutSightField:
                        indexes = _portions.Where(t => t.IntersectVisionRange == false).Select(t => t.Id).ToList();
                        break;
                    case PortionTypeEnum.AllTypes:
                        indexes = _portions.Select(t => t.Id).ToList();
                        break;
                }
            }

            result.PortionIndexes = indexes;

            return result;
        }


        // Constructor & Mapping 

        //ALL BrainInstance construction must be done HERE
        private BrainCaracteristicsInstance BrainCaracteristicBuild(BrainCaracteristicsTemplate caracTemplate, List<int> portionIndexes, string prettyName = "")
        {
            var result = new BrainCaracteristicsInstance(caracTemplate)
            {
                UniqueName = Guid.NewGuid().ToString(),
                PrettyName = prettyName
            };

            // portionindex is a filter that is set using GraphLink.
            // It allows to restrain a template to certain portions.
            // TpA : InputType1 : PortionTypeEnum.AllTypes
            // Without filter ==> PortionIndexes = _portions.Id.tolist
            // If used in a graph with for example a SingleVision GraphLink ==> PortionIndexes = filter (calculated in external method using a switch on GraphLink.Type)
            foreach (var inputPortion in caracTemplate.InputsTypes.Where(t => t is UnityInput))
            {
                var instanceInputPortion = InputPortionCopyAndSetIndexes(inputPortion as UnityInput, portionIndexes);
                result.InputPortions.Add(instanceInputPortion);
            };

            return result;
        }

        // Map the list of InstanceGraph nodes to a dictionary using Instance.UniqueName as key
        private Dictionary<string, BrainCaracteristicsInstance> ToDictionary(List<BrainCaracteristicsInstance> instanceNodes)
        {
            var result = new Dictionary<string, BrainCaracteristicsInstance>();

            foreach (var node in instanceNodes)
                result.Add(node.UniqueName, node);

            return result;
        }



        private BrainCaracteristicsTemplate TemplateCaracteristicsBuild(string name, List<UnityInputTypeEnum> portionInputs, int maxPortionIndex, LayerCaracteristics inputLayer, List<LayerCaracteristics> neutralLayers, LayerCaracteristics outputLayer, GenomeParameters genomeParameters, bool isDecisionBrain = false)
        {
            var result = new BrainCaracteristicsTemplate
            {
                Name = name,
                InputLayer = inputLayer,
                NeutralLayers = neutralLayers,
                OutputLayer = outputLayer,
                GenomeCaracteristics = genomeParameters,
                IsDecisionBrain = isDecisionBrain
            };

            var portionInputObj = new UnityInput(InputTypeEnum.Portion, maxPortionIndex);
            foreach(var portionInputType in portionInputs)
            {
                switch(portionInputType)
                {
                    case UnityInputTypeEnum.PheromoneW:
                    case UnityInputTypeEnum.PheromoneC:
                    case UnityInputTypeEnum.Nest:
                    case UnityInputTypeEnum.Food:
                    case UnityInputTypeEnum.Walls:
                        portionInputObj.UnityInputTypes.Add(portionInputType);
                        break;
                    case UnityInputTypeEnum.CarryFood:
                        result.InputsTypes.Add(new UnityInput(InputTypeEnum.CarryFood));
                        break;
                }
            }
            if (portionInputObj.UnityInputTypes.Any())
                result.InputsTypes.Add(portionInputObj);

            result.NeedUnityInpus = portionInputs.Any();

            return result;
        }

        private LayerCaracteristics LayerCaracteristicsGet(LayerTypeEnum layerType, int layerId, int neuronNumber = 0, ActivationFunctionEnum activationFunction = ActivationFunctionEnum.Identity, float caracteristicValue = 0f)
        {
            return new LayerCaracteristics(layerId, layerType)
            {
                NeuronNumber = neuronNumber,
                ActivationFunction = activationFunction,
                ActivationFunction90PercentTreshold = caracteristicValue
            };
        }

        private GraphLink TemplateGraphLinkGet(BrainCaracteristicsTemplate target, BrainCaracteristicsTemplate origin, GraphLinkTypeEnum linkType)
        {
            return new GraphLink
            {
                Target = target,
                Origin = origin,
                LinkTypeEnum = linkType
            };
        }

        private GraphTemplate GenerateSplittedGraph()
        {
            var result = new GraphTemplate();

            result.Name = "Splitted";
            result.DecisionBrain = GlobalParameters.SplittedDecisionBrain;
            result.Nodes.Add(result.DecisionBrain.Name, result.DecisionBrain);

            var genomeParameters = new GenomeParameters
            {
                NetworkCoverage = 80,
                WeightBitNumber = 4
            };

            var inputLayerVision = LayerCaracteristicsGet(LayerTypeEnum.Input, 0);
            var neutralLayersVision = new List<LayerCaracteristics> { LayerCaracteristicsGet(LayerTypeEnum.Neutral, 1, 2, ActivationFunctionEnum.Tanh, 0.5f) };
            var outputLayerVision = LayerCaracteristicsGet(LayerTypeEnum.Output, 2, 2, ActivationFunctionEnum.Sigmoid, 1);

            var inputLayerNoVision = LayerCaracteristicsGet(LayerTypeEnum.Input, 0);
            var neutralLayersNoVision = new List<LayerCaracteristics> { LayerCaracteristicsGet(LayerTypeEnum.Neutral, 1, 2, ActivationFunctionEnum.Tanh, 0.5f) };
            var outputLayerNoVision = LayerCaracteristicsGet(LayerTypeEnum.Output, 2, 2, ActivationFunctionEnum.Sigmoid, 1);

            var visionInputs = new List<UnityInputTypeEnum> { UnityInputTypeEnum.PheromoneW, UnityInputTypeEnum.PheromoneC, UnityInputTypeEnum.Food, UnityInputTypeEnum.Nest, UnityInputTypeEnum.Walls };
            var noVisionInputs = new List<UnityInputTypeEnum> { UnityInputTypeEnum.PheromoneW, UnityInputTypeEnum.PheromoneC };


            var visionTp = TemplateCaracteristicsBuild("VisionTp", visionInputs, 1, inputLayerVision, neutralLayersVision, outputLayerVision, genomeParameters);
            var noVisionTp = TemplateCaracteristicsBuild("NoVisionTp", noVisionInputs, 1, inputLayerNoVision, neutralLayersNoVision, outputLayerNoVision, genomeParameters);

            //_fileStorageGateway.TemplateCaracteristicStoreAsync(GlobalParameters.SplittedDecisionBrain).GetAwaiter().GetResult();
            //_fileStorageGateway.TemplateCaracteristicStoreAsync(visionTp).GetAwaiter().GetResult();
            //_fileStorageGateway.TemplateCaracteristicStoreAsync(noVisionTp).GetAwaiter().GetResult();

            result.Nodes.Add(visionTp.Name, visionTp);
            result.Nodes.Add(noVisionTp.Name, noVisionTp);

            var linkVision = TemplateGraphLinkGet(result.DecisionBrain, visionTp, GraphLinkTypeEnum.SingleVisionPortions);
            var linkNoVision = TemplateGraphLinkGet(result.DecisionBrain, noVisionTp, GraphLinkTypeEnum.SingleNoVisionPortions);

            result.Edges.Add(result.DecisionBrain.Name, new List<GraphLink> { linkVision, linkNoVision });

            //_fileStorageGateway.TemplateGraphStoreAsync(result).GetAwaiter().GetResult();
            return result;
        }

        private GraphTemplate GenerateBigBrainGraph()
        {
            var result = new GraphTemplate
            {
                Name = "BigBrain",
                DecisionBrain = GlobalParameters.BigBrainDecisionBrain
            };
            result.Nodes.Add(result.DecisionBrain.Name, result.DecisionBrain);

            //_fileStorageGateway.TemplateCaracteristicStoreAsync(GlobalParameters.BigBrainDecisionBrain);
            //_fileStorageGateway.TemplateGraphStoreAsync(result);
            return result;
        }
    }
}
