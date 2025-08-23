






using UnityEngine;
using System.Collections.Generic;

namespace ChainEmpires
{
    public class UnitManager : IManager
    {
        // Unit types
        public enum UnitType
        {
            Worker,
            Scout,
            Warrior,
            Archer,
            Knight,
            SiegeWeapon,
            Hero,
            FlyingUnit,
            MeleeChampion,
            RangedChampion
        }

        // Unit data structure
        private class UnitData
        {
            public UnitType Type;
            public string Name;
            public Vector3 Position;
            public int Level;
            public float Health;
            public float MaxHealth;
            public float Damage;
            public float AttackRange;
            public float MoveSpeed;
            public bool IsSelected;

            public UnitData(UnitType type, string name, Vector3 position)
            {
                Type = type;
                Name = name;
                Position = position;
                Level = 1;
                Health = GetMaxHealth();
                MaxHealth = GetMaxHealth();
                Damage = GetBaseDamage();
                AttackRange = GetAttackRange();
                MoveSpeed = GetMoveSpeed();
                IsSelected = false;
            }

            private float GetMaxHealth()
            {
                // Base health based on unit type
                switch (Type)
                {
                    case UnitType.Worker: return 50f;
                    case UnitType.Scout: return 30f;
                    case UnitType.Warrior: return 80f;
                    case UnitType.Archer: return 40f;
                    case UnitType.Knight: return 120f;
                    case UnitType.SiegeWeapon: return 200f;
                    case UnitType.Hero: return 300f;
                    case UnitType.FlyingUnit: return 60f;
                    case UnitType.MeleeChampion: return 250f;
                    case UnitType.RangedChampion: return 180f;
                    default: return 50f;
                }
            }

            private float GetBaseDamage()
            {
                // Base damage based on unit type
                switch (Type)
                {
                    case UnitType.Worker: return 5f;
                    case UnitType.Scout: return 10f;
                    case UnitType.Warrior: return 20f;
                    case UnitType.Archer: return 30f;
                    case UnitType.Knight: return 40f;
                    case UnitType.SiegeWeapon: return 80f;
                    case UnitType.Hero: return 100f;
                    case UnitType.FlyingUnit: return 25f;
                    case UnitType.MeleeChampion: return 60f;
                    case UnitType.RangedChampion: return 50f;
                    default: return 10f;
                }
            }

            private float GetAttackRange()
            {
                // Base attack range based on unit type
                switch (Type)
                {
                    case UnitType.Worker: return 1f;
                    case UnitType.Scout: return 2f;
                    case UnitType.Warrior: return 1.5f;
                    case UnitType.Archer: return 6f;
                    case UnitType.Knight: return 2f;
                    case UnitType.SiegeWeapon: return 8f;
                    case UnitType.Hero: return 4f;
                    case UnitType.FlyingUnit: return 5f;
                    case UnitType.MeleeChampion: return 1.8f;
                    case UnitType.RangedChampion: return 7f;
                    default: return 2f;
                }
            }

            private float GetMoveSpeed()
            {
                // Base move speed based on unit type
                switch (Type)
                {
                    case UnitType.Worker: return 3f;
                    case UnitType.Scout: return 5f;
                    case UnitType.Warrior: return 4f;
                    case UnitType.Archer: return 3.5f;
                    case UnitType.Knight: return 4.5f;
                    case UnitType.SiegeWeapon: return 2f;
                    case UnitType.Hero: return 6f;
                    case UnitType.FlyingUnit: return 7f;
                    case UnitType.MeleeChampion: return 5f;
                    case UnitType.RangedChampion: return 4.8f;
                    default: return 3f;
                }
            }

            public void LevelUp()
            {
                Level++;
                MaxHealth *= 1.2f; // 20% health increase per level
                Health = MaxHealth;
                Damage *= 1.15f; // 15% damage increase per level

                Debug.Log($"{Name} leveled up to level {Level}! New stats: {Damage} damage, {MaxHealth} health");
            }
        }

        // List of all units
        private List<UnitData> units = new List<UnitData>();

        public void Initialize()
        {
            Debug.Log("UnitManager initialized");

            // Start with a basic worker unit
            AddUnit(UnitType.Worker, "Worker #1", Vector3.zero);
        }

