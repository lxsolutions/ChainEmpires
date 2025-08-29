

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChainEmpires.Api.Models.Entities
{
    public enum TableStatus
    {
        Waiting,
        InProgress,
        Completed,
        Cancelled
    }

    public class Table
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TournamentId { get; set; }

        [Required]
        public int RoundNumber { get; set; } = 1;

        [Required]
        public TableStatus Status { get; set; } = TableStatus.Waiting;

        public Guid? WinnerEntryId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("TournamentId")]
        public virtual Tournament Tournament { get; set; } = null!;

        public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();

        [ForeignKey("WinnerEntryId")]
        public virtual Entry? WinnerEntry { get; set; }
    }
}

