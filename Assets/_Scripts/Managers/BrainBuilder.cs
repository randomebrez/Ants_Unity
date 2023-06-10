using Assets._Scripts.Utilities;
using Assets.Dtos;
using mew;
using Microsoft.EntityFrameworkCore.Query.Internal;
using NeuralNetwork.Interfaces.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using static UnityEngine.UI.Image;

namespace Assets._Scripts.Managers
{
    public class BrainBuilder
    {
        private List<UnitPortionCaracteristics> _portions = new List<UnitPortionCaracteristics>();

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
            if (templateGraphName == "Splitted")
                return GenerateSplittedGraph();

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

            // ToDo : tester si modifier le InputLayer.InputNumber d'un template les modifie pas tous d'un coup ?
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
                carac.BrainName = $"{carac.Template.Name}_{templateUseCounter[carac.Template.Name]}";
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
                case TemplateLinkTypeEnum.Default:
                    result.Add(BrainCaracteristicBuild(link.Origin, new List<int>()));
                    break;
                case TemplateLinkTypeEnum.AllPortions:
                    result.Add(BrainCaracteristicBuild(link.Origin, _portions.Select(t => t.Id).ToList()));
                    break;
                case TemplateLinkTypeEnum.VisionPortions:
                    result.Add(BrainCaracteristicBuild(link.Origin, visionPortions.Select(t => t.Id).ToList()));
                    break;
                case TemplateLinkTypeEnum.NoVisionPortions:
                    result.Add(BrainCaracteristicBuild(link.Origin, noVisionPortions.Select(t => t.Id).ToList()));
                    break;
                case TemplateLinkTypeEnum.SingleAllPortions:
                    foreach (var portion in _portions)
                        result.Add(BrainCaracteristicBuild(link.Origin, new List<int> { portion.Id }));
                    break;
                case TemplateLinkTypeEnum.SingleVisionPortions:
                    foreach (var portion in visionPortions)
                        result.Add(BrainCaracteristicBuild(link.Origin, new List<int> { portion.Id }));
                    break;
                case TemplateLinkTypeEnum.SingleNoVisionPortions:
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
                var decisionBrain = BrainCaracteristicBuild(targetTemplate, new List<int>());
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
                    tempOriginsList.Add(originInstances[j].BrainName);

                // Add it for all instance target
                for (int i = 0; i < targetInstances.Count(); i++)
                    result.Add(targetInstances[i].BrainName, tempOriginsList);
            }

            return result;
        }

        
        // Count CaracteristicInstance input number
        // Requires the InstanceGraph to be set for 'BrainOutput' input type
        private int GetInputNumber(GraphInstance graph, BrainCaracteristicsInstance caracteristics)
        {
            // ToDo : Can be améliorer : On le fait sur chaque InstanceCaracteristic de l'instance graph. peut etre possible de le faire que sur les templates ?
            var result = 0;

            // Cette partie là oui
            foreach (var inputType in caracteristics.Template.InputsTypes)
            {
                switch (inputType.InputType)
                {
                    case InputTypeEnum.Portion:
                        {
                            var input = inputType as InputTypePortion;
                            result += input.UnityInputTypes.Count() * caracteristics.PortionIndexes.Count;
                            break;
                        }
                    case InputTypeEnum.CarryFood:
                        result++;
                        break;
                }
            }

            // Ici a voir si on peut pas imaginer des schémas ou le template n'aurait pas le même nombre d'input que toutes ses instances.
            if (graph.CaracteristicEdges.TryGetValue(caracteristics.BrainName, out var feeders))
            {
                foreach (var feeder in feeders)
                    result += graph.CaracteristicNodes[feeder].Template.OutputLayer.NeuronNumber;
            }
            return result;
        }


        // Constructor & Mapping 

        //ALL BrainInstance construction must be done HERE
        private BrainCaracteristicsInstance BrainCaracteristicBuild(BrainCaracteristicsTemplate caracTemplate, List<int> portionIndex, string instanceName = "")
        {
            if (string.IsNullOrEmpty(instanceName))
                instanceName = Guid.NewGuid().ToString();

            var needUnityInputs = portionIndex.Count > 0 && caracTemplate.InputsTypes.Any(t => GlobalParameters.UnityInputTypes.Contains(t.InputType));

            var result = new BrainCaracteristicsInstance(caracTemplate)
            {
                BrainName = instanceName,
                PortionIndexes = portionIndex
            };

            return result;
        }

        // Map the list of InstanceGraph nodes to a dictionary using Instance.BrainName as key
        private Dictionary<string, BrainCaracteristicsInstance> ToDictionary(List<BrainCaracteristicsInstance> instanceNodes)
        {
            var result = new Dictionary<string, BrainCaracteristicsInstance>();

            foreach (var node in instanceNodes)
                result.Add(node.BrainName, node);

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

            var portionInputObj = new InputTypePortion(maxPortionIndex);
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
                        result.InputsTypes.Add(new InputTypeCarryFood());
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
            return new LayerCaracteristics(layerType, layerId, neuronNumber, activationFunction, caracteristicValue);
        }

        private GraphLink TemplateGraphLinkGet(BrainCaracteristicsTemplate target, BrainCaracteristicsTemplate origin, TemplateLinkTypeEnum linkType)
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

            result.DecisionBrain = GlobalParameters.DecisionBrain;
            result.Nodes.Add(result.DecisionBrain.Name, result.DecisionBrain);

            var genomeParameters = new GenomeParameters
            {
                NetworkCoverage = 80,
                WeightBitNumber = 4
            };

            var inputLayer = LayerCaracteristicsGet(LayerTypeEnum.Input, 0);
            var neutralLayers = new List<LayerCaracteristics> { LayerCaracteristicsGet(LayerTypeEnum.Neutral, 1, 2, ActivationFunctionEnum.Tanh, 0.5f) };
            var outputLayer = LayerCaracteristicsGet(LayerTypeEnum.Output, 2, 2, ActivationFunctionEnum.Sigmoid, 1);

            var visionInputs = new List<UnityInputTypeEnum> { UnityInputTypeEnum.PheromoneW, UnityInputTypeEnum.PheromoneC, UnityInputTypeEnum.Food, UnityInputTypeEnum.Nest, UnityInputTypeEnum.Walls };
            var noVisionInputs = new List<UnityInputTypeEnum> { UnityInputTypeEnum.PheromoneW, UnityInputTypeEnum.PheromoneC };


            var visionTp = TemplateCaracteristicsBuild("VisionTp", visionInputs, 1, inputLayer, neutralLayers, outputLayer, genomeParameters);
            var noVisionTp = TemplateCaracteristicsBuild("NoVisionTp", noVisionInputs, 1, inputLayer, neutralLayers, outputLayer, genomeParameters);

            result.Nodes.Add(visionTp.Name, visionTp);
            result.Nodes.Add(noVisionTp.Name, noVisionTp);

            var linkVision = TemplateGraphLinkGet(result.DecisionBrain, visionTp, TemplateLinkTypeEnum.SingleVisionPortions);
            var linkNoVision = TemplateGraphLinkGet(result.DecisionBrain, noVisionTp, TemplateLinkTypeEnum.SingleNoVisionPortions);

            result.Edges.Add(result.DecisionBrain.Name, new List<GraphLink> { linkVision, linkNoVision });

            return result;
        }
    }
}
