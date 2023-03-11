using NeuralNetwork.Interfaces.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mew
{
    public abstract class UnitBase : MonoBehaviour
    {
        public ScriptableUnitBase.Stats Stats { get; set; }
    }
}
