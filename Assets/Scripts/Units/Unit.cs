

using UnityEngine;
using System.Collections;

namespace ChainEmpires.Units
{
    public abstract class Unit : MonoBehaviour
    {
        [Header("Unit Configuration")]
        public UnitManager.UnitType unitType;
        public string unitName;
        public int level = 1;
        public int maxLevel = 10;
        
        [Header("Combat Stats")]
        public float health = 100f;
        public float maxHealth = 100f;
        public float damage = 20f;
        public float attackRange = 2f;
        public float attackRate = 1f; // Attacks per second
        public float moveSpeed = 4f;
        
        [Header("Current State")]
        public bool isSelected = false;
        public bool isMoving = false;
        public bool isAttacking = false;
        public Vector3 targetPosition;
        public Unit attackTarget;
        
        [Header("Visuals")]
        public GameObject selectionIndicator;
        public Animator animator;
        
        protected float attackTimer = 0f;
        protected ResourceManager resourceManager;
        
        protected virtual void Start()
        {
            resourceManager = GameManager.Instance?.ResourceManager;
            
            // Initialize selection indicator
            if (selectionIndicator != null)
            {
                selectionIndicator.SetActive(isSelected);
            }
            
            // Set unit name if not set
            if (string.IsNullOrEmpty(unitName))
            {
                unitName = $"{unitType}_{GetInstanceID()}";
            }
        }
        
        protected virtual void Update()
        {
            // Handle movement
            if (isMoving)
            {
                MoveToTarget();
            }
            
            // Handle attacking
            if (isAttacking && attackTarget != null)
            {
                attackTimer += Time.deltaTime;
                
                if (attackTimer >= 1f / attackRate)
                {
                    Attack();
                    attackTimer = 0f;
                }
                
                // Check if target is still in range
                if (Vector3.Distance(transform.position, attackTarget.transform.position) > attackRange)
                {
                    StopAttacking();
                }
            }
        }
        
        public virtual void Select()
        {
            isSelected = true;
            if (selectionIndicator != null)
            {
                selectionIndicator.SetActive(true);
            }
            Debug.Log($"Selected unit: {unitName}");
        }
        
        public virtual void Deselect()
        {
            isSelected = false;
            if (selectionIndicator != null)
            {
                selectionIndicator.SetActive(false);
            }
        }
        
        public virtual void MoveTo(Vector3 position)
        {
            targetPosition = position;
            isMoving = true;
            isAttacking = false;
            attackTarget = null;
            
            // Start movement animation
            if (animator != null)
            {
                animator.SetBool("IsMoving", true);
            }
            
            Debug.Log($"{unitName} moving to {position}");
        }
        
        protected virtual void MoveToTarget()
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            
            // Rotate towards movement direction
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
            
            // Check if reached target
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                StopMoving();
            }
        }
        
        protected virtual void StopMoving()
        {
            isMoving = false;
            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
            }
        }
        
        public virtual void AttackTarget(Unit target)
        {
            if (target == null) return;
            
            attackTarget = target;
            isAttacking = true;
            isMoving = false;
            
            // Start attack animation
            if (animator != null)
            {
                animator.SetBool("IsAttacking", true);
            }
            
            Debug.Log($"{unitName} attacking {target.unitName}");
        }
        
        protected virtual void Attack()
        {
            if (attackTarget == null)
            {
                StopAttacking();
                return;
            }
            
            // Apply damage to target
            attackTarget.TakeDamage(damage);
            
            Debug.Log($"{unitName} attacks {attackTarget.unitName} for {damage} damage!");
        }
        
        protected virtual void StopAttacking()
        {
            isAttacking = false;
            attackTarget = null;
            
            if (animator != null)
            {
                animator.SetBool("IsAttacking", false);
            }
        }
        
        public virtual void TakeDamage(float damageAmount)
        {
            health -= damageAmount;
            
            // Play hit animation
            if (animator != null)
            {
                animator.SetTrigger("TakeDamage");
            }
            
            Debug.Log($"{unitName} took {damageAmount} damage. Health: {health}/{maxHealth}");
            
            if (health <= 0f)
            {
                Die();
            }
        }
        
        public virtual void Heal(float healAmount)
        {
            health = Mathf.Min(health + healAmount, maxHealth);
            Debug.Log($"{unitName} healed for {healAmount}. Health: {health}/{maxHealth}");
        }
        
        protected virtual void Die()
        {
            Debug.Log($"{unitName} has been defeated!");
            
            // Play death animation
            if (animator != null)
            {
                animator.SetTrigger("Die");
            }
            
            // TODO: Handle unit death (pooling, rewards, etc.)
            Destroy(gameObject, 2f); // Destroy after animation
        }
        
        public virtual bool LevelUp()
        {
            if (level >= maxLevel) return false;
            
            level++;
            maxHealth *= 1.2f; // 20% health increase
            health = maxHealth;
            damage *= 1.15f; // 15% damage increase
            moveSpeed *= 1.05f; // 5% speed increase
            
            Debug.Log($"{unitName} leveled up to {level}! Stats: {damage} damage, {maxHealth} health");
            return true;
        }
        
        public virtual string GetStatusInfo()
        {
            return $"{unitName} (Lvl {level})\nHealth: {health:0}/{maxHealth:0}\nDamage: {damage:0}";
        }
        
        // Visualize attack range in editor
        private void OnDrawGizmosSelected()
        {
            if (isSelected)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, attackRange);
                
                if (attackTarget != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(transform.position, attackTarget.transform.position);
                }
            }
        }
    }
}

