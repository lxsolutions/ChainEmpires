







using UnityEngine;
using System.Collections.Generic;

namespace ChainEmpires
{
    public class WorldMapManager : IManager
    {
        // Realm types based on player progression
        public enum RealmType
        {
            BeginnerRealm,
            NoviceRealm,
            AdeptRealm,
            MasterRealm,
            ChampionRealm,
            LegendaryRealm
        }

        // Map data structure
        private class MapData
        {
            public string Name;
            public Vector2 Size; // Width and height in world units
            public RealmType Realm;
            public List<Vector3> PlayerSpawnPoints;
            public List<Vector3> ResourceNodes;
            public List<Vector3> NeutralStructures;

            public MapData(string name, Vector2 size, RealmType realm)
            {
                Name = name;
                Size = size;
                Realm = realm;
                PlayerSpawnPoints = new List<Vector3>();
                ResourceNodes = new List<Vector3>();
                NeutralStructures = new List<Vector3>();

                // Initialize with some default values
                GenerateDefaultMapData();
            }

            private void GenerateDefaultMapData()
            {
                // Add spawn points
                PlayerSpawnPoints.Add(new Vector3(10f, 0f, 10f));
                PlayerSpawnPoints.Add(new Vector3(-10f, 0f, -10f));
                PlayerSpawnPoints.Add(new Vector3(15f, 0f, -15f));

                // Add resource nodes
                ResourceNodes.Add(new Vector3(20f, 0f, 20f));
                ResourceNodes.Add(new Vector3(-15f, 0f, 15f));
                ResourceNodes.Add(new Vector3(0f, 0f, 25f));

                // Add neutral structures
                NeutralStructures.Add(new Vector3(8f, 0f, 8f)); // Small outpost
                NeutralStructures.Add(new Vector3(-12f, 0f, -12f)); // Tower
            }
        }

        private MapData currentMap;
        public RealmType CurrentRealm { get; private set; }

        public void Initialize()
        {
            Debug.Log("WorldMapManager initialized");

            // Start in beginner realm with a default map
            CurrentRealm = RealmType.BeginnerRealm;
            LoadMap("BeginnerRealm", new Vector2(100f, 100f), RealmType.BeginnerRealm);
        }

        public void Update()
        {
            // World map updates can go here
        }

        public void OnDestroy()
        {
            // Clean up any pending operations
        }

        private void LoadMap(string name, Vector2 size, RealmType realm)
        {
            currentMap = new MapData(name, size, realm);
            Debug.Log($"Loaded map: {name} ({size.x}x{size.y}) in {realm} realm");
        }

        public bool ExploreNewArea(Vector3 position)
        {
            // Check if position is within map bounds
            if (position.x < 0 || position.x > currentMap.Size.x ||
                position.z < 0 || position.z > currentMap.Size.y)
            {
                Debug.LogWarning($"Cannot explore {position}: out of map bounds");
                return false;
            }

            // In a real implementation, this would reveal new areas on the minimap
            Debug.Log($"Explored new area at {position}");

            // Check for resource nodes nearby
            foreach (var node in currentMap.ResourceNodes)
            {
                if (Vector3.Distance(position, node) < 5f)
                {
                    Debug.Log($"Found resource node at {node}");
                    return true;
                }
            }

            // Check for neutral structures nearby
            foreach (var structure in currentMap.NeutralStructures)
            {
                if (Vector3.Distance(position, structure) < 3f)
                {
                    Debug.Log($"Discovered neutral structure at {structure}");
                    return true;
                }
            }

            return true;
        }

        public bool AttackNeutralStructure(Vector3 position)
        {
            // Find the nearest neutral structure
            Vector3 targetStructure = Vector3.zero;
            float closestDistance = float.MaxValue;

            foreach (var structure in currentMap.NeutralStructures)
            {
                float distance = Vector3.Distance(position, structure);
                if (distance < closestDistance && distance < 5f) // Within attack range
                {
                    closestDistance = distance;
                    targetStructure = structure;
                }
            }

            if (targetStructure == Vector3.zero)
            {
                Debug.LogWarning($"No neutral structures nearby to attack");
                return false;
            }

            // In a real implementation, this would start a battle sequence
            Debug.Log($"Attacking neutral structure at {targetStructure}");

            // Remove the structure from the map
            currentMap.NeutralStructures.Remove(targetStructure);
            Debug.Log($"Neutral structure defeated!");

            return true;
        }

        public bool GatherResources(Vector3 position)
        {
            // Find the nearest resource node
            Vector3 targetNode = Vector3.zero;
            float closestDistance = float.MaxValue;

            foreach (var node in currentMap.ResourceNodes)
            {
                float distance = Vector3.Distance(position, node);
                if (distance < closestDistance && distance < 5f) // Within gathering range
                {
                    closestDistance = distance;
                    targetNode = node;
                }
            }

            if (targetNode == Vector3.zero)
            {
                Debug.LogWarning($"No resource nodes nearby to gather");
                return false;
            }

            // In a real implementation, this would start a resource gathering animation
            Debug.Log($"Gathering resources from {targetNode}");

            // Add resources to player inventory
            ResourceManager resourceManager = GameManager.Instance.ResourceManager;

            float mineralsGathered = 50f;
            if (resourceManager.AddResource(ResourceManager.ResourceType.Minerals, mineralsGathered))
            {
                Debug.Log($"Gathered {mineralsGathered} minerals");
                return true;
            }
            else
            {
                Debug.LogWarning($"Could not gather resources - inventory full");
                return false;
            }
        }

        public void AdvanceToNextRealm()
        {
            RealmType nextRealm = CurrentRealm;

            switch (CurrentRealm)
            {
                case RealmType.BeginnerRealm:
                    nextRealm = RealmType.NoviceRealm;
                    break;
                case RealmType.NoviceRealm:
                    nextRealm = RealmType.AdeptRealm;
                    break;
                case RealmType.AdeptRealm:
                    nextRealm = RealmType.MasterRealm;
                    break;
                case RealmType.MasterRealm:
                    nextRealm = RealmType.ChampionRealm;
                    break;
                case RealmType.ChampionRealm:
                    nextRealm = RealmType.LegendaryRealm;
                    break;
                // Add more realm progression logic...
            }

            if (nextRealm != CurrentRealm)
            {
                CurrentRealm = nextRealm;

                // Load a new map for the next realm
                string realmName = nextRealm.ToString();
                Vector2 mapSize = currentMap.Size * 1.5f; // Increase map size with each realm

                LoadMap(realmName, mapSize, nextRealm);
                Debug.Log($"Advanced to {nextRealm} realm!");
            }
        }

        public List<Vector3> GetPlayerSpawnPoints()
        {
            return currentMap.PlayerSpawnPoints;
        }

        public List<Vector3> GetResourceNodes()
        {
            return currentMap.ResourceNodes;
        }

        public List<Vector3> GetNeutralStructures()
        {
            return currentMap.NeutralStructures;
        }
    }
}






