using Assets.Dtos.Database;
using Assets.Abstractions;
using Mono.Data.Sqlite;
using System.Collections.Generic;
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

        public void BrainTemplateStore(BrainTemplateDb template)
        {
            using (var connection = new SqliteConnection(_dbConnexionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"insert into BrainTemplate (name, value) values ({BrainTemplateToSql(template)});";
                    command.ExecuteReader();
                }
                connection.Close();
            }
        }
        public BrainTemplateDb BrainTemplateFetch(string templateName)
        {
            BrainTemplateDb result;

            using (var connection = new SqliteConnection(_dbConnexionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"Select * from BrainTemplate where name = \'{templateName}\';";
            
                    using (var reader = command.ExecuteReader())
                    {
                        result = BrainTemplateFromSql(reader);
                    }
                }
                connection.Close();
            }

            return result;
        }
        public BrainTemplateDb BrainTemplateFetch(int templateId)
        {
            BrainTemplateDb result;

            using (var connection = new SqliteConnection(_dbConnexionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"Select * from BrainTemplate where id = {templateId};";

                    using (var reader = command.ExecuteReader())
                    {
                        result = BrainTemplateFromSql(reader);
                    }
                }
                connection.Close();
            }

            return result;
        }
        public List<BrainTemplateDb> BrainTemplatesList(List<int> templateIds)
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
                    command.CommandText = $"Select * from BrainTemplate where id in ({sqlFilter});";

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


        public void TemplateGraphStore(TemplateGraphDb templateGraph)
        {
            using (var connection = new SqliteConnection(_dbConnexionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"insert into TemplateGraph (name, decision_template_id) values ({GraphTemplateToSql(templateGraph)});";
                    command.ExecuteReader();
                }

                connection.Close();
            }
        }
        public TemplateGraphDb TemplateGraphFetch(string graphName)
        {
            var result = new TemplateGraphDb();

            using (var connection = new SqliteConnection(_dbConnexionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"Select * from TemplateGraph where name = \'{graphName}\';";

                    using (var reader = command.ExecuteReader())
                    {
                        result = TemplateGraphFromSql(reader);
                    }
                }
                connection.Close();
            }

            return result;
        }
        public bool TemplateGraphExist(string graphName)
        {
            var result = false;
            using (var connection = new SqliteConnection(_dbConnexionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"Select * from TemplateGraphLink where name = {graphName};";

                    using (var reader = command.ExecuteReader())
                    {
                        var graph = GraphLinkFromSql(reader);
                        result = graph != null;
                    }
                }
                connection.Close();
            }

            return result;
        }


        public void GraphLinksStore(List<GraphLinkDb> links)
        {
            var sqlBase = "insert into GraphLink (graph_id, origin_template_id, target_template_id, link_type) values";

            var sqlRequest = new StringBuilder();
            for (int i = 0; i < links.Count; i++)
                sqlRequest.AppendLine($"{sqlBase} ({GraphLinkToSql(links[i])});");

            using (var connection = new SqliteConnection(_dbConnexionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sqlRequest.ToString();
                    command.ExecuteReader();
                }

                connection.Close();
            }
        }
        public List<GraphLinkDb> GraphLinksList(int graphId)
        {
            var result = new List<GraphLinkDb>();

            using (var connection = new SqliteConnection(_dbConnexionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"Select * from GraphLink where graph_id = {graphId};";

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
        private string BrainTemplateToSql(BrainTemplateDb brainTemplateDb)
        {
            var result = new StringBuilder();
            result.Append($"\'{brainTemplateDb.Name}\', ");
            result.Append($"\'{brainTemplateDb.SerialyzedValue}\'");
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


        private string GraphTemplateToSql(TemplateGraphDb templateGraphDb)
        {
            var result = new StringBuilder();
            result.Append($"\'{templateGraphDb.Name}\', ");
            result.Append($"{templateGraphDb.DecisionBrainTemplateId}");
            return result.ToString();
        }
        private TemplateGraphDb TemplateGraphFromSql(SqliteDataReader reader)
        {
            if (reader.HasRows == false)
                return null;

            return new TemplateGraphDb
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

            return new GraphLinkDb
            {
                Id = int.Parse(reader["id"].ToString()),
                GraphId = int.Parse(reader["graph_id"].ToString()),
                OriginId = int.Parse(reader["origin_template_id"].ToString()),
                TargetId = int.Parse(reader["target_template_id"].ToString()),
                LinkType = reader["link_type"].ToString()
            };
        }
    }
}
