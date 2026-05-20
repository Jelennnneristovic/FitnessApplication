using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementDomain.Entities;
using TrainingManagementDomain.Enums;

namespace TrainingManagementInfrastructure.Data
{
    public static class TrainingDbSeeder
    {
        // === GUID-ovi korisnika iz UserService baze ===
        // Treneri
        private static readonly Guid MarkoId = Guid.Parse("488D802E-F184-4B7B-8417-E5061B094842");
        private static readonly Guid AnaId = Guid.Parse("83694D78-554F-464A-BC83-5ACB5C667F3F");
        private static readonly Guid StefanId = Guid.Parse("E114B069-B5A8-4CDD-BE91-9419535C2BB6");

        // Klijenti
        private static readonly Guid JelenaId = Guid.Parse("B3513E0D-8453-4BBF-AD64-93DCF6D52618");
        private static readonly Guid NikolaId = Guid.Parse("5F7B946B-B6B8-42BB-B5E6-45DF9C68BE0D");
        private static readonly Guid MilicaId = Guid.Parse("ADAA688B-DF82-4539-BD41-43C011087025");
        private static readonly Guid FilipId = Guid.Parse("DBEB7532-F91E-4894-9E7B-1A7A305C7C04");
        private static readonly Guid TamaraId = Guid.Parse("B84054D9-BD6C-4863-B0BD-A7C852AD2A48");
        private static readonly Guid LukaId = Guid.Parse("DDA14DC0-B253-404E-8CCA-12AEA1180795");

