using NeuralNetwork.DataBase.Abstraction.Model;
using NeuralNetwork.Interfaces.Model;
using NeuralNetwork.Managers;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Dtos
{
    public class PortionInputs
    {
        public float PheroW { get; set; }

        public float PheroC { get; set; }

        public float WallDist { get; set; }

        public bool FoodToken { get; set; }

        public bool IsNestInSight { get; set; }

        public List<float> ToList() 
        {
            return new List<float>
            {
                PheroW,
                PheroC,
                WallDist,
                FoodToken ? 1 : 0,
                IsNestInSight ? 1 : 0
            };
        }
    }

    public class NeuralNetworkInputs
    {
        private int _portionNumber { get; set; }
        private Dictionary<int, PortionInputs> _portionInputs { get; set; }

        private Dictionary<int, List<float>> _portionOutputs;

        private Dictionary<int, BrainManager> _portionBrains;

        public bool CarryFood { get; set; }

        public NeuralNetworkInputs(int portionNumber, Dictionary<int, Brain> brains)
        {
            _portionNumber = portionNumber;
            _portionInputs = new Dictionary<int, PortionInputs>();
            _portionOutputs = new Dictionary<int, List<float>>();
            _portionBrains = new Dictionary<int, BrainManager>();
            for (int i = 0; i < portionNumber; i++)
            {
                _portionInputs.Add(i, new PortionInputs());
                //_portionBrains.Add(i, new BrainManager(brains[i]));
                //_portionOutputs.Add(i, brains[i].Neurons.Outputs.Select(t => t.Value).ToList());
            }
        }

        public void UpdatePortion(int portionIndex, PortionInputs inputs)
        {
            _portionInputs[portionIndex] = inputs;
            //_portionOutputs[portionIndex] = _portionBrains[portionIndex].ComputeOuputs(inputs.ToList()).Values.ToList();
        }

        public List<float> GetAllInputs()
        {
            var result = new List<float>();
            for(int i = 0; i < _portionNumber; i++)
            {
                result.AddRange(_portionInputs[i].ToList());
            }
            
            result.Add(CarryFood ? 1 : 0);
            //result.Add(IsFoodInSight ? 1 : 0);
            //result.Add(IsNestInSight ? 1 : 0);

            return result;
        }

        public List<float> GetPortionOutputs()
        {
            var result = new List<float>();
            for (int i = 0; i < _portionNumber; i++)
                result.AddRange(_portionOutputs[i]);

            result.Add(CarryFood ? 1 : 0);

            return result;
        }

        public Dictionary<int, Brain> GetBrains()
        {
            var result = new Dictionary<int, Brain>();

            for (int i = 0; i < _portionBrains.Count; i++)
                result.Add(i, _portionBrains[i].GetBrain());

            return result;
        }

    }
}
