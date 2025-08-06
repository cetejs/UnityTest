using System.Collections.Generic;
using UnityEngine;

namespace Algorithm
{
    public interface IPathGrid
    {
        void Bake(Vector2 gridSize, float nodeSize, Vector3 position, LayerMask obstacleMask);
        
        List<Vector3> FindPath(Vector3 startPos, Vector3 endPos);
    }
}
