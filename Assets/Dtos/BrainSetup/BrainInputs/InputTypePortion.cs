using System.Collections.Generic;

namespace Assets.Dtos
{
    public class InputTypePortion : InputTypeBase
    {
        public InputTypePortion(int maxPortionIndexes)
        {
            PortionIndexes = new List<int>();
            MaxPortionIndexes = maxPortionIndexes;
            UnityInputTypes = new HashSet<UnityInputTypeEnum>();
        }

        public override InputTypeEnum InputType => InputTypeEnum.Portion;

        public int MaxPortionIndexes { get; set; }

        public List<int> PortionIndexes { get; set; }

        public HashSet<UnityInputTypeEnum> UnityInputTypes { get; set; }
    }
}
