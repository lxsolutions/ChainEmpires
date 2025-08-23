








using UnityEngine;
using System.Collections.Generic;

namespace ChainEmpires
{
    public class CombatManager : IManager
    {
        // Battle outcome types
        public enum BattleOutcome
        {
            Victory,
            Defeat,
            Draw,
            Retreat
        }

        private bool isInBattle = false;
        private List<string> attackingUnits = new List<string>();
        private List<string> defendingUnits = new List<string>();

        public void Initialize()
        {
            Debug.Log("CombatManager initialized");
        }

        public void Update()
        {
            // Combat updates can go here
        }

        public void OnDestroy()
        {
            // Clean up any pending operations
        }

        public bool StartBattle(List<string> attackerUnits, List<string> defenderUnits)
        {
            if (isInBattle)
            {
                Debug.LogWarning("Already in battle!");
                return false;
            }

            attackingUnits = new List<string>(attackerUnits);
            defendingUnits = new List<string>(defenderUnits);

            isInBattle = true;
            Debug.Log($"Battle started between {attackingUnits.Count} attackers and {defendingUnits.Count} defenders");

            // Start the battle resolution process
            Invoke("ResolveBattle", 2f); // Simulate a 2 second delay for battle resolution

            return true;
        }

        private void ResolveBattle()
        {
            Debug.Log("Resolving battle...");

            UnitManager unitManager = GameManager.Instance.UnitManager;

            // Simple battle resolution logic
            float totalAttackerPower = 0f;
            float totalDefenderPower = 0f;

            foreach (var unitName in attackingUnits)
            {
                var unit = unitManager.FindUnit(unitName);
                if (unit != null)
                {
                    totalAttackerPower += unit.Damage * unit.Health; // Simple power calculation
                }
            }

            foreach (var unitName in defendingUnits)
            {
                var unit = unitManager.FindUnit(unitName);
                if (unit != null)
                {
                    totalDefenderPower += unit.Damage * unit.Health;
                }
            }

            Debug.Log($"Attacker power: {totalAttackerPower}, Defender power: {totalDefenderPower}");

            BattleOutcome outcome = BattleOutcome.Draw;

            if (totalAttackerPower > totalDefenderPower * 1.2f) // Attackers need 20% more power to win
            {
                outcome = BattleOutcome.Victory;
            }
            else if (totalDefenderPower > totalAttackerPower * 1.2f)
            {
                outcome = BattleOutcome.Defeat;
            }

            EndBattle(outcome);
        }

        private void EndBattle(BattleOutcome outcome)
        {
            isInBattle = false;

            string outcomeMessage = "";
            switch (outcome)
            {
                case BattleOutcome.Victory:
                    outcomeMessage = "Victory!";
                    // Award resources/rewards for victory
                    ResourceManager resourceManager = GameManager.Instance.ResourceManager;
                    resourceManager.AddResource(ResourceManager.ResourceType.Minerals, 100f);
                    resourceManager.AddResource(ResourceManager.ResourceType.Energy, 50f);
                    break;
                case BattleOutcome.Defeat:
                    outcomeMessage = "Defeat!";
                    // Apply penalties for defeat
                    ResourceManager resourceManager2 = GameManager.Instance.ResourceManager;
                    resourceManager2.ConsumeResource(ResourceManager.ResourceType.Minerals, 50f);
                    break;
                case BattleOutcome.Draw:
                    outcomeMessage = "Draw - both sides retreated";
                    break;
                case BattleOutcome.Retreat:
                    outcomeMessage = "Retreat - battle avoided";
                    break;
            }

            Debug.Log($"Battle ended in {outcomeMessage}");

            // Clean up units (in a real implementation, this would be more complex)
            attackingUnits.Clear();
            defendingUnits.Clear();

            // Notify the game manager of battle completion
            GameManager.Instance.SetGameState(GameManager.GameState.BaseView);
        }

        public void RetreatFromBattle()
        {
            if (!isInBattle) return;

            Debug.Log("Retreating from battle...");
            EndBattle(BattleOutcome.Retreat);
        }

        public bool CanAttack(string unitName)
        {
            UnitManager unitManager = GameManager.Instance.UnitManager;
            var unit = unitManager.FindUnit(unitName);

            if (unit == null || !unit.IsSelected)
            {
                return false;
            }

            // Check if unit is already in battle
            if (attackingUnits.Contains(unitName) || defendingUnits.Contains(unitName))
            {
                return false;
            }

            return true;
        }

        public void QueueAttack(string attackerUnitName, string defenderUnitName)
        {
            UnitManager unitManager = GameManager.Instance.UnitManager;

            var attacker = unitManager.FindUnit(attackerUnitName);
            var defender = unitManager.FindUnit(defenderUnitName);

            if (attacker == null || defender == null)
            {
                Debug.LogWarning($"Cannot queue attack: attacker or defender not found");
                return;
            }

            // In a real implementation, this would add to an attack queue
            Debug.Log($"{attacker.Name} is queued to attack {defender.Name}");
        }
    }
}







