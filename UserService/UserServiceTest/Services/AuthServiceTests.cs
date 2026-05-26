using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceApplication.DTOs.Requests;
using UserServiceApplication.Interfaces;
using UserServiceApplication.Services;
using UserServiceDomain.Entities;
using UserServiceDomain.Enums;

namespace UserServiceTest.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<ITokenService>();

            _authService = new AuthService(
                _userRepositoryMock.Object,
                _tokenServiceMock.Object);
        }

        // === HELPER ===
        // Pravi User sa hash-om za zadatu lozinku (koristi pravi PasswordHasher)
        private static User CreateUserWithPassword(
            string password,
            UserStatus status = UserStatus.Active,
            UserRole role = UserRole.Client)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "User",
                Role = role,
                Status = status,
                RegistrationDate = DateTime.UtcNow
            };
            // Generisi pravi hash za lozinku
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, password);
            return user;
        }

        private static RegisterUser CreateRegisterRequest(
            UserRole role = UserRole.Client,
            string username = "novi.korisnik",
            string email = "novi@test.com")
        {
            return new RegisterUser
            {
                Username = username,
                Email = email,
                Password = "sifra123",
                FirstName = "Novi",
                LastName = "Korisnik",
                DateOfBirth = new DateOnly(1995, 5, 15),
                Gender = UserGender.Male,
                Location = "Novi Sad",
                Role = role
            };
        }

        // ===========================================
        // RegisterAsync
        // ===========================================

        [Fact]
        public async Task RegisterAsync_WithValidClient_ShouldCreateUserAndReturnToken()
        {
            // Arrange
            var request = CreateRegisterRequest(UserRole.Client);

            _userRepositoryMock.Setup(r => r.GetByUsernameAsync(request.Username)).ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync((User?)null);
            _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<User>())).Returns("fake-token");

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().Be("fake-token");
            result.User.Username.Should().Be(request.Username);

            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_AsClient_ShouldHaveActiveStatus()
        {
            // Arrange
            var request = CreateRegisterRequest(UserRole.Client);
            User? capturedUser = null;

            _userRepositoryMock.Setup(r => r.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            // Uhvati User-a koji se dodaje
            _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u);
            _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<User>())).Returns("token");

            // Act
            await _authService.RegisterAsync(request);

            // Assert - klijent je odmah Active
            capturedUser.Should().NotBeNull();
            capturedUser!.Status.Should().Be(UserStatus.Active);
        }

        [Fact]
        public async Task RegisterAsync_AsTrainer_ShouldHavePendingApprovalStatus()
        {
            // Arrange
            var request = CreateRegisterRequest(UserRole.Trainer);
            User? capturedUser = null;

            _userRepositoryMock.Setup(r => r.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u);
            _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<User>())).Returns("token");

            // Act
            await _authService.RegisterAsync(request);

            // Assert - trener ceka odobrenje
            capturedUser!.Status.Should().Be(UserStatus.PendingApproval);
        }

        [Fact]
        public async Task RegisterAsync_AsTrainer_ShouldCreateEmptyTrainerProfile()
        {
            // Arrange
            var request = CreateRegisterRequest(UserRole.Trainer);

            _userRepositoryMock.Setup(r => r.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<User>())).Returns("token");

            // Act
            await _authService.RegisterAsync(request);

            // Assert - kreiran je prazan trenerski profil
            _userRepositoryMock.Verify(r => r.AddTrainerProfileAsync(It.IsAny<TrainerProfile>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_AsClient_ShouldNotCreateTrainerProfile()
        {
            // Arrange
            var request = CreateRegisterRequest(UserRole.Client);

            _userRepositoryMock.Setup(r => r.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<User>())).Returns("token");

            // Act
            await _authService.RegisterAsync(request);

            // Assert - klijent NEMA trenerski profil
            _userRepositoryMock.Verify(r => r.AddTrainerProfileAsync(It.IsAny<TrainerProfile>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_AsAdmin_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var request = CreateRegisterRequest(UserRole.Admin);

            // Act & Assert
            var act = async () => await _authService.RegisterAsync(request);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Admin*");
        }

        [Fact]
        public async Task RegisterAsync_WithExistingUsername_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var request = CreateRegisterRequest();
            var existingUser = CreateUserWithPassword("x");

            _userRepositoryMock.Setup(r => r.GetByUsernameAsync(request.Username)).ReturnsAsync(existingUser);

            // Act & Assert
            var act = async () => await _authService.RegisterAsync(request);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Username*");

            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var request = CreateRegisterRequest();
            var existingUser = CreateUserWithPassword("x");

            _userRepositoryMock.Setup(r => r.GetByUsernameAsync(request.Username)).ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(existingUser);

            // Act & Assert
            var act = async () => await _authService.RegisterAsync(request);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Email*");
        }

        // ===========================================
        // LoginAsync
        // ===========================================

        [Fact]
        public async Task LoginAsync_WithCorrectCredentials_ShouldReturnToken()
        {
            // Arrange
            var password = "sifra123";
            var user = CreateUserWithPassword(password, UserStatus.Active);
            var request = new LoginUser { Username = user.Username, Password = password };

            _userRepositoryMock.Setup(r => r.GetByUsernameAsync(user.Username)).ReturnsAsync(user);
            _tokenServiceMock.Setup(t => t.GenerateToken(user)).Returns("login-token");

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            result.Token.Should().Be("login-token");
            result.User.Username.Should().Be(user.Username);
        }

        [Fact]
        public async Task LoginAsync_WithWrongPassword_ShouldThrowUnauthorized()
        {
            // Arrange
            var user = CreateUserWithPassword("PravaSifra123!", UserStatus.Active);
            var request = new LoginUser { Username = user.Username, Password = "PogresnaSifra!" };

            _userRepositoryMock.Setup(r => r.GetByUsernameAsync(user.Username)).ReturnsAsync(user);

            // Act & Assert
            var act = async () => await _authService.LoginAsync(request);
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*Pogresan username ili lozinka*");
        }

        [Fact]
        public async Task LoginAsync_WithNonExistentUser_ShouldThrowUnauthorized()
        {
            // Arrange
            var request = new LoginUser { Username = "nepostoji", Password = "bilo" };

            _userRepositoryMock.Setup(r => r.GetByUsernameAsync("nepostoji")).ReturnsAsync((User?)null);

            // Act & Assert
            var act = async () => await _authService.LoginAsync(request);
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*Pogresan username ili lozinka*");
        }

        [Fact]
        public async Task LoginAsync_WithInactiveAccount_ShouldThrowUnauthorized()
        {
            // Arrange
            var password = "sifra123";
            var user = CreateUserWithPassword(password, UserStatus.InActive);
            var request = new LoginUser { Username = user.Username, Password = password };

            _userRepositoryMock.Setup(r => r.GetByUsernameAsync(user.Username)).ReturnsAsync(user);

            // Act & Assert
            var act = async () => await _authService.LoginAsync(request);
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*deaktiviran*");
        }

        [Fact]
        public async Task LoginAsync_WithPendingApprovalAccount_ShouldThrowUnauthorized()
        {
            // Arrange
            var password = "sifra123";
            var user = CreateUserWithPassword(password, UserStatus.PendingApproval);
            var request = new LoginUser { Username = user.Username, Password = password };

            _userRepositoryMock.Setup(r => r.GetByUsernameAsync(user.Username)).ReturnsAsync(user);

            // Act & Assert
            var act = async () => await _authService.LoginAsync(request);
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*ceka odobrenje*");
        }

        [Fact]
        public async Task LoginAsync_PendingAccount_ShouldNotGenerateToken()
        {
            // Arrange - proverava da PendingApproval ne dobija token
            var password = "sifra123";
            var user = CreateUserWithPassword(password, UserStatus.PendingApproval);
            var request = new LoginUser { Username = user.Username, Password = password };

            _userRepositoryMock.Setup(r => r.GetByUsernameAsync(user.Username)).ReturnsAsync(user);

            // Act
            try { await _authService.LoginAsync(request); } catch { }

            // Assert - token NIJE generisan
            _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<User>()), Times.Never);
        }
    }
}

