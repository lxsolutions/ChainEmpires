


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChainEmpires.Api.Models.Entities
{
    public class Result
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TableId { get; set; }

        [Required]
        public Guid WinnerEntryId { get; set; }

        [Required]
        [MaxLength(66)]
        public string LogHash { get; set; } = string.Empty;

        [MaxLength(66)]
        public string? MerkleRoot { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        public bool CommittedToChain { get; set; } = false;
        public string? ChainTransactionHash { get; set; }

        // Navigation properties
        [ForeignKey("TableId")]
        public virtual Table Table { get; set; } = null!;

        [ForeignKey("WinnerEntryId")]
        public virtual Entry WinnerEntry { get; set; } = null!;
    }
}


