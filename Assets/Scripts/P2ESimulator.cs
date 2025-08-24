

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class P2ESimulator : MonoBehaviour
{
    [Header("Simulation Parameters")]
    public int playerCount = 1000;
    public int simulationDays = 84; // 12 weeks
    public float resourceGenerationRate = 50f;
    public float stakingRewardRate = 0.1f;
    public float pveRewardMultiplier = 2f;
    public float pvpWinRewardMultiplier = 3f;
    public float pvpLosePenaltyMultiplier = 0.5f;

    [Header("Economic Balancing")]
    public float maxP2WRatio = 1.2f; // Pay-to-win threshold
    public float autoAdjustFactor = 0.9f;

    private List<PlayerData> players;
    private int currentDay;
    private bool simulationRunning;

    void Start()
    {
        Debug.Log("P2E Simulator initialized");
    }

    public void RunSimulation()
    {
        if (simulationRunning)
        {
            Debug.LogWarning("Simulation already running");
            return;
        }

        simulationRunning = true;
        currentDay = 0;
        players = new List<PlayerData>();

        // Initialize players
        for (int i = 0; i < playerCount; i++)
        {
            players.Add(new PlayerData(i));
        }

        Debug.Log($"Starting P2E simulation with {playerCount} players for {simulationDays} days");

        StartCoroutine(SimulationLoop());
    }

    private IEnumerator SimulationLoop()
    {
        while (currentDay < simulationDays && simulationRunning)
        {
            // Daily resource generation
            foreach (var player in players)
            {
                player.GenerateResources(resourceGenerationRate);
            }

            // PvE activities (50% of players participate daily)
            int pveParticipants = Mathf.RoundToInt(playerCount * 0.5f);
            for (int i = 0; i < pveParticipants; i++)
            {
                players[i].ParticipateInPvE(pveRewardMultiplier);
            }

            // PvP activities (20% of players participate daily)
            int pvpParticipants = Mathf.RoundToInt(playerCount * 0.2f);
            for (int i = 0; i < pvpParticipants; i++)
            {
                int opponentIndex = Random.Range(0, playerCount);
                while (opponentIndex == i) // Ensure different opponents
                {
                    opponentIndex = Random.Range(0, playerCount);
                }

                bool playerWins = Random.value > 0.5f;
                players[i].ParticipateInPvP(playerWins ? pvpWinRewardMultiplier : pvpLosePenaltyMultiplier,
                                           players[opponentIndex]);
            }

            // Staking rewards (10% of players stake daily)
            int stakers = Mathf.RoundToInt(playerCount * 0.1f);
            for (int i = 0; i < stakers; i++)
            {
                players[i].ReceiveStakingRewards(stakingRewardRate);
            }

            // Check P2W ratio and auto-adjust if needed
            float currentP2WRatio = CalculateP2WRatio();
            if (currentP2WRatio > maxP2WRatio)
            {
                Debug.LogWarning($"P2W ratio exceeded threshold: {currentP2WRatio}. Auto-adjusting...");
                AutoAdjustEconomy();
            }

            // Log daily progress
            LogDailyProgress();

            currentDay++;
            yield return new WaitForSeconds(1f); // Simulate one day per second for visualization
        }

        EndSimulation();
    }

    private void AutoAdjustEconomy()
    {
        // Reduce resource generation rate
        resourceGenerationRate *= autoAdjustFactor;

        // Reduce staking rewards
        stakingRewardRate *= autoAdjustFactor;

        Debug.Log($"Auto-adjusted economy. New rates - Resource: {resourceGenerationRate}, Staking: {stakingRewardRate}");
    }

    private float CalculateP2WRatio()
    {
        // Calculate ratio of resources obtained through purchases vs gameplay
        float totalResources = 0f;
        float purchasedResources = 0f;

        foreach (var player in players)
        {
            totalResources += player.totalResourcesEarned;
            purchasedResources += player.resourcesPurchased;
        }

        if (totalResources == 0f) return 1f; // Avoid division by zero

        return purchasedResources / totalResources;
    }

    private void LogDailyProgress()
    {
        string logMessage = $"Day {currentDay}: ";
        logMessage += $"Avg Resources: {CalculateAverageResources():F2}, ";
        logMessage += $"Max Resources: {CalculateMaxResources():F2}, ";
        logMessage += $"P2W Ratio: {CalculateP2WRatio():F2}";

        Debug.Log(logMessage);

        // Log to file for detailed analysis
        string logPath = "p2e_simulation_log.csv";
        using (StreamWriter writer = new StreamWriter(logPath, true))
        {
            writer.WriteLine($"{currentDay},{CalculateAverageResources():F2},{CalculateMaxResources():F2},{CalculateP2WRatio():F2}");
        }
    }

    private float CalculateAverageResources()
    {
        if (players.Count == 0) return 0f;

        float total = 0f;
        foreach (var player in players)
        {
            total += player.currentResources;
        }

        return total / players.Count;
    }

    private float CalculateMaxResources()
    {
        if (players.Count == 0) return 0f;

        float max = 0f;
        foreach (var player in players)
        {
            if (player.currentResources > max)
            {
                max = player.currentResources;
            }
        }

        return max;
    }

    public void StopSimulation()
    {
        simulationRunning = false;
    }

    private class PlayerData
    {
        public int id;
        public float currentResources;
        public float totalResourcesEarned;
        public float resourcesPurchased;

        public PlayerData(int playerId)
        {
            id = playerId;
            currentResources = 0f;
            totalResourcesEarned = 0f;
            resourcesPurchased = 0f;
        }

        public void GenerateResources(float rate)
        {
            currentResources += rate * Random.Range(0.8f, 1.2f);
            totalResourcesEarned += rate;
        }

        public void ParticipateInPvE(float multiplier)
        {
            float reward = resourceGenerationRate * multiplier * Random.Range(0.9f, 1.1f);
            currentResources += reward;
            totalResourcesEarned += reward;
        }

        public void ParticipateInPvP(float multiplier, PlayerData opponent)
        {
            float reward = resourceGenerationRate * multiplier * Random.Range(0.8f, 1.2f);

            if (multiplier > 1f) // Win case
            {
                currentResources += reward;
                totalResourcesEarned += reward;

                // Opponent loses some resources
                float loss = reward * 0.3f;
                opponent.currentResources -= loss;
                if (opponent.currentResources < 0f)
                {
                    opponent.currentResources = 0f;
                }
            }
            else // Lose case
            {
                currentResources += reward; // Still gets some participation reward
                totalResourcesEarned += reward;

                // Opponent wins more resources
                float winBonus = reward * 1.5f;
                opponent.currentResources += winBonus;
                opponent.totalResourcesEarned += winBonus;
            }
        }

        public void ReceiveStakingRewards(float rate)
        {
            float stakedAmount = currentResources * Random.Range(0.2f, 0.4f);
            if (stakedAmount > 0f)
            {
                currentResources -= stakedAmount; // Remove from current resources
                float reward = stakedAmount * rate;
                currentResources += reward;
                totalResourcesEarned += reward;
            }
        }

        public void PurchaseResources(float amount)
        {
            currentResources += amount;
            resourcesPurchased += amount;
        }
    }
}

