

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChainEmpires.Api.Models.Entities
{
    public enum TournamentStatus
    {
        Draft,
        Registration,
        InProgress,
        Completed,
        Cancelled
    }

    public class Tournament
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,8)")]
        public decimal BuyInAmount { get; set; }

        [Required]
        [MaxLength(50)]
        public string BuyInToken { get; set; } = string.Empty;

        [Required]
        public int TableSize { get; set; } = 8;

        [Required]
        public int AdvanceCount { get; set; } = 1;

        [Required]
        public int RakeBps { get; set; } = 200; // 2% in basis points

        [Required]
        public TournamentStatus Status { get; set; } = TournamentStatus.Draft;

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Table> Tables { get; set; } = new List<Table>();
        public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();

        public decimal TotalPrizePool => Entries.Count * BuyInAmount * (1 - RakeBps / 10000m);
    }
}

