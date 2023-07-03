using NeuralNetwork.Abstraction.Model;

namespace Assets.Dtos
{
    public class NeuronLayerCaracteristics
    {
        public LayerTypeEnum LayerType { get; set; }

        public int LayerId { get; set; }

        public int NeuronNumber { get; set; }

        public float NeuronTreshold { get; set; }

        public ActivationFunctionEnum ActivationFunction { get; set; } = ActivationFunctionEnum.Identity;

        public float ActivationFunction90PercentTreshold { get; set; }
    }
}
