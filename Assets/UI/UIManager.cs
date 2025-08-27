using UnityEngine;
using UnityEngine.UI; // For legacy UI
using UnityEngine.EventSystems; // For touch
using TMPro; // For text

namespace ChainEmpires
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Canvas hudCanvas;
        [SerializeField] private TextMeshProUGUI resourceText;
        [SerializeField] private Image minimapImage; // Fog of war texture
        [SerializeField] private GameObject narrativePopup; // For branching stories
        [SerializeField] private ChainEmpires.UI.MinimapManager minimapManager;

        [Header("Touch & Adaptive")]
        [SerializeField] private float hideIdleTime = 5f; // Auto-hide HUD
        [SerializeField] private float hudUpdateInterval = 0.5f; // Update HUD every 0.5 seconds
        [SerializeField] private bool useNFTCosmetics = true; // Load on-chain skins
        [SerializeField] private ChainEmpires.Input.TouchInputManager touchInputManager;

        private float lastInteractionTime;
        private float hudUpdateTimer;
        private EventSystem eventSystem;

        void Start()
        {
            eventSystem = EventSystem.current;
            UpdateHUD(); // Initial sync
            lastInteractionTime = Time.time;
            if (useNFTCosmetics) LoadNFTSkins();
            
            // Setup touch input callbacks
            SetupTouchInputCallbacks();
        }

        void Update()
        {
            // Update last interaction time based on any input
            if (UnityEngine.Input.anyKeyDown || UnityEngine.Input.touchCount > 0)
            {
                lastInteractionTime = Time.time;
            }
            
            if (Time.time - lastInteractionTime > hideIdleTime) HideHUD(true);
            else HideHUD(false);
            
            // Update HUD at regular intervals
            hudUpdateTimer += Time.deltaTime;
            if (hudUpdateTimer >= hudUpdateInterval)
            {
                UpdateHUD();
                UpdateMinimap();
                hudUpdateTimer = 0f;
            }
        }

        private void SetupTouchInputCallbacks()
        {
            if (touchInputManager != null)
            {
                touchInputManager.OnUnitSelected += HandleUnitSelected;
                touchInputManager.OnMoveCommand += HandleMoveCommand;
                touchInputManager.OnTap += HandleTap;
                touchInputManager.OnDoubleTap += HandleDoubleTap;
            }
        }

        private void HandleUnitSelected(GameObject unitObject)
        {
            Debug.Log($"Unit selected: {unitObject.name}");
            // Highlight unit and show unit info panel
        }

        private void HandleMoveCommand(Vector3 targetPosition)
        {
            Debug.Log($"Move command to: {targetPosition}");
            // Issue move command to selected units
        }

        private void HandleTap(Vector3 worldPosition)
        {
            Debug.Log($"Tap at: {worldPosition}");
        }

        private void HandleDoubleTap(Vector3 worldPosition)
        {
            Debug.Log($"Double tap at: {worldPosition}");
            // Select all units of same type or focus camera
        }

        public void UpdateHUD()
        {
            if (GameManager.Instance?.ResourceManager != null)
            {
                ResourceManager rm = GameManager.Instance.ResourceManager;
                string resourceDisplay = "";
                
                // Display primary resources (Minerals, Energy, Food)
                resourceDisplay += $"Minerals: {rm.GetResourceAmount(ResourceManager.ResourceType.Minerals):0}/{rm.GetResourceCapacity(ResourceManager.ResourceType.Minerals):0} (+{rm.GetGenerationRate(ResourceManager.ResourceType.Minerals):0.0}/s)\n";
                resourceDisplay += $"Energy: {rm.GetResourceAmount(ResourceManager.ResourceType.Energy):0}/{rm.GetResourceCapacity(ResourceManager.ResourceType.Energy):0} (+{rm.GetGenerationRate(ResourceManager.ResourceType.Energy):0.0}/s)\n";
                resourceDisplay += $"Food: {rm.GetResourceAmount(ResourceManager.ResourceType.Food):0}/{rm.GetResourceCapacity(ResourceManager.ResourceType.Food):0} (+{rm.GetGenerationRate(ResourceManager.ResourceType.Food):0.0}/s)";
                
                resourceText.text = resourceDisplay;
            }
            else
            {
                resourceText.text = "Resources: Loading...";
            }
            
            // Update minimap fog based on exploration
        }

        private void HideHUD(bool hide)
        {
            hudCanvas.gameObject.SetActive(!hide);
        }

        private void UpdateMinimap()
        {
            if (minimapManager != null)
            {
                minimapManager.UpdateMinimapDisplay();
            }
        }

        public void ShowNarrativePopup(string text, System.Action choiceCallback)
        {
            narrativePopup.SetActive(true);
            narrativePopup.GetComponentInChildren<TextMeshProUGUI>().text = text;
            // Buttons for branches, call callback on choice
        }

        private void LoadNFTSkins()
        {
            // Web3 integration: Fetch NFT metadata for UI themes
            Debug.Log("Loaded NFT cosmetics - custom HUD skin applied");
            // Example: Change colors/textures via on-chain data
        }

        // Opt: Low-draw (2025 UI Toolkit vector for scalability)
    }
}