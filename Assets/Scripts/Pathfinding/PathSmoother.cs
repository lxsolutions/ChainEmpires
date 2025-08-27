


using UnityEngine;
using System.Collections.Generic;

namespace ChainEmpires.Pathfinding
{
    public class PathSmoother : MonoBehaviour
    {
        [Header("Smoothing Settings")]
        public bool enableSmoothing = true;
        public int smoothingIterations = 2;
        public float maxSmoothingAngle = 45f;
        public float obstacleClearance = 0.5f;
        
        [Header("Bezier Curve Settings")]
        public bool useBezierCurves = true;
        public int bezierSegments = 3;
        public float bezierTension = 0.5f;
        
        private Pathfinder pathfinder;
        
        private void Start()
        {
            pathfinder = GetComponent<Pathfinder>();
        }
        
        public List<Vector3> SmoothPath(List<Vector3> originalPath)
        {
            if (!enableSmoothing || originalPath == null || originalPath.Count < 3)
            {
                return originalPath;
            }
            
            List<Vector3> smoothedPath = new List<Vector3>(originalPath);
            
            for (int iteration = 0; iteration < smoothingIterations; iteration++)
            {
                smoothedPath = ApplySmoothingPass(smoothedPath);
            }
            
            if (useBezierCurves)
            {
                smoothedPath = ApplyBezierSmoothing(smoothedPath);
            }
            
            return smoothedPath;
        }
        
        private List<Vector3> ApplySmoothingPass(List<Vector3> path)
        {
            List<Vector3> smoothed = new List<Vector3> { path[0] };
            
            for (int i = 1; i < path.Count - 1; i++)
            {
                Vector3 previous = path[i - 1];
                Vector3 current = path[i];
                Vector3 next = path[i + 1];
                
                // Calculate average position
                Vector3 averaged = (previous + current + next) / 3f;
                
                // Check if the smoothed point is valid (not inside obstacles)
                if (IsPositionValid(averaged, obstacleClearance))
                {
                    smoothed.Add(averaged);
                }
                else
                {
                    // Keep original point if smoothed point is invalid
                    smoothed.Add(current);
                }
            }
            
            smoothed.Add(path[path.Count - 1]);
            return smoothed;
        }
        
        private List<Vector3> ApplyBezierSmoothing(List<Vector3> path)
        {
            if (path.Count < 3) return path;
            
            List<Vector3> bezierPath = new List<Vector3> { path[0] };
            
            for (int i = 0; i < path.Count - 2; i++)
            {
                Vector3 p0 = path[i];
                Vector3 p1 = path[i + 1];
                Vector3 p2 = path[i + 2];
                
                // Create control points with tension
                Vector3 control1 = p1 + (p0 - p1) * bezierTension;
                Vector3 control2 = p1 + (p2 - p1) * bezierTension;
                
                // Generate bezier curve points
                for (int segment = 1; segment <= bezierSegments; segment++)
                {
                    float t = segment / (float)bezierSegments;
                    Vector3 point = CalculateBezierPoint(t, p1, control1, control2, p2);
                    
                    if (IsPositionValid(point, obstacleClearance))
                    {
                        bezierPath.Add(point);
                    }
                }
            }
            
            bezierPath.Add(path[path.Count - 1]);
            return bezierPath;
        }
        
        private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;
            
            Vector3 point = uuu * p0; // (1-t)^3 * p0
            point += 3 * uu * t * p1; // 3*(1-t)^2*t * p1
            point += 3 * u * tt * p2; // 3*(1-t)*t^2 * p2
            point += ttt * p3; // t^3 * p3
            
            return point;
        }
        
        private bool IsPositionValid(Vector3 position, float clearance)
        {
            if (pathfinder == null) return true;
            
            // Check if position is walkable and has sufficient clearance
            return !Physics.CheckSphere(position, clearance, pathfinder.obstacleLayers);
        }
        
        public List<Vector3> SimplifyPath(List<Vector3> path, float angleThreshold = 5f)
        {
            if (path == null || path.Count < 3) return path;
            
            List<Vector3> simplified = new List<Vector3> { path[0] };
            
            for (int i = 1; i < path.Count - 1; i++)
            {
                Vector3 prevDir = (path[i] - path[i - 1]).normalized;
                Vector3 nextDir = (path[i + 1] - path[i]).normalized;
                
                float angle = Vector3.Angle(prevDir, nextDir);
                
                // Keep point if angle is significant
                if (angle > angleThreshold)
                {
                    simplified.Add(path[i]);
                }
            }
            
            simplified.Add(path[path.Count - 1]);
            return simplified;
        }
        
        public List<Vector3> OptimizeForMobile(List<Vector3> path, int maxWaypoints = 10)
        {
            if (path == null || path.Count <= maxWaypoints) return path;
            
            // Simple optimization: keep every nth point
            List<Vector3> optimized = new List<Vector3>();
            int step = Mathf.Max(1, path.Count / maxWaypoints);
            
            for (int i = 0; i < path.Count; i += step)
            {
                optimized.Add(path[i]);
            }
            
            // Ensure start and end points are included
            if (!optimized.Contains(path[0]))
            {
                optimized.Insert(0, path[0]);
            }
            
            if (!optimized.Contains(path[path.Count - 1]))
            {
                optimized.Add(path[path.Count - 1]);
            }
            
            return SmoothPath(optimized);
        }
        
        public float CalculatePathQuality(List<Vector3> path)
        {
            if (path == null || path.Count < 2) return 0f;
            
            float totalLength = 0f;
            float straightLineLength = Vector3.Distance(path[0], path[path.Count - 1]);
            
            for (int i = 0; i < path.Count - 1; i++)
            {
                totalLength += Vector3.Distance(path[i], path[i + 1]);
            }
            
            // Quality metric: how close is the path to a straight line?
            // Lower values are better (closer to optimal)
            return totalLength / straightLineLength;
        }
    }
}


