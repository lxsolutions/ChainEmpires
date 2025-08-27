
using UnityEngine;

namespace ChainEmpires.Buildings
{
    public class Harvester : Building
    {
        [Header("Harvester Settings")]
        public ResourceManager.ResourceType resourceType = ResourceManager.ResourceType.Minerals;
        public float baseGenerationRate = 5f;
        public float generationRatePerLevel = 2f;
        
        private float currentGenerationRate;
        private float generationTimer = 0f;
        private float generationInterval = 1f; // Generate resources every second
        
        protected override void Start()
        {
            base.Start();
            buildingType = BuildingManager.BuildingType.Mine;
            buildingName = "Harvester";
            
            // Set costs specific to harvester
            mineralCost = 150f;
            energyCost = 75f;
            constructionTime = 45f;
            
            if (isConstructed)
            {
                CalculateGenerationRate();
            }
        }
        
        protected override void OnConstructionComplete()
        {
            base.OnConstructionComplete();
            CalculateGenerationRate();
            Debug.Log($"{buildingName} Level {level} is now generating {currentGenerationRate} {resourceType} per second");
        }
        
        private void CalculateGenerationRate()
        {
            currentGenerationRate = baseGenerationRate + (generationRatePerLevel * (level - 1));
        }
        
        protected override void Update()
        {
            base.Update();
            
            if (isConstructed && resourceManager != null)
            {
                generationTimer += Time.deltaTime;
                
                if (generationTimer >= generationInterval)
                {
                    GenerateResources();
                    generationTimer = 0f;
                }
            }
        }
        
        private void GenerateResources()
        {
            resourceManager.AddResource(resourceType, currentGenerationRate);
        }
        
        public override bool Upgrade()
        {
            bool success = base.Upgrade();
            if (success)
            {
                CalculateGenerationRate();
            }
            return success;
        }
        
        public override string GetStatusInfo()
        {
            string baseInfo = base.GetStatusInfo();
            if (isConstructed)
            {
                return $"{baseInfo}\nGeneration: {currentGenerationRate:0.0}/s {resourceType}";
            }
            return baseInfo;
        }
    }
}
