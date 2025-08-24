





using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

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

        // Object pooling for building prefabs
        private Dictionary<BuildingType, ObjectPool<GameObject>> buildingPools = new Dictionary<BuildingType, ObjectPool<GameObject>>();

        public void Initialize()
        {
            Debug.Log("BuildingManager initialized");
            // Start with a basic town hall at origin
            AddBuilding(BuildingType.TownHall, Vector3.zero);

            // Initialize object pools for each building type
            foreach (BuildingType type in System.Enum.GetValues(typeof(BuildingType)))
            {
                buildingPools[type] = new ObjectPool<GameObject>(CreateBuildingPrefab, OnGetBuildingFromPool, OnReleaseBuildingToPool, OnDestroyBuildingFromPool, true, 50, 100);
            }
        }

        private GameObject CreateBuildingPrefab()
        {
            // This will be called when creating a new pooled object
            return new GameObject("Pooled Building");
        }

        private void OnGetBuildingFromPool(GameObject building)
        {
            // Called when getting an building from the pool (activating it)
            building.SetActive(true);
        }

        private void OnReleaseBuildingToPool(GameObject building)
        {
            // Called when releasing an building to the pool (deactivating it)
            building.SetActive(false);
        }

        private void OnDestroyBuildingFromPool(GameObject building)
        {
            // Called when destroying an building from the pool
            Destroy(building);
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

        // New method to spawn buildings using pooling
        public GameObject SpawnBuilding(BuildingType type, Vector3 position)
        {
            if (buildingPools.TryGetValue(type, out ObjectPool<GameObject> pool))
            {
                GameObject buildingPrefab = GetBuildingPrefab(type);

                if (buildingPrefab != null)
                {
                    // Get building from pool instead of instantiating
                    GameObject buildingObj = pool.Get();
                    buildingObj.transform.position = position;
                    buildingObj.transform.rotation = Quaternion.identity;

                    // Set the prefab as parent to maintain visuals and components
                    buildingObj.transform.SetParent(buildingPrefab.transform, false);

                    // Copy components from prefab to pooled object
                    CopyComponentsFromPrefab(buildingObj, buildingPrefab);

                    return buildingObj;
                }
            }

            Debug.LogWarning($"No pool or prefab found for building type {type}");
            return null;
        }

        private GameObject GetBuildingPrefab(BuildingType type)
        {
            // This would be replaced with a proper prefab loading system
            switch (type)
            {
                case BuildingType.TownHall:
                    return Resources.Load<GameObject>("Prefabs/Buildings/TownHall");
                case BuildingType.Barracks:
                    return Resources.Load<GameObject>("Prefabs/Buildings/Barracks");
                case BuildingType.Factory:
                    return Resources.Load<GameObject>("Prefabs/Buildings/Factory");
                // Add more building prefab loading...
                default:
                    Debug.LogWarning($"Building type {type} not implemented");
                    return null;
            }
        }

        private void CopyComponentsFromPrefab(GameObject target, GameObject prefab)
        {
            // Copy essential components from prefab to ensure functionality
            Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                if (target.GetComponent(renderer.GetType()) == null)
                {
                    target.AddComponent(renderer.GetType());
                }
            }

            Collider[] colliders = prefab.GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                if (target.GetComponent(collider.GetType()) == null)
                {
                    target.AddComponent(collider.GetType());
                }
            }

            // Copy specific components needed for building functionality
            Building prefabBuilding = prefab.GetComponent<Building>();
            if (prefabBuilding != null && target.GetComponent<Building>() == null)
            {
                target.AddComponent<Building>().Initialize(prefabBuilding);
            }
        }

        public void DespawnBuilding(GameObject buildingObj)
        {
            // Find which pool this building belongs to and return it
            foreach (var poolEntry in buildingPools)
            {
                if (poolEntry.Value.Contains(buildingObj))
                {
                    poolEntry.Value.Release(buildingObj);
                    break;
                }
            }
        }
    }
}




