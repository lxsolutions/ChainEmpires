


using UnityEngine;

namespace ChainEmpires.Buildings
{
    public class DefenseTower : Building
    {
        [Header("Defense Tower Settings")]
        public float attackRange = 15f;
        public float attackRate = 1f; // Attacks per second
        public float baseDamage = 25f;
        public float damagePerLevel = 10f;
        
        [Header("Targeting")]
        public LayerMask enemyLayerMask;
        public Transform attackPoint;
        
        private float currentDamage;
        private float attackTimer = 0f;
        private Transform currentTarget;
        
        protected override void Start()
        {
            base.Start();
            buildingType = BuildingManager.BuildingType.DefenseTower;
            buildingName = "Defense Tower";
            
            // Set costs specific to defense tower
            mineralCost = 250f;
            energyCost = 150f;
            constructionTime = 90f;
            
            if (isConstructed)
            {
                CalculateStats();
            }
        }
        
        protected override void OnConstructionComplete()
        {
            base.OnConstructionComplete();
            CalculateStats();
            Debug.Log($"{buildingName} Level {level} activated. Damage: {currentDamage}, Range: {attackRange}");
        }
        
        private void CalculateStats()
        {
            currentDamage = baseDamage + (damagePerLevel * (level - 1));
        }
        
        protected override void Update()
        {
            base.Update();
            
            if (isConstructed)
            {
                attackTimer += Time.deltaTime;
                
                // Find targets
                if (currentTarget == null || !IsTargetInRange(currentTarget))
                {
                    FindTarget();
                }
                
                // Attack if target found and cooldown ready
                if (currentTarget != null && attackTimer >= 1f / attackRate)
                {
                    AttackTarget();
                    attackTimer = 0f;
                }
            }
        }
        
        private void FindTarget()
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayerMask);
            
            if (enemies.Length > 0)
            {
                // Simple targeting - first enemy found
                currentTarget = enemies[0].transform;
                Debug.Log($"Defense Tower acquired target: {currentTarget.name}");
            }
            else
            {
                currentTarget = null;
            }
        }
        
        private bool IsTargetInRange(Transform target)
        {
            return Vector3.Distance(transform.position, target.position) <= attackRange;
        }
        
        private void AttackTarget()
        {
            if (currentTarget == null) return;
            
            // TODO: Implement actual attack logic (projectiles, damage application, etc.)
            Debug.Log($"Defense Tower attacking {currentTarget.name} for {currentDamage:0.0} damage");
            
            // Visual feedback for attack
            if (attackPoint != null)
            {
                Debug.DrawLine(attackPoint.position, currentTarget.position, Color.red, 0.5f);
            }
            
            // Apply damage to target (this would interface with enemy health system)
            // currentTarget.GetComponent<EnemyUnit>()?.TakeDamage(currentDamage);
        }
        
        public override bool Upgrade()
        {
            bool success = base.Upgrade();
            if (success)
            {
                CalculateStats();
                attackRange += 2f; // Increase range with level
                attackRate *= 1.1f; // 10% faster attack rate
                
                Debug.Log($"Defense Tower upgraded to Level {level}. Damage: {currentDamage}, Range: {attackRange}, Rate: {attackRate:0.1}/s");
            }
            return success;
        }
        
        public override string GetStatusInfo()
        {
            string baseInfo = base.GetStatusInfo();
            if (isConstructed)
            {
                string targetInfo = currentTarget != null ? 
                    $"Targeting: {currentTarget.name}" : 
                    "Searching for targets";
                
                return $"{baseInfo}\nDamage: {currentDamage:0.0}\nRange: {attackRange:0.0}m\n{targetInfo}";
            }
            return baseInfo;
        }
        
        // Visualize attack range in editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            
            if (currentTarget != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, currentTarget.position);
            }
        }
    }
}


