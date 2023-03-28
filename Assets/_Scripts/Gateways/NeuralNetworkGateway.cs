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
        private readonly IPopulation _populationManager;

        public NeuralNetworkGateway()
        {
            _populationManager = new PopulationManager(GlobalParameters.NetworkCaracteristics);
        }


        public Brain[] GenerateNextGeneration(int childNumber, List<Brain> selectedBrains)
        {
            Brain[] brains;
            if (selectedBrains.Any())
                brains = _populationManager.GenerateNewGeneration(childNumber, selectedBrains);
            else
                brains = _populationManager.GenerateFirstGeneration(childNumber);
            return brains;
        }
    }
}
