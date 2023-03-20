using System.Collections.Generic;
using UnityEngine;

namespace Assets.Dtos
{
    public class Block
    {
        public int Id { get; }
        public Vector3 WorldPosition { get; }
        public bool Walkable { get; }
        public Block Parent = null;
        public List<Block> Neighbours = new List<Block>();

        public int XCoordinate;
        public int ZCoordinate;

        public Block(Vector3 worldPosition, bool walkable, int id)
        {
            Id = id;
            WorldPosition = worldPosition;
            Walkable = walkable;
        }
    }
}
