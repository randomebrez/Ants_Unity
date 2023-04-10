using Assets._Scripts.Utilities;
using Assets.Abstractions;
using NeuralNetwork.Interfaces;
using NeuralNetwork.Interfaces.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assets._Scripts.Gateways
{
    public class DatabaseGateway : IStorage
    {
        private IDatabaseGateway _gateway;

        public DatabaseGateway()
        {
            _gateway = new NeuralNetwork.Implementations.DatabaseGateway(GlobalParameters.SqlFolderPath);
        }

        public async Task StoreBrainsAsync(int generationId, List<Unit> units)
        {
            throw new System.NotImplementedException();
            //await _gateway.StoreBrainsAsync(GetLastSimulationId(), generationId, units).ConfigureAwait(false);
        }

        public async Task<int> AddNewSimulationAsync()
        {
            return await _gateway.AddNewSimulationAsync().ConfigureAwait(false);
        }

        private int GetLastSimulationId()
        {
            return 1;
        }
    }
}
