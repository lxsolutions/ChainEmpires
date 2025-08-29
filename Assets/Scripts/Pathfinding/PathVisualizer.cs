


using UnityEngine;
using System.Collections.Generic;

namespace ChainEmpires.Pathfinding
{
    public class PathVisualizer : MonoBehaviour
    {
        [Header("Visualization Settings")]
        public bool showPaths = true;
        public Color pathColor = Color.green;
        public Color waypointColor = Color.blue;
        public float waypointSize = 0.3f;
        public float pathLineWidth = 0.1f;
        
        [Header("Debug Controls")]
        public bool showGrid = false;
        public bool showObstacles = false;
        
        private Dictionary<Unit, List<Vector3>> unitPaths = new Dictionary<Unit, List<Vector3>>();
        private Pathfinder pathfinder;
        
        private void Start()
        {
            pathfinder = FindObjectOfType<Pathfinder>();
        }
        
        public void RegisterUnitPath(Unit unit, List<Vector3> path)
        {
            if (unit != null && path != null)
            {
                unitPaths[unit] = path;
            }
        }
        
        public void ClearUnitPath(Unit unit)
        {
            if (unitPaths.ContainsKey(unit))
            {
                unitPaths.Remove(unit);
            }
        }
        
        private void OnDrawGizmos()
        {
            if (!showPaths) return;
            
            // Draw unit paths
            foreach (var kvp in unitPaths)
            {
                Unit unit = kvp.Key;
                List<Vector3> path = kvp.Value;
                
                if (unit != null && path != null && path.Count > 0)
                {
                    // Draw path lines
                    Gizmos.color = pathColor;
                    for (int i = 0; i < path.Count - 1; i++)
                    {
                        Gizmos.DrawLine(path[i], path[i + 1]);
                    }
                    
                    // Draw waypoints
                    Gizmos.color = waypointColor;
                    foreach (Vector3 waypoint in path)
                    {
                        Gizmos.DrawSphere(waypoint, waypointSize);
                    }
                    
                    // Draw connection from unit to first waypoint
                    if (unit != null)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawLine(unit.transform.position, path[0]);
                    }
                }
            }
            
            // Draw grid if enabled
            if (showGrid && pathfinder != null)
            {
                pathfinder.showGridGizmos = true;
            }
            else if (pathfinder != null)
            {
                pathfinder.showGridGizmos = false;
            }
        }
        
        public void TogglePathVisualization()
        {
            showPaths = !showPaths;
        }
        
        public void ToggleGridVisualization()
        {
            showGrid = !showGrid;
        }
        
        public void ToggleObstacleVisualization()
        {
            showObstacles = !showObstacles;
            if (pathfinder != null)
            {
                // This would need to be implemented in Pathfinder
            }
        }
    }
}


