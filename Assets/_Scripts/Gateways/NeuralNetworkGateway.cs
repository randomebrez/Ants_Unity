using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mew;
using NeuralNetwork.Interfaces;
using NeuralNetwork.Interfaces.Model;

namespace Assets.Gateways
{
    public class NeuralNetworkGateway
    {
        private readonly IPopulation _populationManager;
        private readonly IDatabaseGateway _dbGateway;

        public NeuralNetworkGateway(IPopulation populationManager)//, IDatabaseGateway dbGateway)
        {
            _populationManager = populationManager;
            //_dbGateway = dbGateway;
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

        public List<BaseAnt> SelectBestUnits(List<BaseAnt> ants, int maxNumberToTake = -1)
        {
            var antScores = new Dictionary<string, float>();
            maxNumberToTake = (maxNumberToTake == -1 || maxNumberToTake > ants.Count()) ? ants.Count() : maxNumberToTake;
            for (int i = 0; i < ants.Count(); i++)
                antScores.Add(ants[i].name, ants[i].GetUnitScore());
            var selectedAntNames = antScores.OrderByDescending(t => t.Value).Take(maxNumberToTake).Select(t => t.Key).ToHashSet();

            return ants.Where(t => selectedAntNames.Contains(t.name)).ToList();
        }
    }
}
