
using UnityEngine;
using System.Collections;

namespace ChainEmpires.Buildings
{
    public abstract class Building : MonoBehaviour
    {
        [Header("Building Configuration")]
        public BuildingManager.BuildingType buildingType;
        public string buildingName;
        public int level = 1;
        public int maxLevel = 5;
        
        [Header("Resource Costs")]
        public float mineralCost = 100f;
        public float energyCost = 50f;
        public float constructionTime = 30f;
        
        [Header("Current State")]
        public bool isConstructed = false;
        public bool isUnderConstruction = false;
        public float constructionProgress = 0f;
        public float constructionTimeRemaining = 0f;
        
        [Header("Visuals")]
        public GameObject constructionVisual;
        public GameObject completedVisual;
        
        protected ResourceManager resourceManager;
        
        protected virtual void Start()
        {
            resourceManager = GameManager.Instance?.ResourceManager;
            
            // Initialize visuals
            if (constructionVisual != null) constructionVisual.SetActive(isUnderConstruction);
            if (completedVisual != null) completedVisual.SetActive(isConstructed);
        }
        
        public virtual void StartConstruction()
        {
            if (isConstructed || isUnderConstruction) return;
            
            isUnderConstruction = true;
            constructionProgress = 0f;
            constructionTimeRemaining = constructionTime;
            
            // Show construction visuals
            if (constructionVisual != null) constructionVisual.SetActive(true);
            if (completedVisual != null) completedVisual.SetActive(false);
            
            Debug.Log($"Started construction of {buildingName}");
        }
        
        public virtual void UpdateConstruction(float deltaTime)
        {
            if (!isUnderConstruction) return;
            
            constructionTimeRemaining -= deltaTime;
            constructionProgress = 1f - (constructionTimeRemaining / constructionTime);
            
            if (constructionTimeRemaining <= 0f)
            {
                CompleteConstruction();
            }
        }
        
        public virtual void CompleteConstruction()
        {
            isUnderConstruction = false;
            isConstructed = true;
            constructionProgress = 1f;
            constructionTimeRemaining = 0f;
            
            // Switch to completed visuals
            if (constructionVisual != null) constructionVisual.SetActive(false);
            if (completedVisual != null) completedVisual.SetActive(true);
            
            Debug.Log($"Completed construction of {buildingName}");
            
            // Building-specific initialization
            OnConstructionComplete();
        }
        
        protected virtual void OnConstructionComplete()
        {
            // Override in derived classes for building-specific behavior
        }
        
        public virtual bool CanUpgrade()
        {
            return isConstructed && level < maxLevel;
        }
        
        public virtual bool Upgrade()
        {
            if (!CanUpgrade()) return false;
            
            // Check resource requirements
            float upgradeMineralCost = mineralCost * level * 1.5f;
            float upgradeEnergyCost = energyCost * level * 1.2f;
            
            if (resourceManager != null)
            {
                if (!resourceManager.ConsumeResource(ResourceManager.ResourceType.Minerals, upgradeMineralCost) ||
                    !resourceManager.ConsumeResource(ResourceManager.ResourceType.Energy, upgradeEnergyCost))
                {
                    Debug.LogWarning($"Not enough resources to upgrade {buildingName}");
                    return false;
                }
            }
            
            level++;
            constructionTime = constructionTime * 1.2f; // Construction time increases with level
            
            // Start upgrade construction
            isUnderConstruction = true;
            constructionProgress = 0f;
            constructionTimeRemaining = constructionTime;
            
            Debug.Log($"Started upgrade of {buildingName} to Level {level}");
            return true;
        }
        
        protected virtual void Update()
        {
            if (isUnderConstruction)
            {
                UpdateConstruction(Time.deltaTime);
            }
        }
        
        // Building-specific functionality
        public virtual void OnSelected()
        {
            // Called when player selects this building
            Debug.Log($"Selected building: {buildingName}");
        }
        
        public virtual void OnDeselected()
        {
            // Called when player deselects this building
        }
        
        public virtual string GetStatusInfo()
        {
            if (isUnderConstruction)
            {
                return $"Constructing: {constructionProgress:P0} ({constructionTimeRemaining:0.0}s)";
            }
            return $"Level {level} {buildingName}";
        }
    }
}
