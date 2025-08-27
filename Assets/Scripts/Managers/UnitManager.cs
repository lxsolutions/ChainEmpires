
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;
using ChainEmpires.Units;

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

        // Active unit instances
        private List<Unit> activeUnits = new List<Unit>();
        
        // Object pooling for unit prefabs
        private Dictionary<UnitType, ObjectPool<GameObject>> unitPools = new Dictionary<UnitType, ObjectPool<GameObject>>();

        public void Initialize()
        {
            Debug.Log("UnitManager initialized");
            
            // Initialize object pools for each unit type
            foreach (UnitType type in System.Enum.GetValues(typeof(UnitType)))
            {
                unitPools[type] = new ObjectPool<GameObject>(
                    () => CreateUnitPrefab(type),
                    OnGetUnitFromPool,
                    OnReleaseUnitToPool,
                    OnDestroyUnitFromPool,
                    true, 50, 100
                );
            }
            
            Debug.Log($"Initialized unit pools for {unitPools.Count} unit types");
        }

        private GameObject CreateUnitPrefab(UnitType type)
        {
            // Create a new GameObject with the appropriate unit component
            GameObject unitObj = new GameObject($"{type}_Prefab");
            
            // Add the appropriate unit component based on type
            switch (type)
            {
                case UnitType.Warrior:
                    unitObj.AddComponent<MeleeUnit>();
                    break;
                case UnitType.Archer:
                    unitObj.AddComponent<RangedUnit>();
                    break;
                default:
                    // For other unit types, add a generic Unit component
                    Unit unit = unitObj.AddComponent<Unit>();
                    unit.unitType = type;
                    unit.unitName = type.ToString();
                    break;
            }
            
            return unitObj;
        }

        private void OnGetUnitFromPool(GameObject unit)
        {
            // Called when getting a unit from the pool (activating it)
            unit.SetActive(true);
        }

        private void OnReleaseUnitToPool(GameObject unit)
        {
            // Called when releasing a unit to the pool (deactivating it)
            unit.SetActive(false);
        }

        private void OnDestroyUnitFromPool(GameObject unit)
        {
            // Called when destroying a unit from the pool
            Destroy(unit);
        }

        public void Update()
        {
            // Unit updates are handled by individual Unit components
        }

        public void OnDestroy()
        {
            // Clean up all unit pools
            foreach (var pool in unitPools.Values)
            {
                pool.Clear();
            }
            unitPools.Clear();
            activeUnits.Clear();
        }

        public Unit CreateUnit(UnitType type, Vector3 position, Quaternion rotation)
        {
            if (unitPools.TryGetValue(type, out ObjectPool<GameObject> pool))
            {
                GameObject unitObj = pool.Get();
                unitObj.transform.position = position;
                unitObj.transform.rotation = rotation;
                
                Unit unit = unitObj.GetComponent<Unit>();
                if (unit != null)
                {
                    activeUnits.Add(unit);
                    return unit;
                }
            }
            
            Debug.LogWarning($"Failed to create unit of type {type}");
            return null;
        }

        public bool DestroyUnit(Unit unit)
        {
            if (unit == null) return false;
            
            if (activeUnits.Contains(unit))
            {
                activeUnits.Remove(unit);
                
                // Return to appropriate pool
                if (unitPools.TryGetValue(unit.unitType, out ObjectPool<GameObject> pool))
                {
                    pool.Release(unit.gameObject);
                    return true;
                }
            }
            
            return false;
        }

        public List<Unit> GetAllUnits()
        {
            return new List<Unit>(activeUnits);
        }

        public List<Unit> GetUnitsOfType(UnitType type)
        {
            return activeUnits.FindAll(u => u.unitType == type);
        }

        public Unit GetUnitAtPosition(Vector3 position, float radius = 1f)
        {
            foreach (var unit in activeUnits)
            {
                if (Vector3.Distance(unit.transform.position, position) <= radius)
                {
                    return unit;
                }
            }
            return null;
        }

        // Helper methods for specific unit types
        public MeleeUnit CreateWarrior(Vector3 position)
        {
            Unit unit = CreateUnit(UnitType.Warrior, position, Quaternion.identity);
            return unit as MeleeUnit;
        }

        public RangedUnit CreateArcher(Vector3 position)
        {
            Unit unit = CreateUnit(UnitType.Archer, position, Quaternion.identity);
            return unit as RangedUnit;
        }

        // Training cost helper methods
        public float GetTrainingCost(UnitType type, int level = 1)
        {
            // Default costs
            switch (type)
            {
                case UnitType.Worker: return 50f * level;
                case UnitType.Scout: return 70f * level;
                case UnitType.Warrior: return 100f * level;
                case UnitType.Archer: return 80f * level;
                case UnitType.Knight: return 200f * level;
                case UnitType.SiegeWeapon: return 300f * level;
                case UnitType.Hero: return 500f * level;
                case UnitType.FlyingUnit: return 150f * level;
                case UnitType.MeleeChampion: return 250f * level;
                case UnitType.RangedChampion: return 180f * level;
                default: return 100f * level;
            }
        }

        public float GetTrainingTime(UnitType type, int level = 1)
        {
            // Base training times
            switch (type)
            {
                case UnitType.Worker: return 15f * Mathf.Pow(1.1f, level - 1);
                case UnitType.Scout: return 20f * Mathf.Pow(1.1f, level - 1);
                case UnitType.Warrior: return 30f * Mathf.Pow(1.1f, level - 1);
                case UnitType.Archer: return 25f * Mathf.Pow(1.1f, level - 1);
                case UnitType.Knight: return 45f * Mathf.Pow(1.1f, level - 1);
                case UnitType.SiegeWeapon: return 60f * Mathf.Pow(1.1f, level - 1);
                case UnitType.Hero: return 90f * Mathf.Pow(1.1f, level - 1);
                case UnitType.FlyingUnit: return 35f * Mathf.Pow(1.1f, level - 1);
                case UnitType.MeleeChampion: return 50f * Mathf.Pow(1.1f, level - 1);
                case UnitType.RangedChampion: return 40f * Mathf.Pow(1.1f, level - 1);
                default: return 30f * Mathf.Pow(1.1f, level - 1);
            }
        }

        // Selection and command methods
        public void SelectUnit(Unit unit)
        {
            // Deselect all other units
            foreach (var u in activeUnits)
            {
                u.Deselect();
            }
            
            // Select the specified unit
            unit.Select();
        }

        public void SelectUnits(List<Unit> unitsToSelect)
        {
            // Deselect all units first
            foreach (var unit in activeUnits)
            {
                unit.Deselect();
            }
            
            // Select the specified units
            foreach (var unit in unitsToSelect)
            {
                unit.Select();
            }
        }

        public void MoveSelectedUnits(Vector3 targetPosition)
        {
            foreach (var unit in activeUnits)
            {
                if (unit.isSelected)
                {
                    unit.MoveTo(targetPosition);
                }
            }
        }

        public void AttackWithSelectedUnits(Unit target)
        {
            foreach (var unit in activeUnits)
            {
                if (unit.isSelected)
                {
                    unit.AttackTarget(target);
                }
            }
        }
    }
}
