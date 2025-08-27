


using UnityEngine;
using System.Collections.Generic;

namespace ChainEmpires.Pathfinding
{
    public class DynamicObstacleAvoidance : MonoBehaviour
    {
        [Header("Avoidance Settings")]
        public float avoidanceRadius = 2f;
        public LayerMask dynamicObstacleLayers = 1 << 9; // Default to "Unit" layer
        public float avoidanceForce = 5f;
        public float predictionTime = 0.5f;
        
        [Header("Debug Visualization")]
        public bool showAvoidanceRays = true;
        public Color avoidanceRayColor = Color.yellow;
        
        private Unit unit;
        private Rigidbody unitRigidbody;
        
        private void Start()
        {
            unit = GetComponent<Unit>();
            unitRigidbody = GetComponent<Rigidbody>();
            
            if (unitRigidbody == null)
            {
                unitRigidbody = gameObject.AddComponent<Rigidbody>();
                unitRigidbody.isKinematic = true;
                unitRigidbody.useGravity = false;
            }
        }
        
        private void Update()
        {
            if (unit != null && unit.isMoving)
            {
                Vector3 avoidanceForce = CalculateAvoidanceForce();
                if (avoidanceForce != Vector3.zero)
                {
                    ApplyAvoidanceForce(avoidanceForce);
                }
            }
        }
        
        private Vector3 CalculateAvoidanceForce()
        {
            Vector3 avoidanceForce = Vector3.zero;
            
            // Check for nearby dynamic obstacles (other units)
            Collider[] nearbyUnits = Physics.OverlapSphere(transform.position, avoidanceRadius, dynamicObstacleLayers);
            
            foreach (Collider col in nearbyUnits)
            {
                if (col.gameObject != gameObject && col.TryGetComponent<Unit>(out var otherUnit))
                {
                    Vector3 toOther = otherUnit.transform.position - transform.position;
                    float distance = toOther.magnitude;
                    
                    if (distance < avoidanceRadius && distance > 0.1f)
                    {
                        // Calculate repulsion force (inverse square law)
                        float repulsionStrength = Mathf.Clamp01(1f - (distance / avoidanceRadius));
                        Vector3 repulsionDirection = -toOther.normalized;
                        
                        avoidanceForce += repulsionDirection * repulsionStrength * avoidanceForce;
                    }
                }
            }
            
            return avoidanceForce;
        }
        
        private void ApplyAvoidanceForce(Vector3 force)
        {
            if (unitRigidbody != null)
            {
                // Apply force to rigidbody for smooth avoidance
                unitRigidbody.AddForce(force * Time.deltaTime, ForceMode.VelocityChange);
            }
            else
            {
                // Fallback: directly modify position
                transform.position += force * Time.deltaTime;
            }
        }
        
        public Vector3 PredictFuturePosition(float timeAhead)
        {
            if (unit != null && unit.isMoving)
            {
                Vector3 movementDirection = (unit.targetPosition - transform.position).normalized;
                return transform.position + movementDirection * unit.moveSpeed * timeAhead;
            }
            return transform.position;
        }
        
        public bool IsCollisionImminent(float predictionTime = 0.5f)
        {
            Vector3 futurePosition = PredictFuturePosition(predictionTime);
            return Physics.CheckSphere(futurePosition, avoidanceRadius / 2f, dynamicObstacleLayers);
        }
        
        private void OnDrawGizmos()
        {
            if (showAvoidanceRays && unit != null && unit.isMoving)
            {
                // Draw avoidance radius
                Gizmos.color = avoidanceRayColor;
                Gizmos.DrawWireSphere(transform.position, avoidanceRadius);
                
                // Draw predicted future position
                Vector3 futurePos = PredictFuturePosition(predictionTime);
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(futurePos, 0.3f);
                Gizmos.DrawLine(transform.position, futurePos);
                
                // Draw avoidance force
                Vector3 avoidanceForce = CalculateAvoidanceForce();
                if (avoidanceForce != Vector3.zero)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawRay(transform.position, avoidanceForce.normalized * 2f);
                }
            }
        }
    }
}


