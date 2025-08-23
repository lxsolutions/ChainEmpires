










using UnityEngine;
using System.Collections;

namespace ChainEmpires
{
    public class EnemyAI : MonoBehaviour
    {
        [Header("Enemy AI Settings")]
        public float moveSpeed = 2f;
        public float attackRange = 1.5f;
        public float attackCooldown = 1f;

        private EnemyManager.EnemyType enemyType;
        private float currentHealth;
        private float maxHealth;
        private float damage;
        private float speed;
        private Transform target; // Player or building to attack
        private float lastAttackTime;

        void Update()
        {
            if (target == null)
            {
                FindTarget();
            }
            else
            {
                MoveTowardsTarget();
                CheckForAttack();
            }

            // Check for death
            if (currentHealth <= 0f)
            {
                Die();
            }
        }

        public void Initialize(EnemyManager.EnemyData enemyData)
        {
            enemyType = enemyData.Type;
            maxHealth = currentHealth = enemyData.Health;
            damage = enemyData.Damage;
            speed = enemyData.Speed;

            lastAttackTime = 0f;
            target = null; // Will find target automatically
        }

        private void FindTarget()
        {
            // Simple target finding - in a real game this would be more sophisticated
            GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag("PlayerBase");

            if (potentialTargets.Length > 0)
            {
                target = potentialTargets[0].transform;
            }
        }

        private void MoveTowardsTarget()
        {
            if (target == null) return;

            Vector3 direction = (target.position - transform.position).normalized;
            float distanceThisFrame = speed * Time.deltaTime;

            // Move towards the target
            transform.position += direction * distanceThisFrame;

            // Face the target
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        private void CheckForAttack()
        {
            if (target == null) return;
            if (Time.time - lastAttackTime < attackCooldown) return;

            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget <= attackRange)
            {
                AttackTarget();
                lastAttackTime = Time.time;
            }
        }

        private void AttackTarget()
        {
            Debug.Log($"Enemy {enemyType} attacking target!");

            // In a real game, this would trigger combat logic
            if (target != null && target.gameObject != null)
            {
                Building building = target.GetComponent<Building>();

                if (building != null)
                {
                    building.TakeDamage(damage);
                    Debug.Log($"Enemy {enemyType} dealt {damage} damage to building");
                }
                else
                {
                    // Try to find a player controller
                    PlayerController player = target.GetComponent<PlayerController>();
                    if (player != null)
                    {
                        Debug.Log($"Enemy {enemyType} attacking player!");
                        // In a real game, this would trigger player combat
                    }
                }
            }

            // Play attack animation/sound here
        }

        public void TakeDamage(float amount)
        {
            currentHealth -= amount;
            Debug.Log($"Enemy {enemyType} took {amount} damage. Health: {currentHealth}/{maxHealth}");

            // Play hit animation/sound here

            if (currentHealth <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log($"Enemy {enemyType} defeated!");

            // Trigger death animation/effects
            StartCoroutine(DieAfterAnimation());

            // Notify the enemy manager about this enemy's defeat
            EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
            if (enemyManager != null)
            {
                // In a real game, we'd pass more specific wave info
                enemyManager.OnWaveCompleted(null);
            }
        }

        private IEnumerator DieAfterAnimation()
        {
            // Simulate death animation duration
            yield return new WaitForSeconds(0.5f);

            // Destroy the enemy object
            Destroy(gameObject);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}




