using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mew
{
    public class ScriptableUnitBase : ScriptableObject
    {
        [SerializeField] private Stats _statistics;
        public Stats BaseStats => _statistics;

        [System.Serializable]
        public struct Stats
        {
            public int MaxSpeed;
        }
    }
}
