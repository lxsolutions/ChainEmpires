
using Microsoft.EntityFrameworkCore;
using ChainEmpires.Api.Models.Entities;

namespace ChainEmpires.Api.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Snapshot> Snapshots { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Payout> Payouts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tournament configuration
            modelBuilder.Entity<Tournament>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Name).IsRequired().HasMaxLength(200);
                entity.Property(t => t.BuyInAmount).HasColumnType("decimal(18,8)");
                entity.Property(t => t.BuyInToken).IsRequired().HasMaxLength(50);
                entity.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
                entity.HasMany(t => t.Tables).WithOne(t => t.Tournament).HasForeignKey(t => t.TournamentId);
                entity.HasMany(t => t.Entries).WithOne(e => e.Tournament).HasForeignKey(e => e.TournamentId);
            });

            // Table configuration
            modelBuilder.Entity<Table>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
                entity.HasMany(t => t.Entries).WithOne(e => e.Table).HasForeignKey(e => e.TableId);
            });

            // Entry configuration
            modelBuilder.Entity<Entry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PlayerAddress).IsRequired().HasMaxLength(42);
                entity.Property(e => e.BaseNFTTokenId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.MMR).HasColumnType("decimal(18,2)");
                entity.HasMany(e => e.Snapshots).WithOne(s => s.Entry).HasForeignKey(s => s.EntryId);
            });

            // Snapshot configuration
            modelBuilder.Entity<Snapshot>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Data).HasColumnType("jsonb");
            });

            // Result configuration
            modelBuilder.Entity<Result>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.LogHash).IsRequired().HasMaxLength(66);
                entity.Property(r => r.MerkleRoot).HasMaxLength(66);
            });

            // Payout configuration
            modelBuilder.Entity<Payout>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Amount).HasColumnType("decimal(18,8)");
                entity.Property(p => p.Token).IsRequired().HasMaxLength(50);
            });
        }
    }
}
