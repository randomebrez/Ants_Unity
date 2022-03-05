using UnityEngine;

namespace Assets.Dtos
{
    public enum TargetTypeEnum
    {
        Nest,
        Food
    }

    public class Target
    {
        public Transform Transform { get; set; }
        public TargetTypeEnum Type { get; set; }
        public bool Active { get; set; }
    }
}
