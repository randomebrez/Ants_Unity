using System.Collections.Generic;

namespace Assets.Dtos
{
    public class BrainCaracteristicsInstance
    {
        public BrainCaracteristicsInstance(BrainCaracteristicsTemplate template)
        {
            Template = template;
            InputPortions = new List<UnityInput>();
        }

        public BrainCaracteristicsTemplate Template { get; private set; }

        public string UniqueName { get; set; }

        public string PrettyName { get; set; }

        public List<UnityInput> InputPortions { get; set; }
    }
}
