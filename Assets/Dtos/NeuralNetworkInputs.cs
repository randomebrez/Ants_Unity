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

        public bool ActivateTriggerObject = true;

        public List<float> ToList() 
        {
            var result = new List<float>
            {
                PheroW,
                PheroC,
                WallDist
            };
            if (ActivateTriggerObject)
            {
                result.Add(FoodToken ? 1 : 0);
                result.Add(IsNestInSight ? 1 : 0);
            }
            return result;
        }
    }

    public class NeuralNetworkInputs
    {
        public bool CarryFood { get; set; }


        private int _portionNumber { get; set; }
        private Dictionary<int, PortionInputs> _portionInputs { get; set; }

        public NeuralNetworkInputs(int portionNumber)
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
