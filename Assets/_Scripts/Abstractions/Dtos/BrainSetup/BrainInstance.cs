using System.Collections.Generic;

namespace Assets.Dtos
{
    public class BrainInstance
    {
        public BrainInstance(BrainTemplate template)
        {
            Template = template;
            InputPortions = new List<UnityInput>();
        }

        public BrainTemplate Template { get; private set; }

        public string UniqueName { get; set; }

        public string PrettyName { get; set; }

        public List<UnityInput> InputPortions { get; set; }
    }
}
