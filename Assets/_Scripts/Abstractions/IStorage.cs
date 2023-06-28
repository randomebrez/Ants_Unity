using Assets.Dtos.Database;
using System.Collections.Generic;

namespace Assets.Abstractions
{
    public interface IStorage
    {
        void BrainTemplateStore(BrainTemplateDb template);
        BrainTemplateDb BrainTemplateFetch(string templateName);
        BrainTemplateDb BrainTemplateFetch(int templateId);
        List<BrainTemplateDb> BrainTemplatesList(List<int> templateIds);

        void TemplateGraphStore(TemplateGraphDb templateGraph);
        TemplateGraphDb TemplateGraphFetch(string graphName);
        bool TemplateGraphExist(string graphName);

        List<GraphLinkDb> GraphLinksList(int graphId);
        void GraphLinksStore(List<GraphLinkDb> links);
    }
}
