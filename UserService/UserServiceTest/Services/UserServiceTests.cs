using FluentAssertions;
using Moq;
using UserServiceApplication.DTOs.Requests;
using UserServiceApplication.Interfaces;
using UserServiceApplication.Services;
using UserServiceDomain.Entities;
using UserServiceDomain.Enums;

namespace UserService.Tests.Services
{
    public class UserServiceTests
    {
        // Mock repozitorijuma - kontrolisemo sta vraca
        private readonly Mock<IUserRepository> _userRepositoryMock;
        // Servis koji testiramo
        private readonly UserServiceApplication.Services.UserService _userService;

        public UserServiceTests()
        {
            // Setup pre svakog testa (xUnit pravi novi instance za svaki test)
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserServiceApplication.Services.UserService(_userRepositoryMock.Object);
        }

        // === HELPER ===
        private static User CreateTestUser(
            Guid? id = null,
            string username = "testuser",
            UserRole role = UserRole.Client,
            UserStatus status = UserStatus.Active)
        {
            return new User
            {
                Id = id ?? Guid.NewGuid(),
                Username = username,
                Email = $"{username}@test.com",
                FirstName = "Test",
                LastName = "User",
                PasswordHash = "hash",
                DateOfBirth = new DateOnly(1995, 5, 15),
                Gender = UserGender.Male,
                Location = "Novi Sad",
                Role = role,
                Status = status,
                RegistrationDate = DateTime.UtcNow.AddMonths(-1)
            };
        }

        // ===========================================
        // GetAllClientsAsync
        // ===========================================

        [Fact]
        public async Task GetAllClientsAsync_ShouldReturnMappedClients()
        {
            // Arrange - pripremamo testne podatke
            var filter = new UserFilterRequest { Keyword = null, Status = null };
            var clients = new List<User>
            {
                CreateTestUser(username: "jelena", role: UserRole.Client),
                CreateTestUser(username: "nikola", role: UserRole.Client)
            };

            // Kazemo mock-u sta da vrati kada bude pozvan
            _userRepositoryMock
                .Setup(r => r.GetByRoleAsync(UserRole.Client, filter.Keyword, filter.Status))
                .ReturnsAsync(clients);

            // Act - pozivamo metodu koju testiramo
            var result = await _userService.GetAllClientsAsync(filter);

            // Assert - proveravamo rezultat
            result.Should().HaveCount(2);
            result.Should().Contain(u => u.Username == "jelena");
            result.Should().Contain(u => u.Username == "nikola");
        }

        [Fact]
        public async Task GetAllClientsAsync_WithEmptyResult_ShouldReturnEmptyList()
        {
            // Arrange
            var filter = new UserFilterRequest();
            _userRepositoryMock
                .Setup(r => r.GetByRoleAsync(UserRole.Client, null, null))
                .ReturnsAsync(new List<User>());

            // Act
            var result = await _userService.GetAllClientsAsync(filter);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllClientsAsync_ShouldPassFiltersToRepository()
        {
            // Arrange
            var filter = new UserFilterRequest
            {
                Keyword = "jelena",
                Status = UserStatus.Active
            };

            _userRepositoryMock
                .Setup(r => r.GetByRoleAsync(UserRole.Client, "jelena", UserStatus.Active))
                .ReturnsAsync(new List<User>());

            // Act
            await _userService.GetAllClientsAsync(filter);

            // Assert - proveravamo da je metoda pozvana sa pravim argumentima
            _userRepositoryMock.Verify(
                r => r.GetByRoleAsync(UserRole.Client, "jelena", UserStatus.Active),
                Times.Once);
        }

        // ===========================================
        // GetAllTrainersAsync
        // ===========================================

        [Fact]
        public async Task GetAllTrainersAsync_ShouldReturnOnlyTrainers()
        {
            // Arrange
            var filter = new UserFilterRequest();
            var trainers = new List<User>
            {
                CreateTestUser(username: "marko", role: UserRole.Trainer)
            };

            _userRepositoryMock
                .Setup(r => r.GetByRoleAsync(UserRole.Trainer, null, null))
                .ReturnsAsync(trainers);

            // Act
            var result = await _userService.GetAllTrainersAsync(filter);

            // Assert
            result.Should().HaveCount(1);
            result.First().Username.Should().Be("marko");
        }

        // ===========================================
        // ActivateAsync
        // ===========================================

        [Fact]
        public async Task ActivateAsync_WithInactiveUser_ShouldSetStatusToActive()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = CreateTestUser(id: userId, status: UserStatus.InActive);

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.ActivateAsync(userId);

            // Assert
            result.Status.Should().Be(UserStatus.Active);
            user.Status.Should().Be(UserStatus.Active);
            _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task ActivateAsync_WithPendingUser_ShouldSetStatusToActive()
        {
            // Arrange - korisnik je u PendingApproval (npr. trener ceka odobrenje)
            var userId = Guid.NewGuid();
            var user = CreateTestUser(id: userId, status: UserStatus.PendingApproval);

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.ActivateAsync(userId);

            // Assert
            result.Status.Should().Be(UserStatus.Active);
        }

        [Fact]
        public async Task ActivateAsync_WithNonExistentUser_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var act = async () => await _userService.ActivateAsync(userId);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Korisnik nije pronadjen.");
        }

