
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace ChainEmpires.Pathfinding
{
    public class Pathfinder : MonoBehaviour
    {
        [Header("Grid Settings")]
        public float gridCellSize = 1f;
        public LayerMask obstacleLayers = 1;
        public bool showGridGizmos = false;

        private Vector2 gridWorldSize;
        private Node[,] grid;
        private int gridSizeX, gridSizeY;

        public class Node
        {
            public bool walkable;
            public Vector3 worldPosition;
            public int gridX;
            public int gridY;
            public int gCost;
            public int hCost;
            public Node parent;

            public int fCost => gCost + hCost;

            public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
            {
                walkable = _walkable;
                worldPosition = _worldPos;
                gridX = _gridX;
                gridY = _gridY;
            }
        }

        private void Start()
        {
            gridWorldSize = new Vector2(50f, 50f);
            CreateGrid();
        }

        private void CreateGrid()
        {
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / gridCellSize);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / gridCellSize);
            grid = new Node[gridSizeX, gridSizeY];

            Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * gridCellSize + gridCellSize / 2) + Vector3.forward * (y * gridCellSize + gridCellSize / 2);
                    bool walkable = !Physics.CheckSphere(worldPoint, gridCellSize / 2, obstacleLayers);
                    grid[x, y] = new Node(walkable, worldPoint, x, y);
                }
            }
        }

        public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Node startNode = NodeFromWorldPoint(startPos);
            Node targetNode = NodeFromWorldPoint(targetPos);

            if (startNode == null || targetNode == null || !targetNode.walkable)
            {
                return null;
            }

            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost || 
                        (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                foreach (Node neighbour in GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }

            return null;
        }

        private List<Vector3> RetracePath(Node startNode, Node endNode)
        {
            List<Vector3> path = new List<Vector3>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode.worldPosition);
                currentNode = currentNode.parent;
            }
            path.Reverse();
            return SimplifyPath(path);
        }

        private List<Vector3> SimplifyPath(List<Vector3> path)
        {
            if (path.Count < 2)
                return path;

            List<Vector3> simplifiedPath = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;

            for (int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew = new Vector2(path[i].x - path[i-1].x, path[i].z - path[i-1].z);
                if (directionNew != directionOld)
                {
                    simplifiedPath.Add(path[i-1]);
                }
                directionOld = directionNew;
            }
            simplifiedPath.Add(path[path.Count - 1]);
            return simplifiedPath;
        }

        private List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }

            return neighbours;
        }

        private int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }

        private Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
            float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
            return grid[x, y];
        }

        private void OnDrawGizmos()
        {
            if (grid != null && showGridGizmos)
            {
                foreach (Node n in grid)
                {
                    Gizmos.color = n.walkable ? Color.white : Color.red;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (gridCellSize - 0.1f));
                }
            }
        }
    }
}
