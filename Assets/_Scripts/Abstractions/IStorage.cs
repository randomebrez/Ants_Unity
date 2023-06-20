using Assets.Dtos;

namespace Assets.Abstractions
{
    public interface IStorage
    {
        void TemplateCaracteristicStore(BrainCaracteristicsTemplate template);

        BrainCaracteristicsTemplate TemplateCaracteristicsFetch(string templateName);
        
        void TemplateGraphStore(GraphTemplate graphTemplate);

        GraphTemplate GraphTemplateFetch(string graphName);
    }
}
