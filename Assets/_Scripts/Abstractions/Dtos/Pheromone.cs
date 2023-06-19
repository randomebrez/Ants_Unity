using System;

namespace Assets.Dtos
{
    public class Pheromone
    {
        public float Density { get; set; }

        public int Lifetime { get; set; }

        public int RemainingTime { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
