


using UnityEngine;

namespace ChainEmpires.Units
{
    public class MeleeUnit : Unit
    {
        [Header("Melee Unit Settings")]
        public float chargeSpeedMultiplier = 1.5f;
        public float knockbackForce = 5f;
        public float armor = 5f;
        
        private bool isCharging = false;
        private float originalMoveSpeed;
        
        protected override void Start()
        {
            base.Start();
            unitType = UnitManager.UnitType.Warrior;
            unitName = "Warrior";
            
            // Set melee-specific stats
            health = 80f;
            maxHealth = 80f;
            damage = 25f;
            attackRange = 1.5f;
            attackRate = 0.8f; // Slower but heavier attacks
            moveSpeed = 3.5f;
            
            originalMoveSpeed = moveSpeed;
        }
        
        public void ChargeAttack(Unit target)
        {
            if (target == null) return;
            
            isCharging = true;
            moveSpeed = originalMoveSpeed * chargeSpeedMultiplier;
            AttackTarget(target);
            
            Debug.Log($"{unitName} charging at {target.unitName}!");
        }
        
        protected override void Attack()
        {
            base.Attack();
            
            // Melee-specific attack effects
            if (isCharging)
            {
                // Apply knockback to target
                if (attackTarget != null)
                {
                    Vector3 knockbackDirection = (attackTarget.transform.position - transform.position).normalized;
                    // attackTarget.ApplyKnockback(knockbackDirection * knockbackForce);
                }
                
                // Reset charge after attack
                isCharging = false;
                moveSpeed = originalMoveSpeed;
            }
        }
        
        public override void TakeDamage(float damageAmount)
        {
            // Apply armor reduction
            float reducedDamage = Mathf.Max(0, damageAmount - armor);
            base.TakeDamage(reducedDamage);
        }
        
        public override bool LevelUp()
        {
            bool success = base.LevelUp();
            if (success)
            {
                // Melee-specific level up bonuses
                armor += 2f;
                knockbackForce *= 1.1f;
                
                Debug.Log($"{unitName} gained +2 armor and increased knockback!");
            }
            return success;
        }
        
        public override string GetStatusInfo()
        {
            string baseInfo = base.GetStatusInfo();
            return $"{baseInfo}\nArmor: {armor:0.0}\nSpecial: Charge Attack";
        }
    }
}


