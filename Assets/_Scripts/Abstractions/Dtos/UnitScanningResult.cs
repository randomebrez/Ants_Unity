using System.Collections.Generic;

namespace Assets.Dtos
{public class UnitScanningResult
    {
        public UnitScanningResult(int portionNumber)
        {
            PortionInputValues = new Dictionary<int, UnitPortionInputValues>();
            for (int i = 0; i < portionNumber; i++)
                PortionInputValues.Add(i, new UnitPortionInputValues());
        }

        public bool CarryFood { get; set; }

        public Dictionary<int, UnitPortionInputValues> PortionInputValues { get; set; }

        public Dictionary<int, List<float>> RestrictedInputListGet(HashSet<UnityInputTypeEnum> requiredTypes)
        {
            var result = new Dictionary<int, List<float>>();
            for (int i = 0; i < PortionInputValues.Count; i++)
                result.Add(i, new List<float>());

            foreach (var inputType in requiredTypes)
            {
                for(int i = 0; i < PortionInputValues.Count; i++)
                {
                    switch(inputType)
                    {
                        case UnityInputTypeEnum.PheromoneW:
                            result[i].Add(PortionInputValues[i].PheroW);
                            break;
                        case UnityInputTypeEnum.PheromoneC:
                            result[i].Add(PortionInputValues[i].PheroC);
                            break;
                        case UnityInputTypeEnum.Food:
                            result[i].Add(PortionInputValues[i].FoodToken ? 1 : 0);
                            break;
                        case UnityInputTypeEnum.Nest:
                            result[i].Add(PortionInputValues[i].IsNestInSight ? 1 : 0);
                            break;
                        case UnityInputTypeEnum.Walls:
                            result[i].Add(PortionInputValues[i].WallDist);
                            break;
                    }
                }
            }

            return result;
        }
    }
}
