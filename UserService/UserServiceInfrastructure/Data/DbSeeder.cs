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
            var adminId = Guid.NewGuid();
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



            var trenerMarkoId = Guid.NewGuid();
            var trenerAnaId = Guid.NewGuid();
            var trenerStefanId = Guid.NewGuid();

            var jelenaId = Guid.NewGuid();
            var nikolaId = Guid.NewGuid();
            var milicaId = Guid.NewGuid();
            var tamaraId = Guid.NewGuid();
            var filipId = Guid.NewGuid();
            var lukaId = Guid.NewGuid();



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
        }
    }
}