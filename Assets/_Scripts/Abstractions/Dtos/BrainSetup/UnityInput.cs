using System.Collections.Generic;

namespace Assets.Dtos
{
    public class UnityInput
    {
        public UnityInput(InputTypeEnum type, int maxPortionIndexes = 0)
        {
            PortionIndexes = new List<int>();
            MaxPortionIndexes = maxPortionIndexes;
            UnityInputTypes = new HashSet<UnityInputTypeEnum>();
        }

        public InputTypeEnum InputType { get; private set; }

        public int MaxPortionIndexes { get; set; }

        public List<int> PortionIndexes { get; set; }

        public PortionTypeEnum PortionTypeToApplyOn { get; set; }

        public HashSet<UnityInputTypeEnum> UnityInputTypes { get; set; }
    }
}
