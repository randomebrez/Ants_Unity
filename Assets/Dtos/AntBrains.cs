using NeuralNetwork.Interfaces.Model;
using System.Collections.Generic;

namespace Assets.Dtos
{
    public class AntBrains
    {
        public AntBrains()
        {
            ScannerBrains = new Dictionary<int, Brain>();
        }


        public Brain MainBrain { get; set; }

        public Dictionary<int, Brain> ScannerBrains { get; set; }
    }
}
