






using UnityEngine;
using ChainEmpires.Shared;

namespace ChainEmpires
{
    /// <summary>
    /// Main game manager that orchestrates core systems.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // Singleton instance
        private static GameManager _instance;
        public static GameManager Instance => _instance;

        // Core managers (will be assigned in scene)
        [Header("Core Managers")]
        public ResourceManager resourceManager;
        public BuildingManager buildingManager;
        public UnitManager unitManager;
        public CombatManager combatManager;
        public UIManager uiManager;

        // Chain service implementation
        private IChainService _chainService;

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeGame();
        }

        /// <summary>
        /// Initializes the game systems.
        /// </summary>
        private void InitializeGame()
        {
            Debug.Log("Initializing Chain Empires Alpha...");

            // Disable Web3 features
            if (!GameSettings.IsWeb3Enabled)
            {
                Debug.Log("Web3 integration disabled for alpha build");
                _chainService = new Client.MockChainService();
            }
            else
            {
                // In future: Initialize real chain service here
                Debug.LogWarning("Web3 integration enabled - this should not happen in alpha!");
            }

            // Initialize core managers
            if (resourceManager != null)
            {
                resourceManager.Initialize();
            }

            if (buildingManager != null)
            {
                buildingManager.Initialize();
            }

            if (unitManager != null)
            {
                unitManager.Initialize();
            }

            Debug.Log("Game initialization complete");
        }

        /// <summary>
        /// Gets the current chain service implementation.
        /// </summary>
        public IChainService GetChainService()
        {
            return _chainService;
        }
    }
}






