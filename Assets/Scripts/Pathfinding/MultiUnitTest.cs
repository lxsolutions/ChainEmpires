

using UnityEngine;
using System.Collections.Generic;
using ChainEmpires.Units;

namespace ChainEmpires.Pathfinding
{
    public class MultiUnitTest : MonoBehaviour
    {
        [Header("Test Settings")]
        public bool runTestOnStart = true;
        public int unitCount = 10;
        public float spawnRadius = 5f;
        public Vector3 targetPosition = new Vector3(20, 0, 20);
        public GameObject unitPrefab;
        
        [Header("Performance Metrics")]
        public float testStartTime;
        public float testDuration;
        public int pathsCompleted;
        public int collisionEvents;
        public float averagePathLength;
        public float minFPS = float.MaxValue;
        public float maxFPS = float.MinValue;
        public float avgFPS;
        
        [Header("Results")]
        public string testStatus = "Not Started";
        public bool testRunning = false;
        
        private List<Unit> testUnits = new List<Unit>();
        private int framesCounted;
        private float totalFPS;
        
        private void Start()
        {
            if (runTestOnStart)
            {
                StartMultiUnitTest();
            }
        }
        
        public void StartMultiUnitTest()
        {
            if (testRunning) return;
            
            testRunning = true;
            testStatus = "Initializing Test...";
            testStartTime = Time.time;
            pathsCompleted = 0;
            collisionEvents = 0;
            framesCounted = 0;
            totalFPS = 0f;
            minFPS = float.MaxValue;
            maxFPS = float.MinValue;
            
            Debug.Log($"Starting multi-unit test with {unitCount} units");
            
            // Clear any existing test units
            CleanupTestUnits();
            
            // Spawn test units
            SpawnTestUnits();
            
            // Start pathfinding for all units
            StartCoroutine(StartUnitMovement());
        }
        
        private void SpawnTestUnits()
        {
            if (unitPrefab == null)
            {
                Debug.LogError("Unit prefab not set for multi-unit test");
                testStatus = "FAIL: No unit prefab";
                testRunning = false;
                return;
            }
            
            for (int i = 0; i < unitCount; i++)
            {
                Vector3 spawnPos = transform.position + Random.insideUnitSphere * spawnRadius;
                spawnPos.y = 0;
                
                GameObject unitObj = Instantiate(unitPrefab, spawnPos, Quaternion.identity);
                unitObj.name = $"TestUnit_{i}";
                
                Unit unit = unitObj.GetComponent<Unit>();
                if (unit != null)
                {
                    unit.usePathfinding = true;
                    unit.OnPathComplete += HandlePathComplete;
                    unit.OnCollision += HandleUnitCollision;
                    testUnits.Add(unit);
                }
            }
            
            Debug.Log($"Spawned {testUnits.Count} test units");
        }
        
        private System.Collections.IEnumerator StartUnitMovement()
        {
            yield return new WaitForSeconds(1f); // Wait for units to initialize
            
            testStatus = "Moving Units to Target...";
            
            // Send all units to the target position
            foreach (Unit unit in testUnits)
            {
                unit.MoveTo(targetPosition);
                yield return new WaitForSeconds(0.1f); // Stagger movement starts
            }
            
            Debug.Log("All units started moving to target");
        }
        
        private void HandlePathComplete(Unit unit, bool success)
        {
            pathsCompleted++;
            
            if (success)
            {
                Debug.Log($"Unit {unit.name} reached target successfully");
            }
            else
            {
                Debug.LogWarning($"Unit {unit.name} failed to reach target");
            }
            
            // Check if all units have completed their paths
            if (pathsCompleted >= testUnits.Count)
            {
                CompleteTest();
            }
        }
        
        private void HandleUnitCollision(Unit unit1, Unit unit2)
        {
            collisionEvents++;
            Debug.Log($"Collision detected between {unit1.name} and {unit2.name}");
        }
        
        private void Update()
        {
            if (testRunning)
            {
                // Track FPS during test
                float currentFPS = 1f / Time.deltaTime;
                totalFPS += currentFPS;
                framesCounted++;
                
                minFPS = Mathf.Min(minFPS, currentFPS);
                maxFPS = Mathf.Max(maxFPS, currentFPS);
                
                // Check for test timeout (30 seconds max)
                if (Time.time - testStartTime > 30f)
                {
                    testStatus = "TIMEOUT: Test took too long";
                    CompleteTest();
                }
            }
        }
        
        private void CompleteTest()
        {
            testRunning = false;
            testDuration = Time.time - testStartTime;
            avgFPS = totalFPS / framesCounted;
            
            // Calculate average path length
            float totalPathLength = 0f;
            int validPaths = 0;
            
            foreach (Unit unit in testUnits)
            {
                if (unit.CurrentPath != null && unit.CurrentPath.Count > 0)
                {
                    totalPathLength += CalculatePathLength(unit.CurrentPath);
                    validPaths++;
                }
            }
            
            averagePathLength = validPaths > 0 ? totalPathLength / validPaths : 0f;
            
            testStatus = "Test Completed";
            
            Debug.Log($"Multi-unit test completed in {testDuration:F2} seconds");
            Debug.Log($"Paths completed: {pathsCompleted}/{testUnits.Count}");
            Debug.Log($"Collision events: {collisionEvents}");
            Debug.Log($"Average path length: {averagePathLength:F2}");
            Debug.Log($"FPS: Min={minFPS:F1}, Max={maxFPS:F1}, Avg={avgFPS:F1}");
            
            // Clean up after a delay
            Invoke("CleanupTestUnits", 5f);
        }
        
        private float CalculatePathLength(List<Vector3> path)
        {
            float length = 0f;
            for (int i = 0; i < path.Count - 1; i++)
            {
                length += Vector3.Distance(path[i], path[i + 1]);
            }
            return length;
        }
        
        private void CleanupTestUnits()
        {
            foreach (Unit unit in testUnits)
            {
                if (unit != null)
                {
                    unit.OnPathComplete -= HandlePathComplete;
                    unit.OnCollision -= HandleUnitCollision;
                    Destroy(unit.gameObject);
                }
            }
            
            testUnits.Clear();
            Debug.Log("Cleaned up test units");
        }
        
        private void OnGUI()
        {
            if (testRunning || testStatus != "Not Started")
            {
                GUILayout.BeginArea(new Rect(10, 320, 350, 200));
                GUILayout.Label("Multi-Unit Pathfinding Test");
                GUILayout.Label($"Status: {testStatus}");
                GUILayout.Label($"Units: {testUnits.Count}/{unitCount}");
                GUILayout.Label($"Completed: {pathsCompleted}/{testUnits.Count}");
                GUILayout.Label($"Collisions: {collisionEvents}");
                GUILayout.Label($"Duration: {testDuration:F1}s");
                GUILayout.Label($"FPS: {avgFPS:F1} (Min: {minFPS:F1}, Max: {maxFPS:F1})");
                
                if (!testRunning && GUILayout.Button("Start Test"))
                {
                    StartMultiUnitTest();
                }
                
                GUILayout.EndArea();
            }
        }
        
        private void OnDestroy()
        {
            CleanupTestUnits();
        }
    }
}

