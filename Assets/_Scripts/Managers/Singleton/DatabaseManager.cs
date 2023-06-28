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

    public void TemplateGraphStore(GraphTemplate graph)
    {
        // Store graph obj
        _databaseGateway.GraphTemplateStore(DbMapper.ToDb(graph));

        // Fetch graph to get its id
        var dbGraph = _databaseGateway.GraphTemplateFetch(graph.Name);

        // Fetch Template Ids
        var templateDbIds = graph.Nodes.Values.ToDictionary(t => t.Name, t => t.DbId);

        // Map GrapLinks to GraphLinksDb
        var linksDb = new List<GraphLinkDb>();
        foreach (var linkList in graph.Edges.Values)
            linksDb.AddRange(DbMapper.ToDb(dbGraph.Id, linkList, templateDbIds));

        // Store links
        _databaseGateway.GraphLinksStore(linksDb);
    }
    public GraphTemplate TemplateGraphFetchByName(string graphName)
    {
        // Get graph obj
        var dbGraph = _databaseGateway.GraphTemplateFetch(graphName);
        if (dbGraph == null)
            return null;

        // Get all links
        var dbLinks = _databaseGateway.GraphLinksFetch(dbGraph.Id);

        // For each link fetch template
        var distinctTemplateIds = new List<int> { dbGraph.DecisionBrainTemplateId };
        foreach (var link in dbLinks)
        {
            if (distinctTemplateIds.Contains(link.TargetId) == false)
                distinctTemplateIds.Add(link.TargetId);
            if (distinctTemplateIds.Contains(link.OriginId) == false)
                distinctTemplateIds.Add(link.OriginId);
        }
        var dbTemplates = _databaseGateway.TemplateCaracteristicsListFetchById(distinctTemplateIds).ToDictionary(t => t.Id, t => t);

        return DbMapper.ToUnity(dbGraph, dbLinks, dbTemplates);
    }


    public void BrainTemplateStore(BrainCaracteristicsTemplate template)
    {
        _databaseGateway.TemplateCaracteristicStore(DbMapper.ToDb(template));
    }
    public BrainCaracteristicsTemplate BrainTemplateFetch(int? id = null, string name = null)
    {
        BrainCaracteristicsTemplate result = null;
        if (id != null)
        {
            var dbTemplate = _databaseGateway.TemplateCaracteristicsFetchById(id.Value);
            if (dbTemplate != null)
                result = DbMapper.ToUnity(dbTemplate);
        }
        else if (string.IsNullOrEmpty(name) == false)
        {
            var dbTemplate = _databaseGateway.TemplateCaracteristicsFetchByName(name);
            if (dbTemplate != null)
                result = DbMapper.ToUnity(dbTemplate);
        }

        return result;
    }
}