        [Fact]
        public async Task ActivateAsync_WithAdminUser_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var admin = CreateTestUser(id: userId, role: UserRole.Admin);

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(admin);

            // Act & Assert
            var act = async () => await _userService.ActivateAsync(userId);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Admin nalog ne moze biti menjan.");

            // Provera da UpdateAsync NIJE pozvan
            _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task ActivateAsync_WithAlreadyActiveUser_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = CreateTestUser(id: userId, status: UserStatus.Active);

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act & Assert
            var act = async () => await _userService.ActivateAsync(userId);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Korisnik je vec aktivan.");
        }

        // ===========================================
        // DeactivateAsync
        // ===========================================

        [Fact]
        public async Task DeactivateAsync_WithActiveUser_ShouldSetStatusToInActive()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = CreateTestUser(id: userId, status: UserStatus.Active);

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.DeactivateAsync(userId);

            // Assert
            result.Status.Should().Be(UserStatus.InActive);
            user.Status.Should().Be(UserStatus.InActive);
            _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task DeactivateAsync_WithNonExistentUser_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var act = async () => await _userService.DeactivateAsync(userId);
            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task DeactivateAsync_WithAdminUser_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var admin = CreateTestUser(id: userId, role: UserRole.Admin);

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(admin);

            // Act & Assert
            var act = async () => await _userService.DeactivateAsync(userId);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Admin nalog ne moze biti deaktiviran.");
        }

        [Fact]
        public async Task DeactivateAsync_WithAlreadyInactiveUser_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = CreateTestUser(id: userId, status: UserStatus.InActive);

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act & Assert
            var act = async () => await _userService.DeactivateAsync(userId);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Korisnik je vec deaktiviran.");
        }

        // ===========================================
        // GetByIdAsync
        // ===========================================

        [Fact]
        public async Task GetByIdAsync_WithExistingUser_ShouldReturnUserDetails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = CreateTestUser(id: userId, username: "marko");

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(userId);
            result.Username.Should().Be("marko");
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistentUser_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var act = async () => await _userService.GetByIdAsync(userId);
            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        // ===========================================
        // UpdateAsync
        // ===========================================

        

        [Fact]
        public async Task UpdateAsync_ShouldTrimStringFields()
        {
            // Arrange - testiramo da li se .Trim() poziva
            var userId = Guid.NewGuid();
            var user = CreateTestUser(id: userId);

            var request = new UpdateUserRequest
            {
                FirstName = "  Marko  ",
                LastName = "  Petrovic  ",
                Location = "  Novi Sad  "
            };

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            await _userService.UpdateAsync(userId, request);

            // Assert
            user.FirstName.Should().Be("Marko");
            user.LastName.Should().Be("Petrovic");
            user.Location.Should().Be("Novi Sad");
        }

        [Fact]
        public async Task UpdateAsync_WithEmptyFirstName_ShouldThrowArgumentException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = CreateTestUser(id: userId);

            var request = new UpdateUserRequest { FirstName = "   " };

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act & Assert
            var act = async () => await _userService.UpdateAsync(userId, request);
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Ime ne moze biti prazno.");
        }

      

        [Fact]
        public async Task UpdateAsync_WithNonExistentUser_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new UpdateUserRequest { FirstName = "Novo" };

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var act = async () => await _userService.UpdateAsync(userId, request);
            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

      
    }
}