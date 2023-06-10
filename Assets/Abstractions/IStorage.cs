using NeuralNetwork.Abstraction.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assets.Abstractions
{
    public interface IStorage
    {
        Task StoreBrainsAsync(int generationId, List<Unit> units);
    }
}
