using System.Collections.Generic;

namespace Assets.Dtos
{
    public class TemplateGraph
    {
        public TemplateGraph()
        {
            Nodes = new Dictionary<string, BrainCaracteristicsTemplate>();
            Edges = new Dictionary<string, List<TemplateGraphLink>>();
        }

        public string Name { get; set; }

        public BrainCaracteristicsTemplate DecisionBrain { get; set; }

        public Dictionary<string, BrainCaracteristicsTemplate> Nodes { get; set; }

        public Dictionary<string, List<TemplateGraphLink>> Edges { get; set; }
    }
}
