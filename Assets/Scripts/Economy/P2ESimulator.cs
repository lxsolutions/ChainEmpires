using UnityEngine;
using System.Collections.Generic; // For lists
using Random = UnityEngine.Random; // For RNG

namespace ChainEmpires
{
    public class P2ESimulator : MonoBehaviour
    {
        [Header("Simulation Params (2025 Best Practices)")]
        [SerializeField] private int numSessions = 1000; // Monte Carlo runs
        [SerializeField] private float baseEarnRate = 10f; // Tokens per quest
        [SerializeField] private float nftYieldBonus = 0.2f; // Utility boost, not P2W
        [SerializeField] private float grindMultiplier = 1.1f; // Free player streak bonus
        [SerializeField] private float inflationCap = 0.05f; // Daily supply limit




        [Header("Extended Simulation Parameters")]
        [SerializeField] private int numberOfPlayers = 1000; // Extended for beta testing
        [SerializeField] private float simulationDurationDays = 56; // 8 weeks
        [SerializeField] private bool runOnStart = false;
        [SerializeField] private bool autoAdjustEconomy = true; // Auto-adjust params if P2W ratio exceeds target

        [Header("Economic Settings")]
        [SerializeField] private float p2wRatioTarget = 1.2f; // Target P2W ratio (free vs paid)
        [SerializeField] private float initialFreePlayerResources = 500f;
        [SerializeField] private float initialPaidPlayerResources = 1000f;


        private List<float> tokenBalances = new(); // Sim results
        private float totalSupply;
        private List<PlayerData> players;
        private float simulationTimeElapsed;
        private bool isRunning;

        void Start()
        {
            if (runOnStart)
            {
                StartExtendedSimulation();
            }
            else
            {
                RunSimulation();
                LogResults(); // To hub/CSV
            }
        }

        private void RunSimulation()
        {
            for (int i = 0; i < numSessions; i++)
            {
                bool hasNFT = Random.value > 0.5f; // 50% NFT holders
                float earnings = baseEarnRate * Random.Range(5, 15); // Quests/raids
                if (hasNFT) earnings *= (1f + nftYieldBonus); // Utility
                else earnings *= grindMultiplier; // Free grind equals

                // Innovative Anti-P2W: Cap if overpaid
                earnings = Mathf.Min(earnings, baseEarnRate * 2f);

                // Inflation Control: Burn if over cap
                if (totalSupply + earnings > totalSupply * (1f + inflationCap))
                    earnings *= 0.9f; // Deflate

                tokenBalances.Add(earnings);
                totalSupply += earnings;
            }
        }

