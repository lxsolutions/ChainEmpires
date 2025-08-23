using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem; // For touch input

namespace ChainEmpires
{
    [RequireComponent(typeof(Camera))]
    public class ThirdPersonStrategyCamera : MonoBehaviour
    {
        [Header("Camera Modes")]
        [SerializeField] private CinemachineVirtualCamera overheadCam; // Overhead RTS view
        [SerializeField] private CinemachineVirtualCamera thirdPersonCam; // 3rd-person action view
        [SerializeField] private float zoomThreshold = 10f; // Distance to switch modes

        [Header("Controls")]
        [SerializeField] private float panSpeed = 20f;
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private Vector2 zoomLimits = new Vector2(5f, 50f); // Min/max orthographic size or FOV

        [Header("AR Integration")]
        [SerializeField] private bool arEnabled = false; // Toggle for AR mode
        [SerializeField] private ARSession arSession; // AR Foundation reference

        private Camera mainCam;
        private Vector3 dragOrigin;
        private float currentZoom;

        void Start()
        {
            mainCam = GetComponent<Camera>();
            currentZoom = mainCam.orthographicSize; // Assume ortho for strategy
            SwitchMode(false); // Start in overhead
            if (arEnabled && ARSession.state == ARSessionState.None) arSession.enabled = true;
        }

        void Update()
        {
            HandleTouchInput();
            if (arEnabled) HandleAROverlay();
        }

        private void HandleTouchInput()
        {
            if (Touchscreen.current == null) return;

            // Pan (drag)
            if (Touchscreen.current.primaryTouch.press.isPressed)
            {
                Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
                if (dragOrigin == Vector3.zero) dragOrigin = mainCam.ScreenToWorldPoint(touchPos);
                else
                {
                    Vector3 currentPos = mainCam.ScreenToWorldPoint(touchPos);
                    Vector3 delta = dragOrigin - currentPos;
                    transform.position += delta * panSpeed * Time.deltaTime;
                }
            }
            else dragOrigin = Vector3.zero;

            // Zoom (pinch)
            if (Touchscreen.current.touches.Count >= 2)
            {
                Touch touch0 = Touchscreen.current.touches[0];
                Touch touch1 = Touchscreen.current.touches[1];

                Vector2 prevPos0 = touch0.position.ReadValue() - touch0.delta.ReadValue();
                Vector2 prevPos1 = touch1.position.ReadValue() - touch1.delta.ReadValue();

                float prevDist = Vector2.Distance(prevPos0, prevPos1);
                float currDist = Vector2.Distance(touch0.position.ReadValue(), touch1.position.ReadValue());

                float delta = currDist - prevDist;
                currentZoom -= delta * zoomSpeed * 0.01f;
                currentZoom = Mathf.Clamp(currentZoom, zoomLimits.x, zoomLimits.y);

                mainCam.orthographicSize = currentZoom; // Or adjust FOV for perspective

                // Innovative Switch: Auto-blend modes on zoom
                bool isThirdPerson = currentZoom < zoomThreshold;
                SwitchMode(isThirdPerson);
            }
        }

        private void SwitchMode(bool isThirdPerson)
        {
            overheadCam.Priority = isThirdPerson ? 0 : 10;
            thirdPersonCam.Priority = isThirdPerson ? 10 : 0;
            mainCam.orthographic = !isThirdPerson; // Switch projection if needed
            Debug.Log(isThirdPerson ? "Switched to 3rd-person action view" : "Switched to overhead strategy view");
        }

        private void HandleAROverlay()
        {
            // Innovative AR: Scan real-world for "import" buffs (e.g., tree = forest resource)
            if (ARSession.state == ARSessionState.SessionTracking)
            {
                // Placeholder: Use ARFoundation to detect planes/objects
                Debug.Log("AR active: Scanning real-world for custom buffs");
                // Example: If detect tree, add temp resource bonus via ResourceManager
            }
        }

        // Optimization: Cull distant objects for mobile perf
        void LateUpdate()
        {
            // Use Layer Cull Distances or LODGroup for efficiency
        }
    }
}