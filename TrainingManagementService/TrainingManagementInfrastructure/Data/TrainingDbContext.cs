using Microsoft.EntityFrameworkCore;
using TrainingManagementDomain.Entities;

namespace TrainingManagementInfrastructure.Data
{
    public class TrainingDbContext : DbContext
    {
        public TrainingDbContext(DbContextOptions<TrainingDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<TrainingPlan> TrainingPlans { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<TrainingSession> TrainingSessions { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === Category ===
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Description)
                    .HasMaxLength(500);

                entity.HasIndex(c => c.Name)
                    .IsUnique();
            });

            // === TrainingPlan ===
            modelBuilder.Entity<TrainingPlan>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Title)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(p => p.Description)
                    .HasMaxLength(2000);

                entity.Property(p => p.Location)
                    .HasMaxLength(200);

                entity.Property(p => p.Price)
                    .HasColumnType("decimal(10,2)");

                entity.Property(p => p.Type)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(p => p.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                // Foreign key ka Category (Restrict — ne brisati kategoriju ako se koristi)
                entity.HasOne(p => p.Category)
                    .WithMany()
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Indeks za brže pretrage po treneru
                entity.HasIndex(p => p.TrainerId);
            });

            // === Enrollment ===
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(e => e.RejectionReason)
                    .HasMaxLength(500);

                entity.Property(e => e.ClientNote)
                    .HasMaxLength(500);

                // Foreign key ka TrainingPlan
                entity.HasOne(e => e.TrainingPlan)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(e => e.TrainingPlanId)
                    .OnDelete(DeleteBehavior.Cascade); //ako se obriše plan, brišu se i svi njegovi enrollments.

                // Indeks za brze upite "moji zahtevi"
                entity.HasIndex(e => e.ClientId);

                // Indeks za brze upite "zahtevi za moj plan"
                entity.HasIndex(e => new { e.TrainingPlanId, e.Status });
            });

            // === TrainingSession ===
            modelBuilder.Entity<TrainingSession>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(s => s.Notes)
                    .HasMaxLength(1000);

                entity.HasOne(s => s.TrainingPlan)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(s => s.TrainingPlanId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(s => new { s.TrainingPlanId, s.StartTime });
            });

            // === Attendance ===
            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.Notes)
                    .HasMaxLength(500);

                entity.HasOne(a => a.TrainingSession)
                    .WithMany(s => s.Attendances)
                    .HasForeignKey(a => a.TrainingSessionId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Jedan klijent moze imati samo jedan Attendance po sesiji
                entity.HasIndex(a => new { a.TrainingSessionId, a.ClientId })
                    .IsUnique();

                entity.HasIndex(a => a.ClientId);
            });

        }
    }
}