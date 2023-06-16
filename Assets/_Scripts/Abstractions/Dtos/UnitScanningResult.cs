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

        public List<float> InputsFromInputPortionsList(List<InputTypePortion> inputPortions)
        {
            var result = new List<float>();

            foreach (var inputPortion in inputPortions)
            {
                foreach(var portionIndex in inputPortion.PortionIndexes)
                {
                    foreach(var inputType in inputPortion.UnityInputTypes)
                    {
                        switch (inputType)
                        {
                            case UnityInputTypeEnum.PheromoneW:
                                result.Add(PortionInputValues[portionIndex].PheroW);
                                break;
                            case UnityInputTypeEnum.PheromoneC:
                                result.Add(PortionInputValues[portionIndex].PheroC);
                                break;
                            case UnityInputTypeEnum.Food:
                                result.Add(PortionInputValues[portionIndex].FoodToken ? 1 : 0);
                                break;
                            case UnityInputTypeEnum.Nest:
                                result.Add(PortionInputValues[portionIndex].IsNestInSight ? 1 : 0);
                                break;
                            case UnityInputTypeEnum.Walls:
                                result.Add(PortionInputValues[portionIndex].WallDist);
                                break;
                        }
                    }
                }
            }

            return result;
        }
    }
}
