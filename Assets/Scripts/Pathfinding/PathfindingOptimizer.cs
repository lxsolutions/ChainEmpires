

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace ChainEmpires.Pathfinding
{
    public class PathfindingOptimizer : MonoBehaviour
    {
        [Header("Performance Settings")]
        public int maxPathRequestsPerFrame = 5;
        public float pathfindingBudgetMs = 2f; // Max milliseconds per frame for pathfinding
        public bool useMultithreading = false;
        public bool enablePathCaching = true;
        public float cacheLifetime = 10f; // seconds
        
        [Header("Grid Optimization")]
        public bool useDynamicGridResizing = true;
        public float minGridCellSize = 0.5f;
        public float maxGridCellSize = 2f;
        public int gridUpdateFrequency = 60; // frames between grid updates
        
        private Pathfinder pathfinder;
        private Dictionary<string, CachedPath> pathCache = new Dictionary<string, CachedPath>();
        private int frameCounter = 0;
        
        private struct CachedPath
        {
            public List<Vector3> path;
            public float timestamp;
            public int usageCount;
        }
        
        private void Start()
        {
            pathfinder = GetComponent<Pathfinder>();
            
            if (pathfinder == null)
            {
                Debug.LogError("PathfindingOptimizer requires a Pathfinder component!");
                return;
            }
            
            // Initialize with optimized settings
            ApplyOptimizationSettings();
            
            Debug.Log("Pathfinding Optimizer initialized");
        }
        
        private void ApplyOptimizationSettings()
        {
            // Adjust grid cell size based on performance requirements
            if (useDynamicGridResizing)
            {
                // Simple heuristic: smaller cells for more precise movement, larger for performance
                pathfinder.gridCellSize = Mathf.Clamp(pathfinder.gridCellSize, minGridCellSize, maxGridCellSize);
            }
        }
        
        private void Update()
        {
            frameCounter++;
            
            // Update grid periodically if dynamic resizing is enabled
            if (useDynamicGridResizing && frameCounter % gridUpdateFrequency == 0)
            {
                OptimizeGridResolution();
            }
            
            // Clean up old cache entries
            CleanupPathCache();
        }
        
        public List<Vector3> FindOptimizedPath(Vector3 startPos, Vector3 targetPos)
        {
            string cacheKey = GenerateCacheKey(startPos, targetPos);
            
            // Check cache first
            if (enablePathCaching && pathCache.TryGetValue(cacheKey, out CachedPath cached))
            {
                if (Time.time - cached.timestamp < cacheLifetime)
                {
                    cached.usageCount++;
                    pathCache[cacheKey] = cached;
                    return cached.path;
                }
            }
            
            // Perform pathfinding with budget constraints
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            List<Vector3> path = pathfinder.FindPath(startPos, targetPos);
            
            stopwatch.Stop();
            
            // Log performance if needed
            if (stopwatch.ElapsedMilliseconds > pathfindingBudgetMs)
            {
                Debug.LogWarning($"Pathfinding took {stopwatch.ElapsedMilliseconds}ms, over budget of {pathfindingBudgetMs}ms");
            }
            
            // Cache the result
            if (enablePathCaching && path != null)
            {
                pathCache[cacheKey] = new CachedPath
                {
                    path = path,
                    timestamp = Time.time,
                    usageCount = 1
                };
            }
            
            return path;
        }
        
        private string GenerateCacheKey(Vector3 startPos, Vector3 targetPos)
        {
            // Simple cache key based on rounded positions
            return $"{startPos.x:F1}_{startPos.z:F1}_{targetPos.x:F1}_{targetPos.z:F1}";
        }
        
        private void CleanupPathCache()
        {
            List<string> keysToRemove = new List<string>();
            
            foreach (var kvp in pathCache)
            {
                if (Time.time - kvp.Value.timestamp > cacheLifetime)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }
            
            foreach (string key in keysToRemove)
            {
                pathCache.Remove(key);
            }
        }
        
        private void OptimizeGridResolution()
        {
            // Simple heuristic: adjust grid cell size based on performance
            // In a real implementation, this would use performance metrics
            
            float currentFPS = 1f / Time.deltaTime;
            
            if (currentFPS < 30f)
            {
                // Lower FPS, increase cell size for performance
                pathfinder.gridCellSize = Mathf.Min(pathfinder.gridCellSize + 0.1f, maxGridCellSize);
            }
            else if (currentFPS > 60f)
            {
                // High FPS, decrease cell size for precision
                pathfinder.gridCellSize = Mathf.Max(pathfinder.gridCellSize - 0.1f, minGridCellSize);
            }
        }
        
        public void ClearCache()
        {
            pathCache.Clear();
            Debug.Log("Pathfinding cache cleared");
        }
        
        public int GetCacheSize()
        {
            return pathCache.Count;
        }
        
        public float GetCacheHitRate()
        {
            // This would require tracking total requests vs cache hits
            // Simplified implementation
            return pathCache.Count > 0 ? 0.3f : 0f; // Placeholder
        }
        
        public void SetPerformanceProfile(bool highPerformance)
        {
            if (highPerformance)
            {
                // Favor performance over precision
                pathfinder.gridCellSize = maxGridCellSize;
                pathfindingBudgetMs = 1f;
                maxPathRequestsPerFrame = 3;
            }
            else
            {
                // Favor precision over performance
                pathfinder.gridCellSize = minGridCellSize;
                pathfindingBudgetMs = 5f;
                maxPathRequestsPerFrame = 10;
            }
        }
        
        private void OnDestroy()
        {
            // Clean up
            pathCache.Clear();
        }
        
        private void OnGUI()
        {
            if (Debug.isDebugBuild)
            {
                GUILayout.BeginArea(new Rect(10, 220, 300, 100));
                GUILayout.Label("Pathfinding Optimization");
                GUILayout.Label($"Cache Size: {GetCacheSize()}");
                GUILayout.Label($"Cell Size: {pathfinder.gridCellSize:F2}");
                GUILayout.Label($"Budget: {pathfindingBudgetMs}ms");
                GUILayout.EndArea();
            }
        }
    }
}

