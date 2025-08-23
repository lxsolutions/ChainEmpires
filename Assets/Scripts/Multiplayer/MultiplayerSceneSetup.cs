







using UnityEngine;
using Photon.Pun;

namespace ChainEmpires
{
    public class MultiplayerSceneSetup : MonoBehaviourPun
    {
        [Header("Multiplayer Scene Setup")]
        public GameObject playerPrefab; // Player character prefab
        public Transform[] spawnPoints; // Array of spawn points

        void Start()
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                Debug.Log("Setting up multiplayer scene");

                // Instantiate player character for local player
                if (PhotonNetwork.IsMasterClient || PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    SetupMasterClient();
                }
                else
                {
                    SetupOtherPlayers();
                }

                // Load the game manager and initialize it for multiplayer
                InitializeGameManager();
            }
        }

        private void SetupMasterClient()
        {
            Debug.Log("Setting up master client");

            // Master client spawns first to set up initial conditions
            if (spawnPoints.Length > 0)
            {
                Vector3 spawnPosition = spawnPoints[0].position;
                Quaternion spawnRotation = spawnPoints[0].rotation;

                GameObject playerObj = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, spawnRotation);
                PlayerController playerController = playerObj.GetComponent<PlayerController>();

                if (playerController != null)
                {
                    playerController.Initialize(PhotonNetwork.LocalPlayer.ActorNumber, true);
                }
            }

            // Master client can also initialize shared game state
            InitializeSharedGameState();
        }

        private void SetupOtherPlayers()
        {
            Debug.Log("Setting up other players");

            // Other players spawn at available spawn points
            int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1; // 0-based index

            if (playerIndex < spawnPoints.Length)
            {
                Vector3 spawnPosition = spawnPoints[playerIndex].position;
                Quaternion spawnRotation = spawnPoints[playerIndex].rotation;

                GameObject playerObj = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, spawnRotation);
                PlayerController playerController = playerObj.GetComponent<PlayerController>();

                if (playerController != null)
                {
                    playerController.Initialize(PhotonNetwork.LocalPlayer.ActorNumber, false);
                }
            }
        }

        private void InitializeSharedGameState()
        {
            Debug.Log("Initializing shared game state");

            // Set up initial resources, buildings, etc. for multiplayer
            ResourceManager resourceManager = GameManager.Instance.ResourceManager;

            // Example: Initialize shared resources
            resourceManager.SetGenerationRate(ResourceManager.ResourceType.Minerals, 10f);
            resourceManager.SetMaxCapacity(ResourceManager.ResourceType.Minerals, 2000f);

            // Initialize buildings for multiplayer
            BuildingManager buildingManager = GameManager.Instance.BuildingManager;

            // Example: Add a shared town hall
            if (buildingManager.GetAllBuildings().Count == 0)
            {
                buildingManager.AddBuilding(BuildingManager.BuildingType.TownHall, Vector3.zero);
            }

            Debug.Log("Shared game state initialized");
        }

        private void InitializeGameManager()
        {
            GameManager gameManager = GameManager.Instance;

            if (gameManager != null)
            {
                // Set up game manager for multiplayer
                gameManager.SetGameState(GameManager.GameState.WorldMap);

                Debug.Log("Game manager initialized for multiplayer");
            }
        }

        public void OnPlayerLeftRoom()
        {
            Debug.Log("Player left room, cleaning up scene");

            // Clean up player objects when leaving room
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
            }
        }
    }
}



