











using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

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

        // Object pooling for enemies and towers
        private ObjectPool<GameObject> enemyPool;
        private ObjectPool<GameObject> towerPool;

        private int currentWave = 1;
        private bool waveActive = false;

        void Start()
        {
            if (towerConfigs.Count == 0)
            {
                Debug.LogWarning("No tower configurations found. Please add tower configs.");
                return;
            }

            // Initialize object pools
            enemyPool = new ObjectPool<GameObject>(CreateEnemy, OnGetEnemyFromPool, OnReleaseEnemyToPool, OnDestroyEnemyFromPool, true, 100, 200);
            towerPool = new ObjectPool<GameObject>(CreateTower, OnGetTowerFromPool, OnReleaseTowerToPool, OnDestroyTowerFromPool, true, 100, 200);

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

        private GameObject CreateEnemy()
        {
            // This will be called when creating a new pooled object
            return new GameObject("Pooled Enemy");
        }

        private void OnGetEnemyFromPool(GameObject enemy)
        {
            // Called when getting an enemy from the pool (activating it)
            enemy.SetActive(true);
        }

        private void OnReleaseEnemyToPool(GameObject enemy)
        {
            // Called when releasing an enemy to the pool (deactivating it)
            enemy.SetActive(false);
        }

        private void OnDestroyEnemyFromPool(GameObject enemy)
        {
            // Called when destroying an enemy from the pool
            Destroy(enemy);
        }

        private GameObject CreateTower()
        {
            // This will be called when creating a new pooled object
            return new GameObject("Pooled Tower");
        }

        private void OnGetTowerFromPool(GameObject tower)
        {
            // Called when getting a tower from the pool (activating it)
            tower.SetActive(true);
        }

        private void OnReleaseTowerToPool(GameObject tower)
        {
            // Called when releasing a tower to the pool (deactivating it)
            tower.SetActive(false);
        }

        private void OnDestroyTowerFromPool(GameObject tower)
        {
            // Called when destroying a tower from the pool
            Destroy(tower);
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

                    // Get enemy from pool instead of instantiating
                    GameObject enemyObj = enemyPool.Get();
                    enemyObj.transform.position = spawnPosition;
                    enemyObj.transform.rotation = Quaternion.identity;

                    // Set the prefab as parent to maintain visuals and components
                    enemyObj.transform.SetParent(enemyPrefab.transform, false);

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

                    // Copy components from prefab to pooled object
                    CopyComponentsFromPrefab(enemyObj, enemyPrefab);
                }
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

            // Copy specific components needed for enemy functionality
            EnemyAI prefabEnemyAI = prefab.GetComponent<EnemyAI>();
            if (prefabEnemyAI != null && target.GetComponent<EnemyAI>() == null)
            {
                target.AddComponent<EnemyAI>().Initialize(prefabEnemyAI);
            }
        }

        public void OnEnemyDefeated(GameObject defeatedEnemy)
        {
            // Return the enemy to the pool instead of destroying it
            if (defeatedEnemy != null)
            {
                enemyPool.Release(defeatedEnemy);
            }

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
                    // Get tower from pool instead of instantiating
                    GameObject towerObj = towerPool.Get();
                    towerObj.transform.position = position;
                    towerObj.transform.rotation = Quaternion.identity;

                    // Set the prefab as parent to maintain visuals and components
                    towerObj.transform.SetParent(towerPrefab.transform, false);

                    // Copy components from prefab to pooled object
                    CopyComponentsFromPrefab(towerObj, towerPrefab);

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






