using NeuralNetwork.Interfaces.Model;
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


        // AntParameters
        public static int ColonyMaxPopulation = 10;
        public static int BaseScannerRate = 100;
        //public static NetworkCaracteristics NetworkCaracteristics = new NetworkCaracteristics
        //{
        //    GeneNumber = 300,
        //    InputNumber = 31,
        //    OutputNumber = 6,
        //    NeutralNumbers = new List<int> { 6, 3 },
        //    WeighBytesNumber = 4
        //};
        public static NetworkCaracteristics NetworkCaracteristics = new NetworkCaracteristics
        {
            GeneNumber = 300,
            InputNumber = 17,
            OutputNumber = 6,
            NeutralNumbers = new List<int> { 6, 3 },
            WeighBytesNumber = 4
        };


        //AntColony
        public static float GenerationLifeTime = 5;
    }
}
