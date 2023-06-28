using Assets.Dtos.Database;
using Assets.Abstractions;
using Mono.Data.Sqlite;
using System.Collections.Generic;
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
                    command.CommandText = $"Select * from CaracteristicsTemplate where name = \'{templateName}\';";
            
                    using (var reader = command.ExecuteReader())
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
                    command.CommandText = $"Select * from CaracteristicsTemplate where id = {templateId};";

                    using (var reader = command.ExecuteReader())
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
                sqlFilter.Append($"{templateIds[i]}, ");
            sqlFilter.Append($"{templateIds[^1]}");
            using (var connection = new SqliteConnection(_dbConnexionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"Select * from CaracteristicsTemplate where id in ({sqlFilter});";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            result.Add(BrainTemplateFromSql(reader));
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
                    command.CommandText = $"insert into TemplateGraph (name, decision_template_id) values ({GraphTemplateToSql(graphTemplate)});";
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
                    command.CommandText = $"Select * from TemplateGraph where name = \'{graphName}\';";

                    using (var reader = command.ExecuteReader())
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
            var sqlBase = "insert into TemplateGraphLink (graph_id, origin_template_id, target_template_id, link_type) values";
            for (int i = 0; i < links.Count; i++)
                sqlRequestValues.AppendLine($"{sqlBase} ({GraphLinkToSql(links[i])});");

            using (var connection = new SqliteConnection(_dbConnexionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sqlRequestValues.ToString();
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
                    command.CommandText = $"Select * from TemplateGraphLink where graph_id = {graphId};";

                    using (var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            result.Add(GraphLinkFromSql(reader));
                        }
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
        private BrainTemplateDb BrainTemplateFromSql(SqliteDataReader reader)
        {
            if (reader.HasRows == false)
                return null;

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
            result.Append($"{graphTemplateDb.DecisionBrainTemplateId}");
            return result.ToString();
        }
        private GraphTemplateDb GraphTemplateFromSql(SqliteDataReader reader)
        {
            if (reader.HasRows == false)
                return null;

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
            result.Append($"{graphLink.GraphId}, ");
            result.Append($"{graphLink.OriginId}, ");
            result.Append($"{graphLink.TargetId}, ");
            result.Append($"\'{graphLink.LinkType}\'");
            return result.ToString();
        }
        private GraphLinkDb GraphLinkFromSql(SqliteDataReader reader)
        {
            if (reader.HasRows == false)
                return null;

            var caracTemplate = new GraphLinkDb
            {
                Id = int.Parse(reader["id"].ToString()),
                GraphId = int.Parse(reader["graph_id"].ToString()),
                OriginId = int.Parse(reader["origin_template_id"].ToString()),
                TargetId = int.Parse(reader["target_template_id"].ToString()),
                LinkType = reader["link_type"].ToString()
            };
            return caracTemplate;
        }
    }
}
