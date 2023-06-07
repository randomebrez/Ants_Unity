using Assets.Dtos;
using NeuralNetwork.Interfaces.Model;
using System.Collections.Generic;

namespace Assets._Scripts.Utilities
{
    public static class GlobalBrainParameters
    {
        #region Caracteristics templates

        public static BrainCaracteristicsTemplate DecisionBrainMeta = new BrainCaracteristicsTemplate
        {
            Name = "DecisionMeta",
            IsDecisionBrain = true,
            InputLayer = new LayerCaracteristics(LayerTypeEnum.Input, 0),
            NeutralLayers = new List<LayerCaracteristics>
                {
                    new LayerCaracteristics(LayerTypeEnum.Neutral, 1, 2, ActivationFunctionEnum.Tanh, 0.5f)
                },
            OutputLayer = new LayerCaracteristics(LayerTypeEnum.Output, 2, 6, ActivationFunctionEnum.Sigmoid, 1),
            GenomeCaracteristics = new GenomeParameters
            {
                NetworkCoverage = 80,
                WeightBitNumber = 4
            }
        };

        public static HashSet<InputTypeEnum> UnityInputTypes = new HashSet<InputTypeEnum>
        {
            InputTypeEnum.Portion,
            InputTypeEnum.CarryFood
        };

        //public static BrainCaracteristicsTemplate InterpretationBrain = new BrainCaracteristicsTemplate
        //{
        //    Name = "InterpretationBrain",
        //    IsDecisionBrain = true,
        //    InputsTypes = new List<InputTypeBase> { BrainOutputInput },
        //    InputLayer = new LayerCaracteristics(LayerTypeEnum.Input, 0),
        //    NeutralLayers = new List<LayerCaracteristics>
        //        {
        //            new LayerCaracteristics(LayerTypeEnum.Neutral, 1, 2, ActivationFunctionEnum.Tanh, 0.5f)
        //        },
        //    OutputLayer = new LayerCaracteristics(LayerTypeEnum.Output, 2, 6, ActivationFunctionEnum.Sigmoid, 1),
        //};

        #endregion
    }
}
