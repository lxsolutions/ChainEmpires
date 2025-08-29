using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChainEmpires.Api.Models.Entities
{
    public class Payout
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TournamentId { get; set; }

        [Required]
        public Guid EntryId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,8)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(50)]
        public string Token { get; set; } = string.Empty;

        [Required]
        public int Position { get; set; }

        public bool Distributed { get; set; } = false;
        public string? DistributionTransactionHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DistributedAt { get; set; }

        // Navigation properties
        [ForeignKey("TournamentId")]
        public virtual Tournament Tournament { get; set; } = null!;

        [ForeignKey("EntryId")]
        public virtual Entry Entry { get; set; } = null!;
    }
}