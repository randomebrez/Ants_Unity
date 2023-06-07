using NeuralNetwork.Interfaces.Model.Etc;
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


        public static string SelectedBrainGraph = "Splitted";

        // Reproduction parameters
        public static ReproductionCaracteristics ReproductionCaracteristics = new ReproductionCaracteristics
        {
            PercentToSelect = 40,
            MeanChildNumberByUnit = 5,
            CrossOverNumber = 1,
            MutationRate = 0.01f
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
