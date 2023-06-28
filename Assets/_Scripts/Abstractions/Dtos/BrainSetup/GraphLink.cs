using System.Collections.Generic;

namespace Assets.Dtos
{
    public class GraphLink
    {
        public string Key => Target.Name;

        public BrainTemplate Target { get; set; }

        public BrainTemplate Origin { get; set; }

        public GraphLinkTypeEnum LinkTypeEnum { get; set; }
    }
}
