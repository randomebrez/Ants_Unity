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
        public static float LifeTimeFrame = 300;

        public static int ScannerSubdivision = 6;
        public static int VisionAngle = 60;
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
            PercentToSelect = 40,
            MeanChildNumberByUnit = 5,
            CrossOverNumber = 1,
            MutationRate = 0.01f
        };

        // Selected brain graph
        public static string SelectedBrainGraph = "Splitted";
        public static BrainCaracteristicsTemplate DecisionBrain = new BrainCaracteristicsTemplate
        {
            Name = "DecisionSplitted",
            IsDecisionBrain = true,
            InputLayer = new LayerCaracteristics(0, LayerTypeEnum.Input),
            NeutralLayers = new List<LayerCaracteristics>
                {
                    new LayerCaracteristics(1, LayerTypeEnum.Neutral)
                    {
                        NeuronNumber = 2,
                        ActivationFunction = ActivationFunctionEnum.Tanh,
                        ActivationFunction90PercentTreshold = 0.5f
                    }
                },
            OutputLayer = new LayerCaracteristics(2, LayerTypeEnum.Output)
            {
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
