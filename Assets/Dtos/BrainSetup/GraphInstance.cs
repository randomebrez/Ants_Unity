using System.Collections.Generic;

namespace Assets.Dtos
{
    public class GraphInstance
    {
        public GraphInstance()
        {
            CaracteristicNodes = new Dictionary<string, BrainCaracteristicsInstance>();
            CaracteristicEdges = new Dictionary<string, List<string>>();
            UnityInputsUsingTemplates = new Dictionary<string, BrainCaracteristicsTemplate>();
            InstanceByTemplate = new Dictionary<string, List<BrainCaracteristicsInstance>>();
        }

        public Dictionary<string, BrainCaracteristicsInstance> CaracteristicNodes { get; set; }

        public Dictionary<string, List<string>> CaracteristicEdges { get; set; }

        public Dictionary<string, BrainCaracteristicsTemplate> UnityInputsUsingTemplates { get; set; }

        public Dictionary<string, List<BrainCaracteristicsInstance>> InstanceByTemplate { get; set; }
    }
}
