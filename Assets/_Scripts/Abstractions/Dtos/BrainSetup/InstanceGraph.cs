using System.Collections.Generic;

namespace Assets.Dtos
{
    public class InstanceGraph
    {
        public InstanceGraph()
        {
            CaracteristicNodes = new Dictionary<string, BrainInstance>();
            CaracteristicEdges = new Dictionary<string, List<string>>();
            UnityInputsUsingTemplates = new Dictionary<string, BrainTemplate>();
            InstanceByTemplate = new Dictionary<string, List<BrainInstance>>();
        }

        public Dictionary<string, BrainInstance> CaracteristicNodes { get; set; }

        public Dictionary<string, List<string>> CaracteristicEdges { get; set; }

        public Dictionary<string, BrainTemplate> UnityInputsUsingTemplates { get; set; }

        public Dictionary<string, List<BrainInstance>> InstanceByTemplate { get; set; }
    }
}
