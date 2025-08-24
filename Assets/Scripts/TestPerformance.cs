
using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class TestPerformance : MonoBehaviour
{
    public int enemyCount = 300; // Number of enemies to spawn for stress test
    public int waveCount = 5; // Number of waves to simulate
    public float timeBetweenWaves = 10f; // Time between waves

    private TowerDefenseManager towerDefenseManager;
    private Stopwatch stopwatch;

    void Start()
    {
        Debug.Log("Performance test started");
        stopwatch = new Stopwatch();
        stopwatch.Start();

        towerDefenseManager = FindObjectOfType<TowerDefenseManager>();

        if (towerDefenseManager == null)
        {
            Debug.LogError("TowerDefenseManager not found in scene!");
            return;
        }

        StartCoroutine(RunPerformanceTest());
    }

    private IEnumerator RunPerformanceTest()
    {
        for (int wave = 1; wave <= waveCount; wave++)
        {
            Debug.Log($"Starting wave {wave} with {enemyCount} enemies...");

            // Spawn all enemies at once to test pooling
            for (int i = 0; i < enemyCount; i++)
            {
                towerDefenseManager.SpawnEnemy(EnemyManager.EnemyType.Barbarian);
            }

            yield return new WaitForSeconds(timeBetweenWaves);
        }

        stopwatch.Stop();
        Debug.Log($"Performance test completed in {stopwatch.Elapsed.TotalSeconds} seconds");

        // Log GC stats
        long totalMemoryUsed = 0;
        int collectionCount = 0;

        foreach (var info in System.GC.GetGCMemoryInfo(1))
        {
            totalMemoryUsed += info.TotalAvailableMemoryBytes;
            collectionCount++;
        }

        Debug.Log($"Total memory used: {totalMemoryUsed} bytes, GC collections: {collectionCount}");
    }
}
