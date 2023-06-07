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
    }

    public class NeuralNetworkInputs
    {
        public bool CarryFood { get; set; }

        public Dictionary<int, PortionInputs> PortionInputs { get; set; }

        public NeuralNetworkInputs(int portionNumber)
        {
            PortionInputs = new Dictionary<int, PortionInputs>();
            for (int i = 0; i < portionNumber; i++)
                PortionInputs.Add(i, new PortionInputs());
        }

        public Dictionary<int, List<float>> RestrictedInputListGet(HashSet<UnityInputTypeEnum> requiredTypes)
        {
            var result = new Dictionary<int, List<float>>();
            for (int i = 0; i < PortionInputs.Count; i++)
                result.Add(i, new List<float>());

            foreach (var inputType in requiredTypes)
            {
                for(int i = 0; i < PortionInputs.Count; i++)
                {
                    switch(inputType)
                    {
                        case UnityInputTypeEnum.PheromoneW:
                            result[i].Add(PortionInputs[i].PheroW);
                            break;
                        case UnityInputTypeEnum.PheromoneC:
                            result[i].Add(PortionInputs[i].PheroC);
                            break;
                        case UnityInputTypeEnum.Food:
                            result[i].Add(PortionInputs[i].FoodToken ? 1 : 0);
                            break;
                        case UnityInputTypeEnum.Nest:
                            result[i].Add(PortionInputs[i].IsNestInSight ? 1 : 0);
                            break;
                        case UnityInputTypeEnum.Walls:
                            result[i].Add(PortionInputs[i].WallDist);
                            break;
                    }
                }
            }

            return result;
        }
    }
}
