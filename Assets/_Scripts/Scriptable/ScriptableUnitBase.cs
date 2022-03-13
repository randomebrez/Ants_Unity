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
            // Base stats
            public float MaxSpeed;
            public float SteerStrength;
            public float WanderStrength;
            public Color Color;

            // Vision
            public float VisionRadius;
            public float VisionAngle;

            // Collision
            public float TargetDestroyTreshold;
            public float BoundRadius;
            public float AvoidCollisionForce;
        }
    }
}
