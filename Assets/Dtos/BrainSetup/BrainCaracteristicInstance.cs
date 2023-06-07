using System.Collections.Generic;

namespace Assets.Dtos
{
    public class BrainCaracteristicsInstance
    {
        public BrainCaracteristicsInstance(BrainCaracteristicsTemplate template)
        {
            Template = template;
        }

        public BrainCaracteristicsTemplate Template { get; private set; }

        public string BrainName { get; set; }

        public List<int> PortionIndexes { get; set; }
    }
}
