





using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;
using ChainEmpires.Buildings;

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

        // Active building instances
        private List<Building> activeBuildings = new List<Building>();
        
        // Object pooling for building prefabs
        private Dictionary<BuildingType, ObjectPool<GameObject>> buildingPools = new Dictionary<BuildingType, ObjectPool<GameObject>>();

        public void Initialize()
        {
            Debug.Log("BuildingManager initialized");
            
            // Initialize object pools for each building type
            foreach (BuildingType type in System.Enum.GetValues(typeof(BuildingType)))
            {
                buildingPools[type] = new ObjectPool<GameObject>(
                    () => CreateBuildingPrefab(type),
                    OnGetBuildingFromPool,
                    OnReleaseBuildingToPool,
                    OnDestroyBuildingFromPool,
                    true, 50, 100
                );
            }
            
            Debug.Log($"Initialized building pools for {buildingPools.Count} building types");
        }

        private GameObject CreateBuildingPrefab(BuildingType type)
        {
            // Create a new GameObject with the appropriate building component
            GameObject buildingObj = new GameObject($"{type}_Prefab");
            
            // Add the appropriate building component based on type
            switch (type)
            {
                case BuildingType.Mine:
                    buildingObj.AddComponent<Harvester>();
                    break;
                case BuildingType.Barracks:
                    buildingObj.AddComponent<Barracks>();
                    break;
                case BuildingType.Wall:
                    buildingObj.AddComponent<Wall>();
                    break;
                case BuildingType.DefenseTower:
                    buildingObj.AddComponent<DefenseTower>();
                    break;
                default:
                    // For other building types, add a generic Building component
                    Building building = buildingObj.AddComponent<Building>();
                    building.buildingType = type;
                    building.buildingName = type.ToString();
                    break;
            }
            
            return buildingObj;
        }

        private void OnGetBuildingFromPool(GameObject building)
        {
            // Called when getting a building from the pool (activating it)
            building.SetActive(true);
        }

        private void OnReleaseBuildingToPool(GameObject building)
        {
            // Called when releasing a building to the pool (deactivating it)
            building.SetActive(false);
        }

        private void OnDestroyBuildingFromPool(GameObject building)
        {
            // Called when destroying a building from the pool
            Destroy(building);
        }

        public void Update()
        {
            // Building updates are handled by individual Building components
        }

        public void OnDestroy()
        {
            // Clean up all building pools
            foreach (var pool in buildingPools.Values)
            {
                pool.Clear();
            }
            buildingPools.Clear();
            activeBuildings.Clear();
        }

        public Building CreateBuilding(BuildingType type, Vector3 position, Quaternion rotation)
        {
            if (buildingPools.TryGetValue(type, out ObjectPool<GameObject> pool))
            {
                GameObject buildingObj = pool.Get();
                buildingObj.transform.position = position;
                buildingObj.transform.rotation = rotation;
                
                Building building = buildingObj.GetComponent<Building>();
                if (building != null)
                {
                    activeBuildings.Add(building);
                    building.StartConstruction();
                    return building;
                }
            }
            
            Debug.LogWarning($"Failed to create building of type {type}");
            return null;
        }

        public bool DestroyBuilding(Building building)
        {
            if (building == null) return false;
            
            if (activeBuildings.Contains(building))
            {
                activeBuildings.Remove(building);
                
                // Return to appropriate pool
                if (buildingPools.TryGetValue(building.buildingType, out ObjectPool<GameObject> pool))
                {
                    pool.Release(building.gameObject);
                    return true;
                }
            }
            
            return false;
        }

        public List<Building> GetAllBuildings()
        {
            return new List<Building>(activeBuildings);
        }

        public List<Building> GetBuildingsOfType(BuildingType type)
        {
            return activeBuildings.FindAll(b => b.buildingType == type);
        }

        public Building GetBuildingAtPosition(Vector3 position, float radius = 1f)
        {
            foreach (var building in activeBuildings)
            {
                if (Vector3.Distance(building.transform.position, position) <= radius)
                {
                    return building;
                }
            }
            return null;
        }

        // Helper methods for specific building types
        public Harvester CreateHarvester(Vector3 position, ResourceManager.ResourceType resourceType = ResourceManager.ResourceType.Minerals)
        {
            Building building = CreateBuilding(BuildingType.Mine, position, Quaternion.identity);
            if (building is Harvester harvester)
            {
                harvester.resourceType = resourceType;
                return harvester;
            }
            return null;
        }

        public Barracks CreateBarracks(Vector3 position)
        {
            Building building = CreateBuilding(BuildingType.Barracks, position, Quaternion.identity);
            return building as Barracks;
        }

        public Wall CreateWall(Vector3 position)
        {
            Building building = CreateBuilding(BuildingType.Wall, position, Quaternion.identity);
            return building as Wall;
        }

        public DefenseTower CreateDefenseTower(Vector3 position)
        {
            Building building = CreateBuilding(BuildingType.DefenseTower, position, Quaternion.identity);
            return building as DefenseTower;
        }

        // Resource cost helper methods
        public float GetConstructionCost(BuildingType type, int level = 1)
        {
            // Default costs - these would be overridden by actual building components
            switch (type)
            {
                case BuildingType.Mine: return 150f * level;
                case BuildingType.Barracks: return 200f * level;
                case BuildingType.Wall: return 80f * level;
                case BuildingType.DefenseTower: return 250f * level;
                case BuildingType.TownHall: return 500f * level;
                default: return 100f * level;
            }
        }

        public float GetConstructionTime(BuildingType type, int level = 1)
        {
            // Base construction times
            switch (type)
            {
                case BuildingType.TownHall: return 60f * Mathf.Pow(1.2f, level - 1);
                case BuildingType.Barracks: return 30f * Mathf.Pow(1.2f, level - 1);
                case BuildingType.Factory: return 45f * Mathf.Pow(1.2f, level - 1);
                case BuildingType.PowerPlant: return 90f * Mathf.Pow(1.2f, level - 1);
                case BuildingType.Mine: return 20f * Mathf.Pow(1.2f, level - 1);
                case BuildingType.Farm: return 15f * Mathf.Pow(1.2f, level - 1);
                case BuildingType.DefenseTower: return 30f * Mathf.Pow(1.2f, level - 1);
                case BuildingType.Wall: return 10f * Mathf.Pow(1.2f, level - 1);
                case BuildingType.ResearchLab: return 120f * Mathf.Pow(1.2f, level - 1);
                case BuildingType.Storage: return 45f * Mathf.Pow(1.2f, level - 1);
                case BuildingType.Marketplace: return 75f * Mathf.Pow(1.2f, level - 1);
                default: return 30f * Mathf.Pow(1.2f, level - 1);
            }
        }
    }
}




