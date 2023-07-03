using Assets.Dtos;
using NeuralNetwork.Abstraction.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts.Utilities
{
    public static class GlobalParameters
    {
        // Environment
        public static Vector3 GroundSize = new(125, 0, 75);
        public static float NodeRadius = 1f;
        public static int InitialFoodTokenNumber = 100;
        public static int SpawnRandomFoodFreq = 10;

        // AntParameters
        public static int ColonyMaxPopulation = 50;
        public static float LifeTimeFrame = 200;

        public static int ScannerSubdivision = 6;
        public static int VisionAngle = 120;
        public static int VisionRange = 4;
        public static int UnitNumberToSelect => ReproductionCaracteristics.PercentToSelect * ColonyMaxPopulation / 100;

        public static HashSet<InputTypeEnum> UnityInputTypes = new HashSet<InputTypeEnum>
        {
            InputTypeEnum.Portion,
            InputTypeEnum.CarryFood
        };

        // Reproduction parameters
        public static ReproductionCaracteristics ReproductionCaracteristics = new ReproductionCaracteristics
        {
            PercentToSelect = 67,
            MeanChildNumberByUnit = 3,
            CrossOverNumber = 1,
            MutationRate = 0.01f
        };

        // Selected brain graph
        //BigBrain
        //Splitted
        public static string SelectedBrainGraph = "BigBrain";


        //
        public static string LogFileBase = "D:\\Codes\\Test\\AntWinners";

        // Database
        public static int StoreFrequency = 20;
        public static string SqlFolderPath = @".\Database";
        public static string DbFileName = "TestDb.db";

        // Saved brains
        public static string SavedBrainsFolder = @".\SavedBrains";
        public static string TemplateFileName = @"BrainTemplate";
        public static string GraphTemplatesFileName = @"TemplateGraphs";


        // A bouger de là
        public static BrainTemplate SplittedDecisionBrain = new BrainTemplate
        {
            Name = "DecisionSplitted",
            IsDecisionBrain = true,
            InputLayer = new NeuronLayerCaracteristics
            {
                LayerType = LayerTypeEnum.Input,
                LayerId = 0,
                ActivationFunction = ActivationFunctionEnum.Identity
            },
            NeutralLayers = new List<NeuronLayerCaracteristics>
                {
                    new NeuronLayerCaracteristics
                    {
                        LayerType = LayerTypeEnum.Neutral,
                        LayerId = 1,
                        NeuronNumber = 2,
                        ActivationFunction = ActivationFunctionEnum.Tanh,
                        ActivationFunction90PercentTreshold = 0.5f
                    }
                },
            OutputLayer = new NeuronLayerCaracteristics
            {
                LayerType = LayerTypeEnum.Output,
                LayerId = 2,
                NeuronNumber = 6,
                ActivationFunction = ActivationFunctionEnum.Sigmoid,
                ActivationFunction90PercentTreshold = 1
            },
            GenomeCaracteristics = new GenomeParameters
            {
                NetworkCoverage = 80,
                WeightBitNumber = 4
            }
        };
        public static BrainTemplate BigBrainDecisionBrain = new BrainTemplate
        {
            Name = "BigBrainDecision",
            IsDecisionBrain = true,
            NeedUnityInpus = true,
            InputLayer = new NeuronLayerCaracteristics
            {
                LayerType = LayerTypeEnum.Input,
                LayerId = 0,
                ActivationFunction = ActivationFunctionEnum.Identity
            },
            InputsTypes = new List<UnityInput>
            {
                //Vision
                new UnityInput(InputTypeEnum.Portion, 6)
                {
                    PortionTypeToApplyOn = PortionTypeEnum.WithinSightField,
                    UnityInputTypes = new HashSet<UnityInputTypeEnum> { UnityInputTypeEnum.PheromoneW, UnityInputTypeEnum.PheromoneC, UnityInputTypeEnum.Food, UnityInputTypeEnum.Nest, UnityInputTypeEnum.Walls }
                },
                //NoVision
                new UnityInput(InputTypeEnum.Portion, 6)
                {
                    PortionTypeToApplyOn = PortionTypeEnum.OutSightField,
                    UnityInputTypes = new HashSet<UnityInputTypeEnum> { UnityInputTypeEnum.PheromoneW, UnityInputTypeEnum.PheromoneC }
                },
                new UnityInput(InputTypeEnum.CarryFood)
            },
            NeutralLayers = new List<NeuronLayerCaracteristics>
                {
                    new NeuronLayerCaracteristics
                    {
                        LayerType = LayerTypeEnum.Neutral,
                        LayerId = 1,
                        NeuronNumber = 8,
                        ActivationFunction = ActivationFunctionEnum.Tanh,
                        ActivationFunction90PercentTreshold = 0.5f
                    },
                    new NeuronLayerCaracteristics
                    {
                        LayerType = LayerTypeEnum.Neutral,
                        LayerId = 2,
                        NeuronNumber = 4,
                        ActivationFunction = ActivationFunctionEnum.Tanh,
                        ActivationFunction90PercentTreshold = 0.5f
                    }
                },
            OutputLayer = new NeuronLayerCaracteristics
            {
                LayerType = LayerTypeEnum.Output,
                LayerId = 3,
                NeuronNumber = 6,
                ActivationFunction = ActivationFunctionEnum.Sigmoid,
                ActivationFunction90PercentTreshold = 1
            },
            GenomeCaracteristics = new GenomeParameters
            {
                NetworkCoverage = 80,
                WeightBitNumber = 4
            }
        };
    }
}
