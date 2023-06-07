using System.Collections.Generic;
using NeuralNetwork.Implementations;
using NeuralNetwork.Interfaces;
using NeuralNetwork.Interfaces.Model;

namespace Assets.Gateways
{
    public class NeuralNetworkGateway
    {
        private readonly IGenomeManager _genomeManager;

        public NeuralNetworkGateway()
        {
            _genomeManager = new GenomeManager();
        }

        public List<Genome> GetGenomes(int number, BrainCaracteristics caracteristics)
        {
            return _genomeManager.GetGenomes(number, caracteristics);
        }

        public Unit[] GetUnits(List<GenomeGraph> graphs)
        {
            return _genomeManager.GetUnitFromGenomeGraphs(graphs);
        }

        public Genome GetMixedGenome(Genome parentA, Genome parentB, BrainCaracteristics caracteristics, int crossOverNumber, float mutationRate)
        {
            return _genomeManager.GetMixedGenome(parentA, parentB, caracteristics, crossOverNumber, mutationRate);
        }
    }
}
