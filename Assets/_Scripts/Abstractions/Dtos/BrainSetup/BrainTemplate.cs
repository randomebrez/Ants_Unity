using NeuralNetwork.Abstraction.Model;
using System.Collections.Generic;

namespace Assets.Dtos
{
    public class BrainTemplate
    {
        public BrainTemplate()
        {
            InputsTypes = new List<UnityInput>();
            NeutralLayers = new List<NeuronLayerCaracteristics>();
        }

        public int DbId { get; set; }

        public string Name { get; set; }

        public bool IsDecisionBrain { get; set; }

        public List<UnityInput> InputsTypes { get; set; }

        public NeuronLayerCaracteristics InputLayer { get; set; }

        public List<NeuronLayerCaracteristics> NeutralLayers { get; set; }

        public NeuronLayerCaracteristics OutputLayer { get; set; }

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
