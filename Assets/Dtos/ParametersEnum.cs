
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Dtos
{
    public enum SimulationParameterTypeEnum
    {
        MaxPopulation,
        LifeTime,
        SelectPercent,
        ChildMean,
        StorageFrequency
    }

    public enum NeuralNetworkParameterTypeEnum
    {
        NeutralLayers,
        GeneWeightBitNumber,
        GeneNumber
    }

    public enum AntCaracteristicsParameterTypeEnum
    {
        VisionRadius,
        VisionAngle,
        PheroDurationW,
        PheroDurationC
    }
}
