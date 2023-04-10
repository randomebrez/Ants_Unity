using System.Collections.Generic;
using System.Linq;
using Assets._Scripts.Utilities;
using NeuralNetwork.Interfaces;
using NeuralNetwork.Interfaces.Model;
using NeuralNetwork.Managers;

namespace Assets.Gateways
{
    public class NeuralNetworkGateway
    {
        private readonly IPopulationManager _populationManager;

        public NeuralNetworkGateway()
        {
            _populationManager = new PopulationManager();
        }


        public Unit[] GenerateNextGeneration(int childNumber, List<Unit> selectedBrains)
        {
            Unit[] units;
            if (selectedBrains.Any())
                units = _populationManager.GenerateNewGeneration(childNumber, selectedBrains, GlobalParameters.AntBrains, GlobalParameters.CrossOverNumber, GlobalParameters.MutationRate);
            else
                units = _populationManager.GenerateFirstGeneration(childNumber, GlobalParameters.AntBrains);
            return units;
        }

        public List<Unit> GetBrainsFromString(List<string> stringBrains)
        {
            var stringGenomes = stringBrains.Where(t => t != string.Empty).Select(t => t.Split(";")[5]).ToList();
            return _populationManager.GetUnitFromGenomes(GlobalParameters.NetworkCaracteristics, stringGenomes).ToList();
        }
    }
}
