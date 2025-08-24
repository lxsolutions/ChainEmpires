




using UnityEngine;

namespace ChainEmpires
{
    public class TestP2ESimulator : MonoBehaviour
    {
        [SerializeField] private P2ESimulator p2eSimulator;

        void Start()
        {
            Debug.Log("Testing P2E Simulator...");

            if (p2eSimulator != null)
            {
                // Run the extended simulation
                Debug.Log("Starting extended P2E simulation...");
                p2eSimulator.StartExtendedSimulation();
            }
            else
            {
                Debug.LogError("P2ESimulator component not assigned!");
            }
        }
    }
}




