using System;

namespace Assets.Dtos
{
    public class Pheromone
    {
        public float Density { get; set; }

        public float Lifetime { get; set; }

        public float RemainingTime { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
