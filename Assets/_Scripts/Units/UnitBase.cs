using NeuralNetwork.Interfaces.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mew
{
    public abstract class UnitBase : MonoBehaviour
    {
        public ScriptableUnitBase.Stats Stats { get; private set; }

        public virtual void Initialyze(ScriptableUnitBase.Stats stats, Brain brain)
        {
            Stats = stats;
        }
    }
}
