using UnityEngine;
using System.Collections.Generic;
using ChainEmpires.Buildings;

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

        // Resource generation rates per second (base + building contributions)
        private Dictionary<ResourceType, float> generationRates = new Dictionary<ResourceType, float>();

        // Maximum storage capacity for each resource
        private Dictionary<ResourceType, float> maxCapacities = new Dictionary<ResourceType, float>();

        // Building contributions to generation rates
        private Dictionary<ResourceType, List<Harvester>> harvesters = new Dictionary<ResourceType, List<Harvester>>();

        // Resource generation timer
        private float generationTimer = 0f;
        private float generationInterval = 1f; // Generate every second

        // Starting resources
        private Dictionary<ResourceType, float> startingResources = new Dictionary<ResourceType, float>()
        {
            { ResourceType.Minerals, 500f },
            { ResourceType.Energy, 300f },
            { ResourceType.Food, 200f },
            { ResourceType.Gold, 100f },
            { ResourceType.Wood, 400f },
            { ResourceType.Metal, 250f },
            { ResourceType.Crystal, 150f }
        };

        // Starting capacities
        private Dictionary<ResourceType, float> startingCapacities = new Dictionary<ResourceType, float>()
        {
            { ResourceType.Minerals, 1000f },
            { ResourceType.Energy, 800f },
            { ResourceType.Food, 600f },
            { ResourceType.Gold, 500f },
            { ResourceType.Wood, 900f },
            { ResourceType.Metal, 700f },
            { ResourceType.Crystal, 400f }
        };

        public void Initialize()
        {
            Debug.Log("ResourceManager initialized");

            // Initialize resources with starting values
            foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
            {
                currentResources[type] = startingResources[type];
                generationRates[type] = 0f; // Start with 0, buildings will add to this
                maxCapacities[type] = startingCapacities[type];
                harvesters[type] = new List<Harvester>();
            }

            Debug.Log("Resource system initialized with starting resources");
        }

        public void Update()
        {
            // Handle resource generation on Update instead of InvokeRepeating
            generationTimer += Time.deltaTime;
            
            if (generationTimer >= generationInterval)
            {
                GenerateResources();
                generationTimer = 0f;
            }
        }

        public void OnDestroy()
        {
            // Clean up
            currentResources.Clear();
            generationRates.Clear();
            maxCapacities.Clear();
            harvesters.Clear();
        }

        private void GenerateResources()
        {
            // Calculate generation from all harvesters
            CalculateGenerationRates();

            foreach (ResourceType type in currentResources.Keys)
            {
                if (generationRates[type] > 0)
                {
                    float newAmount = currentResources[type] + generationRates[type];

                    // Cap at maximum capacity
                    if (newAmount > maxCapacities[type])
                    {
                        newAmount = maxCapacities[type];
                    }

                    currentResources[type] = newAmount;
                    
                    // Only log if we're actually generating something
                    if (generationRates[type] > 0.1f)
                    {
                        Debug.Log($"Generated {generationRates[type]:0.0} of {type}, now at {currentResources[type]:0.0}/{maxCapacities[type]:0.0}");
                    }
                }
            }
        }

        private void CalculateGenerationRates()
        {
            // Reset all generation rates
            foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
            {
                generationRates[type] = 0f;
            }

            // Add contributions from all harvesters
            foreach (var harvesterList in harvesters.Values)
            {
                foreach (Harvester harvester in harvesterList)
                {
                    if (harvester != null && harvester.isConstructed)
                    {
                        generationRates[harvester.resourceType] += harvester.currentGenerationRate;
                    }
                }
            }
        }

        public void RegisterHarvester(Harvester harvester)
        {
            if (harvester != null && !harvesters[harvester.resourceType].Contains(harvester))
            {
                harvesters[harvester.resourceType].Add(harvester);
                Debug.Log($"Registered {harvester.buildingName} for {harvester.resourceType} generation");
            }
        }

        public void UnregisterHarvester(Harvester harvester)
        {
            if (harvester != null && harvesters[harvester.resourceType].Contains(harvester))
            {
                harvesters[harvester.resourceType].Remove(harvester);
                Debug.Log($"Unregistered {harvester.buildingName} from {harvester.resourceType} generation");
            }
        }

        public float GetResourceAmount(ResourceType type)
        {
            return currentResources.ContainsKey(type) ? currentResources[type] : 0f;
        }

        public float GetResourceCapacity(ResourceType type)
        {
            return maxCapacities.ContainsKey(type) ? maxCapacities[type] : 0f;
        }

        public float GetResourcePercentage(ResourceType type)
        {
            if (maxCapacities[type] <= 0) return 0f;
            return currentResources[type] / maxCapacities[type];
        }

        public float GetGenerationRate(ResourceType type)
        {
            return generationRates.ContainsKey(type) ? generationRates[type] : 0f;
        }

        public bool ConsumeResource(ResourceType type, float amount)
        {
            if (currentResources[type] >= amount)
            {
                currentResources[type] -= amount;
                Debug.Log($"Consumed {amount} of {type}, remaining: {currentResources[type]:0.0}/{maxCapacities[type]:0.0}");
                return true;
            }
            else
            {
                Debug.LogWarning($"Not enough {type} to consume {amount}. Available: {currentResources[type]:0.0}/{maxCapacities[type]:0.0}");
                return false;
            }
        }

        public bool AddResource(ResourceType type, float amount)
        {
            if (currentResources[type] + amount <= maxCapacities[type])
            {
                currentResources[type] += amount;
                Debug.Log($"Added {amount} of {type}, now at {currentResources[type]:0.0}/{maxCapacities[type]:0.0}");
                return true;
            }
            else
            {
                float remainingSpace = maxCapacities[type] - currentResources[type];
                currentResources[type] = maxCapacities[type];
                Debug.Log($"Could only add {remainingSpace:0.0} of {type} due to capacity limit. Now at max: {currentResources[type]:0.0}");
                return false;
            }
        }

        public void SetGenerationRate(ResourceType type, float rate)
        {
            if (generationRates.ContainsKey(type))
            {
                generationRates[type] = rate;
                Debug.Log($"Set base generation rate for {type} to {rate:0.0}/s");
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
                    Debug.Log($"Capped {type} at new capacity limit: {capacity:0.0}");
                }

                Debug.Log($"Set max capacity for {type} to {capacity:0.0}");
            }
        }

        public void IncreaseMaxCapacity(ResourceType type, float amount)
        {
            if (maxCapacities.ContainsKey(type))
            {
                maxCapacities[type] += amount;
                Debug.Log($"Increased {type} capacity by {amount:0.0}, now: {maxCapacities[type]:0.0}");
            }
        }

        public bool CanAfford(Dictionary<ResourceType, float> costs)
        {
            foreach (var cost in costs)
            {
                if (currentResources[cost.Key] < cost.Value)
                {
                    return false;
                }
            }
            return true;
        }

        public bool ConsumeResources(Dictionary<ResourceType, float> costs)
        {
            // First check if we can afford all
            if (!CanAfford(costs))
            {
                Debug.LogWarning("Cannot afford all required resources");
                return false;
            }

            // Then consume all
            foreach (var cost in costs)
            {
                currentResources[cost.Key] -= cost.Value;
                Debug.Log($"Consumed {cost.Value:0.0} {cost.Key} for construction");
            }

            return true;
        }

        public Dictionary<ResourceType, float> GetAllResources()
        {
            return new Dictionary<ResourceType, float>(currentResources);
        }

        public Dictionary<ResourceType, float> GetAllCapacities()
        {
            return new Dictionary<ResourceType, float>(maxCapacities);
        }

        public Dictionary<ResourceType, float> GetAllGenerationRates()
        {
            return new Dictionary<ResourceType, float>(generationRates);
        }

        public string GetResourceSummary()
        {
            string summary = "Resource Summary:\n";
            foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
            {
                summary += $"{type}: {currentResources[type]:0.0}/{maxCapacities[type]:0.0} (+{generationRates[type]:0.0}/s)\n";
            }
            return summary;
        }
    }
}