        private void LogResults()
        {
            float avgFree = 0f, avgPaid = 0f, countFree = 0, countPaid = 0;




        public void StartExtendedSimulation()
        {
            Debug.Log("Starting extended P2E economic simulation...");

            players = new List<PlayerData>();
            simulationTimeElapsed = 0f;
            isRunning = true;

            // Initialize players
            for (int i = 0; i < numberOfPlayers; i++)
            {
                bool isPaid = Random.value < 0.2f; // 20% chance of being a paid player
                float initialResources = isPaid ? initialPaidPlayerResources : initialFreePlayerResources;

                players.Add(new PlayerData
                {
                    id = i,
                    isPaid = isPaid,
                    resources = initialResources,
                    nftCount = Random.Range(1, 5),
                    stakedAmount = Random.Range(0f, initialResources * 0.3f)
                });
            }

            StartCoroutine(RunExtendedSimulation());
        }

        private IEnumerator RunExtendedSimulation()
        {
            while (simulationTimeElapsed < simulationDurationDays)
            {
                // Simulate daily activities
                for (int i = 0; i < players.Count; i++)
                {
                    PlayerData player = players[i];

                    // Daily resource generation
                    float dailyEarnings = CalculateDailyEarnings(player);
                    player.resources += dailyEarnings;

                    // Staking rewards
                    if (player.stakedAmount > 0)
                    {
                        float stakingReward = player.stakedAmount * 0.05f; // 5% daily return
                        player.resources += stakingReward;
                    }

                    // NFT bonuses
                    if (player.nftCount > 0)
                    {
                        float nftBonus = player.nftCount * 10f;
                        player.resources += nftBonus;
                    }

                    // Random events
                    HandleRandomEvents(player);

                    players[i] = player; // Update player data
                }

                simulationTimeElapsed++;
                Debug.Log($"Simulation day {simulationTimeElapsed}: Total resources = {CalculateTotalResources()}");

                yield return new WaitForSeconds(1f); // Simulate one day per second
            }

            isRunning = false;
            Debug.Log("Extended simulation completed!");

            // Calculate final metrics
            float totalFreeResources = 0f;
            float totalPaidResources = 0f;

            foreach (var player in players)
            {
                if (player.isPaid)
                {
                    totalPaidResources += player.resources;
                }
                else
                {
                    totalFreeResources += player.resources;
                }
            }

            float p2wRatio = totalPaidResources / totalFreeResources;
            Debug.Log($"Final P2W Ratio: {p2wRatio.ToString("F2")} (Target: {p2wRatioTarget})");

            // Save results to CSV
            SaveSimulationResults();

            if (autoAdjustEconomy && p2wRatio > p2wRatioTarget)
            {
                Debug.LogWarning($"P2W ratio exceeds target! Auto-adjusting economic parameters...");

                // Adjust economic parameters to balance P2W ratio
                float adjustmentFactor = p2wRatio / p2wRatioTarget;

                if (adjustmentFactor > 1.5f) // If significantly unbalanced
                {
                    // Increase free player bonuses
                    grindMultiplier *= 1.1f;
                    nftYieldBonus *= 1.05f;

                    // Decrease paid player advantages
                    baseEarnRate *= 0.95f;

                    Debug.Log($"Adjusted parameters: grindMultiplier={grindMultiplier}, nftYieldBonus={nftYieldBonus}, baseEarnRate={baseEarnRate}");
                }
            }
            else if (p2wRatio > p2wRatioTarget)
            {
                Debug.LogWarning("P2W ratio exceeds target! Consider adjusting economic parameters.");
            }
        }

        private float CalculateDailyEarnings(PlayerData player)
        {
            // Base earnings + bonuses for paid players and NFT owners
            float baseEarnings = 50f;
            if (player.isPaid) baseEarnings *= 1.5f; // Paid players earn more

            return baseEarnings;
        }

        private void HandleRandomEvents(PlayerData player)
        {
            // Random events that can affect resources
            if (Random.value < 0.05f) // 5% chance per day
            {
                float eventImpact = Random.Range(-20f, 50f);
                Debug.Log($"Player {player.id} experienced random event: {eventImpact.ToString("F1")} resources");

                player.resources += eventImpact;
            }
        }

        private float CalculateTotalResources()
        {
            float total = 0f;
            foreach (var player in players)
            {
                total += player.resources;
            }
            return total;
        }

        private void SaveSimulationResults()
        {
            string path = Path.Combine(Application.persistentDataPath, "p2e_simulation_results.csv");
            Debug.Log($"Saving simulation results to: {path}");

            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine("PlayerID,IsPaid,Resources,NFTs,StakedAmount");

                foreach (var player in players)
                {
                    writer.WriteLine($"{player.id},{player.isPaid},{player.resources.ToString("F2")},{player.nftCount},{player.stakedAmount.ToString("F2")}");
                }
            }

            Debug.Log("Simulation results saved successfully!");
        }

        [System.Serializable]
        private class PlayerData
        {
            public int id;
            public bool isPaid;
            public float resources;
            public int nftCount;
            public float stakedAmount;
        }



            foreach (var bal in tokenBalances)
            {
                // Assume even split for analysis
                if (countFree < numSessions / 2) { avgFree += bal; countFree++; }
                else { avgPaid += bal; countPaid++; }
            }
            avgFree /= countFree;
            avgPaid /= countPaid;

            Debug.Log($"P2E Balance Sim: Avg Free Earnings: {avgFree}, Avg Paid: {avgPaid} (Ratio: {avgPaid / avgFree:P2} - Aim <1.2 for anti-P2W)");
            // Append to hub: Use File.AppendAllText("/workspace/ChainEmpires/docs/grok-progress-hub.md", results);
            // Or CSV for charts: File.WriteAllText("p2e_sim.csv", string.Join("\n", tokenBalances));
        }

        // Expand: Run parallel (2025 Unity Jobs for speed)
        // Use Unity.Jobs for multi-thread sims if numSessions large
    }
}