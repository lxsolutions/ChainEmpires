





using UnityEngine;
using System.Collections.Generic;

namespace ChainEmpires
{
    public class BuildingManager : IManager
    {
        // Building types
        public enum BuildingType
        {
            TownHall,
            Barracks,
            Factory,
            PowerPlant,
            Mine,
            Farm,
            DefenseTower,
            Wall,
            ResearchLab,
            Storage,
            Marketplace
        }

        // Building data structure
        private class BuildingData
        {
            public BuildingType Type;
            public Vector3 Position;
            public int Level;
            public float Progress; // 0-1 for construction/upgrade progress
            public bool IsUnderConstruction;
            public float ConstructionTimeRemaining;

            public BuildingData(BuildingType type, Vector3 position)
            {
                Type = type;
                Position = position;
                Level = 1;
                Progress = 0f;
                IsUnderConstruction = false;
                ConstructionTimeRemaining = 0f;
            }
        }

        // List of all buildings
        private List<BuildingData> buildings = new List<BuildingData>();

        public void Initialize()
        {
            Debug.Log("BuildingManager initialized");
            // Start with a basic town hall at origin
            AddBuilding(BuildingType.TownHall, Vector3.zero);
        }

        public void Update()
        {
            // Update building construction progress
            foreach (var building in buildings)
            {
                if (building.IsUnderConstruction)
                {
                    building.ConstructionTimeRemaining -= Time.deltaTime;

                    if (building.ConstructionTimeRemaining <= 0f)
                    {
                        CompleteBuildingConstruction(building);
                    }
                }
            }
        }

        public void OnDestroy()
        {
            // Clean up any pending operations
        }

        public void AddBuilding(BuildingType type, Vector3 position)
        {
            BuildingData newBuilding = new BuildingData(type, position);
            buildings.Add(newBuilding);

            Debug.Log($"Added {type} building at {position}");

            // Start construction if needed
            StartBuildingConstruction(newBuilding);
        }

        private void StartBuildingConstruction(BuildingData building)
        {
            if (building.IsUnderConstruction) return;

            building.IsUnderConstruction = true;
            building.Progress = 0f;

            // Set construction time based on building type and level
            float baseConstructionTime = GetBaseConstructionTime(building.Type);
            float constructionTime = baseConstructionTime * Mathf.Pow(1.2f, building.Level - 1); // 20% increase per level

            building.ConstructionTimeRemaining = constructionTime;
            Debug.Log($"Started construction of {building.Type} Level {building.Level}. Time remaining: {constructionTime}s");
        }

        private void CompleteBuildingConstruction(BuildingData building)
        {
            building.IsUnderConstruction = false;
            building.Progress = 1f;

            Debug.Log($"Completed construction of {building.Type} Level {building.Level} at {building.Position}");
        }

        public bool UpgradeBuilding(BuildingType type, Vector3 position)
        {
            BuildingData building = FindBuilding(type, position);

            if (building == null)
            {
                Debug.LogWarning($"No {type} building found at {position}");
                return false;
            }

            // Check if already upgrading
            if (building.IsUnderConstruction)
            {
                Debug.LogWarning($"{type} building is already under construction/upgrade");
                return false;
            }

            // Check resource requirements
            ResourceManager resourceManager = GameManager.Instance.ResourceManager;

            float mineralCost = 100f * building.Level; // Example cost scaling
            if (!resourceManager.ConsumeResource(ResourceManager.ResourceType.Minerals, mineralCost))
            {
                Debug.LogWarning($"Not enough resources to upgrade {type} building");
                return false;
            }

            building.Level++;
            StartBuildingConstruction(building);

            Debug.Log($"Started upgrade of {type} building to Level {building.Level}");
            return true;
        }

        private BuildingData FindBuilding(BuildingType type, Vector3 position)
        {
            foreach (var building in buildings)
            {
                if (building.Type == type && building.Position == position)
                {
                    return building;
                }
            }
            return null;
        }

        private float GetBaseConstructionTime(BuildingType type)
        {
            // Base construction times for each building type
            switch (type)
            {
                case BuildingType.TownHall: return 60f; // 1 minute
                case BuildingType.Barracks: return 30f; // 30 seconds
                case BuildingType.Factory: return 45f; // 45 seconds
                case BuildingType.PowerPlant: return 90f; // 1.5 minutes
                case BuildingType.Mine: return 20f; // 20 seconds
                case BuildingType.Farm: return 15f; // 15 seconds
                case BuildingType.DefenseTower: return 30f; // 30 seconds
                case BuildingType.Wall: return 10f; // 10 seconds per segment
                case BuildingType.ResearchLab: return 120f; // 2 minutes
                case BuildingType.Storage: return 45f; // 45 seconds
                case BuildingType.Marketplace: return 75f; // 1.25 minutes
                default: return 30f; // Default construction time
            }
        }

        public List<BuildingData> GetAllBuildings()
        {
            return buildings;
        }
    }
}




