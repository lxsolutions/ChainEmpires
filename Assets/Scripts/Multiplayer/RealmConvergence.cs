








using UnityEngine;
using System.Collections.Generic;

namespace ChainEmpires
{
    public class RealmConvergence : MonoBehaviour
    {
        [Header("Realm Settings")]
        public int initialRealmLevel = 1; // Stone Age
        public int maxRealmLevel = 5; // Galactic Age

        [Header("Portal Event Settings")]
        public float portalEventChance = 0.05f; // 5% chance per day
        public int minPlayersForPortal = 2;

        [Header("AI Adaptation")]
        public float techInterferenceDebuff = 0.8f; // 20% reduction for advanced players in lower realms
        public float ancientArtifactBuff = 1.3f; // 30% boost for beginners in higher realms

        private int currentRealmLevel;
        private List<PlayerData> playersInRealm = new List<PlayerData>();
        private System.Random random;

        void Start()
        {
            random = new System.Random();
            currentRealmLevel = initialRealmLevel;
            InitializeRealm();

            Debug.Log($"Realm Convergence system initialized at level {currentRealmLevel}");
        }

        private void InitializeRealm()
        {
            // Set up initial realm conditions
            UpdateRealmConditions();
            StartCoroutine(CheckForPortalEvents());
        }

        private void UpdateRealmConditions()
        {
            string[] eraNames = { "Stone Age", "Medieval", "Industrial", "Space Age", "Galactic" };
            string currentEra = eraNames[Mathf.Clamp(currentRealmLevel - 1, 0, eraNames.Length - 1)];

            Debug.Log($"Realm updated to: {currentEra} (Level {currentRealmLevel})");

            // Update game conditions based on realm level
            ResourceManager resourceManager = GameManager.Instance.ResourceManager;
            BuildingManager buildingManager = GameManager.Instance.BuildingManager;

            switch (currentRealmLevel)
            {
                case 1: // Stone Age
                    resourceManager.SetGenerationRate(ResourceManager.ResourceType.Minerals, 5f);
                    resourceManager.SetGenerationRate(ResourceManager.ResourceType.Wood, 3f);
                    break;
                case 2: // Medieval
                    resourceManager.SetGenerationRate(ResourceManager.ResourceType.Gold, 2f);
                    buildingManager.UnlockBuilding(BuildingManager.BuildingType.Castle);
                    break;
                case 3: // Industrial
                    resourceManager.SetGenerationRate(ResourceManager.ResourceType.Energy, 5f);
                    buildingManager.UnlockBuilding(BuildingManager.BuildingType.Factory);
                    break;
                case 4: // Space Age
                    resourceManager.SetGenerationRate(ResourceManager.ResourceType.Crystals, 1f);
                    buildingManager.UnlockBuilding(BuildingManager.BuildingType.Spaceport);
                    break;
                case 5: // Galactic
                    resourceManager.SetGenerationRate(ResourceManager.ResourceType.DarkMatter, 0.1f);
                    buildingManager.UnlockBuilding(BuildingManager.BuildingType.GalacticHub);
                    break;
            }

            // Update UI and notifications
            GameManager.Instance.UIManager.ShowRealmUpdateNotification(currentEra);
        }

        private System.Collections.IEnumerator CheckForPortalEvents()
        {
            while (true)
            {
                yield return new WaitForDays(1); // Wait for 1 real day

                if (ShouldTriggerPortalEvent())
                {
                    TriggerPortalEvent();
                }
            }
        }

        private bool ShouldTriggerPortalEvent()
        {
            // Check if conditions are met for a portal event
            float chance = random.NextDouble() * 100f;
            return chance < (portalEventChance * 100f) && playersInRealm.Count >= minPlayersForPortal;
        }

        private void TriggerPortalEvent()
        {
            Debug.Log("🌟 Portal Event triggered! Cross-realm interactions initiated.");

            // Determine which realms will converge
            int targetRealm = currentRealmLevel + random.Next(-1, 2); // Can go up or down by 1 level
            targetRealm = Mathf.Clamp(targetRealm, initialRealmLevel, maxRealmLevel);

            if (targetRealm != currentRealmLevel)
            {
                Debug.Log($"Portal connecting Realm {currentRealmLevel} with Realm {targetRealm}");

                // Apply adaptive balancing for cross-realm interactions
                foreach (var player in playersInRealm)
                {
                    float balanceFactor = 1f;

                    if (player.RealmLevel > targetRealm)
                    {
                        // Advanced player in lower realm gets debuff
                        balanceFactor = techInterferenceDebuff;
                        Debug.Log($"Player {player.PlayerId} (level {player.RealmLevel}) entering lower realm - applying tech interference (-20%)");
                    }
                    else if (player.RealmLevel < targetRealm)
                    {
                        // Beginner in higher realm gets buff
                        balanceFactor = ancientArtifactBuff;
                        Debug.Log($"Player {player.PlayerId} (level {player.RealmLevel}) entering higher realm - granting ancient artifact bonus (+30%)");
                    }

                    // Apply balance adjustments to player's current actions
                    ApplyBalanceAdjustments(player, balanceFactor);
                }

                // Update the current realm level for this session
                currentRealmLevel = targetRealm;
                UpdateRealmConditions();
            }
        }

        private void ApplyBalanceAdjustments(PlayerData player, float balanceFactor)
        {
            // Example: Adjust resource generation rates temporarily
            ResourceManager resourceManager = GameManager.Instance.ResourceManager;

            foreach (ResourceManager.ResourceType resourceType in System.Enum.GetValues(typeof(ResourceManager.ResourceType)))
            {
                float originalRate = resourceManager.GetGenerationRate(resourceType);
                float adjustedRate = originalRate * balanceFactor;
                resourceManager.SetGenerationRate(resourceType, adjustedRate);

                Debug.Log($"Player {player.PlayerId}: {resourceType} generation rate adjusted from {originalRate} to {adjustedRate}");
            }

            // Reset after a short duration
            StartCoroutine(ResetBalanceAdjustments(player, originalRate, balanceFactor));
        }

        private System.Collections.IEnumerator ResetBalanceAdjustments(PlayerData player, float originalRate, float balanceFactor)
        {
            yield return new WaitForSeconds(300); // 5 minutes

            ResourceManager resourceManager = GameManager.Instance.ResourceManager;
            resourceManager.SetGenerationRate(ResourceManager.ResourceType.Minerals, originalRate);

            Debug.Log($"Player {player.PlayerId}: Balance adjustments reset to normal");
        }

        public void AddPlayerToRealm(int playerId, int realmLevel)
        {
            playersInRealm.Add(new PlayerData
            {
                PlayerId = playerId,
                RealmLevel = realmLevel
            });

            Debug.Log($"Player {playerId} added to realm (level {realmLevel})");
        }

        public void RemovePlayerFromRealm(int playerId)
        {
            playersInRealm.RemoveAll(p => p.PlayerId == playerId);
            Debug.Log($"Player {playerId} removed from realm");
        }

        public void TriggerEraShift()
        {
            // Global event that advances all players simultaneously
            if (currentRealmLevel < maxRealmLevel)
            {
                currentRealmLevel++;
                UpdateRealmConditions();

                Debug.Log("🌐 Era Shift triggered! All players advance to next era.");
                GameManager.Instance.UIManager.ShowGlobalNotification("Era Shift: New technologies unlocked!");
            }
        }

        private class PlayerData
        {
            public int PlayerId;
            public int RealmLevel;
        }
    }
}




