

using UnityEngine;

namespace ChainEmpires.Pathfinding
{
    public class Obstacle : MonoBehaviour
    {
        [Header("Obstacle Settings")]
        public bool blocksPathfinding = true;
        public LayerMask obstacleLayer = 1 << 8; // Default to "Obstacle" layer
        
        [Header("Visualization")]
        public bool showObstacleBounds = true;
        public Color obstacleColor = new Color(1f, 0f, 0f, 0.3f);
        
        private Collider obstacleCollider;
        
        private void Start()
        {
            obstacleCollider = GetComponent<Collider>();
            
            // Ensure this object is on the correct layer
            if (blocksPathfinding && gameObject.layer != LayerMask.NameToLayer("Obstacle"))
            {
                Debug.LogWarning($"Obstacle {name} is not on the Obstacle layer. Pathfinding may not work correctly.");
            }
            
            // Register with pathfinding system if available
            if (PathfindingManager.Instance != null)
            {
                // The pathfinder will automatically detect this obstacle through physics checks
            }
        }
        
        private void OnEnable()
        {
            // When enabled, the obstacle should be considered by pathfinding
            if (PathfindingManager.Instance != null && obstacleCollider != null)
            {
                obstacleCollider.enabled = true;
            }
        }
        
        private void OnDisable()
        {
            // When disabled, the obstacle should be ignored by pathfinding
            if (obstacleCollider != null)
            {
                obstacleCollider.enabled = false;
            }
        }
        
        private void OnDestroy()
        {
            // Clean up when destroyed
        }
        
        private void OnDrawGizmos()
        {
            if (showObstacleBounds && blocksPathfinding)
            {
                Gizmos.color = obstacleColor;
                
                // Draw bounds based on collider type
                Collider col = GetComponent<Collider>();
                if (col != null)
                {
                    if (col is BoxCollider boxCollider)
                    {
                        Gizmos.matrix = transform.localToWorldMatrix;
                        Gizmos.DrawCube(boxCollider.center, boxCollider.size);
                        Gizmos.matrix = Matrix4x4.identity;
                    }
                    else if (col is SphereCollider sphereCollider)
                    {
                        Gizmos.DrawSphere(transform.position + sphereCollider.center, sphereCollider.radius);
                    }
                    else if (col is CapsuleCollider capsuleCollider)
                    {
                        // Simplified capsule visualization
                        Vector3 center = transform.position + capsuleCollider.center;
                        float radius = capsuleCollider.radius;
                        float height = capsuleCollider.height;
                        
                        Gizmos.DrawWireSphere(center + Vector3.up * (height / 2 - radius), radius);
                        Gizmos.DrawWireSphere(center - Vector3.up * (height / 2 - radius), radius);
                    }
                    else if (col is MeshCollider meshCollider)
                    {
                        if (meshCollider.sharedMesh != null)
                        {
                            Gizmos.DrawWireMesh(meshCollider.sharedMesh, transform.position, transform.rotation, transform.lossyScale);
                        }
                    }
                }
            }
        }
        
        public Bounds GetWorldBounds()
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                return col.bounds;
            }
            return new Bounds(transform.position, Vector3.one);
        }
        
        public bool ContainsPoint(Vector3 point)
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                return col.bounds.Contains(point);
            }
            return false;
        }
    }
}

