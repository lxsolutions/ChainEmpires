





using UnityEngine;
using System.Collections.Generic;

namespace ChainEmpires
{
    public class ProgressionManager : IManager
    {
        // Era progression
        public enum Era
        {
            StoneAge,
            BronzeAge,
            IronAge,
            Medieval,
            Renaissance,
            Industrial,
            Modern,
            SpaceAge,
            Galactic
        }

        public Era CurrentEra { get; private set; }
        public int EraLevel { get; private set; } // Level within current era

        // Tech tree structure
        [System.Serializable]
        public class TechNode
        {
            public string Name;
            public string Description;
            public Era RequiredEra;
            public int RequiredEraLevel;
            public List<string> PrerequisiteTechs;
            public ResourceManager.ResourceType[] RequiredResources;
            public float[] RequiredAmounts;
            public BuildingManager.BuildingType RequiredBuilding;

            // Effects of this tech
            public float ResourceBonusMultiplier;
            public float UnitDamageBonus;
            public float UnitHealthBonus;
        }

        private List<TechNode> availableTechs = new List<TechNode>();
        private HashSet<string> researchedTechs = new HashSet<string>();

        public void Initialize()
        {
            CurrentEra = Era.StoneAge;
            EraLevel = 1;

            Debug.Log("ProgressionManager initialized");
            Debug.Log($"Starting in {CurrentEra} era, level {EraLevel}");

            // Initialize tech tree with some basic technologies
            InitializeTechTree();
        }

        public void Update()
        {
            // Progression updates can go here
        }

        public void OnDestroy()
        {
            // Clean up any pending operations
        }

        private void InitializeTechTree()
        {
            // Basic stone age technologies
            availableTechs.Add(new TechNode
            {
                Name = "Stone Tools",
                Description = "Basic tools for gathering resources more efficiently",
                RequiredEra = Era.StoneAge,
                RequiredEraLevel = 1,
                PrerequisiteTechs = new List<string>(),
                RequiredResources = new[] { ResourceManager.ResourceType.Minerals },
                RequiredAmounts = new float[] { 50f },
                RequiredBuilding = BuildingManager.BuildingType.TownHall,

                ResourceBonusMultiplier = 1.2f,
                UnitDamageBonus = 0f,
                UnitHealthBonus = 0f
            });

            availableTechs.Add(new TechNode
            {
                Name = "Basic Shelter",
                Description = "Improved housing for your units",
                RequiredEra = Era.StoneAge,
                RequiredEraLevel = 2,
                PrerequisiteTechs = new List<string> { "Stone Tools" },
                RequiredResources = new[] { ResourceManager.ResourceType.Wood, ResourceManager.ResourceType.Minerals },
                RequiredAmounts = new float[] { 30f, 70f },
                RequiredBuilding = BuildingManager.BuildingType.TownHall,

                UnitHealthBonus = 1.1f,
                ResourceBonusMultiplier = 0f
            });

            // Bronze age technologies
            availableTechs.Add(new TechNode
            {
                Name = "Bronze Working",
                Description = "Discover bronze for better tools and weapons",
                RequiredEra = Era.BronzeAge,
                RequiredEraLevel = 1,
                PrerequisiteTechs = new List<string>(),
                RequiredResources = new[] { ResourceManager.ResourceType.Metal, ResourceManager.ResourceType.Crystal },
                RequiredAmounts = new float[] { 100f, 50f },
                RequiredBuilding = BuildingManager.BuildingType.Factory,

                UnitDamageBonus = 1.3f,
                ResourceBonusMultiplier = 1.1f
            });

            // Add more technologies as needed...
        }

        public List<TechNode> GetAvailableTechs()
        {
            List<TechNode> available = new List<TechNode>();

            foreach (var tech in availableTechs)
            {
                if (CanResearchTech(tech))
                {
                    available.Add(tech);
                }
            }

            return available;
        }

