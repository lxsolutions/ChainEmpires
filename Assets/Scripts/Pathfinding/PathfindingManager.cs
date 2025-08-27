
using UnityEngine;
using System.Collections.Generic;

namespace ChainEmpires.Pathfinding
{
    public class PathfindingManager : MonoBehaviour
    {
        public static PathfindingManager Instance { get; private set; }

        [Header("Pathfinding Settings")]
        public float pathUpdateInterval = 0.1f;
        public int maxSimultaneousPathRequests = 10;

        private Pathfinder pathfinder;
        private PathfindingOptimizer optimizer;
        private PathSmoother smoother;
        private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
        private bool isProcessingPath = false;

        private struct PathRequest
        {
            public Vector3 startPos;
            public Vector3 targetPos;
            public System.Action<List<Vector3>> callback;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            pathfinder = GetComponent<Pathfinder>();
            if (pathfinder == null)
            {
                pathfinder = gameObject.AddComponent<Pathfinder>();
            }
            
            optimizer = GetComponent<PathfindingOptimizer>();
            if (optimizer == null)
            {
                optimizer = gameObject.AddComponent<PathfindingOptimizer>();
            }
            
            smoother = GetComponent<PathSmoother>();
            if (smoother == null)
            {
                smoother = gameObject.AddComponent<PathSmoother>();
            }
        }

        public static void RequestPath(Vector3 startPos, Vector3 targetPos, System.Action<List<Vector3>> callback)
        {
            if (Instance != null)
            {
                Instance.EnqueuePathRequest(startPos, targetPos, callback);
            }
            else
            {
                callback?.Invoke(null);
            }
        }

        private void EnqueuePathRequest(Vector3 startPos, Vector3 targetPos, System.Action<List<Vector3>> callback)
        {
            PathRequest newRequest = new PathRequest
            {
                startPos = startPos,
                targetPos = targetPos,
                callback = callback
            };

            pathRequestQueue.Enqueue(newRequest);
            TryProcessNext();
        }

        private void TryProcessNext()
        {
            if (!isProcessingPath && pathRequestQueue.Count > 0)
            {
                isProcessingPath = true;
                PathRequest request = pathRequestQueue.Dequeue();
                ProcessPathRequest(request);
            }
        }

        private void ProcessPathRequest(PathRequest request)
        {
            List<Vector3> path;
            
            // Use optimized pathfinding if available
            if (optimizer != null)
            {
                path = optimizer.FindOptimizedPath(request.startPos, request.targetPos);
            }
            else
            {
                path = pathfinder.FindPath(request.startPos, request.targetPos);
            }
            
            // Apply path smoothing if available
            if (smoother != null && path != null)
            {
                path = smoother.SmoothPath(path);
            }
            
            request.callback?.Invoke(path);
            isProcessingPath = false;
            TryProcessNext();
        }

        public void CancelAllRequests()
        {
            pathRequestQueue.Clear();
            isProcessingPath = false;
        }

        public bool IsPositionWalkable(Vector3 position)
        {
            return !Physics.CheckSphere(position, 0.5f, pathfinder.obstacleLayers);
        }

        public Vector3 FindNearestWalkablePosition(Vector3 position, float maxDistance = 5f)
        {
            // Simple implementation - can be improved with proper sampling
            for (int i = 0; i < 10; i++)
            {
                Vector3 randomOffset = Random.insideUnitSphere * maxDistance;
                randomOffset.y = 0;
                Vector3 testPosition = position + randomOffset;

                if (IsPositionWalkable(testPosition))
                {
                    return testPosition;
                }
            }

            return position; // Fallback to original position
        }
    }
}
