using Microsoft.EntityFrameworkCore;
using UserServiceDomain.Entities;

namespace UserServiceInfrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }

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
    }
}