        public bool CanResearchTech(TechNode tech)
        {
            // Check era requirements
            if (CurrentEra != tech.RequiredEra || EraLevel < tech.RequiredEraLevel)
            {
                Debug.Log($"Cannot research {tech.Name}: Requires {tech.RequiredEra} era level {tech.RequiredEraLevel}");
                return false;
            }

            // Check prerequisite technologies
            foreach (var prereq in tech.PrerequisiteTechs)
            {
                if (!researchedTechs.Contains(prereq))
                {
                    Debug.Log($"Cannot research {tech.Name}: Missing prerequisite {prereq}");
                    return false;
                }
            }

            // Check required building
            if (tech.RequiredBuilding != BuildingManager.BuildingType.TownHall)
            {
                var buildingManager = GameManager.Instance.BuildingManager;

                bool hasRequiredBuilding = false;
                foreach (var building in buildingManager.GetAllBuildings())
                {
                    if (building.Type == tech.RequiredBuilding)
                    {
                        hasRequiredBuilding = true;
                        break;
                    }
                }

                if (!hasRequiredBuilding)
                {
                    Debug.Log($"Cannot research {tech.Name}: Requires {tech.RequiredBuilding} building");
                    return false;
                }
            }

            // Check resource requirements
            var resourceManager = GameManager.Instance.ResourceManager;

            for (int i = 0; i < tech.RequiredResources.Length; i++)
            {
                if (resourceManager.GetResourceAmount(tech.RequiredResources[i]) < tech.RequiredAmounts[i])
                {
                    Debug.Log($"Cannot research {tech.Name}: Not enough resources");
                    return false;
                }
            }

            return true;
        }

        public bool ResearchTech(TechNode tech)
        {
            if (!CanResearchTech(tech))
            {
                Debug.LogWarning($"Cannot research {tech.Name} - requirements not met");
                return false;
            }

            // Consume resources
            var resourceManager = GameManager.Instance.ResourceManager;

            for (int i = 0; i < tech.RequiredResources.Length; i++)
            {
                resourceManager.ConsumeResource(tech.RequiredResources[i], tech.RequiredAmounts[i]);
            }

            // Apply tech effects
            ApplyTechEffects(tech);

            // Mark as researched
            researchedTechs.Add(tech.Name);
            Debug.Log($"Researched {tech.Name}");

            return true;
        }

        private void ApplyTechEffects(TechNode tech)
        {
            var resourceManager = GameManager.Instance.ResourceManager;

            if (tech.ResourceBonusMultiplier > 0f)
            {
                // Apply resource bonus to all resources
                foreach (ResourceManager.ResourceType type in System.Enum.GetValues(typeof(ResourceManager.ResourceType)))
                {
                    float currentRate = resourceManager.GetGenerationRate(type);
                    resourceManager.SetGenerationRate(type, currentRate * tech.ResourceBonusMultiplier);
                }
            }

            if (tech.UnitDamageBonus > 0f)
            {
                // Apply unit damage bonus
                Debug.Log($"All units now have +{tech.UnitDamageBonus * 100}% damage");
            }

            if (tech.UnitHealthBonus > 0f)
            {
                // Apply unit health bonus
                Debug.Log($"All units now have +{tech.UnitHealthBonus * 100}% health");
            }
        }

        public void AdvanceEra()
        {
            Era nextEra = CurrentEra;
            int nextLevel = EraLevel;

            switch (CurrentEra)
            {
                case Era.StoneAge:
                    if (EraLevel >= 5) nextEra = Era.BronzeAge;
                    else nextLevel++;
                    break;
                case Era.BronzeAge:
                    if (EraLevel >= 5) nextEra = Era.IronAge;
                    else nextLevel++;
                    break;
                case Era.IronAge:
                    if (EraLevel >= 5) nextEra = Era.Medieval;
                    else nextLevel++;
                    break;
                // Add more era progression logic...
            }

            if (nextEra != CurrentEra || nextLevel != EraLevel)
            {
                CurrentEra = nextEra;
                EraLevel = nextLevel;

                Debug.Log($"Advanced to {CurrentEra} era, level {EraLevel}");
            }
        }
    }
}





