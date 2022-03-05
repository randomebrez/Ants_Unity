using System;

namespace Assets.Dtos
{
    public class Pheromone
    {
        public int Id { get; set; }

        public float Density { get; set; }

        public TimeSpan Lifetime { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
