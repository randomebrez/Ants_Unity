using Assets.Dtos;
using System.Threading.Tasks;

namespace Assets.Abstractions
{
    public interface IStorage
    {
        Task TemplateCaracteristicStoreAsync(BrainCaracteristicsTemplate template);

        Task<BrainCaracteristicsTemplate> TemplateCaracteristicsFetchAsync(string templateName);

        Task TemplateGraphStoreAsync(GraphTemplate graphTemplate);

        Task<GraphTemplate> GraphTemplateFetchAsync(string graphName);
    }
}
