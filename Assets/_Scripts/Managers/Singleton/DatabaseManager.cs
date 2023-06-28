using Assets._Scripts.Gateways;
using Assets._Scripts.Utilities;
using Assets.Abstractions;
using Assets.Dtos;
using Assets.Dtos.Database;
using System.Collections.Generic;
using System.Linq;

internal class DatabaseManager : BaseManager<DatabaseManager>
{
    private IStorage _databaseGateway;

    protected override void Awake()
    {
        _databaseGateway = new DatabaseGateway(GlobalParameters.SqlFolderPath, GlobalParameters.DbFileName);
        base.Awake();
    }

    public void TemplateGraphStore(TemplateGraph graph)
    {
        // Store graph obj
        _databaseGateway.TemplateGraphStore(DbMapper.ToDb(graph));

        // Fetch graph to get its id
        var dbGraph = _databaseGateway.TemplateGraphFetch(graph.Name);

        // Fetch Template Ids
        var templateDbIds = graph.Nodes.Values.ToDictionary(t => t.Name, t => t.DbId);

        // Map GrapLinks to GraphLinksDb
        var linksDb = new List<GraphLinkDb>();
        foreach (var linkList in graph.Edges.Values)
            linksDb.AddRange(DbMapper.ToDb(dbGraph.Id, linkList, templateDbIds));

        // Store links
        _databaseGateway.GraphLinksStore(linksDb);
    }
    public TemplateGraph TemplateGraphFetch(string graphName)
    {
        // Get graph obj
        var dbGraph = _databaseGateway.TemplateGraphFetch(graphName);
        if (dbGraph == null)
            return null;

        // Get all links
        var dbLinks = _databaseGateway.GraphLinksList(dbGraph.Id);

        // For each link fetch template
        var distinctTemplateIds = new List<int> { dbGraph.DecisionBrainTemplateId };
        foreach (var link in dbLinks)
        {
            if (distinctTemplateIds.Contains(link.TargetId) == false)
                distinctTemplateIds.Add(link.TargetId);
            if (distinctTemplateIds.Contains(link.OriginId) == false)
                distinctTemplateIds.Add(link.OriginId);
        }
        var dbTemplates = _databaseGateway.BrainTemplatesList(distinctTemplateIds).ToDictionary(t => t.Id, t => t);

        return DbMapper.ToUnity(dbGraph, dbLinks, dbTemplates);
    }

    public bool TemplateGraphExist(string graphName)
    {
        return _databaseGateway.TemplateGraphExist(graphName);
    }


    public void BrainTemplateStore(BrainTemplate brainTemplate)
    {
        _databaseGateway.BrainTemplateStore(DbMapper.ToDb(brainTemplate));
    }
    public BrainTemplate BrainTemplateFetch(int? id = null, string name = null)
    {
        BrainTemplate result = null;
        if (id != null)
        {
            var dbTemplate = _databaseGateway.BrainTemplateFetch(id.Value);
            if (dbTemplate != null)
                result = DbMapper.ToUnity(dbTemplate);
        }
        else if (string.IsNullOrEmpty(name) == false)
        {
            var dbTemplate = _databaseGateway.BrainTemplateFetch(name);
            if (dbTemplate != null)
                result = DbMapper.ToUnity(dbTemplate);
        }

        return result;
    }
}
