


using UnityEngine;

namespace ChainEmpires.Test
{
    public class SystemTestController : MonoBehaviour
    {
        [Header("Test Settings")]
        public bool autoStartSession = true;
        public bool spawnTestBuildings = true;
        public bool spawnTestUnits = true;
        
        [Header("Test Buildings")]
        public GameObject testHarvesterPrefab;
        public GameObject testBarracksPrefab;
        public Vector3[] buildingSpawnPositions;
        
        [Header("Test Units")]
        public GameObject testMeleeUnitPrefab;
        public GameObject testRangedUnitPrefab;
        public Vector3[] unitSpawnPositions;
        
        private void Start()
        {
            Debug.Log("=== Chain Empires System Test ===");
            
            // Test Resource Manager
            TestResourceManager();
            
            // Test Building Manager
            if (spawnTestBuildings)
            {
                TestBuildingManager();
            }
            
            // Test Unit Manager
            if (spawnTestUnits)
            {
                TestUnitManager();
            }
            
            // Test Session Manager
            if (autoStartSession && GameManager.Instance?.SessionManager != null)
            {
                GameManager.Instance.SessionManager.StartSession();
                Debug.Log("Session started automatically for testing");
            }
            
            Debug.Log("=== System Test Complete ===");
        }
        
        private void TestResourceManager()
        {
            if (GameManager.Instance?.ResourceManager != null)
            {
                ResourceManager rm = GameManager.Instance.ResourceManager;
                Debug.Log("Resource Manager Test:");
                Debug.Log(rm.GetResourceSummary());
                
                // Test resource consumption
                bool canAfford = rm.CanAfford(new System.Collections.Generic.Dictionary<ResourceManager.ResourceType, float>
                {
                    { ResourceManager.ResourceType.Minerals, 100f },
                    { ResourceManager.ResourceType.Energy, 50f }
                });
                
                Debug.Log($"Can afford 100 Minerals + 50 Energy: {canAfford}");
            }
            else
            {
                Debug.LogWarning("Resource Manager not found!");
            }
        }
        
        private void TestBuildingManager()
        {
            if (GameManager.Instance?.BuildingManager != null && testHarvesterPrefab != null && testBarracksPrefab != null)
            {
                BuildingManager bm = GameManager.Instance.BuildingManager;
                Debug.Log("Building Manager Test:");
                
                // Spawn test buildings
                if (buildingSpawnPositions.Length >= 2)
                {
                    GameObject harvester = Instantiate(testHarvesterPrefab, buildingSpawnPositions[0], Quaternion.identity);
                    GameObject barracks = Instantiate(testBarracksPrefab, buildingSpawnPositions[1], Quaternion.identity);
                    
                    Debug.Log($"Spawned test buildings: {harvester.name}, {barracks.name}");
                    Debug.Log($"Total buildings: {bm.GetAllBuildings()?.Count ?? 0}");
                }
            }
            else
            {
                Debug.LogWarning("Building Manager or prefabs not found!");
            }
        }
        
        private void TestUnitManager()
        {
            if (GameManager.Instance?.UnitManager != null && testMeleeUnitPrefab != null && testRangedUnitPrefab != null)
            {
                UnitManager um = GameManager.Instance.UnitManager;
                Debug.Log("Unit Manager Test:");
                
                // Spawn test units
                if (unitSpawnPositions.Length >= 2)
                {
                    GameObject meleeUnit = Instantiate(testMeleeUnitPrefab, unitSpawnPositions[0], Quaternion.identity);
                    GameObject rangedUnit = Instantiate(testRangedUnitPrefab, unitSpawnPositions[1], Quaternion.identity);
                    
                    Debug.Log($"Spawned test units: {meleeUnit.name}, {rangedUnit.name}");
                    Debug.Log($"Total units: {um.GetAllUnits()?.Count ?? 0}");
                }
            }
            else
            {
                Debug.LogWarning("Unit Manager or prefabs not found!");
            }
        }
        
        private void Update()
        {
            // Quick test commands
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (GameManager.Instance?.SessionManager != null)
                {
                    if (GameManager.Instance.SessionManager.currentState == SessionManager.SessionState.RaidAvailable)
                    {
                        GameManager.Instance.SessionManager.StartRaid();
                        Debug.Log("Manual raid start triggered");
                    }
                }
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (GameManager.Instance?.SessionManager != null)
                {
                    GameManager.Instance.SessionManager.CompleteRaid(true);
                    Debug.Log("Manual raid completion triggered");
                }
            }
            
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (GameManager.Instance?.SessionManager != null)
                {
                    GameManager.Instance.SessionManager.StartSession();
                    Debug.Log("Manual session start triggered");
                }
            }
        }
    }
}


