


using UnityEngine;
using System.Collections.Generic;

namespace ChainEmpires
{
    public class GameManager : MonoBehaviour
    {
        // Singleton instance
        public static GameManager Instance { get; private set; }

        // Game state management
        public enum GameState
        {
            MainMenu,
            BaseView,
            WorldMap,
            Battle,
            Settings
        }

        public GameState CurrentGameState { get; private set; }

        // Managers
        public ResourceManager ResourceManager { get; private set; }
        public BuildingManager BuildingManager { get; private set; }
        public UnitManager UnitManager { get; private set; }
        public ProgressionManager ProgressionManager { get; private set; }
        public WorldMapManager WorldMapManager { get; private set; }
        public CombatManager CombatManager { get; private set; }
        public Web3Manager Web3Manager { get; private set; }

        // Game settings
        public bool IsMultiplayerEnabled = true;
        public bool IsWeb3Enabled = false;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            InitializeManagers();
        }

        void Start()
        {
            SetGameState(GameState.MainMenu);
        }

        void InitializeManagers()
        {
            ResourceManager = new ResourceManager();
            BuildingManager = new BuildingManager();
            UnitManager = new UnitManager();
            ProgressionManager = new ProgressionManager();
            WorldMapManager = new WorldMapManager();
            CombatManager = new CombatManager();
            Web3Manager = new Web3Manager();

            // Initialize each manager
            ResourceManager.Initialize();
            BuildingManager.Initialize();
            UnitManager.Initialize();
            ProgressionManager.Initialize();
            WorldMapManager.Initialize();
            CombatManager.Initialize();
            Web3Manager.Initialize();
        }

        public void SetGameState(GameState newState)
        {
            CurrentGameState = newState;
            Debug.Log($"Game state changed to: {newState}");

            // Handle state transitions
            switch (newState)
            {
                case GameState.MainMenu:
                    // Load main menu scene
                    break;
                case GameState.BaseView:
                    // Load base view scene
                    break;
                case GameState.WorldMap:
                    // Load world map scene
                    break;
                case GameState.Battle:
                    // Load battle scene
                    break;
                case GameState.Settings:
                    // Load settings scene
                    break;
            }
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}

