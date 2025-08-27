


using UnityEngine;

namespace ChainEmpires.Units
{
    public class Projectile : MonoBehaviour
    {
        [Header("Projectile Settings")]
        public float speed = 15f;
        public float damage = 15f;
        public bool isCritical = false;
        public GameObject impactEffect;
        
        private Transform target;
        private Vector3 targetPosition;
        private bool hasTarget = false;
        
        public void Initialize(Transform targetTransform, float projectileDamage, float projectileSpeed, bool criticalHit)
        {
            target = targetTransform;
            damage = projectileDamage;
            speed = projectileSpeed;
            isCritical = criticalHit;
            hasTarget = target != null;
            
            if (hasTarget)
            {
                targetPosition = target.position;
            }
            
            // Set visual based on critical hit
            if (isCritical)
            {
                // Change color or add particle effect for critical hits
                GetComponent<Renderer>().material.color = Color.yellow;
            }
            
            // Auto-destroy after 5 seconds if it doesn't hit anything
            Destroy(gameObject, 5f);
        }
        
        void Update()
        {
            if (hasTarget && target != null)
            {
                // Update target position
                targetPosition = target.position;
                
                // Move towards target
                Vector3 direction = (targetPosition - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
                
                // Rotate towards target
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }
                
                // Check if hit target
                if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
                {
                    OnHitTarget();
                }
            }
            else if (hasTarget)
            {
                // Target was destroyed, continue moving to last known position
                Vector3 direction = (targetPosition - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
                
                // Destroy if reached last known position or after delay
                if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
                {
                    Destroy(gameObject);
                }
            }
        }
        
        private void OnHitTarget()
        {
            // Apply damage to target
            Unit targetUnit = target.GetComponent<Unit>();
            if (targetUnit != null)
            {
                targetUnit.TakeDamage(damage);
            }
            
            // Spawn impact effect
            if (impactEffect != null)
            {
                Instantiate(impactEffect, transform.position, Quaternion.identity);
            }
            
            // Destroy projectile
            Destroy(gameObject);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            // Alternative hit detection using colliders
            Unit hitUnit = other.GetComponent<Unit>();
            if (hitUnit != null && hitUnit.transform == target)
            {
                OnHitTarget();
            }
        }
    }
}


