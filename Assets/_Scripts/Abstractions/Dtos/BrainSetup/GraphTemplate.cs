using System.Collections.Generic;

namespace Assets.Dtos
{
    public class GraphTemplate
    {
        public GraphTemplate()
        {
            Nodes = new Dictionary<string, BrainCaracteristicsTemplate>();
            Edges = new Dictionary<string, List<GraphLink>>();
        }

        public int DbId { get; set; }
        public string Name { get; set; }

        public BrainCaracteristicsTemplate DecisionBrain { get; set; }

        public Dictionary<string, BrainCaracteristicsTemplate> Nodes { get; set; }

        public Dictionary<string, List<GraphLink>> Edges { get; set; }
    }
}
