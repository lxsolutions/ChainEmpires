





using System.Threading.Tasks;

namespace ChainEmpires.Shared
{
    /// <summary>
    /// Interface for chain services that abstracts Web3 integration.
    /// This allows the game to work with different blockchain platforms
    /// while maintaining a consistent API surface.
    /// </summary>
    public interface IChainService
    {
        /// <summary>
        /// Authenticates the user with the blockchain service.
        /// </summary>
        Task<bool> LoginAsync();

        /// <summary>
        /// Gets the player's inventory from the blockchain.
        /// </summary>
        Task<InventoryData> GetInventoryAsync();

        /// <summary>
        /// Mints a cosmetic item on-chain and adds it to player inventory.
        /// </summary>
        Task<bool> MintCosmeticAsync(string blueprintId);

        /// <summary>
        /// Transfers a cosmetic item from one player to another.
        /// </summary>
        Task<bool> TransferCosmeticAsync(string recipientAddress, string itemId);
    }

    /// <summary>
    /// Data transfer object for inventory information.
    /// </summary>
    public class InventoryData
    {
        /// <summary>
        /// Player's blockchain address.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// List of cosmetic item IDs owned by the player.
        /// </summary>
        public string[] CosmeticItems { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Soft currency balance (Gold).
        /// </summary>
        public int GoldBalance { get; set; }

        /// <summary>
        /// Premium currency balance (Gems).
        /// </summary>
        public int GemBalance { get; set; }
    }
}





