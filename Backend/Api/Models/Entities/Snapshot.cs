


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ChainEmpires.Api.Models.Entities
{
    public class Snapshot
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid EntryId { get; set; }

        [Required]
        public int RoundNumber { get; set; }

        [Required]
        [Column(TypeName = "jsonb")]
        public string Data { get; set; } = string.Empty;

        [Required]
        [MaxLength(66)]
        public string Hash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("EntryId")]
        public virtual Entry Entry { get; set; } = null!;

        public T GetData<T>() where T : class
        {
            return JsonSerializer.Deserialize<T>(Data) ?? throw new InvalidOperationException("Invalid snapshot data");
        }

        public void SetData<T>(T data) where T : class
        {
            Data = JsonSerializer.Serialize(data);
        }
    }
}