        public void Update()
        {
            // Unit management updates can go here
        }

        public void OnDestroy()
        {
            // Clean up any pending operations
        }

        public void AddUnit(UnitType type, string name, Vector3 position)
        {
            UnitData newUnit = new UnitData(type, name, position);
            units.Add(newUnit);

            Debug.Log($"Added {type} unit '{name}' at {position}");
        }

        public bool TrainUnit(UnitType type, string name)
        {
            // Check resource requirements
            ResourceManager resourceManager = GameManager.Instance.ResourceManager;

            float mineralCost = 0f;
            switch (type)
            {
                case UnitType.Worker: mineralCost = 50f; break;
                case UnitType.Scout: mineralCost = 70f; break;
                case UnitType.Warrior: mineralCost = 100f; break;
                case UnitType.Archer: mineralCost = 80f; break;
                case UnitType.Knight: mineralCost = 200f; break;
                // Add more unit training costs...
            }

            if (!resourceManager.ConsumeResource(ResourceManager.ResourceType.Minerals, mineralCost))
            {
                Debug.LogWarning($"Not enough resources to train {type}");
                return false;
            }

            // Find a valid spawn position near the town hall
            Vector3 spawnPosition = GetValidSpawnPosition();

            AddUnit(type, name, spawnPosition);
            Debug.Log($"Trained new {type} unit '{name}'");

            return true;
        }

        private Vector3 GetValidSpawnPosition()
        {
            // For now, just return a position near the town hall
            // In a real implementation, this would check for valid terrain and space
            return new Vector3(2f, 0f, 2f);
        }

        public void LevelUpUnit(string unitName)
        {
            UnitData unit = FindUnit(unitName);

            if (unit != null)
            {
                // Check resource requirements for leveling up
                ResourceManager resourceManager = GameManager.Instance.ResourceManager;

                float experienceCost = 100f * unit.Level; // Experience cost scales with level

                if (!resourceManager.ConsumeResource(ResourceManager.ResourceType.Energy, experienceCost))
                {
                    Debug.LogWarning($"Not enough energy to level up {unitName}");
                    return;
                }

                unit.LevelUp();
            }
            else
            {
                Debug.LogWarning($"Unit '{unitName}' not found");
            }
        }

        private UnitData FindUnit(string name)
        {
            foreach (var unit in units)
            {
                if (unit.Name == name)
                {
                    return unit;
                }
            }
            return null;
        }

        public List<UnitData> GetAllUnits()
        {
            return units;
        }

        public void SelectUnit(string unitName)
        {
            UnitData unit = FindUnit(unitName);

            if (unit != null)
            {
                // Deselect all other units
                foreach (var u in units)
                {
                    u.IsSelected = false;
                }

                unit.IsSelected = true;
                Debug.Log($"Selected unit: {unit.Name}");
            }
            else
            {
                Debug.LogWarning($"Unit '{unitName}' not found");
            }
        }

        public void MoveUnit(string unitName, Vector3 targetPosition)
        {
            UnitData unit = FindUnit(unitName);

            if (unit != null && unit.IsSelected)
            {
                // In a real implementation, this would start movement animation
                unit.Position = targetPosition;
                Debug.Log($"Moved {unit.Name} to {targetPosition}");
            }
            else
            {
                Debug.LogWarning($"Cannot move unit: either not found or not selected");
            }
        }

        public void AttackUnit(string unitName, string targetUnitName)
        {
            UnitData attacker = FindUnit(unitName);
            UnitData target = FindUnit(targetUnitName);

            if (attacker != null && target != null && attacker.IsSelected)
            {
                // Simple attack logic - in a real implementation this would be more complex
                float damageDealt = attacker.Damage;
                target.Health -= damageDealt;

                Debug.Log($"{attacker.Name} attacks {target.Name} for {damageDealt} damage!");

                if (target.Health <= 0f)
                {
                    units.Remove(target);
                    Debug.Log($"{target.Name} has been defeated!");
                }
            }
            else
            {
                Debug.LogWarning($"Cannot attack: attacker or target not found");
            }
        }
    }
}





