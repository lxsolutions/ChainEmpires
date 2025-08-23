










using UnityEngine;

namespace ChainEmpires
{
    public class TestTowerDefenseSetup : MonoBehaviour
    {
        [Header("Test Setup")]
        public GameObject testEnemyPrefab;
        public Transform[] spawnPoints;
        public Transform towerBuildPoint;

        void Start()
        {
            Debug.Log("Test Tower Defense setup initialized");

            // Spawn a test enemy
            if (spawnPoints.Length > 0 && testEnemyPrefab != null)
            {
                Vector3 spawnPosition = spawnPoints[0].position;
                GameObject enemyObj = Instantiate(testEnemyPrefab, spawnPosition, Quaternion.identity);
                EnemyAI enemyAI = enemyObj.GetComponent<EnemyAI>();

                if (enemyAI != null)
                {
                    // Initialize with test data
                    var enemyData = new EnemyManager.EnemyData
                    {
                        Type = EnemyManager.EnemyType.Barbarian,
                        Health = 100f,
                        Damage = 10f,
                        Speed = 2f
                    };

                    enemyAI.Initialize(enemyData);

                    // Set target to a dummy building
                    GameObject dummyBuilding = new GameObject("DummyBuilding");
                    dummyBuilding.transform.position = Vector3.zero;
                    dummyBuilding.tag = "PlayerBuilding";
                    enemyAI.SetTarget(dummyBuilding.transform);
                }
            }

            // Build a test tower
            if (towerBuildPoint != null)
            {
                TowerDefenseManager manager = FindObjectOfType<TowerDefenseManager>();

                if (manager != null && manager.towerConfigs.Count > 0)
                {
                    TowerDefenseManager.TowerConfig config = manager.towerConfigs[0];
                    manager.BuildTower(config, towerBuildPoint.position);
                }
            }

            Debug.Log("Test Tower Defense setup complete");
        }
    }
}





