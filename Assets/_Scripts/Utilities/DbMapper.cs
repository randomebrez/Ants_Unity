using Assets.Dtos.Database;
using Assets.Dtos;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Scripts.Utilities
{
    public static class DbMapper
    {
        public static GraphTemplateDb ToDb(GraphTemplate tpGraph)
        {
            return new GraphTemplateDb
            {
                Name = tpGraph.Name,
                DecisionBrainTemplateId = tpGraph.DecisionBrain.DbId
            };
        }
        public static GraphTemplate ToUnity(GraphTemplateDb tpGraph, List<GraphLinkDb> graphLinks, Dictionary<int, BrainTemplateDb> templates)
        {
            var result = new GraphTemplate
            {
                DbId = tpGraph.Id,
                Name = tpGraph.Name,
                DecisionBrain = ToUnity(templates[tpGraph.DecisionBrainTemplateId]),
                Nodes = templates.ToDictionary(t => t.Value.Name, t => ToUnity(t.Value))
            };

            // Set decision brain boolean in template object
            result.Nodes[result.DecisionBrain.Name].IsDecisionBrain = true;

            // Build links
            for(int i = 0; i < graphLinks.Count; i++)
            {
                var dbLink = graphLinks[i];
                var target = ToUnity(templates[dbLink.TargetId]);
                var origin = ToUnity(templates[dbLink.OriginId]);
                var graphLink = new GraphLink
                {
                    Target = target,
                    Origin = origin,
                    LinkTypeEnum = ToUnity(dbLink.LinkType)
                };

                if (result.Edges.TryGetValue(target.Name, out var origins))
                    origins.Add(graphLink);
                else
                    result.Edges.Add(target.Name, new List<GraphLink> { graphLink });
            }
            return result;
        }


        public static BrainCaracteristicsTemplate ToUnity(BrainTemplateDb templateDb)
        {
            var caracTemplate = JsonConvert.DeserializeObject<BrainCaracteristicsTemplate>(templateDb.SerialyzedValue);
            caracTemplate.DbId = templateDb.Id;
            return caracTemplate;
        }
        public static BrainTemplateDb ToDb(BrainCaracteristicsTemplate caracTemplate)
        {
            var result = new BrainTemplateDb
            {
                Name = caracTemplate.Name,
                SerialyzedValue = JsonConvert.SerializeObject(caracTemplate)
            };

            return result;
        }


        public static List<GraphLinkDb> ToDb(int graphId, List<GraphLink> graphLinks, Dictionary<string, int> templatesDb)
        {
            var result = new List<GraphLinkDb>();
            for(int i = 0; i < graphLinks.Count; i++)
            {
                var link = graphLinks[i];
                var linkDb = new GraphLinkDb
                {
                    GraphId = graphId,
                    TargetId = templatesDb[link.Target.Name],
                    OriginId = templatesDb[link.Origin.Name],
                    LinkType = ToDb(link.LinkTypeEnum)
                };
                result.Add(linkDb);
            }

            return result;
        }

        public static GraphLinkTypeEnum ToUnity(string dbType)
        {
            switch (dbType)
            {
                case "Default":
                    return GraphLinkTypeEnum.Default;
                case "AllPortions":
                    return GraphLinkTypeEnum.AllPortions;
                case "VisionPortions":
                    return GraphLinkTypeEnum.VisionPortions;
                case "NoVisionPortions":
                    return GraphLinkTypeEnum.NoVisionPortions;
                case "SingleAllPortions":
                    return GraphLinkTypeEnum.SingleAllPortions;
                case "SingleVisionPortions":
                    return GraphLinkTypeEnum.SingleVisionPortions;
                case "SingleNoVisionPortions":
                    return GraphLinkTypeEnum.SingleNoVisionPortions;
                default:
                    return GraphLinkTypeEnum.Default;
            }
        }

        public static string ToDb(GraphLinkTypeEnum dbType)
        {
            switch (dbType)
            {
                case GraphLinkTypeEnum.Default:
                    return "Default";
                case GraphLinkTypeEnum.AllPortions:
                    return "AllPortions";
                case GraphLinkTypeEnum.VisionPortions:
                    return "VisionPortions";
                case GraphLinkTypeEnum.NoVisionPortions:
                    return "NoVisionPortions";
                case GraphLinkTypeEnum.SingleAllPortions:
                    return "SingleAllPortions";
                case GraphLinkTypeEnum.SingleVisionPortions:
                    return "SingleVisionPortions";
                case GraphLinkTypeEnum.SingleNoVisionPortions:
                    return "SingleNoVisionPortions";
                default:
                    return "Default";
            }
        }
    }
}
