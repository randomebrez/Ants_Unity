using Assets.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    internal class TrashTemp
    {
        //private BrainCaracteristicsTemplate TemplateCaracteristicsBuild(string name, List<UnityInputTypeEnum> portionInputs, int maxPortionIndex, LayerCaracteristics inputLayer, List<LayerCaracteristics> neutralLayers, LayerCaracteristics outputLayer, GenomeParameters genomeParameters, bool isDecisionBrain = false)
        //{
        //    var result = new BrainCaracteristicsTemplate
        //    {
        //        Name = name,
        //        InputLayer = inputLayer,
        //        NeutralLayers = neutralLayers,
        //        OutputLayer = outputLayer,
        //        GenomeCaracteristics = genomeParameters,
        //        IsDecisionBrain = isDecisionBrain
        //    };
        //
        //    var portionInputObj = new UnityInput(InputTypeEnum.Portion, maxPortionIndex);
        //    foreach (var portionInputType in portionInputs)
        //    {
        //        switch (portionInputType)
        //        {
        //            case UnityInputTypeEnum.PheromoneW:
        //            case UnityInputTypeEnum.PheromoneC:
        //            case UnityInputTypeEnum.Nest:
        //            case UnityInputTypeEnum.Food:
        //            case UnityInputTypeEnum.Walls:
        //                portionInputObj.UnityInputTypes.Add(portionInputType);
        //                break;
        //            case UnityInputTypeEnum.CarryFood:
        //                result.InputsTypes.Add(new UnityInput(InputTypeEnum.CarryFood));
        //                break;
        //        }
        //    }
        //    if (portionInputObj.UnityInputTypes.Any())
        //        result.InputsTypes.Add(portionInputObj);
        //
        //    result.NeedUnityInpus = portionInputs.Any();
        //
        //    return result;
        //}
        //
        //private LayerCaracteristics LayerCaracteristicsGet(LayerTypeEnum layerType, int layerId, int neuronNumber = 0, ActivationFunctionEnum activationFunction = ActivationFunctionEnum.Identity, float caracteristicValue = 0f)
        //{
        //    return new LayerCaracteristics(layerId, layerType)
        //    {
        //        NeuronNumber = neuronNumber,
        //        ActivationFunction = activationFunction,
        //        ActivationFunction90PercentTreshold = caracteristicValue
        //    };
        //}
        //
        //private GraphLink TemplateGraphLinkGet(BrainCaracteristicsTemplate target, BrainCaracteristicsTemplate origin, GraphLinkTypeEnum linkType)
        //{
        //    return new GraphLink
        //    {
        //        Target = target,
        //        Origin = origin,
        //        LinkTypeEnum = linkType
        //    };
        //}
        //
        //private GraphTemplate GenerateSplittedGraph()
        //{
        //    var result = new GraphTemplate();
        //    //result.Name = "Splitted";
        //    //
        //    //var decisionBrain = _fileStorageGateway.TemplateCaracteristicsFetchByName(GlobalParameters.SplittedDecisionBrain.Name);
        //    //if (decisionBrain == null)
        //    //{
        //    //    _fileStorageGateway.TemplateCaracteristicStore(GlobalParameters.SplittedDecisionBrain);
        //    //    decisionBrain = GlobalParameters.SplittedDecisionBrain;
        //    //}
        //    //result.DecisionBrain = decisionBrain;
        //    //result.Nodes.Add(result.DecisionBrain.Name, result.DecisionBrain);
        //    //
        //    //
        //    //var visionTp = _fileStorageGateway.TemplateCaracteristicsFetchByName("VisionTp");
        //    //if (visionTp == null)
        //    //{
        //    //    var genomeParameters = new GenomeParameters
        //    //    {
        //    //        NetworkCoverage = 80,
        //    //        WeightBitNumber = 4
        //    //    };
        //    //    var inputLayerVision = LayerCaracteristicsGet(LayerTypeEnum.Input, 0);
        //    //    var neutralLayersVision = new List<LayerCaracteristics> { LayerCaracteristicsGet(LayerTypeEnum.Neutral, 1, 2, ActivationFunctionEnum.Tanh, 0.5f) };
        //    //    var outputLayerVision = LayerCaracteristicsGet(LayerTypeEnum.Output, 2, 2, ActivationFunctionEnum.Sigmoid, 1);
        //    //    var visionInputs = new List<UnityInputTypeEnum> { UnityInputTypeEnum.PheromoneW, UnityInputTypeEnum.PheromoneC, UnityInputTypeEnum.Food, UnityInputTypeEnum.Nest, UnityInputTypeEnum.Walls };
        //    //
        //    //    visionTp = TemplateCaracteristicsBuild("VisionTp", visionInputs, 1, inputLayerVision, neutralLayersVision, outputLayerVision, genomeParameters);
        //    //    _fileStorageGateway.TemplateCaracteristicStore(visionTp);
        //    //}
        //    //result.Nodes.Add(visionTp.Name, visionTp);
        //    //
        //    //
        //    //var noVisionTp = _fileStorageGateway.TemplateCaracteristicsFetchByName("NoVisionTp");
        //    //if (noVisionTp == null)
        //    //{
        //    //    var genomeParameters = new GenomeParameters
        //    //    {
        //    //        NetworkCoverage = 80,
        //    //        WeightBitNumber = 4
        //    //    };
        //    //    var inputLayerNoVision = LayerCaracteristicsGet(LayerTypeEnum.Input, 0);
        //    //    var neutralLayersNoVision = new List<LayerCaracteristics> { LayerCaracteristicsGet(LayerTypeEnum.Neutral, 1, 2, ActivationFunctionEnum.Tanh, 0.5f) };
        //    //    var outputLayerNoVision = LayerCaracteristicsGet(LayerTypeEnum.Output, 2, 2, ActivationFunctionEnum.Sigmoid, 1);
        //    //    var noVisionInputs = new List<UnityInputTypeEnum> { UnityInputTypeEnum.PheromoneW, UnityInputTypeEnum.PheromoneC };
        //    //
        //    //    noVisionTp = TemplateCaracteristicsBuild("NoVisionTp", noVisionInputs, 1, inputLayerNoVision, neutralLayersNoVision, outputLayerNoVision, genomeParameters);
        //    //    _fileStorageGateway.TemplateCaracteristicStore(noVisionTp);
        //    //}
        //    //result.Nodes.Add(noVisionTp.Name, noVisionTp);
        //    //
        //    //
        //    //var linkVision = TemplateGraphLinkGet(result.DecisionBrain, visionTp, GraphLinkTypeEnum.SingleVisionPortions);
        //    //var linkNoVision = TemplateGraphLinkGet(result.DecisionBrain, noVisionTp, GraphLinkTypeEnum.SingleNoVisionPortions);
        //    //result.Edges.Add(result.DecisionBrain.Name, new List<GraphLink> { linkVision, linkNoVision });
        //
        //
        //    return result;
        //    //_fileStorageGateway.TemplateGraphStoreAsync(result).GetAwaiter().GetResult();
        //}
        //
        //private GraphTemplate GenerateBigBrainGraph()
        //{
        //    //var brainCarac = _fileStorageGateway.TemplateCaracteristicsFetchByName(GlobalParameters.BigBrainDecisionBrain.Name);
        //    //if (brainCarac == null)
        //    //{
        //    //    _fileStorageGateway.TemplateCaracteristicStore(GlobalParameters.BigBrainDecisionBrain);
        //    //    brainCarac = GlobalParameters.BigBrainDecisionBrain;
        //    //}
        //    //
        //    //var result = new GraphTemplate
        //    //{
        //    //    Name = "BigBrain",
        //    //    DecisionBrain = brainCarac
        //    //};
        //    //result.Nodes.Add(result.DecisionBrain.Name, result.DecisionBrain);
        //    //
        //    //_fileStorageGateway.TemplateGraphStoreAsync(result);
        //    //return result;
        //    return new GraphTemplate();
        //}
    }
}
