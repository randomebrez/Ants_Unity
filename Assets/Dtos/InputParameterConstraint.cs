using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Dtos
{
    public class InputParameterConstraint
    {
        public string RegExp { get; set; }

        public int? MaxValue { get; set; }

        public int? MinValue { get; set; }
    }
}
