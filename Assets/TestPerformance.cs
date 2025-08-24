



using UnityEngine;
using System.Diagnostics;

namespace ChainEmpires
{
    public class TestPerformance : MonoBehaviour
    {
        [SerializeField] private TowerDefenseManager towerDefenseManager;
        [SerializeField] private int testDurationSeconds = 10;
        [SerializeField] private int enemiesPerWave = 50;
        [SerializeField] private int wavesToTest = 3;

        private float startTime;
        private int totalEnemiesSpawned;
        private long totalMemoryUsedBefore;
        private long totalMemoryUsedAfter;

        void Start()
        {
            Debug.Log("Starting performance test...");

            // Measure memory before
            GC.Collect();
            totalMemoryUsedBefore = GetTotalMemory();

            startTime = Time.time;
            totalEnemiesSpawned = 0;

            // Run the test
            StartCoroutine(RunPerformanceTest());
        }

        private IEnumerator RunPerformanceTest()
        {
            for (int wave = 1; wave <= wavesToTest; wave++)
            {
                Debug.Log($"Starting wave {wave} with {enemiesPerWave} enemies...");

                towerDefenseManager.StartWave(enemiesPerWave, false); // Don't end wave automatically

                yield return new WaitForSeconds(testDurationSeconds);
            }

            // Measure memory after
            GC.Collect();
            totalMemoryUsedAfter = GetTotalMemory();

            // Calculate results
            float elapsedTime = Time.time - startTime;
            Debug.Log($"Performance test completed in {elapsedTime:F2} seconds");
            Debug.Log($"Total enemies spawned: {totalEnemiesSpawned}");
            Debug.Log($"Memory used before: {totalMemoryUsedBefore / (1024 * 1024):F2} MB");
            Debug.Log($"Memory used after: {totalMemoryUsedAfter / (1024 * 1024):F2} MB");
            Debug.Log($"Memory increase: {(totalMemoryUsedAfter - totalMemoryUsedBefore) / (1024 * 1024):F2} MB");

            // Calculate FPS
            float averageFPS = totalEnemiesSpawned / elapsedTime;
            Debug.Log($"Average enemies per second: {averageFPS:F2}");

            // Test GC behavior
            long gcBefore = GC.CollectionCount(0);
            GC.Collect();
            long gcAfter = GC.CollectionCount(0);

            Debug.Log($"GC Collection Count (Gen 0): {gcAfter - gcBefore}");
        }

        private void OnEnemySpawned()
        {
            totalEnemiesSpawned++;
        }

        private long GetTotalMemory()
        {
            return Process.GetCurrentProcess().WorkingSet64;
        }
    }
}



