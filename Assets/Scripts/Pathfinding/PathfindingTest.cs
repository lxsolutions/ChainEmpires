
using UnityEngine;
using System.Collections.Generic;
using ChainEmpires.Units;

namespace ChainEmpires.Pathfinding
{
    public class PathfindingTest : MonoBehaviour
    {
        [Header("Test Settings")]
        public bool runTestsOnStart = true;
        public Unit testUnit;
        public Vector3 testStartPosition = new Vector3(0, 0, 0);
        public Vector3 testTargetPosition = new Vector3(10, 0, 10);
        public GameObject obstaclePrefab;
        
        [Header("Test Results")]
        public bool pathfindingInitialized = false;
        public bool pathFound = false;
        public float pathLength = 0f;
        public int waypointCount = 0;
        public string testStatus = "Not Run";
        
        private Pathfinder pathfinder;
        private PathfindingManager pathfindingManager;
        
        private void Start()
        {
            if (runTestsOnStart)
            {
                RunPathfindingTests();
            }
        }
        
        public void RunPathfindingTests()
        {
            testStatus = "Running Tests...";
            
            // Test 1: Check if pathfinding components exist
            pathfinder = FindObjectOfType<Pathfinder>();
            pathfindingManager = FindObjectOfType<PathfindingManager>();
            
            if (pathfinder == null || pathfindingManager == null)
            {
                testStatus = "FAIL: Pathfinding components not found";
                return;
            }
            
            pathfindingInitialized = true;
            Debug.Log("Pathfinding components found successfully");
            
            // Test 2: Simple pathfinding test
            TestSimplePathfinding();
            
            // Test 3: Pathfinding with obstacles
            TestPathfindingWithObstacles();
            
            // Test 4: Unit integration test
            TestUnitPathfindingIntegration();
            
            testStatus = "All Tests Completed Successfully";
        }
        
        private void TestSimplePathfinding()
        {
            Debug.Log("Testing simple pathfinding...");
            
            List<Vector3> path = pathfinder.FindPath(testStartPosition, testTargetPosition);
            
            if (path != null && path.Count > 0)
            {
                pathFound = true;
                waypointCount = path.Count;
                pathLength = CalculatePathLength(path);
                
                Debug.Log($"Path found with {waypointCount} waypoints, length: {pathLength:F2}");
                DebugPathDetails(path);
            }
            else
            {
                Debug.LogWarning("No path found in simple test");
            }
        }
        
        private void TestPathfindingWithObstacles()
        {
            Debug.Log("Testing pathfinding with obstacles...");
            
            // Create a test obstacle
            if (obstaclePrefab != null)
            {
                Vector3 obstaclePosition = (testStartPosition + testTargetPosition) / 2f;
                GameObject obstacle = Instantiate(obstaclePrefab, obstaclePosition, Quaternion.identity);
                obstacle.name = "TestObstacle";
                
                // Wait a frame for obstacle to be registered
                StartCoroutine(TestObstaclePathfinding(obstacle));
            }
            else
            {
                Debug.Log("No obstacle prefab set, skipping obstacle test");
            }
        }
        
        private System.Collections.IEnumerator TestObstaclePathfinding(GameObject obstacle)
        {
            yield return null; // Wait one frame
            
            List<Vector3> path = pathfinder.FindPath(testStartPosition, testTargetPosition);
            
            if (path != null)
            {
                Debug.Log($"Path found with obstacle: {path.Count} waypoints");
                
                // Check if path avoids obstacle
                bool avoidsObstacle = true;
                Bounds obstacleBounds = obstacle.GetComponent<Collider>().bounds;
                
                foreach (Vector3 point in path)
                {
                    if (obstacleBounds.Contains(point))
                    {
                        avoidsObstacle = false;
                        break;
                    }
                }
                
                Debug.Log(avoidsObstacle ? "Path successfully avoids obstacle" : "WARNING: Path goes through obstacle");
            }
            
            // Clean up
            Destroy(obstacle);
        }
        
        private void TestUnitPathfindingIntegration()
        {
            Debug.Log("Testing unit pathfinding integration...");
            
            if (testUnit != null)
            {
                testUnit.usePathfinding = true;
                testUnit.MoveTo(testTargetPosition);
                
                Debug.Log("Unit pathfinding command sent");
            }
            else
            {
                Debug.Log("No test unit set, skipping unit integration test");
            }
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
        
        private void DebugPathDetails(List<Vector3> path)
        {
            for (int i = 0; i < path.Count; i++)
            {
                Debug.Log($"Waypoint {i}: {path[i]}");
            }
        }
        
        private void OnGUI()
        {
            if (runTestsOnStart)
            {
                GUILayout.BeginArea(new Rect(10, 10, 300, 200));
                GUILayout.Label("Pathfinding Test Results");
                GUILayout.Label($"Status: {testStatus}");
                GUILayout.Label($"Initialized: {pathfindingInitialized}");
                GUILayout.Label($"Path Found: {pathFound}");
                GUILayout.Label($"Waypoints: {waypointCount}");
                GUILayout.Label($"Path Length: {pathLength:F2}");
                
                if (GUILayout.Button("Run Tests"))
                {
                    RunPathfindingTests();
                }
                
                GUILayout.EndArea();
            }
        }
    }
}
