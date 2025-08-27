

using UnityEngine;

namespace ChainEmpires.Buildings
{
    public class Wall : Building
    {
        [Header("Wall Settings")]
        public float baseHealth = 100f;
        public float healthPerLevel = 50f;
        public float defenseRating = 10f;
        public float defenseRatingPerLevel = 5f;
        
        private float currentHealth;
        private float currentDefenseRating;
        
        protected override void Start()
        {
            base.Start();
            buildingType = BuildingManager.BuildingType.Wall;
            buildingName = "Wall";
            
            // Set costs specific to wall
            mineralCost = 80f;
            energyCost = 20f;
            constructionTime = 20f;
            
            if (isConstructed)
            {
                CalculateStats();
            }
        }
        
        protected override void OnConstructionComplete()
        {
            base.OnConstructionComplete();
            CalculateStats();
            Debug.Log($"{buildingName} Level {level} constructed. Health: {currentHealth}, Defense: {currentDefenseRating}");
        }
        
        private void CalculateStats()
        {
            currentHealth = baseHealth + (healthPerLevel * (level - 1));
            currentDefenseRating = defenseRating + (defenseRatingPerLevel * (level - 1));
        }
        
        public void TakeDamage(float damage)
        {
            if (!isConstructed) return;
            
            float reducedDamage = Mathf.Max(0, damage - currentDefenseRating);
            currentHealth -= reducedDamage;
            
            Debug.Log($"Wall took {reducedDamage:0.0} damage (reduced from {damage:0.0}). Health: {currentHealth:0.0}/{GetMaxHealth()}");
            
            if (currentHealth <= 0)
            {
                DestroyWall();
            }
        }
        
        private float GetMaxHealth()
        {
            return baseHealth + (healthPerLevel * (level - 1));
        }
        
        private void DestroyWall()
        {
            Debug.Log($"Wall at {transform.position} has been destroyed!");
            // TODO: Add destruction visual effects
            Destroy(gameObject);
        }
        
        public void Repair(float repairAmount)
        {
            if (!isConstructed) return;
            
            float maxHealth = GetMaxHealth();
            currentHealth = Mathf.Min(currentHealth + repairAmount, maxHealth);
            
            Debug.Log($"Wall repaired by {repairAmount:0.0}. Health: {currentHealth:0.0}/{maxHealth}");
        }
        
        public override bool Upgrade()
        {
            bool success = base.Upgrade();
            if (success)
            {
                CalculateStats();
                Debug.Log($"Wall upgraded to Level {level}. Health: {currentHealth}, Defense: {currentDefenseRating}");
            }
            return success;
        }
        
        public override string GetStatusInfo()
        {
            string baseInfo = base.GetStatusInfo();
            if (isConstructed)
            {
                return $"{baseInfo}\nHealth: {currentHealth:0.0}/{GetMaxHealth()}\nDefense: {currentDefenseRating:0.0}";
            }
            return baseInfo;
        }
        
        // Wall-specific functionality for connecting to other walls
        public void ConnectToNeighbor(Wall neighbor)
        {
            // TODO: Implement wall connection logic for visual continuity
            Debug.Log($"Connecting wall at {transform.position} to neighbor at {neighbor.transform.position}");
        }
    }
}

