




using UnityEngine;
using System.Collections.Generic;

namespace ChainEmpires
{
    public class ResourceManager : IManager
    {
        // Resource types
        public enum ResourceType
        {
            Minerals,
            Energy,
            Food,
            Gold,
            Wood,
            Metal,
            Crystal
        }

        // Current resource amounts
        private Dictionary<ResourceType, float> currentResources = new Dictionary<ResourceType, float>();

        // Resource generation rates per second
        private Dictionary<ResourceType, float> generationRates = new Dictionary<ResourceType, float>();

        // Maximum storage capacity for each resource
        private Dictionary<ResourceType, float> maxCapacities = new Dictionary<ResourceType, float>();

        public void Initialize()
        {
            // Initialize default resources
            foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
            {
                currentResources[type] = 0f;
                generationRates[type] = 1f; // Default rate of 1 per second
                maxCapacities[type] = 1000f; // Default capacity of 1000 units
            }

            // Start resource generation
            InvokeRepeating("GenerateResources", 1f, 1f);
        }

        public void Update()
        {
            // Resource management updates can go here
        }

        public void OnDestroy()
        {
            CancelInvoke("GenerateResources");
        }

        private void GenerateResources()
        {
            foreach (ResourceType type in currentResources.Keys)
            {
                float newAmount = currentResources[type] + generationRates[type];

                // Cap at maximum capacity
                if (newAmount > maxCapacities[type])
                {
                    newAmount = maxCapacities[type];
                }

                currentResources[type] = newAmount;
                Debug.Log($"Generated {generationRates[type]} of {type}, now at {currentResources[type]}");
            }
        }

        public float GetResourceAmount(ResourceType type)
        {
            return currentResources.ContainsKey(type) ? currentResources[type] : 0f;
        }

        public bool ConsumeResource(ResourceType type, float amount)
        {
            if (currentResources[type] >= amount)
            {
                currentResources[type] -= amount;
                Debug.Log($"Consumed {amount} of {type}, remaining: {currentResources[type]}");
                return true;
            }
            else
            {
                Debug.LogWarning($"Not enough {type} to consume {amount}. Available: {currentResources[type]}");
                return false;
            }
        }

        public bool AddResource(ResourceType type, float amount)
        {
            if (currentResources[type] + amount <= maxCapacities[type])
            {
                currentResources[type] += amount;
                Debug.Log($"Added {amount} of {type}, now at {currentResources[type]}");
                return true;
            }
            else
            {
                float remainingSpace = maxCapacities[type] - currentResources[type];
                currentResources[type] = maxCapacities[type];
                Debug.LogWarning($"Could only add {remainingSpace} of {type} due to capacity limit. Now at max: {currentResources[type]}");
                return false;
            }
        }

        public void SetGenerationRate(ResourceType type, float rate)
        {
            if (generationRates.ContainsKey(type))
            {
                generationRates[type] = rate;
                Debug.Log($"Set generation rate for {type} to {rate}");
            }
        }

        public void SetMaxCapacity(ResourceType type, float capacity)
        {
            if (maxCapacities.ContainsKey(type))
            {
                maxCapacities[type] = capacity;

                // If current amount exceeds new capacity, cap it
                if (currentResources[type] > capacity)
                {
                    currentResources[type] = capacity;
                }

                Debug.Log($"Set max capacity for {type} to {capacity}");
            }
        }
    }
}



