

using UnityEngine;
using UnityEngine.UI;

namespace ChainEmpires.UI
{
    public class MinimapManager : MonoBehaviour
    {
        [Header("Minimap Settings")]
        public Camera minimapCamera;
        public RawImage minimapImage;
        public RenderTexture minimapRenderTexture;
        
        [Header("Minimap Configuration")]
        public float mapSize = 100f;
        public float cameraHeight = 50f;
        public bool showFogOfWar = true;
        public float explorationRadius = 20f;
        
        [Header("Icons")]
        public Texture2D playerIcon;
        public Texture2D enemyIcon;
        public Texture2D resourceIcon;
        public Texture2D buildingIcon;
        
        private Texture2D fogOfWarTexture;
        private Color32[] fogPixels;
        private int textureSize = 256;
        
        private void Start()
        {
            InitializeMinimap();
            InitializeFogOfWar();
        }
        
        private void InitializeMinimap()
        {
            if (minimapCamera != null)
            {
                // Set up orthographic camera for top-down view
                minimapCamera.orthographic = true;
                minimapCamera.orthographicSize = mapSize / 2f;
                minimapCamera.transform.position = new Vector3(0, cameraHeight, 0);
                minimapCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
                
                // Create render texture if not assigned
                if (minimapRenderTexture == null)
                {
                    minimapRenderTexture = new RenderTexture(textureSize, textureSize, 16);
                    minimapRenderTexture.Create();
                }
                
                minimapCamera.targetTexture = minimapRenderTexture;
                
                if (minimapImage != null)
                {
                    minimapImage.texture = minimapRenderTexture;
                }
            }
        }
        
        private void InitializeFogOfWar()
        {
            if (showFogOfWar)
            {
                fogOfWarTexture = new Texture2D(textureSize, textureSize);
                fogPixels = new Color32[textureSize * textureSize];
                
                // Initialize all pixels to black (unexplored)
                for (int i = 0; i < fogPixels.Length; i++)
                {
                    fogPixels[i] = new Color32(0, 0, 0, 200); // Black with alpha
                }
                
                fogOfWarTexture.SetPixels32(fogPixels);
                fogOfWarTexture.Apply();
            }
        }
        
        private void Update()
        {
            UpdateMinimapCamera();
            
            if (showFogOfWar)
            {
                UpdateFogOfWar();
            }
        }
        
        private void UpdateMinimapCamera()
        {
            // Follow player or center on base
            // This would be implemented based on your game's camera system
            /*
            if (GameManager.Instance?.PlayerController != null)
            {
                Vector3 playerPos = GameManager.Instance.PlayerController.transform.position;
                minimapCamera.transform.position = new Vector3(playerPos.x, cameraHeight, playerPos.z);
            }
            */
        }
        
        private void UpdateFogOfWar()
        {
            // Update fog of war based on player/unit positions
            // This is a simplified implementation
            
            /*
            if (GameManager.Instance?.PlayerController != null)
            {
                Vector3 playerPos = GameManager.Instance.PlayerController.transform.position;
                RevealArea(playerPos, explorationRadius);
            }
            
            // Also reveal around units and buildings
            foreach (var unit in GameManager.Instance?.UnitManager?.GetAllUnits() ?? new List<Unit>())
            {
                RevealArea(unit.transform.position, explorationRadius * 0.5f);
            }
            
            foreach (var building in GameManager.Instance?.BuildingManager?.GetAllBuildings() ?? new List<Building>())
            {
                RevealArea(building.transform.position, explorationRadius * 0.7f);
            }
            */
            
            fogOfWarTexture.SetPixels32(fogPixels);
            fogOfWarTexture.Apply();
        }
        
        private void RevealArea(Vector3 worldPosition, float radius)
        {
            // Convert world position to minimap texture coordinates
            Vector2 normalizedPos = WorldToMinimapUV(worldPosition);
            int centerX = Mathf.RoundToInt(normalizedPos.x * textureSize);
            int centerY = Mathf.RoundToInt(normalizedPos.y * textureSize);
            
            int pixelRadius = Mathf.RoundToInt(radius / mapSize * textureSize);
            
            for (int x = -pixelRadius; x <= pixelRadius; x++)
            {
                for (int y = -pixelRadius; y <= pixelRadius; y++)
                {
                    int pixelX = centerX + x;
                    int pixelY = centerY + y;
                    
                    if (pixelX >= 0 && pixelX < textureSize && pixelY >= 0 && pixelY < textureSize)
                    {
                        float distance = Mathf.Sqrt(x * x + y * y);
                        if (distance <= pixelRadius)
                        {
                            // Reveal this pixel (set to transparent)
                            int index = pixelY * textureSize + pixelX;
                            fogPixels[index] = new Color32(0, 0, 0, 0);
                        }
                    }
                }
            }
        }
        
        private Vector2 WorldToMinimapUV(Vector3 worldPosition)
        {
            // Convert world position to normalized minimap coordinates (0-1)
            float normalizedX = (worldPosition.x + mapSize / 2f) / mapSize;
            float normalizedZ = (worldPosition.z + mapSize / 2f) / mapSize;
            
            return new Vector2(Mathf.Clamp01(normalizedX), Mathf.Clamp01(normalizedZ));
        }
        
        public void ToggleMinimap(bool show)
        {
            if (minimapImage != null)
            {
                minimapImage.gameObject.SetActive(show);
            }
        }
        
        public void SetMinimapSize(float size)
        {
            mapSize = Mathf.Max(10f, size);
            if (minimapCamera != null)
            {
                minimapCamera.orthographicSize = mapSize / 2f;
            }
        }
        
        // Call this from UIManager to update the minimap display
        public void UpdateMinimapDisplay()
        {
            // Update any minimap overlays or icons here
        }
        
        private void OnDestroy()
        {
            if (minimapRenderTexture != null)
            {
                minimapRenderTexture.Release();
            }
            
            if (fogOfWarTexture != null)
            {
                Destroy(fogOfWarTexture);
            }
        }
    }
}

