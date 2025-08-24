




using UnityEngine;
using System.Collections;

namespace ChainEmpires
{
    public class TestWeb3Async : MonoBehaviour
    {
        [SerializeField] private Web3Integration web3Integration;

        void Start()
        {
            Debug.Log("Testing Web3 Async Integration...");

            if (web3Integration != null)
            {
                // Test wallet connection
                Debug.Log("Connecting wallet...");
                web3Integration.ConnectWallet();

                // Wait a bit then test NFT minting
                StartCoroutine(TestMintAfterDelay());
            }
            else
            {
                Debug.LogError("Web3Integration component not assigned!");
            }
        }

        private IEnumerator TestMintAfterDelay()
        {
            yield return new WaitForSeconds(5f);

            if (web3Integration != null)
            {
                Debug.Log("Testing async NFT minting...");
                web3Integration.MintNFT("Test Async Hero", "{\"name\":\"Async Test Hero\", \"attributes\":[{\"trait_type\":\"Speed\",\"value\":8}]}");
            }
        }

        private IEnumerator TestStakingAfterDelay()
        {
            yield return new WaitForSeconds(10f);

            if (web3Integration != null)
            {
                Debug.Log("Testing async resource staking...");
                web3Integration.StakeResources(ResourceManager.ResourceType.ChainCrystals, 500f);
            }
        }
    }
}




