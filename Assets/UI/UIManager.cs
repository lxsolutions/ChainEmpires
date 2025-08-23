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
        [SerializeField] private Image minimap; // Fog of war texture
        [SerializeField] private GameObject narrativePopup; // For branching stories

        [Header("Touch & Adaptive")]
        [SerializeField] private float hideIdleTime = 5f; // Auto-hide HUD
        [SerializeField] private bool useNFTCosmetics = true; // Load on-chain skins

        private float lastInteractionTime;
        private EventSystem eventSystem;

        void Start()
        {
            eventSystem = EventSystem.current;
            UpdateHUD(); // Initial sync
            lastInteractionTime = Time.time;
            if (useNFTCosmetics) LoadNFTSkins();
        }

        void Update()
        {
            HandleTouch();
            if (Time.time - lastInteractionTime > hideIdleTime) HideHUD(true);
            else HideHUD(false);
        }

        private void HandleTouch()
        {
            if (Input.touchCount > 0)
            {
                lastInteractionTime = Time.time;
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    // Drag-select or command
                    if (!eventSystem.IsPointerOverGameObject(touch.fingerId)) SelectUnits(touch.position);
                }
            }
        }

        private void SelectUnits(Vector2 position)
        {
            // Raycast for unit selection
            Ray ray = Camera.main.ScreenPointToRay(position);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag("Unit"))
            {
                // Highlight/queue actions
                Debug.Log("Selected unit: " + hit.collider.name);
            }
        }

        public void UpdateHUD()
        {
            resourceText.text = $"Minerals: {ResourceManager.Instance.Minerals} Energy: {ResourceManager.Instance.Energy}";
            // Update minimap fog based on exploration
        }

        private void HideHUD(bool hide)
        {
            hudCanvas.gameObject.SetActive(!hide);
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