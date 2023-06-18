using Assets.Abstractions;
using Assets.Dtos;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Scripts.Gateways
{
    public class FileStorageGateway : IStorage
    {
        private string _filePath;
        private string _folderPath;
        private string _templatesFilePath;
        private string _graphTemplatesFilePath;

        public FileStorageGateway(string folderPath, string templateFileName, string graphTemplatesFileName) 
        {
            _folderPath = folderPath;
            if (Directory.Exists(_folderPath) == false)
                Directory.CreateDirectory(_folderPath);

            //_filePath = $"{folderPath}/{GetLastSimulationId() + 1}.txt";
            //if (File.Exists(_filePath) == false)
            //    File.Create(_filePath).Close();

            _templatesFilePath = $"{folderPath}/{templateFileName}.txt";
            if (File.Exists(_templatesFilePath) == false)
                File.Create(_templatesFilePath).Close();

            _graphTemplatesFilePath = $"{folderPath}/{graphTemplatesFileName}.txt";
            if (File.Exists(_graphTemplatesFilePath) == false)
                File.Create(_graphTemplatesFilePath).Close();
        }        

        public async Task TemplateCaracteristicStoreAsync(BrainCaracteristicsTemplate template)
        {
            var serializedTemplate = $"{JsonConvert.SerializeObject(template)}\n\n";
            using (var stream = File.AppendText(_templatesFilePath))
            {
                var bytes = new UTF8Encoding().GetBytes(serializedTemplate.ToString());
                await stream.WriteAsync(serializedTemplate).ConfigureAwait(false);
                await stream.FlushAsync().ConfigureAwait(false);
                stream.Close();
            }
        }

        public async Task<BrainCaracteristicsTemplate> TemplateCaracteristicsFetchAsync(string templateName)
        {
            var templates = await File.ReadAllTextAsync(_templatesFilePath).ConfigureAwait(false);
            var allTemplates = templates.Split("\n\n");
            var templateWeWant = allTemplates.Where(t => string.IsNullOrEmpty(t) == false).Select(t => JsonConvert.DeserializeObject(t) as BrainCaracteristicsTemplate).FirstOrDefault(t => t.Name == templateName);
            return templateWeWant;
        }

        public async Task<GraphTemplate> GraphTemplateFetchAsync(string graphName)
        {
            var graphs = await File.ReadAllTextAsync(_graphTemplatesFilePath).ConfigureAwait(false);
            var allGraphs = graphs.Split("\n\n");
            var graphWeWant = allGraphs.Where(t => string.IsNullOrEmpty(t) == false).Select(t => JsonConvert.DeserializeObject<GraphTemplate>(t)).FirstOrDefault(t => t.Name == graphName);
            return graphWeWant;
            //return new GraphTemplate();
        }

        public async Task TemplateGraphStoreAsync(GraphTemplate graphTemplate)
        {
            var serializedGraphTemplate = $"{JsonConvert.SerializeObject(graphTemplate)}\n\n";
            using (var stream = File.AppendText(_graphTemplatesFilePath))
            {
                var bytes = new UTF8Encoding().GetBytes(serializedGraphTemplate.ToString());
                await stream.WriteAsync(serializedGraphTemplate).ConfigureAwait(false);
                await stream.FlushAsync().ConfigureAwait(false);
            }
        }

        private int GetLastSimulationId()
        {
            var directories = Directory.GetFiles(_folderPath);
            var simulationIds = directories.Select(t => int.Parse(t.Split("\\").Last().Split(".").First())).OrderByDescending(t => t);
            return simulationIds.Any() ? simulationIds.First() : 0;
        }
    }
}
