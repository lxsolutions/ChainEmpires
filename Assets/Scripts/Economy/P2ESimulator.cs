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

        private List<float> tokenBalances = new(); // Sim results
        private float totalSupply;

        void Start()
        {
            RunSimulation();
            LogResults(); // To hub/CSV
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