using System;
using System.Collections.Generic;
using UnityEngine;

namespace Algorithm
{
    public class PathFinding : MonoBehaviour
    {
        private IPathGrid grid;
        [Min(0.5f)]
        [SerializeField]
        private Vector2 gridSize = new Vector2(100, 100);
        [Min(0.5f)]
        [SerializeField]
        private float nodeSize = 1;
        [SerializeField]
        private LayerMask obstacleMask = 1;
        private List<Vector3> path;
        [SerializeField]
        private bool drawGizmos = true;

        private void Awake()
        {
            grid = new AstarGrid();
            grid.Bake(gridSize, nodeSize, transform.position, obstacleMask);
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos)
            {
                return;
            }

            for (int i = 0; i < Mathf.RoundToInt(gridSize.x / nodeSize); i++)
            {
                for (int j = 0; j < Mathf.RoundToInt(gridSize.y / nodeSize); j++)
                {
                    float x = i * nodeSize - gridSize.x / 2f + nodeSize / 2f;
                    float y = j * nodeSize - gridSize.y / 2f + nodeSize / 2f;
                    Vector3 pos = transform.position + new Vector3(x, 0, y);

                    if (Physics.CheckBox(pos, Vector3.one * (nodeSize - 0.1f) / 2f, Quaternion.identity, obstacleMask))
                    {
                        Gizmos.color = Color.red;
                    }

                    Gizmos.DrawCube(pos, new Vector3(nodeSize - 0.1f, 0.1f, nodeSize - 0.1f));
                    Gizmos.color = Color.white;
                }
            }

            if (path != null && path.Count >= 2)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(path[i], 0.2f);
                    Gizmos.DrawLine(path[i], path[i + 1]);
                    Gizmos.color = Color.white;
                }
            }
        }

        public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos)
        {
            path = grid.FindPath(startPos, endPos);
            return path;
        }
    }
}
