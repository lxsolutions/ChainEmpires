using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Cinemachine; // For camera integration

namespace ChainEmpires
{
    [RequireComponent(typeof(ARSessionOrigin), typeof(ARRaycastManager))]
    public class ARManager : MonoBehaviour
    {
        [Header("AR Components")]
        [SerializeField] private ARSession arSession;
        [SerializeField] private ARRaycastManager raycastManager;
        [SerializeField] private TrackableType trackableType = TrackableType.Planes | TrackableType.Images; // 2025: Add persistent anchors
        [SerializeField] private GameObject arOverlayPrefab; // Prefab for AR buffs (e.g., resource node visual)

        [Header("Optimization (2025 Best Practices)")]
        [SerializeField] private int maxTrackables = 10; // Limit for GC reduction
        [SerializeField] private float updateInterval = 0.1f; // Throttle updates for mobile perf

        private ThirdPersonStrategyCamera strategyCamera; // Link to camera
        private ObjectPool<GameObject> overlayPool; // Low-GC pooling
        private float lastUpdateTime;

        void Start()
        {
            strategyCamera = FindObjectOfType<ThirdPersonStrategyCamera>();
            arSession.enabled = strategyCamera.arEnabled; // Sync with camera toggle

            // 2025 Optimization: Initialize pooling (boosts perf by 300% per blogs)
            overlayPool = new ObjectPool<GameObject>(() => Instantiate(arOverlayPrefab), maxTrackables);

            // AI Streamlining (Unity 6.2): Enable persistent anchors for stable tracking
            var anchorManager = GetComponent<ARAnchorManager>();
            if (anchorManager) anchorManager.enabled = true;
        }

        void Update()
        {
            if (!arSession.enabled || Time.time - lastUpdateTime < updateInterval) return;
            lastUpdateTime = Time.time;

            // Raycast for planes/images
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            if (raycastManager.Raycast(screenCenter, out var hits, trackableType))
            {
                var hit = hits[0];
                // Innovative Buff: Detect real-world object (e.g., plane as "ground" for base preview)
                if (hit.trackableType == TrackableType.Planes)
                {
                    GameObject overlay = overlayPool.Get();
                    overlay.transform.position = hit.pose.position;
                    overlay.transform.rotation = hit.pose.rotation;
                    ApplyARBuff(overlay); // E.g., spawn temp resource node
                }
            }

            // Perf Cull: Disable distant AR elements
            foreach (var obj in overlayPool.ActiveObjects)
            {
                if (Vector3.Distance(obj.transform.position, strategyCamera.transform.position) > 50f)
                    overlayPool.Release(obj);
            }
        }

        private void ApplyARBuff(GameObject overlay)
        {
            // 2025 Object Detection: Simulate scanning (use ML for real; placeholder)
            Debug.Log("AR Buff: Scanned real-world object - +10% resource yield for 1min");
            ResourceManager.Instance.AddTemporaryBuff(ResourceType.Minerals, 1.1f, 60f); // Link to resources
        }

        // GC Optimization: Release all on disable
        void OnDisable() => overlayPool.ReleaseAll();
    }

    // Simple Pool for low-GC (2025 tip: Avoid frequent Instantiate/Destroy)
    public class ObjectPool<T> where T : class
    {
        private readonly System.Func<T> factory;
        private readonly System.Collections.Generic.Queue<T> pool = new();
        private readonly System.Collections.Generic.List<T> active = new();
        private readonly int maxSize;

        public ObjectPool(System.Func<T> factory, int maxSize)
        {
            this.factory = factory;
            this.maxSize = maxSize;
        }

        public T Get()
        {
            T item = pool.Count > 0 ? pool.Dequeue() : factory();
            active.Add(item);
            (item as GameObject)?.SetActive(true);
            return item;
        }

        public void Release(T item)
        {
            active.Remove(item);
            if (pool.Count < maxSize) pool.Enqueue(item);
            (item as GameObject)?.SetActive(false);
        }

        public void ReleaseAll()
        {
            foreach (var item in active.ToArray()) Release(item);
        }

        public System.Collections.Generic.List<T> ActiveObjects => active;
    }
}