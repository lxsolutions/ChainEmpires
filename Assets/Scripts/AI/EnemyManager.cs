










using UnityEngine;
using System.Collections.Generic;

namespace ChainEmpires
{
    public class EnemyManager : MonoBehaviour
    {
        [Header("AI Enemy Settings")]
        public List<WaveConfig> waveConfigs; // Wave configurations for different difficulty levels
        public Transform[] spawnPoints; // Spawn locations for enemies

        [Header("Adaptive AI")]
        public float learningRate = 0.1f; // How quickly AI adapts to player defenses
        public int maxAdaptationLevel = 5; // Maximum adaptation level

        private List<EnemyWave> activeWaves = new List<EnemyWave>();
        private int currentWaveIndex = 0;
        private int currentAdaptationLevel = 1;

        void Start()
        {
            if (waveConfigs.Count == 0)
            {
                Debug.LogWarning("No wave configurations found. Please add wave configs.");
                return;
            }

            // Initialize the first wave
            StartCoroutine(StartWaveCycle());
        }

        private System.Collections.IEnumerator StartWaveCycle()
        {
            while (true)
            {
                yield return new WaitForSeconds(30); // Wait between waves

                if (currentWaveIndex < waveConfigs.Count)
                {
                    WaveConfig config = waveConfigs[currentWaveIndex];
                    StartNewWave(config);
                    currentWaveIndex++;
                }
                else
                {
                    // Cycle back to first wave with increased difficulty
                    currentWaveIndex = 0;
                    currentAdaptationLevel = Mathf.Min(currentAdaptationLevel + 1, maxAdaptationLevel);

                    Debug.Log($"Increasing difficulty to adaptation level {currentAdaptationLevel}");
                }
            }
        }

        private void StartNewWave(WaveConfig config)
        {
            EnemyWave newWave = new EnemyWave
            {
                WaveIndex = currentWaveIndex,
                AdaptationLevel = currentAdaptationLevel,
                Enemies = new List<EnemyData>()
            };

            // Spawn enemies based on wave configuration
            foreach (var enemyType in config.EnemyTypes)
            {
                for (int i = 0; i < enemyType.Count; i++)
                {
                    EnemyData enemyData = new EnemyData
                    {
                        Type = enemyType.Type,
                        Health = enemyType.Health * currentAdaptationLevel,
                        Damage = enemyType.Damage * currentAdaptationLevel,
                        Speed = enemyType.Speed * (1f + learningRate * currentAdaptationLevel)
                    };

                    SpawnEnemy(enemyData);
                    newWave.Enemies.Add(enemyData);
                }
            }

            activeWaves.Add(newWave);

            Debug.Log($"Wave {currentWaveIndex} started with adaptation level {currentAdaptationLevel}");
        }

        private void SpawnEnemy(EnemyData enemyData)
        {
            if (spawnPoints.Length == 0) return;

            int spawnIndex = Random.Range(0, spawnPoints.Length);
            Vector3 spawnPosition = spawnPoints[spawnIndex].position;

            GameObject enemyPrefab = GetEnemyPrefab(enemyData.Type);

            if (enemyPrefab != null)
            {
                GameObject enemyObj = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                EnemyAI enemyAI = enemyObj.GetComponent<EnemyAI>();

                if (enemyAI != null)
                {
                    enemyAI.Initialize(enemyData);
                }
            }
        }

        private GameObject GetEnemyPrefab(EnemyType type)
        {
            // This would be replaced with a proper prefab loading system
            switch (type)
            {
                case EnemyType.Barbarian:
                    return Resources.Load<GameObject>("Prefabs/Enemies/Barbarian");
                case EnemyType.Skeleton:
                    return Resources.Load<GameObject>("Prefabs/Enemies/Skeleton");
                case EnemyType.Alien:
                    return Resources.Load<GameObject>("Prefabs/Enemies/Alien");
                default:
                    Debug.LogWarning($"Enemy type {type} not implemented");
                    return null;
            }
        }

        public void OnPlayerDefenseUpdated()
        {
            // AI learns from player defenses and adapts
            currentAdaptationLevel = Mathf.Min(currentAdaptationLevel + 1, maxAdaptationLevel);

            Debug.Log($"AI adaptation increased to level {currentAdaptationLevel} based on player defenses");
        }

        public void OnWaveCompleted(EnemyWave wave)
        {
            activeWaves.Remove(wave);
            Debug.Log($"Wave {wave.WaveIndex} completed with adaptation level {wave.AdaptationLevel}");
        }

        [System.Serializable]
        public class WaveConfig
        {
            public List<EnemyGroup> EnemyTypes;
        }

        [System.Serializable]
        public class EnemyGroup
        {
            public EnemyType Type;
            public int Count;
            public float Health = 100f;
            public float Damage = 10f;
            public float Speed = 2f;
        }

        public enum EnemyType
        {
            Barbarian,
            Skeleton,
            Alien
        }

        private class EnemyWave
        {
            public int WaveIndex;
            public int AdaptationLevel;
            public List<EnemyData> Enemies;
        }

        private class EnemyData
        {
            public EnemyType Type;
            public float Health;
            public float Damage;
            public float Speed;
        }
    }
}





