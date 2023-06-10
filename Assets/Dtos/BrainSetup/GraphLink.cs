using System.Collections.Generic;

namespace Assets.Dtos
{
    public class GraphLink
    {
        public string Key => Target.Name;

        public BrainCaracteristicsTemplate Target { get; set; }

        public BrainCaracteristicsTemplate Origin { get; set; }

        public TemplateLinkTypeEnum LinkTypeEnum { get; set; }
    }
}
