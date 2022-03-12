using UnityEngine;

namespace mew
{
    [CreateAssetMenu(menuName = "Pheromone")]
    public class ScriptablePheromoneBase : ScriptableObject
    {
        [SerializeField] private Caracteristics _caracteristics;
        public Caracteristics BaseCaracteristics => _caracteristics;

        public BasePheromone PheromonePrefab;

        public enum PheromoneTypeEnum
        {
            Wander = 0,
            CarryFood = 1
        }

        [System.Serializable]
        public struct Caracteristics
        {
            public PheromoneTypeEnum PheromoneType;
            public int Duration;
            public Color Color;
        }
    }
}
