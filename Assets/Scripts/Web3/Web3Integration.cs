











using UnityEngine;
using System.Collections;

namespace ChainEmpires
{
    public class Web3Integration : MonoBehaviour
    {
        [Header("Web3 Settings")]
        public string walletConnectProjectId = "your-project-id";
        public bool autoConnectWallet = true;

        private string connectedWalletAddress;
        private bool isWalletConnected = false;

        void Start()
        {
            Debug.Log("Web3 Integration initialized");

            if (autoConnectWallet)
            {
                StartCoroutine(InitializeWalletConnection());
            }
        }

        private IEnumerator InitializeWalletConnection()
        {
            yield return new WaitForSeconds(1f); // Give time for other systems to initialize

            #if UNITY_WEBGL && !UNITY_EDITOR
            // WebGL-specific wallet connection logic
            Debug.Log("Attempting to connect wallet in WebGL build...");

            // In a real implementation, this would use WalletConnect or similar
            string walletAddress = "0xExampleWalletAddress"; // Placeholder

            if (!string.IsNullOrEmpty(walletAddress))
            {
                connectedWalletAddress = walletAddress;
                isWalletConnected = true;
                Debug.Log($"Wallet connected: {walletAddress}");

                OnWalletConnected();
            }
            else
            {
                Debug.LogWarning("Wallet connection failed");
            }
            #else
            // Standalone/mobile wallet connection logic (e.g., Phantom for Solana)
            Debug.Log("Web3 integration available on mobile/WebGL builds");

            // In a real implementation, this would use native plugins or deep linking
            #endif
        }

        public void ConnectWallet()
        {
            StartCoroutine(InitializeWalletConnection());
        }

        private void OnWalletConnected()
        {
            if (isWalletConnected)
            {
                Debug.Log($"Wallet connected: {connectedWalletAddress}");

                // Initialize Web3 features
                GameManager.Instance.UIManager.ShowNotification("Wallet connected successfully!");

                // Load player NFTs/balances
                StartCoroutine(LoadPlayerAssets());
            }
        }

        private IEnumerator LoadPlayerAssets()
        {
            yield return new WaitForSeconds(1f); // Simulate API call delay

            Debug.Log($"Loading assets for wallet: {connectedWalletAddress}");

            #if UNITY_WEBGL && !UNITY_EDITOR
            // In a real implementation, this would fetch NFTs/tokens from blockchain
            string[] playerNFTs = new string[]
            {
                "Legendary Hero #42",
                "Rare Resource Mine NFT",
                "Epic Castle Blueprint"
            };

            int tokenBalance = 500; // Example balance

            if (playerNFTs.Length > 0)
            {
                Debug.Log($"Found {playerNFTs.Length} NFTs for player:");
                foreach (var nft in playerNFTs)
                {
                    Debug.Log($"- {nft}");
                }

                GameManager.Instance.UIManager.ShowNotification($"Loaded {playerNFTs.Length} NFTs!");
            }

            if (tokenBalance > 0)
            {
                Debug.Log($"Player has {tokenBalance} Chain Crystals");
                GameManager.Instance.ResourceManager.AddResource(ResourceManager.ResourceType.ChainCrystals, tokenBalance);
            }
            #else
            // Standalone/mobile logic would use native APIs
            Debug.Log("NFT/token loading available on mobile/WebGL builds");
            #endif

            yield return null;
        }

        public void MintNFT(string nftName, string metadata)
        {
            if (!isWalletConnected)
            {
                Debug.LogWarning("Cannot mint NFT: Wallet not connected");
                return;
            }

            StartCoroutine(MintNFTRoutine(nftName, metadata));
        }

        private IEnumerator MintNFTRoutine(string nftName, string metadata)
        {
            yield return new WaitForSeconds(2f); // Simulate minting delay

            #if UNITY_WEBGL && !UNITY_EDITOR
            Debug.Log($"Minting NFT: {nftName} with metadata: {metadata}");

            // In a real implementation, this would call Solana/Moralis API
            bool success = true; // Simulated success

            if (success)
            {
                Debug.Log($"NFT minted successfully: {nftName}");
                GameManager.Instance.UIManager.ShowNotification($"Minted NFT: {nftName}!");
            }
            else
            {
                Debug.LogError("NFT minting failed");
                GameManager.Instance.UIManager.ShowError("NFT minting failed. Please try again.");
            }
            #else
            // Standalone/mobile logic would use native APIs
            Debug.Log("NFT minting available on mobile/WebGL builds");
            #endif

            yield return null;
        }

        public void StakeResources(ResourceManager.ResourceType resourceType, float amount)
        {
            if (!isWalletConnected)
            {
                Debug.LogWarning("Cannot stake resources: Wallet not connected");
                return;
            }

            StartCoroutine(StakeResourcesRoutine(resourceType, amount));
        }

        private IEnumerator StakeResourcesRoutine(ResourceManager.ResourceType resourceType, float amount)
        {
            yield return new WaitForSeconds(2f); // Simulate staking delay

            #if UNITY_WEBGL && !UNITY_EDITOR
            Debug.Log($"Staking {amount} {resourceType} for wallet: {connectedWalletAddress}");

            // In a real implementation, this would call staking smart contract
            bool success = true; // Simulated success

            if (success)
            {
                Debug.Log($"Successfully staked {amount} {resourceType}");
                GameManager.Instance.UIManager.ShowNotification($"Staked {amount} {resourceType}!");

                // Update UI and add rewards
                StartCoroutine(ApplyStakingRewards(resourceType, amount));
            }
            else
            {
                Debug.LogError("Resource staking failed");
                GameManager.Instance.UIManager.ShowError("Resource staking failed. Please try again.");
            }
            #else
            // Standalone/mobile logic would use native APIs
            Debug.Log("Resource staking available on mobile/WebGL builds");
            #endif

            yield return null;
        }

        private IEnumerator ApplyStakingRewards(ResourceManager.ResourceType resourceType, float amount)
        {
            yield return new WaitForSeconds(5f); // Simulate reward calculation delay

            float rewardAmount = amount * 0.1f; // 10% daily reward
            Debug.Log($"Applying staking rewards: {rewardAmount} {resourceType}");

            GameManager.Instance.ResourceManager.AddResource(resourceType, rewardAmount);
            GameManager.Instance.UIManager.ShowNotification($"Received staking reward: {rewardAmount} {resourceType}!");

            yield return null;
        }

        public void DisconnectWallet()
        {
            if (isWalletConnected)
            {
                isWalletConnected = false;
                connectedWalletAddress = string.Empty;

                Debug.Log("Wallet disconnected");
                GameManager.Instance.UIManager.ShowNotification("Wallet disconnected.");
            }
        }

        public bool IsWalletConnected()
        {
            return isWalletConnected;
        }

        public string GetConnectedWalletAddress()
        {
            return connectedWalletAddress;
        }
    }
}






