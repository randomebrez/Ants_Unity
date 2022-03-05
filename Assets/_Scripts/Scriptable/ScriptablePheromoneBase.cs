using UnityEngine;

namespace mew
{
    [CreateAssetMenu(menuName = "Pheromone")]
    public class ScriptablePheromoneBase : ScriptableObject
    {
        [SerializeField] private Caracteristics _caracteristics;
        public Caracteristics BaseCaracteristics => _caracteristics;

        public PheromoneTypeEnum PheromoneType;
        public BasePheromone PheromonePrefab;

        public enum PheromoneTypeEnum
        {
            Wander = 0,
            CarryFood = 1
        }

        [System.Serializable]
        public struct Caracteristics
        {
            public int Duration;
            public Color Color;
        }
    }
}
