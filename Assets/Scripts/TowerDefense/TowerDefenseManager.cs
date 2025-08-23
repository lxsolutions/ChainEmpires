











using UnityEngine;
using System.Collections.Generic;

namespace ChainEmpires
{
    public class TowerDefenseManager : MonoBehaviour
    {
        [Header("Tower Defense Settings")]
        public List<TowerConfig> towerConfigs; // Available towers
        public Transform[] buildPoints; // Where players can build towers

        [Header("Wave System")]
        public float timeBetweenWaves = 30f;
        public int maxWaves = 10;

        private List<EnemyManager.EnemyType> enemyTypes = new List<EnemyManager.EnemyType>
        {
            EnemyManager.EnemyType.Barbarian,
            EnemyManager.EnemyType.Skeleton,
            EnemyManager.EnemyType.Alien
        };

        private int currentWave = 1;
        private bool waveActive = false;

        void Start()
        {
            if (towerConfigs.Count == 0)
            {
                Debug.LogWarning("No tower configurations found. Please add tower configs.");
                return;
            }

            // Start the wave cycle
            StartCoroutine(WaveCycle());
        }

        private System.Collections.IEnumerator WaveCycle()
        {
            while (currentWave <= maxWaves)
            {
                yield return new WaitForSeconds(timeBetweenWaves);

                if (!waveActive)
                {
                    StartNewWave();
                }
            }

            Debug.Log("Tower defense campaign completed!");
        }

        private void StartNewWave()
        {
            waveActive = true;
            Debug.Log($"Starting Wave {currentWave}");

            // Spawn enemies based on wave number
            int enemyCount = currentWave * 2; // Scales with wave number

            foreach (var enemyType in enemyTypes)
            {
                for (int i = 0; i < enemyCount / enemyTypes.Count; i++)
                {
                    SpawnEnemy(enemyType);
                }
            }

            currentWave++;
        }

        private void SpawnEnemy(EnemyManager.EnemyType type)
        {
            EnemyManager enemyManager = FindObjectOfType<EnemyManager>();

            if (enemyManager != null)
            {
                // Create a simple enemy data object
                var enemyData = new EnemyManager.EnemyData
                {
                    Type = type,
                    Health = 100f * currentWave,
                    Damage = 10f * currentWave,
                    Speed = 2f + (currentWave / 5f)
                };

                // Spawn the enemy using the enemy manager
                GameObject enemyPrefab = enemyManager.GetEnemyPrefab(type);

                if (enemyPrefab != null && buildPoints.Length > 0)
                {
                    Vector3 spawnPosition = buildPoints[Random.Range(0, buildPoints.Length)].position;
                    GameObject enemyObj = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                    EnemyAI enemyAI = enemyObj.GetComponent<EnemyAI>();

                    if (enemyAI != null)
                    {
                        enemyAI.Initialize(enemyData);

                        // Set target to a random player building
                        GameObject[] playerBuildings = GameObject.FindGameObjectsWithTag("PlayerBuilding");
                        if (playerBuildings.Length > 0)
                        {
                            int targetIndex = Random.Range(0, playerBuildings.Length);
                            enemyAI.SetTarget(playerBuildings[targetIndex].transform);
                        }
                    }
                }
            }
        }

        public void OnEnemyDefeated()
        {
            // Check if wave is completed
            GameObject[] activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");

            if (activeEnemies.Length == 0)
            {
                Debug.Log($"Wave {currentWave - 1} completed!");
                waveActive = false;
            }
        }

        public void BuildTower(TowerConfig config, Vector3 position)
        {
            if (towerConfigs.Contains(config))
            {
                GameObject towerPrefab = GetTowerPrefab(config);

                if (towerPrefab != null)
                {
                    Instantiate(towerPrefab, position, Quaternion.identity);
                    Debug.Log($"Built {config.TowerName} at {position}");
                }
            }
        }

        private GameObject GetTowerPrefab(TowerConfig config)
        {
            // This would be replaced with a proper prefab loading system
            switch (config.TowerType)
            {
                case TowerType.Arrow:
                    return Resources.Load<GameObject>("Prefabs/Towers/ArrowTower");
                case TowerType.Cannon:
                    return Resources.Load<GameObject>("Prefabs/Towers/CannonTower");
                case TowerType.Laser:
                    return Resources.Load<GameObject>("Prefabs/Towers/LaserTower");
                default:
                    Debug.LogWarning($"Tower type {config.TowerType} not implemented");
                    return null;
            }
        }

        [System.Serializable]
        public class TowerConfig
        {
            public string TowerName;
            public TowerType TowerType;
            public float Range;
            public float Damage;
            public float AttackSpeed;
            public int Cost;
        }

        public enum TowerType
        {
            Arrow,
            Cannon,
            Laser
        }
    }
}






