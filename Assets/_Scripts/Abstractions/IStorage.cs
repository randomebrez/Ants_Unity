using Assets.Dtos.Database;
using System.Collections.Generic;

namespace Assets.Abstractions
{
    public interface IStorage
    {
        void TemplateCaracteristicStore(BrainTemplateDb template);
        BrainTemplateDb TemplateCaracteristicsFetchByName(string templateName);
        public BrainTemplateDb TemplateCaracteristicsFetchById(int templateId);
        List<BrainTemplateDb> TemplateCaracteristicsListFetchById(List<int> templateIds);

        void GraphTemplateStore(GraphTemplateDb graphTemplate);
        GraphTemplateDb GraphTemplateFetch(string graphName);

        public List<GraphLinkDb> GraphLinksFetch(int graphId);
        public void GraphLinksStore(List<GraphLinkDb> links);
    }
}
