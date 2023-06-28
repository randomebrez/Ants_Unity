using System.Collections.Generic;

namespace Assets.Dtos
{
    public class TemplateGraph
    {
        public TemplateGraph()
        {
            Nodes = new Dictionary<string, BrainTemplate>();
            Edges = new Dictionary<string, List<GraphLink>>();
        }

        public int DbId { get; set; }
        public string Name { get; set; }

        public BrainTemplate DecisionBrain { get; set; }

        public Dictionary<string, BrainTemplate> Nodes { get; set; }

        public Dictionary<string, List<GraphLink>> Edges { get; set; }
    }
}
