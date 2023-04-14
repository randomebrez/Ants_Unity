using System;
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


        public Unit[] GenerateNextGeneration(int childNumber, List<Unit> selectedUnits)
        {
            Unit[] units;
            if (selectedUnits.Any())
                units = _populationManager.GenerateNewGeneration(childNumber, selectedUnits, GlobalParameters.BrainCaracteristics, GlobalParameters.ReproductionCaracteristics);
            else
                units = _populationManager.GenerateFirstGeneration(childNumber, GlobalParameters.BrainCaracteristics);
            return units;
        }

        public List<Unit> GetBrainsFromString(List<string> stringBrains)
        {

            throw new NotImplementedException();
            //var stringGenomes = stringBrains.Where(t => t != string.Empty).Select(t => t.Split(";")[5]).ToList();
            //return _populationManager.GetUnitFromGenomes(GlobalParameters.NetworkCaracteristics, stringGenomes).ToList();
        }
    }
}
