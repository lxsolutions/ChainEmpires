


using UnityEngine;

namespace ChainEmpires
{
    public class TestWeb3Integration : MonoBehaviour
    {
        [SerializeField] private Web3Integration web3Integration;

        void Start()
        {
            Debug.Log("Testing Web3 Integration...");

            // Test wallet connection
            if (web3Integration != null)
            {
                Debug.Log("Web3Integration component found. Testing wallet connection...");
                web3Integration.ConnectWallet();
            }
            else
            {
                Debug.LogError("Web3Integration component not assigned!");
            }

            // Test NFT minting
            StartCoroutine(TestMintNFT());
        }

        private IEnumerator TestMintNFT()
        {
            yield return new WaitForSeconds(5f); // Give wallet connection time

            if (web3Integration != null)
            {
                Debug.Log("Testing NFT minting...");
                web3Integration.MintNFT("Test Hero NFT", "{\"name\":\"Test Hero\", \"attributes\":[{\"trait_type\":\"Strength\",\"value\":10}]}");
            }
        }
    }
}


