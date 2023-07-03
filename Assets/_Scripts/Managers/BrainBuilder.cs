using Assets._Scripts.Utilities;
using Assets.Dtos;
using NeuralNetwork.Abstraction.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Scripts.Managers
{
    public class BrainBuilder
    {
        private List<UnitPortionCaracteristics> _portions = new List<UnitPortionCaracteristics>();

        public BrainBuilder()
        {
            GenerateBaseBrainAndGraph();
        }

        // Ask to retrieve a TemplateGraph by its name
        public InstanceGraph UserSelectionInstanceGraphGet(string templateGraphName)
        {
            // Retrieve the TemplateGraph from DataSource
            var templateGraph = DatabaseManager.Instance.TemplateGraphFetch(templateGraphName);
            // Map it to an InstanceGraph
            var result = InstanceGraphGet(templateGraph);
            return result;
        }

        // Transform a TemplateGraph in a InstanceGraph using simulation parameters
        private InstanceGraph InstanceGraphGet(TemplateGraph templateGraph)
        {
            // Calculate Unit ScannerPortion using GlobalParameters
            _portions = TypePortionIndexesGet(GlobalParameters.ScannerSubdivision, GlobalParameters.VisionAngle);
            // Create all nodes of the InstanceGraph 
            var nodes = BrainGraphNodesRecBuild(templateGraph, templateGraph.DecisionBrain);
            // Create all edges of the InstanceGraph
            var edges = BrainCaracteristicsEdgesBuild(templateGraph, nodes);

            var unityInputUsingTemplates = templateGraph.Nodes.Where(t => t.Value.NeedUnityInpus).ToDictionary(t => t.Key, t => t.Value);
            var result = new InstanceGraph
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


        // Reading the TemplateGraph from it's DecisionBrain recursively build all CaracteristicInstance (Nodes) of the graph. (WITHOUT EDGES)
        private List<BrainInstance> BrainGraphNodesRecBuild(TemplateGraph graph, BrainTemplate targetTemplate)
        {
            var result = new List<BrainInstance>();
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
                var newOrigins = new List<BrainInstance>();
                var originTemplates = new Dictionary<string, BrainTemplate>();
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
        private Dictionary<string, List<string>> BrainCaracteristicsEdgesBuild(TemplateGraph tpGraph, List<BrainInstance> instanceNodes)
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

        // Objective here is to build all CaracteristicInstance, given a link in a TemplateGraph.
        // There can be many to build for a single link only if portion are required
        // Link contains information about ScannerPortion that generated brains should use.
        // Ex :
        // 1. If we have a link with SingleVisionPortion info => create one CaracteristicInstance for each VisionPortion
        // 2. If we have a link with VisionPortion info => create one CaracteristicInstance using inputs from all VisionPortions
        private List<BrainInstance> EdgeOriginCaracteristicInstancesBuild(GraphLink link)
        {
            var result = new List<BrainInstance>();

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









        // Count CaracteristicInstance input number
        // Requires the InstanceGraph to be set for 'BrainOutput' input type
        private int GetInputNumber(InstanceGraph graph, BrainInstance caracteristics)
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


        // Return the list of portions of a unit
        // Portion is a couple (minAngle, MaxAngle) with Id and a boolean that is equal to 'true' if the portion intersects the unit VisionField
        private List<UnitPortionCaracteristics> TypePortionIndexesGet(int portionNumber, float visionAngle)
        {
            var result = new List<UnitPortionCaracteristics>();

            var deltaTheta = 360f / portionNumber;
            var currentMin = -180 - deltaTheta / 2f;
            var minVisionAngle = -visionAngle / 2f;
            var maxVisionAngle = visionAngle / 2f;

            for (int i = 0; i < portionNumber; i++)
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
        private BrainInstance BrainCaracteristicBuild(BrainTemplate caracTemplate, List<int> portionIndexes, string prettyName = "")
        {
            var result = new BrainInstance(caracTemplate)
            {
                UniqueName = Guid.NewGuid().ToString(),
                PrettyName = prettyName
            };

            // portionindex is a filter that is set using GraphLink.
            // It allows to restrain a template to certain portions.
            // TpA : InputType1 : PortionTypeEnum.AllTypes
            // Without filter ==> PortionIndexes = _portions.Id.tolist
            // If used in a graph with for example a SingleVision GraphLink ==> PortionIndexes = filter (calculated in external method using a switch on GraphLink.Type)
            foreach (var inputPortion in caracTemplate.InputsTypes.Where(t => t.InputType == InputTypeEnum.Portion))
            {
                var instanceInputPortion = InputPortionCopyAndSetIndexes(inputPortion, portionIndexes);
                result.InputPortions.Add(instanceInputPortion);
            };

            return result;
        }

        // Map the list of InstanceGraph nodes to a dictionary using Instance.UniqueName as key
        private Dictionary<string, BrainInstance> ToDictionary(List<BrainInstance> instanceNodes)
        {
            var result = new Dictionary<string, BrainInstance>();

            foreach (var node in instanceNodes)
                result.Add(node.UniqueName, node);

            return result;
        }




        private void GenerateBaseBrainAndGraph()
        {
            try
            {
                GenerateSplittedGraph();
                GenerateBigBrainGraph();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void GenerateSplittedGraph()
        {
            if (DatabaseManager.Instance.TemplateGraphExist("Splitted"))
                return;

            var result = new TemplateGraph();
            result.Name = "Splitted";

            var decisionBrain = DatabaseManager.Instance.BrainTemplateFetch(name: GlobalParameters.SplittedDecisionBrain.Name);
            if (decisionBrain == null)
            {
                DatabaseManager.Instance.BrainTemplateStore(GlobalParameters.SplittedDecisionBrain);
                decisionBrain = DatabaseManager.Instance.BrainTemplateFetch(name: GlobalParameters.SplittedDecisionBrain.Name);
            }
            result.DecisionBrain = decisionBrain;
            result.Nodes.Add(result.DecisionBrain.Name, result.DecisionBrain);


            var visionTp = DatabaseManager.Instance.BrainTemplateFetch(name: "VisionTp");
            if (visionTp == null)
            {
                var genomeParameters = new GenomeParameters
                {
                    NetworkCoverage = 80,
                    WeightBitNumber = 4
                };
                var inputLayerVision = LayerCaracteristicsGet(LayerTypeEnum.Input, 0);
                var neutralLayersVision = new List<NeuronLayerCaracteristics> { LayerCaracteristicsGet(LayerTypeEnum.Neutral, 1, 2, ActivationFunctionEnum.Tanh, 0.5f) };
                var outputLayerVision = LayerCaracteristicsGet(LayerTypeEnum.Output, 2, 2, ActivationFunctionEnum.Sigmoid, 1);
                var visionInputs = new List<UnityInputTypeEnum> { UnityInputTypeEnum.PheromoneW, UnityInputTypeEnum.PheromoneC, UnityInputTypeEnum.Food, UnityInputTypeEnum.Nest, UnityInputTypeEnum.Walls, UnityInputTypeEnum.CarryFood };

                visionTp = TemplateCaracteristicsBuild("VisionTp", visionInputs, 1, inputLayerVision, neutralLayersVision, outputLayerVision, genomeParameters);
                DatabaseManager.Instance.BrainTemplateStore(visionTp);
                visionTp = DatabaseManager.Instance.BrainTemplateFetch(name: "VisionTp");
            }
            result.Nodes.Add(visionTp.Name, visionTp);


            var noVisionTp = DatabaseManager.Instance.BrainTemplateFetch(name: "NoVisionTp");
            if (noVisionTp == null)
            {
                var genomeParameters = new GenomeParameters
                {
                    NetworkCoverage = 80,
                    WeightBitNumber = 4
                };
                var inputLayerNoVision = LayerCaracteristicsGet(LayerTypeEnum.Input, 0);
                var neutralLayersNoVision = new List<NeuronLayerCaracteristics> { LayerCaracteristicsGet(LayerTypeEnum.Neutral, 1, 2, ActivationFunctionEnum.Tanh, 0.5f) };
                var outputLayerNoVision = LayerCaracteristicsGet(LayerTypeEnum.Output, 2, 2, ActivationFunctionEnum.Sigmoid, 1);
                var noVisionInputs = new List<UnityInputTypeEnum> { UnityInputTypeEnum.PheromoneW, UnityInputTypeEnum.PheromoneC, UnityInputTypeEnum.CarryFood };

                noVisionTp = TemplateCaracteristicsBuild("NoVisionTp", noVisionInputs, 1, inputLayerNoVision, neutralLayersNoVision, outputLayerNoVision, genomeParameters);
                DatabaseManager.Instance.BrainTemplateStore(noVisionTp);
                noVisionTp = DatabaseManager.Instance.BrainTemplateFetch(name: "NoVisionTp");
            }
            result.Nodes.Add(noVisionTp.Name, noVisionTp);


            var linkVision = TemplateGraphLinkGet(result.DecisionBrain, visionTp, GraphLinkTypeEnum.SingleVisionPortions);
            var linkNoVision = TemplateGraphLinkGet(result.DecisionBrain, noVisionTp, GraphLinkTypeEnum.SingleNoVisionPortions);
            result.Edges.Add(result.DecisionBrain.Name, new List<GraphLink> { linkVision, linkNoVision });


            DatabaseManager.Instance.TemplateGraphStore(result);
        }

        private void GenerateBigBrainGraph()
        {
            if (DatabaseManager.Instance.TemplateGraphExist("BigBrain"))
                return;

            var brainCarac = DatabaseManager.Instance.BrainTemplateFetch(name: GlobalParameters.BigBrainDecisionBrain.Name);
            if (brainCarac == null)
            {
                DatabaseManager.Instance.BrainTemplateStore(GlobalParameters.BigBrainDecisionBrain);
                brainCarac = DatabaseManager.Instance.BrainTemplateFetch(name: GlobalParameters.BigBrainDecisionBrain.Name);
            }

            var result = new TemplateGraph
            {
                Name = "BigBrain",
                DecisionBrain = brainCarac
            };
            result.Nodes.Add(result.DecisionBrain.Name, result.DecisionBrain);

            DatabaseManager.Instance.TemplateGraphStore(result);
        }

        private BrainTemplate TemplateCaracteristicsBuild(string name, List<UnityInputTypeEnum> portionInputs, int maxPortionIndex, NeuronLayerCaracteristics inputLayer, List<NeuronLayerCaracteristics> neutralLayers, NeuronLayerCaracteristics outputLayer, GenomeParameters genomeParameters, bool isDecisionBrain = false)
        {
            var result = new BrainTemplate
            {
                Name = name,
                InputLayer = inputLayer,
                NeutralLayers = neutralLayers,
                OutputLayer = outputLayer,
                GenomeCaracteristics = genomeParameters,
                IsDecisionBrain = isDecisionBrain
            };
        
            var portionInputObj = new UnityInput(InputTypeEnum.Portion, maxPortionIndex);
            foreach (var portionInputType in portionInputs)
            {
                switch (portionInputType)
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
        
        private NeuronLayerCaracteristics LayerCaracteristicsGet(LayerTypeEnum layerType, int layerId, int neuronNumber = 0, ActivationFunctionEnum activationFunction = ActivationFunctionEnum.Identity, float caracteristicValue = 0f)
        {
            return new NeuronLayerCaracteristics
            {
                LayerType = layerType,
                LayerId = layerId,
                NeuronNumber = neuronNumber,
                ActivationFunction = activationFunction,
                ActivationFunction90PercentTreshold = caracteristicValue
            };
        }
        
        private GraphLink TemplateGraphLinkGet(BrainTemplate target, BrainTemplate origin, GraphLinkTypeEnum linkType)
        {
            return new GraphLink
            {
                Target = target,
                Origin = origin,
                LinkTypeEnum = linkType
            };
        }
    }
}
