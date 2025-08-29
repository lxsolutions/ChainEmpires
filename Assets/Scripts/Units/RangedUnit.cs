


using UnityEngine;

namespace ChainEmpires.Units
{
    public class RangedUnit : Unit
    {
        [Header("Ranged Unit Settings")]
        public GameObject projectilePrefab;
        public Transform projectileSpawnPoint;
        public float projectileSpeed = 15f;
        public float criticalHitChance = 0.2f;
        public float criticalHitMultiplier = 2f;
        public float evasion = 0.1f; // 10% chance to evade attacks
        
        protected override void Start()
        {
            base.Start();
            unitType = UnitManager.UnitType.Archer;
            unitName = "Archer";
            
            // Set ranged-specific stats
            health = 40f;
            maxHealth = 40f;
            damage = 15f;
            attackRange = 6f;
            attackRate = 1.2f; // Faster attacks
            moveSpeed = 3.8f;
        }
        
        protected override void Attack()
        {
            if (attackTarget == null) return;
            
            // Calculate critical hit
            bool isCritical = Random.value <= criticalHitChance;
            float finalDamage = isCritical ? damage * criticalHitMultiplier : damage;
            
            // Fire projectile
            if (projectilePrefab != null && projectileSpawnPoint != null)
            {
                GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
                Projectile proj = projectile.GetComponent<Projectile>();
                
                if (proj != null)
                {
                    proj.Initialize(attackTarget.transform, finalDamage, projectileSpeed, isCritical);
                }
            }
            else
            {
                // Fallback to direct damage if no projectile
                attackTarget.TakeDamage(finalDamage);
            }
            
            string critText = isCritical ? " CRITICAL!" : "";
            Debug.Log($"{unitName} shoots {attackTarget.unitName} for {finalDamage} damage!{critText}");
        }
        
        public override void TakeDamage(float damageAmount)
        {
            // Check for evasion
            if (Random.value <= evasion)
            {
                Debug.Log($"{unitName} evaded the attack!");
                // Play dodge animation
                if (animator != null)
                {
                    animator.SetTrigger("Dodge");
                }
                return;
            }
            
            base.TakeDamage(damageAmount);
        }
        
        public void MultiShot(Unit[] targets)
        {
            if (targets == null || targets.Length == 0) return;
            
            Debug.Log($"{unitName} performing multi-shot on {targets.Length} targets!");
            
            foreach (Unit target in targets)
            {
                if (target != null && Vector3.Distance(transform.position, target.transform.position) <= attackRange * 1.5f)
                {
                    // Fire projectile at each target with reduced damage
                    if (projectilePrefab != null && projectileSpawnPoint != null)
                    {
                        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
                        Projectile proj = projectile.GetComponent<Projectile>();
                        
                        if (proj != null)
                        {
                            proj.Initialize(target.transform, damage * 0.6f, projectileSpeed, false);
                        }
                    }
                }
            }
        }
        
        public override bool LevelUp()
        {
            bool success = base.LevelUp();
            if (success)
            {
                // Ranged-specific level up bonuses
                criticalHitChance += 0.05f;
                evasion += 0.03f;
                attackRange += 0.5f;
                
                Debug.Log($"{unitName} gained +5% crit, +3% evasion, and +0.5 range!");
            }
            return success;
        }
        
        public override string GetStatusInfo()
        {
            string baseInfo = base.GetStatusInfo();
            return $"{baseInfo}\nCrit: {criticalHitChance:P0}\nEvasion: {evasion:P0}\nSpecial: Multi-Shot";
        }
    }
}


