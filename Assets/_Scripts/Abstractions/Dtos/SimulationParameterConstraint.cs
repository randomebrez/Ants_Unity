using System.Collections.Generic;

namespace Assets.Dtos
{
    public class SimulationParameterConstraint
    {
        public SimulationParameterConstraint()
        {
            PossibleValues= new HashSet<string>();
        }

        public string RegExp { get; set; }

        public int? MaxValue { get; set; }

        public int? MinValue { get; set; }

        public HashSet<string> PossibleValues { get; set; }
    }
}
