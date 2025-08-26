





using System.Threading.Tasks;
using ChainEmpires.Shared;

namespace ChainEmpires.Client
{
    /// <summary>
    /// Mock implementation of IChainService for development and testing.
    /// This allows the game to function without actual blockchain integration.
    /// </summary>
    public class MockChainService : IChainService
    {
        private readonly InventoryData _mockInventory;

        /// <summary>
        /// Initializes a new instance of the MockChainService with default inventory data.
        /// </summary>
        public MockChainService()
        {
            // Create mock inventory data for testing
            _mockInventory = new InventoryData
            {
                Address = "0xMockAddress123456789",
                CosmeticItems = { "skin_1", "banner_1" },
                GoldBalance = 1000,
                GemBalance = 50
            };
        }

        /// <summary>
        /// Mock login implementation that always succeeds.
        /// </summary>
        public Task<bool> LoginAsync()
        {
            // Simulate successful login with slight delay
            return Task.FromResult(true);
        }

        /// <summary>
        /// Returns the mock inventory data.
        /// </summary>
        public Task<InventoryData> GetInventoryAsync()
        {
            // Return static mock inventory data
            return Task.FromResult(_mockInventory);
        }

        /// <summary>
        /// Mock implementation that always succeeds for minting cosmetics.
        /// </summary>
        public Task<bool> MintCosmeticAsync(string blueprintId)
        {
            // Simulate successful mint with slight delay
            System.Console.WriteLine($"Mock: Minted cosmetic {blueprintId}");
            return Task.FromResult(true);
        }

        /// <summary>
        /// Mock implementation that always succeeds for transferring cosmetics.
        /// </summary>
        public Task<bool> TransferCosmeticAsync(string recipientAddress, string itemId)
        {
            // Simulate successful transfer with slight delay
            System.Console.WriteLine($"Mock: Transferred {itemId} to {recipientAddress}");
            return Task.FromResult(true);
        }
    }
}





