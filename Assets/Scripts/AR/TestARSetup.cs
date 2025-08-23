











using UnityEngine;

namespace ChainEmpires
{
    public class TestARSetup : MonoBehaviour
    {
        [Header("Test AR Setup")]
        public GameObject testDecorationPrefab;
        public Transform[] testPositions;

        void Start()
        {
            Debug.Log("Test AR setup initialized");

            // Enable AR mode
            ARManager arManager = FindObjectOfType<ARManager>();
            if (arManager != null)
            {
                arManager.EnableAR(true);

                // Test surface scanning
                arManager.ScanSurfaceForResourceBonus();

                // Test placing a decoration
                if (testPositions.Length > 0 && testDecorationPrefab != null)
                {
                    Vector3 position = testPositions[0].position;
                    arManager.PlaceARDecoration(testDecorationPrefab, position);
                }
            }

            Debug.Log("Test AR setup complete");
        }
    }
}






