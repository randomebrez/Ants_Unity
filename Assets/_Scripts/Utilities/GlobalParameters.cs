using NeuralNetwork.Interfaces.Model;
using NeuralNetwork.Interfaces.Model.Etc;
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

        // Reproduction parameters
        public static ReproductionCaracteristics ReproductionCaracteristics = new ReproductionCaracteristics
        {
            PercentToSelect = 40,
            MeanChildNumberByUnit = 5,
            CrossOverNumber = 2,
            MutationRate = 0.001f
        };

        // AntParameters
        public static int ColonyMaxPopulation = 42;

        // AntColony
        public static float GenerationFrameCount = 300;

        // Brain caracteristics
        public static BrainCaracteristics MainBrain = new BrainCaracteristics("Main")
        {
            InputLayer = new LayerCaracteristics(LayerTypeEnum.Input, 0, 6, ActivationFunctionEnum.Identity, 0),
            OutputLayer = new LayerCaracteristics(LayerTypeEnum.Output, 2, 6, ActivationFunctionEnum.Sigmoid, 3),
            NeutralLayers = new List<LayerCaracteristics>
                {
                    new LayerCaracteristics(LayerTypeEnum.Neutral, 1, 4, ActivationFunctionEnum.Tanh, 2)
                },
            GenomeCaracteristics = new GenomeCaracteristics
            {
                GeneNumber = 60,
                WeighBytesNumber = 3
            }
        };
        public static BrainCaracteristics VisionBrain = new BrainCaracteristics("VisionPortion")
        {
            InputLayer = new LayerCaracteristics(LayerTypeEnum.Input, 0, 6, ActivationFunctionEnum.Identity, 0),
            OutputLayer = new LayerCaracteristics(LayerTypeEnum.Output, 2, 1, ActivationFunctionEnum.Sigmoid, 3),
            NeutralLayers = new List<LayerCaracteristics>
                {
                    new LayerCaracteristics(LayerTypeEnum.Neutral, 1, 2, ActivationFunctionEnum.Tanh, 2)
                },
            GenomeCaracteristics = new GenomeCaracteristics
            {
                GeneNumber = 15,
                WeighBytesNumber = 3
            }
        };
        public static BrainCaracteristics NoVisionBrain = new BrainCaracteristics("NoVisionPortion")
        {
            InputLayer = new LayerCaracteristics(LayerTypeEnum.Input, 0, 3, ActivationFunctionEnum.Identity, 0),
            OutputLayer = new LayerCaracteristics(LayerTypeEnum.Output, 2, 1, ActivationFunctionEnum.Sigmoid, 3),
            NeutralLayers = new List<LayerCaracteristics>
                {
                    new LayerCaracteristics(LayerTypeEnum.Neutral, 1, 1, ActivationFunctionEnum.Tanh, 1)
                },
            GenomeCaracteristics = new GenomeCaracteristics
            {
                GeneNumber = 7,
                WeighBytesNumber = 3
            }
        };
        public static List<BrainCaracteristics> BrainCaracteristics = new List<BrainCaracteristics>
        {
            MainBrain,
            VisionBrain,
            NoVisionBrain
        };

        

        //
        public static string LogFileBase = "D:\\Codes\\Test\\AntWinners";

        // Database
        public static int StoreFrequency = 20;
        public static string SqlFolderPath = @".\Database";

        // Saved brains
        public static string FirstBrainsFolderPath = @".\SavedBrains";
        public static string FirstBrainsFilePath = @".\SavedBrains\1.txt";
    }
}
