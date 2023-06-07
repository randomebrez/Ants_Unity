using NeuralNetwork.Interfaces.Model;
using System.Collections.Generic;

namespace Assets.Dtos
{
    public class BrainCaracteristicsTemplate
    {
        public BrainCaracteristicsTemplate()
        {
            InputsTypes = new List<InputTypeBase>();
            NeutralLayers = new List<LayerCaracteristics>();
        }

        public string Name { get; set; }

        public bool IsDecisionBrain { get; set; }

        public List<InputTypeBase> InputsTypes { get; set; }

        public LayerCaracteristics InputLayer { get; set; }

        public List<LayerCaracteristics> NeutralLayers { get; set; }

        public LayerCaracteristics OutputLayer { get; set; }

        public bool NeedUnityInpus { get; set; }

        public GenomeParameters GenomeCaracteristics { get; set; }

        public int MaxEdgeNumberGet()
        {
            var res = 0;
            var sum = OutputLayer.NeuronNumber;
            for(int i = NeutralLayers.Count - 1; i >= 0; i--)
            {
                res += NeutralLayers[i].NeuronNumber * sum;
                sum += NeutralLayers[i].NeuronNumber;
            }

            res += InputLayer.NeuronNumber * sum;

            return res;
        }
    }
}
