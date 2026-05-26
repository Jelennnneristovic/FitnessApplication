using UserServiceDomain.Entities;
using UserServiceDomain.Enums;

namespace UserServiceInfrastructure.Data
{
    public static class DbSeeder

    {
        private const string AdminPasswordHash = "AQAAAAIAAYagAAAAEEHxgpAaOfxLD3dptVgzenPLBtgQRCdKiD1rEOaTV7SMjRG86rn9LChpK35LUgpKXw=="; //admin123
        private const string UserPasswordHash = "AQAAAAIAAYagAAAAEOry4DUYsGyiRO2BO6Bh2LbOdwFUMfHUN0/355Ipct9kAGLowr6ZcwpCVW8veftTyQ=="; //sifra123

        public static async Task SeedAsync(AppDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (context.Users.Any(u => u.Username == "admin"))
                return;

            // 1. Generišemo fiksni ID za admina da bi znali kako da nazovemo sliku
            var adminId = Guid.Parse("5363D1BE-4EAB-4F74-8411-3C5E293C844C");
            var admin = new User
            {
                Id = adminId,
                Username = "admin",
                Email = "admin@gym.local",
                FirstName = "System",
                LastName = "Admin",
                PasswordHash = AdminPasswordHash,
                DateOfBirth = new DateOnly(1990, 1, 1),
                Gender = UserGender.Male,
                Location = "HQ",
                Status = UserStatus.Active,
                Role = UserRole.Admin,
               
                RegistrationDate = DateTime.UtcNow,
                ProfileImageUrl = $"/uploads/users/{adminId}.jpg"

            };
            context.Users.Add(admin);
            await context.SaveChangesAsync();



            var trenerMarkoId = Guid.Parse("488D802E-F184-4B7B-8417-E5061B094842");
            var trenerAnaId = Guid.Parse("83694D78-554F-464A-BC83-5ACB5C667F3F");
            var trenerStefanId = Guid.Parse("E114B069-B5A8-4CDD-BE91-9419535C2BB6");

            var jelenaId = Guid.Parse("B3513E0D-8453-4BBF-AD64-93DCF6D52618");
            var nikolaId = Guid.Parse("5F7B946B-B6B8-42BB-B5E6-45DF9C68BE0D");
            var milicaId = Guid.Parse("ADAA688B-DF82-4539-BD41-43C011087025");
            var tamaraId = Guid.Parse("B84054D9-BD6C-4863-B0BD-A7C852AD2A48");
            var filipId = Guid.Parse("DBEB7532-F91E-4894-9E7B-1A7A305C7C04");
            var lukaId = Guid.Parse("DDA14DC0-B253-404E-8CCA-12AEA1180795");



            var users = new List<User>
            {
            // === TRENERI ===
            new User
                {
                    Id = trenerMarkoId,
                    Username = "marko.petrovic",
                    Email = "marko.petrovic@gym.local",
                    FirstName = "Marko",
                    LastName = "Petrovic",
                    PasswordHash = UserPasswordHash,
                    DateOfBirth = new DateOnly(1988, 5, 14),
                    Gender = UserGender.Male,
                    Location = "Novi Sad",
                    Status = UserStatus.Active,
                    Role = UserRole.Trainer,
                    
                    RegistrationDate = DateTime.UtcNow.AddMonths(-8),
                    ProfileImageUrl = $"/uploads/users/{trenerMarkoId}.jpg",
                },
                new User
                {
                    Id = trenerAnaId,
                    Username = "ana.jovanovic",
                    Email = "ana.jovanovic@gym.local",
                    FirstName = "Ana",
                    LastName = "Jovanovic",
                    PasswordHash = UserPasswordHash,
                    DateOfBirth = new DateOnly(1992, 9, 23),
                    Gender = UserGender.Female,
                    Location = "Beograd",
                    Status = UserStatus.Active,
                    Role = UserRole.Trainer,
                    
                    RegistrationDate = DateTime.UtcNow.AddMonths(-6),
                    ProfileImageUrl = $"/uploads/users/{trenerAnaId}.jpg",
                },
                new User
                {
                    Id = trenerStefanId,
                    Username = "stefan.nikolic",
                    Email = "stefan.nikolic@gym.local",
                    FirstName = "Stefan",
                    LastName = "Nikolic",
                    PasswordHash = UserPasswordHash,
                    DateOfBirth = new DateOnly(1985, 3, 7),
                    Gender = UserGender.Male,
                    Location = "Nis",
                    Status = UserStatus.PendingApproval,
                    Role = UserRole.Trainer,
                  
                    RegistrationDate = DateTime.UtcNow.AddMonths(-12),
                     ProfileImageUrl = $"/uploads/users/{trenerStefanId}.jpg"
                },

                // === KLIJENTI ===
                new User
                {
                    Id = jelenaId,
                    Username = "jelena.markovic",
                    Email = "jelena.markovic@gmail.com",
                    FirstName = "Jelena",
                    LastName = "Markovic",
                    PasswordHash = UserPasswordHash,
                    DateOfBirth = new DateOnly(1995, 7, 12),
                    Gender = UserGender.Female,
                    Location = "Novi Sad",
                    Status = UserStatus.Active,
                    Role = UserRole.Client,
                   
                    RegistrationDate = DateTime.UtcNow.AddMonths(-4),
                     ProfileImageUrl = $"/uploads/users/{jelenaId}.jpg",
                },
                new User
                {
                    Id = nikolaId,
                    Username = "nikola.djordjevic",
                    Email = "nikola.djordjevic@gmail.com",
                    FirstName = "Nikola",
                    LastName = "Djordjevic",
                    PasswordHash = UserPasswordHash,
                    DateOfBirth = new DateOnly(1998, 11, 3),
                    Gender = UserGender.Male,
                    Location = "Beograd",
                    Status = UserStatus.Active,
                    Role = UserRole.Client,
                    
                    RegistrationDate = DateTime.UtcNow.AddMonths(-3),
                    ProfileImageUrl = $"/uploads/users/{nikolaId}.jpg",
                },
                new User
                {
                    Id = milicaId,
                    Username = "milica.stojanovic",
                    Email = "milica.stojanovic@gmail.com",
                    FirstName = "Milica",
                    LastName = "Stojanovic",
                    PasswordHash = UserPasswordHash,
                    DateOfBirth = new DateOnly(2000, 2, 18),
                    Gender = UserGender.Female,
                    Location = "Novi Sad",
                    Status = UserStatus.Active,
                    Role = UserRole.Client,
                   
                    RegistrationDate = DateTime.UtcNow.AddMonths(-2),
                    ProfileImageUrl = $"/uploads/users/{milicaId}.jpg"
                },
                new User
                {
                    Id = filipId,
                    Username = "filip.ilic",
                    Email = "filip.ilic@gmail.com",
                    FirstName = "Filip",
                    LastName = "Ilic",
                    PasswordHash = UserPasswordHash,
                    DateOfBirth = new DateOnly(1993, 6, 25),
                    Gender = UserGender.Male,
                    Location = "Nis",
                    Status = UserStatus.Active,
                    Role = UserRole.Client,
                   
                    RegistrationDate = DateTime.UtcNow.AddMonths(-5),
                    ProfileImageUrl = $"/uploads/users/{filipId}.jpg"
                },
                new User
                {
                    Id = tamaraId,
                    Username = "tamara.popovic",
                    Email = "tamara.popovic@gmail.com",
                    FirstName = "Tamara",
                    LastName = "Popovic",
                    PasswordHash = UserPasswordHash,
                    DateOfBirth = new DateOnly(1997, 10, 8),
                    Gender = UserGender.Female,
                    Location = "Subotica",
                    Status = UserStatus.InActive,
                    Role = UserRole.Client,
                   
                    RegistrationDate = DateTime.UtcNow.AddDays(-3),
                    ProfileImageUrl = $"/uploads/users/{tamaraId}.jpg"
                },
                new User
                {
                    Id = lukaId,
                    Username = "luka.savic",
                    Email = "luka.savic@gmail.com",
                    FirstName = "Luka",
                    LastName = "Savic",
                    PasswordHash = UserPasswordHash,
                    DateOfBirth = new DateOnly(1996, 4, 30),
                    Gender = UserGender.Male,
                    Location = "Kragujevac",
                    Status = UserStatus.Active,
                    Role = UserRole.Client,
                  
                    RegistrationDate = DateTime.UtcNow.AddMonths(-1),
                     ProfileImageUrl = $"/uploads/users/{lukaId}.jpg"
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();


            // === TRENERSKI PROFILI ===
            var trainerProfiles = new List<TrainerProfile>
            {
                new TrainerProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = trenerMarkoId,
                    Specialization = "Kardio, Mrsavljenje",
                    YearsOfExperience = 8,
                    Description = "Specijalizovan za kardio treninge i programe za mrsavljenje. Pomazem klijentima da postignu svoje ciljeve.",
                    UpdatedAt = DateTime.UtcNow.AddDays(-30)
                },
                new TrainerProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = trenerAnaId,
                    Specialization = "Joga, Pilates",
                    YearsOfExperience = 6,
                    Description = "Sertifikovani joga i pilates instruktor sa fokusom na fleksibilnost i mentalnu opustenost.",
                    UpdatedAt = DateTime.UtcNow.AddDays(-20)
                },
                new TrainerProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = trenerStefanId,
                    // Stefan je PendingApproval - prazan profil (jos nije popunio)
                    Specialization = null,
                    YearsOfExperience = null,
                    Description = null,
                    UpdatedAt = null
                }
            };

            await context.TrainerProfiles.AddRangeAsync(trainerProfiles);
            await context.SaveChangesAsync();
        }
    }
}

        