using NeuralNetwork.Interfaces.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Abstractions
{
    public interface IStorage
    {
        Task StoreBrainsAsync(int generationId, List<Brain> brains);
    }
}
