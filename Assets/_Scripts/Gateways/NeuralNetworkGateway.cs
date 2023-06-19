using System.Collections.Generic;
using NeuralNetwork.Implementations;
using NeuralNetwork.Abstraction;
using NeuralNetwork.Abstraction.Model;

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
            return _genomeManager.GenomesListGet(number, caracteristics);
        }

        public Unit[] GetUnits(List<GenomeGraph> graphs)
        {
            return _genomeManager.UnitsFromGenomeGraphList(graphs);
        }

        public Genome GetMixedGenome(Genome parentA, Genome parentB, BrainCaracteristics caracteristics, int crossOverNumber, float mutationRate)
        {
            return _genomeManager.GenomeCrossOverGet(parentA, parentB, caracteristics, crossOverNumber, mutationRate);
        }
    }
}
