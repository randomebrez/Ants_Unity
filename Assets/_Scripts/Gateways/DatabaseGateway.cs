using Assets.Abstractions;
using Assets.Dtos;
using Mono.Data.Sqlite;
using Newtonsoft.Json;
using System.Data;
using System.IO;
using System.Text;

namespace Assets._Scripts.Gateways
{
    public class DatabaseGateway : IStorage
    {
        private string _dbConnexionString;

        public DatabaseGateway(string sqlFolderPath, string dbFileName) 
        {
            _dbConnexionString = $"URI=file:{sqlFolderPath}\\{dbFileName}";

            //ToDo : add a creation db script (incremental script in a folder ? LOL)
            if (Directory.Exists(sqlFolderPath) == false)
                Directory.CreateDirectory(sqlFolderPath);
        }        

        public void TemplateCaracteristicStore(BrainCaracteristicsTemplate template)
        {
            using (var connection = new SqliteConnection(_dbConnexionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"insert into CaracteristicsTemplate (name, value) values ({ToData(template)});";
                    command.ExecuteReader();
                }

                connection.Close();
            }
        }

        public  BrainCaracteristicsTemplate TemplateCaracteristicsFetch(string templateName)
        {
            BrainCaracteristicsTemplate result;

            using (var connection = new SqliteConnection(_dbConnexionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"Select * from CaracteristicsTemplate where name = \"{templateName}\";";
            
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        result = ToObj(reader);
                    }
                }
                connection.Close();
            }

            return result;
        }

        private BrainCaracteristicsTemplate ToObj(IDataReader reader)
        {
            var caracTemplate = JsonConvert.DeserializeObject<BrainCaracteristicsTemplate>(reader["value"].ToString());
            return caracTemplate;
        }

        private string ToData(BrainCaracteristicsTemplate caracTemplate)
        {
            var result = new StringBuilder();
            result.Append($"'{caracTemplate.Name}', ");
            result.Append($"'{JsonConvert.SerializeObject(caracTemplate)}'");
            return result.ToString();
        }

        //private GraphTemplate ToObj(IDataReader reader)
        //{
        //    var caracTemplate = JsonConvert.DeserializeObject<BrainCaracteristicsTemplate>(reader["value"].ToString());
        //    return caracTemplate;
        //}
        //
        //private string ToData(GraphTemplate caracTemplate)
        //{
        //    var result = new StringBuilder();
        //    result.Append($"'{caracTemplate.Name}', ");
        //    result.Append($"'{JsonConvert.SerializeObject(caracTemplate)}'");
        //    return result.ToString();
        //}

        public GraphTemplate GraphTemplateFetch(string graphName)
        {
            //var graphs = await File.ReadAllTextAsync(_graphTemplatesFilePath).ConfigureAwait(false);
            //var allGraphs = graphs.Split("\n\n");
            //var graphWeWant = allGraphs.Where(t => string.IsNullOrEmpty(t) == false).Select(t => JsonConvert.DeserializeObject<GraphTemplate>(t)).FirstOrDefault(t => t.Name == graphName);
            //return graphWeWant;
            return new GraphTemplate();
        }

        public void TemplateGraphStore(GraphTemplate graphTemplate)
        {
            //var serializedGraphTemplate = $"{JsonConvert.SerializeObject(graphTemplate)}\n\n";
            //using (var stream = File.AppendText(_graphTemplatesFilePath))
            //{
            //    var bytes = new UTF8Encoding().GetBytes(serializedGraphTemplate.ToString());
            //    await stream.WriteAsync(serializedGraphTemplate).ConfigureAwait(false);
            //    await stream.FlushAsync().ConfigureAwait(false);
            //}
        }

        private int GetLastSimulationId()
        {
            //var directories = Directory.GetFiles(_folderPath);
            //var simulationIds = directories.Select(t => int.Parse(t.Split("\\").Last().Split(".").First())).OrderByDescending(t => t);
            //return simulationIds.Any() ? simulationIds.First() : 0;
            return 0;
        }
    }
}
