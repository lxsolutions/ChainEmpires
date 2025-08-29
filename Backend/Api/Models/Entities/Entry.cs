


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChainEmpires.Api.Models.Entities
{
    public class Entry
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TournamentId { get; set; }

        public Guid? TableId { get; set; }

        [Required]
        [MaxLength(42)]
        public string PlayerAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string BaseNFTTokenId { get; set; } = string.Empty;

        [Required]
        public bool IsActive { get; set; } = true;

        [Column(TypeName = "decimal(18,2)")]
        public decimal MMR { get; set; } = 1000m;

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
        public DateTime? EliminatedAt { get; set; }

        // Navigation properties
        [ForeignKey("TournamentId")]
        public virtual Tournament Tournament { get; set; } = null!;

        [ForeignKey("TableId")]
        public virtual Table? Table { get; set; }

        public virtual ICollection<Snapshot> Snapshots { get; set; } = new List<Snapshot>();

        public int CurrentRound => Snapshots.Max(s => (int?)s.RoundNumber) ?? 0;
    }
}


