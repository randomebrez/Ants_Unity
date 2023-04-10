using Assets.Abstractions;
using NeuralNetwork.Interfaces.Model;
using System.Collections.Generic;
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

        public FileStorageGateway(string folderPath) 
        {
            _folderPath = folderPath;
            if (Directory.Exists(_folderPath) == false)
                Directory.CreateDirectory(_folderPath);

            _filePath = $"{folderPath}/{GetLastSimulationId() + 1}.txt";
            if (File.Exists(_filePath) == false)
                File.Create(_filePath).Close();
        }

        public async Task StoreBrainsAsync(int generationId, List<Unit> units)
        {
            //var stringBrains = new StringBuilder();
            //for (int i = 0; i < brains.Count; i++)
            //    stringBrains.AppendLine(ToSaveFormat(generationId, brains[i]));
            //stringBrains.AppendLine();
            //
            //using (var stream = File.AppendText(_filePath))
            //{
            //    var bytes = new UTF8Encoding().GetBytes(stringBrains.ToString());
            //    await stream.WriteAsync(stringBrains.ToString()).ConfigureAwait(false);
            //    await stream.FlushAsync().ConfigureAwait(false);
            //}
        }

        private int GetLastSimulationId()
        {
            var directories = Directory.GetFiles(_folderPath);
            var simulationIds = directories.Select(t => int.Parse(t.Split("\\").Last().Split(".").First())).OrderByDescending(t => t);
            return simulationIds.Any() ? simulationIds.First() : 0;
        }

        private string ToSaveFormat(int generationId, Unit brain)
        {
            var result = new StringBuilder();

            //result.Append($"{generationId};{brain.UniqueIdentifier};{brain.ParentA};{brain.ParentB};{brain.Score};{brain.Genome}");

            return result.ToString();
        }
    }
}
