using System;
using UnityEngine;

namespace Tests
{
    public class AstarNode : IComparable<AstarNode>
    {
        public Vector2Int Pos;
        public float G;
        public float H;
        public float F => G + H;
        public AstarNode Parent;
        public bool Obstacle;

        public int CompareTo(AstarNode other)
        {
            return -F.CompareTo(other.F);
        }
    }
}
