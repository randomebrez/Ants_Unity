using System.Collections.Generic;

namespace Assets.Dtos
{
    public class PortionInputs
    {
        public float PheroW { get; set; }

        public float PheroC { get; set; }

        public float WallDist { get; set; }

        public float FoodToken { get; set; }
    }

    public class NeuralNetworkInputs
    {
        private int _portionNumber { get; set; }
        private Dictionary<int, PortionInputs> _portionInputs { get; set; }

        public bool CarryFood { get; set; }
        public bool IsNestInSight { get; set; }

        public NeuralNetworkInputs(int portionNumber)
        {
            _portionNumber = portionNumber;
            _portionInputs = new Dictionary<int, PortionInputs>();
            for (int i = 0; i < portionNumber; i++)
                _portionInputs.Add(i, new PortionInputs());
        }

        public void UpdatePortion(int portionIndex, PortionInputs inputs)
        {
            _portionInputs[portionIndex] = inputs;
        }

        public List<float> GetInputs()
        {
            var result = new List<float>();
            for(int i = 0; i < _portionNumber; i++)
            {
                var portionInputs = _portionInputs[i];
                result.Add(portionInputs.PheroW);
                result.Add(portionInputs.PheroC);
                result.Add(portionInputs.WallDist);
            }

            result.Add(CarryFood ? 1 : 0);
            result.Add(IsNestInSight ? 1 : 0);

            return result;
        }
    }
}
