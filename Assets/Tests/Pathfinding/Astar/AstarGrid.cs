using System.Collections.Generic;
using UnityEngine;

namespace Algorithm
{
    internal class AstarGrid : IPathGrid
    {
        private AstarNode[,] nodes;
        private List<AstarNode> openList;
        private List<AstarNode> closeList;
        private List<Vector3> path;

        private Vector2 gridSize;
        private float nodeSize;
        private Vector3 position;

        public AstarGrid()
        {
            openList = new List<AstarNode>();
            closeList = new List<AstarNode>();
            path = new List<Vector3>();
        }

        public void Bake(Vector2 gridSize, float nodeSize, Vector3 position, LayerMask obstacleMask)
        {
            this.position = position;
            if (Mathf.Abs(this.gridSize.x - gridSize.x) >= 0.1f ||
                Mathf.Abs(this.gridSize.y - gridSize.y) >= 0.1f ||
                Mathf.Abs(this.nodeSize - nodeSize) >= 0.1f)
            {
                this.gridSize = gridSize;
                this.nodeSize = nodeSize;
                nodes = new AstarNode[Mathf.RoundToInt(gridSize.x / nodeSize), Mathf.RoundToInt(gridSize.y / nodeSize)];
                for (int i = 0; i < nodes.GetLength(0); i++)
                {
                    for (int j = 0; j < nodes.GetLength(1); j++)
                    {
                        AstarNode node = new AstarNode
                        {
                            Pos = new Vector2Int(i, j)
                        };
                        nodes[i, j] = node;
                    }
                }
            }

            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                for (int j = 0; j < nodes.GetLength(1); j++)
                {
                    nodes[i, j].Obstacle = Physics.CheckBox(OffsetToPos(nodes[i, j].Pos), Vector3.one * (nodeSize - 0.1f) / 2f, UnityEngine.Quaternion.identity, obstacleMask);
                }
            }
        }

        public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos)
        {
            Vector2Int startNodePos = PosToOffset(startPos);
            Vector2Int endNodePos = PosToOffset(endPos);
            if (OutOfRange(startNodePos) || OutOfRange(endNodePos))
            {
                return null;
            }

            AstarNode startNode = nodes[startNodePos.x, startNodePos.y];
            AstarNode endNode = nodes[endNodePos.x, endNodePos.y];
            if (startNode.Obstacle || endNode.Obstacle)
            {
                return null;
            }

            startNode.H = 0;
            startNode.G = 0;
            startNode.Parent = null;
            openList.Clear();
            closeList.Clear();
            openList.Add(startNode);
            while (openList.Count > 0)
            {
                int index = openList.Count - 1;
                AstarNode currentNode = openList[index];
                if (currentNode == endNode)
                {
                    break;
                }

                openList.RemoveAt(index);
                closeList.Add(currentNode);
                FindNearNodesToOpenList(currentNode, endNode);
                openList.Sort();
            }

            if (endNode.Parent == null)
            {
                return null;
            }

            path.Clear();
            path.Add(endPos);
            while (endNode.Parent != null)
            {
                endNode = endNode.Parent;
                if (endNode == startNode)
                {
                    path.Add(startPos);
                }
                else
                {
                    path.Add(OffsetToPos(endNode.Pos));
                }
            }

            path.Reverse();
            return path;
        }

        private bool OutOfRange(Vector2Int pos)
        {
            return pos.x < 0 || pos.x >= nodes.GetLength(0) || pos.y < 0 || pos.y >= nodes.GetLength(1);
        }

        private Vector2Int PosToOffset(Vector3 pos)
        {
            pos -= position;
            int x = Mathf.RoundToInt((pos.x + gridSize.x / 2f) / nodeSize - nodeSize / 2f);
            int y = Mathf.RoundToInt((pos.z + gridSize.y / 2f) / nodeSize - nodeSize / 2f);
            return new Vector2Int(x, y);
        }

        private Vector3 OffsetToPos(Vector2Int offset)
        {
            float x = offset.x * nodeSize - gridSize.x / 2f + nodeSize / 2f;
            float y = offset.y * nodeSize - gridSize.y / 2f + nodeSize / 2f;
            return position + new Vector3(x, 0, y);
        }

        private void FindNearNodesToOpenList(AstarNode node, AstarNode endNode)
        {
            for (int i = node.Pos.x - 1; i <= node.Pos.x + 1 && i >= 0 && i < nodes.GetLength(0); i++)
            {
                for (int j = node.Pos.y - 1; j <= node.Pos.y + 1 && j >= 0 && j < nodes.GetLength(1); j++)
                {
                    AstarNode tempNode = nodes[i, j];
                    if (tempNode.Obstacle || openList.Contains(tempNode) || closeList.Contains(tempNode))
                    {
                        continue;
                    }

                    tempNode.G = node.G + (Mathf.Abs(i - node.Pos.x) == 1 && Mathf.Abs(j - node.Pos.y) == 1 ? 1.4f : 1f);
                    tempNode.H = Mathf.Abs(tempNode.Pos.x - endNode.Pos.x) + Mathf.Abs(tempNode.Pos.y - endNode.Pos.y);
                    tempNode.Parent = node;
                    openList.Add(tempNode);
                }
            }
        }
    }
}
