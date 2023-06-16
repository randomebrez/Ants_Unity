using System.Collections.Generic;

namespace Assets.Dtos
{
    public class BrainCaracteristicsInstance
    {
        public BrainCaracteristicsInstance(BrainCaracteristicsTemplate template)
        {
            Template = template;
            InputPortions = new List<InputTypePortion>();
        }

        public BrainCaracteristicsTemplate Template { get; private set; }

        public string UniqueName { get; set; }

        public string PrettyName { get; set; }

        public List<InputTypePortion> InputPortions { get; set; }
    }
}
