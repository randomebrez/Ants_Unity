using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mew
{
    public abstract class UnitBase : MonoBehaviour
    {
        public ScriptableUnitBase.Stats Stats { get; private set; }

        public virtual void Initialyze(ScriptableUnitBase.Stats stats)
        {
            Stats = stats;
        }
    }
}
