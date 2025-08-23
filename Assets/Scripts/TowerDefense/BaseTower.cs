










using UnityEngine;

namespace ChainEmpires
{
    public class BaseTower : MonoBehaviour
    {
        [Header("Tower Settings")]
        public float range = 5f;
        public float damage = 20f;
        public float attackSpeed = 1f; // Attacks per second

        private Transform target;
        private float lastAttackTime;

        void Update()
        {
            if (target == null)
            {
                FindTarget();
            }
            else
            {
                AttackTarget();
            }
        }

        private void FindTarget()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (var enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                if (distance <= range && CanSeeTarget(enemy.transform))
                {
                    target = enemy.transform;
                    break;
                }
            }
        }

        private bool CanSeeTarget(Transform target)
        {
            // Simple line of sight check
            Vector3 direction = target.position - transform.position;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, direction, out hit, range))
            {
                return hit.collider.gameObject == target.gameObject;
            }

            return false;
        }

        private void AttackTarget()
        {
            if (target == null) return;
            if (Time.time - lastAttackTime < 1f / attackSpeed) return;

            // Calculate direction and rotate tower
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

            // Attack the target
            EnemyAI enemyAI = target.GetComponent<EnemyAI>();

            if (enemyAI != null)
            {
                enemyAI.TakeDamage(damage);
                Debug.Log($"Tower attacked {target.name} for {damage} damage");

                lastAttackTime = Time.time;

                // Play attack animation/sound here
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}