        public static async Task SeedAsync(TrainingDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            // Ako vec ima kategorija, smatra da je sve seedovano
            if (await context.Categories.AnyAsync())
                return;

            var now = DateTime.UtcNow;

            // === 1. KATEGORIJE ===
            var kardio = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Kardio",
                Description = "Treninzi za podizanje kondicije i izdrzljivosti",
                IsActive = true,
                CreatedAt = now.AddMonths(-3)
            };
            var snaga = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Snaga",
                Description = "Trening sa tegovima za izgradnju misicne mase",
                IsActive = true,
                CreatedAt = now.AddMonths(-3)
            };
            var joga = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Joga",
                Description = "Vezbe za fleksibilnost, balans i mentalnu opustanost",
                IsActive = true,
                CreatedAt = now.AddMonths(-3)
            };
            var pilates = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Pilates",
                Description = "Kontrolisani pokreti za jacanje core misica",
                IsActive = true,
                CreatedAt = now.AddMonths(-2)
            };
            var crossfit = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Crossfit",
                Description = "Funkcionalni treninzi visokog intenziteta",
                IsActive = true,
                CreatedAt = now.AddMonths(-2)
            };

            await context.Categories.AddRangeAsync(kardio, snaga, joga, pilates, crossfit);
            await context.SaveChangesAsync();

            // === 2. TRAINING PLANS ===
            var markoKardio = new TrainingPlan
            {
                Id = Guid.NewGuid(),
                TrainerId = MarkoId,
                CategoryId = kardio.Id,
                Title = "Personal Training - Kardio",
                Description = "Individualni kardio trening 3x nedeljno. Fokus na sagorevanje masti i poboljsanje kondicije.",
                Type = TrainingType.Individual,
                Price = 50.00m,
                MaxParticipants = 1,
                DurationMinutes = 60,
                Location = "Fitness Plus, Novi Sad",
                Status = TrainingPlanStatus.Active,
                CreatedAt = now.AddMonths(-2)
            };

            var markoCrossfit = new TrainingPlan
            {
                Id = Guid.NewGuid(),
                TrainerId = MarkoId,
                CategoryId = crossfit.Id,
                Title = "Crossfit Grupa",
                Description = "Grupni crossfit treninzi za sve nivoe. Maksimalno 8 osoba po grupi.",
                Type = TrainingType.Group,
                Price = 25.00m,
                MaxParticipants = 8,
                DurationMinutes = 75,
                Location = "Crossfit Box, Novi Sad",
                Status = TrainingPlanStatus.Active,
                CreatedAt = now.AddMonths(-2)
            };

            var anaJoga = new TrainingPlan
            {
                Id = Guid.NewGuid(),
                TrainerId = AnaId,
                CategoryId = joga.Id,
                Title = "Joga 1-na-1",
                Description = "Individualne joga sesije prilagodjene tvom telu i ciljevima.",
                Type = TrainingType.Individual,
                Price = 40.00m,
                MaxParticipants = 1,
                DurationMinutes = 60,
                Location = "Joga studio, Beograd",
                Status = TrainingPlanStatus.Active,
                CreatedAt = now.AddMonths(-3)
            };

            var anaPilates = new TrainingPlan
            {
                Id = Guid.NewGuid(),
                TrainerId = AnaId,
                CategoryId = pilates.Id,
                Title = "Pilates Grupa",
                Description = "Pilates grupni treninzi 2x nedeljno. Idealno za pocetnike i napredne.",
                Type = TrainingType.Group,
                Price = 20.00m,
                MaxParticipants = 10,
                DurationMinutes = 60,
                Location = "Joga studio, Beograd",
                Status = TrainingPlanStatus.Active,
                CreatedAt = now.AddMonths(-3)
            };

            var stefanSnaga = new TrainingPlan
            {
                Id = Guid.NewGuid(),
                TrainerId = StefanId,
                CategoryId = snaga.Id,
                Title = "Strength Coaching",
                Description = "Individualni program za izgradnju snage i misicne mase. Plan ishrane ukljucen.",
                Type = TrainingType.Individual,
                Price = 60.00m,
                MaxParticipants = 1,
                DurationMinutes = 75,
                Location = "Gold Gym, Nis",
                Status = TrainingPlanStatus.Active,
                CreatedAt = now.AddMonths(-1)
            };

            var stefanKardio = new TrainingPlan
            {
                Id = Guid.NewGuid(),
                TrainerId = StefanId,
                CategoryId = kardio.Id,
                Title = "HIIT Grupa",
                Description = "High intensity interval training u grupi do 6 ljudi.",
                Type = TrainingType.Group,
                Price = 18.00m,
                MaxParticipants = 6,
                DurationMinutes = 45,
                Location = "Gold Gym, Nis",
                Status = TrainingPlanStatus.Active,
                CreatedAt = now.AddMonths(-1)
            };

            await context.TrainingPlans.AddRangeAsync(
                markoKardio, markoCrossfit, anaJoga, anaPilates, stefanSnaga, stefanKardio);
            await context.SaveChangesAsync();

            // === 3. ENROLLMENTS ===
            // Markov Kardio - Jelena Approved (Individual plan popunjen)
            var enr1 = new Enrollment
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = markoKardio.Id,
                ClientId = JelenaId,
                Status = EnrollmentStatus.Approved,
                RequestedAt = now.AddDays(-30),
                RespondedAt = now.AddDays(-29),
                ClientNote = "Hocu da smrsam 10kg za leto"
            };

            // Markov Crossfit - vise klijenata Approved
            var enr2 = new Enrollment
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = markoCrossfit.Id,
                ClientId = NikolaId,
                Status = EnrollmentStatus.Approved,
                RequestedAt = now.AddDays(-25),
                RespondedAt = now.AddDays(-24)
            };

            var enr3 = new Enrollment
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = markoCrossfit.Id,
                ClientId = FilipId,
                Status = EnrollmentStatus.Approved,
                RequestedAt = now.AddDays(-20),
                RespondedAt = now.AddDays(-19)
            };

            // Markov Crossfit - Luka Pending (ceka odluku)
            var enr4 = new Enrollment
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = markoCrossfit.Id,
                ClientId = LukaId,
                Status = EnrollmentStatus.Pending,
                RequestedAt = now.AddDays(-2),
                ClientNote = "Imam vec iskustvo sa crossfitom"
            };

            // Anin Joga - Milica Approved
            var enr5 = new Enrollment
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = anaJoga.Id,
                ClientId = MilicaId,
                Status = EnrollmentStatus.Approved,
                RequestedAt = now.AddDays(-40),
                RespondedAt = now.AddDays(-39),
                ClientNote = "Pocetnik sam, molim strpljenje"
            };

            // Anin Pilates - vise klijenata
            var enr6 = new Enrollment
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = anaPilates.Id,
                ClientId = JelenaId,
                Status = EnrollmentStatus.Approved,
                RequestedAt = now.AddDays(-15),
                RespondedAt = now.AddDays(-14)
            };

            var enr7 = new Enrollment
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = anaPilates.Id,
                ClientId = MilicaId,
                Status = EnrollmentStatus.Approved,
                RequestedAt = now.AddDays(-12),
                RespondedAt = now.AddDays(-11)
            };

            // Anin Pilates - Tamara Rejected (jer je deaktivirana? trener odbio)
            var enr8 = new Enrollment
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = anaPilates.Id,
                ClientId = TamaraId,
                Status = EnrollmentStatus.Rejected,
                RequestedAt = now.AddDays(-10),
                RespondedAt = now.AddDays(-9),
                RejectionReason = "Trenutno popunjena grupa, javite se sledeci mesec"
            };

            // Stefanov Snaga - Filip Pending
            var enr9 = new Enrollment
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = stefanSnaga.Id,
                ClientId = FilipId,
                Status = EnrollmentStatus.Pending,
                RequestedAt = now.AddDays(-1),
                ClientNote = "Cilj: nabiti 5kg cistih misica"
            };

            // Stefanov HIIT - Nikola Cancelled (otkazao sam)
            var enr10 = new Enrollment
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = stefanKardio.Id,
                ClientId = NikolaId,
                Status = EnrollmentStatus.Cancelled,
                RequestedAt = now.AddDays(-7),
                RespondedAt = now.AddDays(-6)
            };

            await context.Enrollments.AddRangeAsync(
                enr1, enr2, enr3, enr4, enr5, enr6, enr7, enr8, enr9, enr10);
            await context.SaveChangesAsync();

            // === 4. SESSIONS ===
            // Markov Kardio - prosle 2 sesije + 3 buduce
            var session1 = new TrainingSession
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = markoKardio.Id,
                StartTime = now.AddDays(-14).Date.AddHours(18),
                EndTime = now.AddDays(-14).Date.AddHours(19),
                Status = TrainingSessionStatus.Completed,
                Notes = "Prvi trening - upoznavanje, lakse vezbe",
                CreatedAt = now.AddDays(-20)
            };

            var session2 = new TrainingSession
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = markoKardio.Id,
                StartTime = now.AddDays(-7).Date.AddHours(18),
                EndTime = now.AddDays(-7).Date.AddHours(19),
                Status = TrainingSessionStatus.Completed,
                Notes = "Intervalni trening na traci",
                CreatedAt = now.AddDays(-14)
            };

            var session3 = new TrainingSession
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = markoKardio.Id,
                StartTime = now.AddDays(2).Date.AddHours(18),
                EndTime = now.AddDays(2).Date.AddHours(19),
                Status = TrainingSessionStatus.Scheduled,
                CreatedAt = now.AddDays(-7)
            };

            var session4 = new TrainingSession
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = markoKardio.Id,
                StartTime = now.AddDays(5).Date.AddHours(18),
                EndTime = now.AddDays(5).Date.AddHours(19),
                Status = TrainingSessionStatus.Scheduled,
                CreatedAt = now.AddDays(-5)
            };

            // Markov Crossfit - prosle 3 + 2 buduce
            var session5 = new TrainingSession
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = markoCrossfit.Id,
                StartTime = now.AddDays(-10).Date.AddHours(19),
                EndTime = now.AddDays(-10).Date.AddHours(20).AddMinutes(15),
                Status = TrainingSessionStatus.Completed,
                Notes = "WOD: Fran",
                CreatedAt = now.AddDays(-15)
            };

            var session6 = new TrainingSession
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = markoCrossfit.Id,
                StartTime = now.AddDays(-5).Date.AddHours(19),
                EndTime = now.AddDays(-5).Date.AddHours(20).AddMinutes(15),
                Status = TrainingSessionStatus.Completed,
                CreatedAt = now.AddDays(-10)
            };

            var session7 = new TrainingSession
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = markoCrossfit.Id,
                StartTime = now.AddDays(1).Date.AddHours(19),
                EndTime = now.AddDays(1).Date.AddHours(20).AddMinutes(15),
                Status = TrainingSessionStatus.Scheduled,
                CreatedAt = now.AddDays(-5)
            };

            var session8 = new TrainingSession
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = markoCrossfit.Id,
                StartTime = now.AddDays(4).Date.AddHours(19),
                EndTime = now.AddDays(4).Date.AddHours(20).AddMinutes(15),
                Status = TrainingSessionStatus.Scheduled,
                CreatedAt = now.AddDays(-3)
            };

            // Anin Joga - prosla 1 + 2 buduce
            var session9 = new TrainingSession
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = anaJoga.Id,
                StartTime = now.AddDays(-3).Date.AddHours(17),
                EndTime = now.AddDays(-3).Date.AddHours(18),
                Status = TrainingSessionStatus.Completed,
                Notes = "Vinyasa flow za pocetnike",
                CreatedAt = now.AddDays(-7)
            };

            var session10 = new TrainingSession
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = anaJoga.Id,
                StartTime = now.AddDays(3).Date.AddHours(17),
                EndTime = now.AddDays(3).Date.AddHours(18),
                Status = TrainingSessionStatus.Scheduled,
                CreatedAt = now.AddDays(-3)
            };

            // Anin Pilates - prosla 1 + 2 buduce
            var session11 = new TrainingSession
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = anaPilates.Id,
                StartTime = now.AddDays(-2).Date.AddHours(19),
                EndTime = now.AddDays(-2).Date.AddHours(20),
                Status = TrainingSessionStatus.Completed,
                CreatedAt = now.AddDays(-7)
            };

            var session12 = new TrainingSession
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = anaPilates.Id,
                StartTime = now.AddDays(2).Date.AddHours(19),
                EndTime = now.AddDays(2).Date.AddHours(20),
                Status = TrainingSessionStatus.Scheduled,
                CreatedAt = now.AddDays(-2)
            };

            var session13 = new TrainingSession
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = anaPilates.Id,
                StartTime = now.AddDays(6).Date.AddHours(19),
                EndTime = now.AddDays(6).Date.AddHours(20),
                Status = TrainingSessionStatus.Scheduled,
                CreatedAt = now.AddDays(-1)
            };

            // Otkazana sesija
            var session14 = new TrainingSession
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = markoCrossfit.Id,
                StartTime = now.AddDays(-1).Date.AddHours(19),
                EndTime = now.AddDays(-1).Date.AddHours(20).AddMinutes(15),
                Status = TrainingSessionStatus.Cancelled,
                Notes = "Otkazano zbog kvara na opremi",
                CreatedAt = now.AddDays(-5)
            };

            // Stefanov Snaga (jos nema enrollmentsa odobrenih)
            var session15 = new TrainingSession
            {
                Id = Guid.NewGuid(),
                TrainingPlanId = stefanSnaga.Id,
                StartTime = now.AddDays(7).Date.AddHours(20),
                EndTime = now.AddDays(7).Date.AddHours(21).AddMinutes(15),
                Status = TrainingSessionStatus.Scheduled,
                CreatedAt = now.AddDays(-2)
            };

            await context.TrainingSessions.AddRangeAsync(
                session1, session2, session3, session4, session5, session6, session7, session8,
                session9, session10, session11, session12, session13, session14, session15);
            await context.SaveChangesAsync();

            // === 5. ATTENDANCES (za prosle sesije) ===
            // Jelena na Markov Kardio
            var att1 = new Attendance
            {
                Id = Guid.NewGuid(),
                TrainingSessionId = session1.Id,
                ClientId = JelenaId,
                Attended = true,
                MarkedAt = session1.EndTime.AddMinutes(30),
                MarkedByUserId = JelenaId
            };

            var att2 = new Attendance
            {
                Id = Guid.NewGuid(),
                TrainingSessionId = session2.Id,
                ClientId = JelenaId,
                Attended = true,
                MarkedAt = session2.EndTime.AddHours(1),
                MarkedByUserId = JelenaId,
                Notes = "Super trening!"
            };

            // Markov Crossfit - Nikola dosao, Filip nije na jednoj
            var att3 = new Attendance
            {
                Id = Guid.NewGuid(),
                TrainingSessionId = session5.Id,
                ClientId = NikolaId,
                Attended = true,
                MarkedAt = session5.EndTime.AddMinutes(15),
                MarkedByUserId = NikolaId
            };

            var att4 = new Attendance
            {
                Id = Guid.NewGuid(),
                TrainingSessionId = session5.Id,
                ClientId = FilipId,
                Attended = true,
                MarkedAt = session5.EndTime.AddMinutes(20),
                MarkedByUserId = MarkoId,
                Notes = "Dobar trening, napravio progres na pull-up"
            };

            var att5 = new Attendance
            {
                Id = Guid.NewGuid(),
                TrainingSessionId = session6.Id,
                ClientId = NikolaId,
                Attended = false,
                MarkedAt = session6.EndTime.AddHours(2),
                MarkedByUserId = MarkoId,
                Notes = "Bolestan"
            };

            var att6 = new Attendance
            {
                Id = Guid.NewGuid(),
                TrainingSessionId = session6.Id,
                ClientId = FilipId,
                Attended = true,
                MarkedAt = session6.EndTime.AddMinutes(10),
                MarkedByUserId = FilipId
            };

            // Anin Joga - Milica dosla
            var att7 = new Attendance
            {
                Id = Guid.NewGuid(),
                TrainingSessionId = session9.Id,
                ClientId = MilicaId,
                Attended = true,
                MarkedAt = session9.EndTime.AddMinutes(5),
                MarkedByUserId = MilicaId,
                Notes = "Mnogo opustajuce"
            };

            // Anin Pilates - Jelena i Milica dosle
            var att8 = new Attendance
            {
                Id = Guid.NewGuid(),
                TrainingSessionId = session11.Id,
                ClientId = JelenaId,
                Attended = true,
                MarkedAt = session11.EndTime.AddMinutes(10),
                MarkedByUserId = JelenaId
            };

            var att9 = new Attendance
            {
                Id = Guid.NewGuid(),
                TrainingSessionId = session11.Id,
                ClientId = MilicaId,
                Attended = true,
                MarkedAt = session11.EndTime.AddMinutes(15),
                MarkedByUserId = MilicaId
            };

            await context.Attendances.AddRangeAsync(
                att1, att2, att3, att4, att5, att6, att7, att8, att9);
            await context.SaveChangesAsync();
        }
    }
}
