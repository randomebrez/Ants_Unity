using Assets.Dtos.Database;
using Assets.Abstractions;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Linq;

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

        public void TemplateCaracteristicStore(BrainTemplateDb template)
        {
            using (var connection = new SqliteConnection(_dbConnexionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"insert into CaracteristicsTemplate (name, value) values ({BrainTemplateToSql(template)});";
                    command.ExecuteReader();
                }
                connection.Close();
            }
        }
        public BrainTemplateDb TemplateCaracteristicsFetchByName(string templateName)
        {
            BrainTemplateDb result;

            using (var connection = new SqliteConnection(_dbConnexionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"Select * from CaracteristicsTemplate where name = \"{templateName}\";";
            
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        result = BrainTemplateFromSql(reader);
                    }
                }
                connection.Close();
            }

            return result;
        }
        public BrainTemplateDb TemplateCaracteristicsFetchById(int templateId)
        {
            BrainTemplateDb result;

            using (var connection = new SqliteConnection(_dbConnexionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"Select * from CaracteristicsTemplate where id = \"{templateId}\";";

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        result = BrainTemplateFromSql(reader);
                    }
                }
                connection.Close();
            }

            return result;
        }
        public List<BrainTemplateDb> TemplateCaracteristicsListFetchById(List<int> templateIds)
        {
            var result = new List<BrainTemplateDb>();
            var sqlFilter = new StringBuilder();
            for (int i = 0; i < templateIds.Count - 1; i++)
                sqlFilter.Append($"\'{templateIds[i]}\',");
            sqlFilter.Append($"\'{templateIds[^1]}\'");
            using (var connection = new SqliteConnection(_dbConnexionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"Select * from CaracteristicsTemplate where id in ({sqlFilter});";

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            result.Append(BrainTemplateFromSql(reader));
                    }
                }
                connection.Close();
            }

            return result;
        }


        public void GraphTemplateStore(GraphTemplateDb graphTemplate)
        {
            using (var connection = new SqliteConnection(_dbConnexionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"insert into TemplateGraph (name) values ({GraphTemplateToSql(graphTemplate)});";
                    command.ExecuteReader();
                }

                connection.Close();
            }
        }
        public GraphTemplateDb GraphTemplateFetch(string graphName)
        {
            var result = new GraphTemplateDb();

            using (var connection = new SqliteConnection(_dbConnexionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"Select * from TemplateGraph where name = \"{graphName}\";";

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        result = GraphTemplateFromSql(reader);
                    }
                }
                connection.Close();
            }

            return result;
        }


        public void GraphLinksStore(List<GraphLinkDb> links)
        {
            var sqlRequestValues = new StringBuilder();
            for (int i = 0; i < links.Count - 1; i++)
                sqlRequestValues.Append($"({GraphLinkToSql(links[i])}),");
            sqlRequestValues.Append($"({GraphLinkToSql(links[^1])})");

            using (var connection = new SqliteConnection(_dbConnexionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"insert into TemplateGraphLink (graph_id, template_origin_id, template_target_id) values {sqlRequestValues};";
                    command.ExecuteReader();
                }

                connection.Close();
            }
        }
        public List<GraphLinkDb> GraphLinksFetch(int graphId)
        {
            var result = new List<GraphLinkDb>();

            using (var connection = new SqliteConnection(_dbConnexionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"Select * from TemplateGraphLink where template_graph_id = \"{graphId}\";";

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        result.Add(GraphLinkFromSql(reader));
                    }
                }
                connection.Close();
            }

            return result;
        }


        private int GetLastSimulationId()
        {
            //var directories = Directory.GetFiles(_folderPath);
            //var simulationIds = directories.Select(t => int.Parse(t.Split("\\").Last().Split(".").First())).OrderByDescending(t => t);
            //return simulationIds.Any() ? simulationIds.First() : 0;
            return 0;
        }



        // Mapping
        private string BrainTemplateToSql(BrainTemplateDb templateDb)
        {
            var result = new StringBuilder();
            result.Append($"\'{templateDb.Name}\', ");
            result.Append($"\'{templateDb.SerialyzedValue}\'");
            return result.ToString();
        }
        private BrainTemplateDb BrainTemplateFromSql(IDataReader reader)
        {
            var templateDb = new BrainTemplateDb
            {
                Id = int.Parse(reader["id"].ToString()),
                Name = reader["name"].ToString(),
                SerialyzedValue = reader["value"].ToString()
            };
            return templateDb;
        }


        private string GraphTemplateToSql(GraphTemplateDb graphTemplateDb)
        {
            var result = new StringBuilder();
            result.Append($"\'{graphTemplateDb.Name}\', ");
            result.Append($"'{graphTemplateDb.DecisionBrainTemplateId}'");
            return result.ToString();
        }
        private GraphTemplateDb GraphTemplateFromSql(IDataReader reader)
        {
            return new GraphTemplateDb
            {
                Id = int.Parse(reader["id"].ToString()),
                Name = reader["name"].ToString(),
                DecisionBrainTemplateId = int.Parse(reader["decision_template_id"].ToString())
            };
        }


        private string GraphLinkToSql(GraphLinkDb graphLink)
        {
            var result = new StringBuilder();
            result.Append($"\'{graphLink.GraphId}\', ");
            result.Append($"\'{graphLink.OriginId}\', ");
            result.Append($"\'{graphLink.TargetId}\'");
            return result.ToString();
        }
        private GraphLinkDb GraphLinkFromSql(IDataReader reader)
        {
            var caracTemplate = new GraphLinkDb
            {
                GraphId = int.Parse(reader["graph_id"].ToString()),
                OriginId = int.Parse(reader["origin_template_id"].ToString()),
                TargetId = int.Parse(reader["target_template_id"].ToString())
            };
            return caracTemplate;
        }
    }
}
