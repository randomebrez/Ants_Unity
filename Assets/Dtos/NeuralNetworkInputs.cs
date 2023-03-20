using NeuralNetwork.Interfaces.Model;
using System.Collections.Generic;

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
        public bool CarryFood { get; set; }


        private int _portionNumber { get; set; }
        private Dictionary<int, PortionInputs> _portionInputs { get; set; }

        public NeuralNetworkInputs(int portionNumber, Dictionary<int, Brain> brains)
        {
            _portionNumber = portionNumber;
            _portionInputs = new Dictionary<int, PortionInputs>();
            for (int i = 0; i < portionNumber; i++)
            {
                _portionInputs.Add(i, new PortionInputs());
            }
        }

        public void UpdatePortion(int portionIndex, PortionInputs inputs)
        {
            _portionInputs[portionIndex] = inputs;
        }

        public List<float> GetAllInputs()
        {
            var result = new List<float>();
            for(int i = 0; i < _portionNumber; i++)
                result.AddRange(_portionInputs[i].ToList());

            result.Add(CarryFood ? 1 : 0);

            return result;
        }
    }
}
