
using UnityEngine;
using UnityEngine.EventSystems;

namespace ChainEmpires.Input
{
    public class TouchInputManager : MonoBehaviour
    {
        [Header("Touch Settings")]
        public float dragThreshold = 10f;
        public float tapTimeThreshold = 0.2f;
        public float doubleTapTimeThreshold = 0.3f;
        
        [Header("Selection")]
        public LayerMask selectableLayers = -1;
        public LayerMask groundLayer = 1;
        
        private Vector2 touchStartPosition;
        private float touchStartTime;
        private float lastTapTime;
        private bool isDragging = false;
        
        // Events
        public System.Action<Vector3> OnTap;
        public System.Action<Vector3> OnDoubleTap;
        public System.Action<Vector3, Vector3> OnDragStart;
        public System.Action<Vector3, Vector3> OnDrag;
        public System.Action<Vector3, Vector3> OnDragEnd;
        public System.Action<GameObject> OnUnitSelected;
        public System.Action<Vector3> OnMoveCommand;
        
        private void Update()
        {
            HandleTouchInput();
        }
        
        private void HandleTouchInput()
        {
            if (UnityEngine.Input.touchCount > 0)
            {
                Touch touch = UnityEngine.Input.GetTouch(0);
                
                // Skip if touching UI
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return;
                
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchStartPosition = touch.position;
                        touchStartTime = Time.time;
                        isDragging = false;
                        break;
                        
                    case TouchPhase.Moved:
                        if (!isDragging && Vector2.Distance(touch.position, touchStartPosition) > dragThreshold)
                        {
                            isDragging = true;
                            Vector3 worldStart = ScreenToWorldPoint(touchStartPosition);
                            Vector3 worldCurrent = ScreenToWorldPoint(touch.position);
                            OnDragStart?.Invoke(worldStart, worldCurrent);
                        }
                        
                        if (isDragging)
                        {
                            Vector3 worldStart = ScreenToWorldPoint(touchStartPosition);
                            Vector3 worldCurrent = ScreenToWorldPoint(touch.position);
                            OnDrag?.Invoke(worldStart, worldCurrent);
                        }
                        break;
                        
                    case TouchPhase.Ended:
                        if (isDragging)
                        {
                            Vector3 worldStart = ScreenToWorldPoint(touchStartPosition);
                            Vector3 worldEnd = ScreenToWorldPoint(touch.position);
                            OnDragEnd?.Invoke(worldStart, worldEnd);
                        }
                        else
                        {
                            HandleTap(touch.position);
                        }
                        break;
                }
            }
        }
        
        private void HandleTap(Vector2 screenPosition)
        {
            float tapDuration = Time.time - touchStartTime;
            
            if (tapDuration <= tapTimeThreshold)
            {
                Vector3 worldPosition = ScreenToWorldPoint(screenPosition);
                
                // Check for double tap
                if (Time.time - lastTapTime <= doubleTapTimeThreshold)
                {
                    OnDoubleTap?.Invoke(worldPosition);
                    lastTapTime = 0f;
                }
                else
                {
                    OnTap?.Invoke(worldPosition);
                    lastTapTime = Time.time;
                    
                    // Try to select unit or issue move command
                    TrySelectUnit(screenPosition);
                }
            }
        }
        
        private void TrySelectUnit(Vector2 screenPosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectableLayers))
            {
                if (hit.collider.CompareTag("Unit"))
                {
                    OnUnitSelected?.Invoke(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Building"))
                {
                    // Handle building selection
                    Debug.Log($"Building selected: {hit.collider.name}");
                }
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                // Ground tap - issue move command to selected units
                OnMoveCommand?.Invoke(hit.point);
            }
        }
        
        private Vector3 ScreenToWorldPoint(Vector2 screenPosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                return hit.point;
            }
            
            // Fallback: project onto ground plane at y=0
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float distance;
            if (groundPlane.Raycast(ray, out distance))
            {
                return ray.GetPoint(distance);
            }
            
            return Vector3.zero;
        }
        
        public void EnableInput(bool enable)
        {
            this.enabled = enable;
        }
        
        // Mobile-optimized pinch-to-zoom and rotation
        public void HandlePinchZoom(float pinchAmount)
        {
            // Implement camera zoom
            // Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - pinchAmount, minZoom, maxZoom);
        }
        
        public void HandleRotation(float rotationAmount)
        {
            // Implement camera rotation
            // Camera.main.transform.RotateAround(focusPoint, Vector3.up, rotationAmount);
        }
    }
}
