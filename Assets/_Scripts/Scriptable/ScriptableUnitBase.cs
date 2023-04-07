using UnityEngine;

namespace mew
{
    public class ScriptableUnitBase : ScriptableObject
    {
        [SerializeField] private Stats _statistics;
        public Stats BaseStats => _statistics;

        [System.Serializable]
        public class Stats
        {
            // Base stats
            public float MaxSpeed;
            public float SteerStrength;
            public float WanderStrength;
            public Color Color;

            // Vision
            public float VisionRadius { get; set; } = 4;
            public float VisionAngle { get; set; } = 130;
            public int ScannerSubdivisions;

            // Collision
            public float TargetDestroyTreshold;
            public float BoundRadius;
            public float AvoidCollisionForce;
        }
    }
}
