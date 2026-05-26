using Microsoft.EntityFrameworkCore;
using UserServiceDomain.Entities;

namespace UserServiceInfrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<TrainerProfile> TrainerProfiles { get; set; }
    public DbSet<TrainerReview> TrainerReviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.HasIndex(u => u.Username).IsUnique();

            entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(u => u.Email).IsUnique();

            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(u => u.LastName).IsRequired().HasMaxLength(50);
            entity.Property(u => u.Location).HasMaxLength(100);

            // Enum-ove čuva kao stringove (čitljivije u bazi)
            entity.Property(u => u.Gender).HasConversion<string>().HasMaxLength(20);
            entity.Property(u => u.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
        });

        // === TrainerProfile ===
        modelBuilder.Entity<TrainerProfile>(entity =>
        {
            entity.HasKey(tp => tp.Id);

            entity.Property(tp => tp.Specialization).HasMaxLength(200);
            entity.Property(tp => tp.Description).HasMaxLength(2000);

            // 1:1 veza sa User-om
            entity.HasOne(tp => tp.User)
                .WithOne(u => u.TrainerProfile)
                .HasForeignKey<TrainerProfile>(tp => tp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(tp => tp.UserId).IsUnique();
        });

        // === TrainerReview ===
        modelBuilder.Entity<TrainerReview>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.Rating).IsRequired();
            entity.Property(r => r.Comment).HasMaxLength(1000);

            // Veza ka treneru
            entity.HasOne(r => r.Trainer)
                .WithMany()
                .HasForeignKey(r => r.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Veza ka klijentu
            entity.HasOne(r => r.Client)
                .WithMany()
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(r => r.TrainerId);
        });
    }
}