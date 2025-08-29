
using System.ComponentModel.DataAnnotations;

namespace ChainEmpires.Api.Models.DTOs
{
    public class CreateTournamentRequest
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.0001, double.MaxValue)]
        public decimal BuyInAmount { get; set; }

        [Required]
        [MaxLength(50)]
        public string BuyInToken { get; set; } = string.Empty;

        [Required]
        [Range(2, 16)]
        public int TableSize { get; set; } = 8;

        [Required]
        [Range(1, 8)]
        public int AdvanceCount { get; set; } = 1;

        [Required]
        [Range(0, 1000)]
        public int RakeBps { get; set; } = 200;

        [Required]
        public DateTime StartTime { get; set; }
    }

    public class TournamentResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal BuyInAmount { get; set; }
        public string BuyInToken { get; set; } = string.Empty;
        public int TableSize { get; set; }
        public int AdvanceCount { get; set; }
        public int RakeBps { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int EntryCount { get; set; }
        public decimal TotalPrizePool { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class BuyInRequest
    {
        [Required]
        [MaxLength(42)]
        public string PlayerAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string BaseNFTTokenId { get; set; } = string.Empty;
    }

    public class BuyInResponse
    {
        public Guid EntryId { get; set; }
        public string PaymentAddress { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    public class TableAssignmentResponse
    {
        public Guid TableId { get; set; }
        public int RoundNumber { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<TableOpponent> Opponents { get; set; } = new();
        public DateTime? StartTime { get; set; }
    }

    public class TableOpponent
    {
        public string BaseNFTTokenId { get; set; } = string.Empty;
        public decimal MMR { get; set; }
        public int CurrentRound { get; set; }
    }

    public class SubmitResultRequest
    {
        [Required]
        [MaxLength(66)]
        public string LogHash { get; set; } = string.Empty;

        [MaxLength(66)]
        public string? MerkleRoot { get; set; }
    }

    public class AdvanceRequest
    {
        [Required]
        public Guid TableId { get; set; }

        [Required]
        public Guid WinnerEntryId { get; set; }
    }

    public class PayoutResponse
    {
        public Guid EntryId { get; set; }
        public string PlayerAddress { get; set; } = string.Empty;
        public string BaseNFTTokenId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Token { get; set; } = string.Empty;
        public int Position { get; set; }
        public bool Distributed { get; set; }
        public string? TransactionHash { get; set; }
    }
}
