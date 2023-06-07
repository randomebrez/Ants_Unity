using NeuralNetwork.Interfaces.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Dtos
{
    public class UnitWrapper
    {
        public Unit NugetUnit { get; set; }

        public BrainCaracteristicGraph BrainCaracteristicsGraph { get; set; }
    }
}
