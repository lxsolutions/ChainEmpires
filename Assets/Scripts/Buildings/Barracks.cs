

using UnityEngine;
using System.Collections;

namespace ChainEmpires.Buildings
{
    public class Barracks : Building
    {
        [Header("Barracks Settings")]
        public float unitProductionTime = 10f;
        public int maxQueueSize = 5;
        
        [Header("Unit Costs")]
        public float meleeUnitMineralCost = 50f;
        public float meleeUnitEnergyCost = 25f;
        public float rangedUnitMineralCost = 75f;
        public float rangedUnitEnergyCost = 40f;
        
        private int currentQueueSize = 0;
        private float productionTimer = 0f;
        private bool isProducing = false;
        
        protected override void Start()
        {
            base.Start();
            buildingType = BuildingManager.BuildingType.Barracks;
            buildingName = "Barracks";
            
            // Set costs specific to barracks
            mineralCost = 200f;
            energyCost = 100f;
            constructionTime = 60f;
        }
        
        protected override void OnConstructionComplete()
        {
            base.OnConstructionComplete();
            Debug.Log($"{buildingName} Level {level} is ready for unit production");
        }
        
        protected override void Update()
        {
            base.Update();
            
            if (isConstructed && isProducing)
            {
                productionTimer += Time.deltaTime;
                
                if (productionTimer >= unitProductionTime)
                {
                    CompleteUnitProduction();
                    productionTimer = 0f;
                    
                    // Check if there are more units in queue
                    if (currentQueueSize > 0)
                    {
                        currentQueueSize--;
                        StartProduction();
                    }
                    else
                    {
                        isProducing = false;
                    }
                }
            }
        }
        
        public bool TrainMeleeUnit()
        {
            if (!isConstructed) return false;
            
            if (currentQueueSize >= maxQueueSize)
            {
                Debug.LogWarning("Barracks queue is full");
                return false;
            }
            
            // Check resource requirements
            if (resourceManager != null)
            {
                if (!resourceManager.ConsumeResource(ResourceManager.ResourceType.Minerals, meleeUnitMineralCost) ||
                    !resourceManager.ConsumeResource(ResourceManager.ResourceType.Energy, meleeUnitEnergyCost))
                {
                    Debug.LogWarning("Not enough resources to train melee unit");
                    return false;
                }
            }
            
            currentQueueSize++;
            
            if (!isProducing)
            {
                StartProduction();
            }
            
            Debug.Log($"Started training melee unit. Queue: {currentQueueSize}/{maxQueueSize}");
            return true;
        }
        
        public bool TrainRangedUnit()
        {
            if (!isConstructed) return false;
            
            if (currentQueueSize >= maxQueueSize)
            {
                Debug.LogWarning("Barracks queue is full");
                return false;
            }
            
            // Check resource requirements
            if (resourceManager != null)
            {
                if (!resourceManager.ConsumeResource(ResourceManager.ResourceType.Minerals, rangedUnitMineralCost) ||
                    !resourceManager.ConsumeResource(ResourceManager.ResourceType.Energy, rangedUnitEnergyCost))
                {
                    Debug.LogWarning("Not enough resources to train ranged unit");
                    return false;
                }
            }
            
            currentQueueSize++;
            
            if (!isProducing)
            {
                StartProduction();
            }
            
            Debug.Log($"Started training ranged unit. Queue: {currentQueueSize}/{maxQueueSize}");
            return true;
        }
        
        private void StartProduction()
        {
            isProducing = true;
            productionTimer = 0f;
        }
        
        private void CompleteUnitProduction()
        {
            // This would spawn a unit in the game world
            Debug.Log("Unit production completed!");
            
            // TODO: Integrate with UnitManager to spawn actual units
            if (GameManager.Instance?.UnitManager != null)
            {
                // GameManager.Instance.UnitManager.SpawnUnit(UnitManager.UnitType.Melee, transform.position);
            }
        }
        
        public override bool Upgrade()
        {
            bool success = base.Upgrade();
            if (success)
            {
                // Improve barracks with upgrades
                maxQueueSize += 2;
                unitProductionTime *= 0.8f; // 20% faster production
                
                Debug.Log($"Barracks upgraded to Level {level}. Max queue: {maxQueueSize}, Production time: {unitProductionTime:0.0}s");
            }
            return success;
        }
        
        public override string GetStatusInfo()
        {
            string baseInfo = base.GetStatusInfo();
            if (isConstructed)
            {
                string productionInfo = isProducing ? 
                    $"Producing: {productionTimer:0.0}/{unitProductionTime:0.0}s" : 
                    "Idle";
                
                return $"{baseInfo}\nQueue: {currentQueueSize}/{maxQueueSize}\n{productionInfo}";
            }
            return baseInfo;
        }
    }
}

