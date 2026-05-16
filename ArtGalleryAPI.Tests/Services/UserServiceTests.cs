// User Service Tests - Business Logic Testing
using ArtGalleryAPI.DTOs;
using ArtGalleryAPI.Models;
using ArtGalleryAPI.Repositories.Interfaces;
using ArtGalleryAPI.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace ArtGalleryAPI.Tests.Services
{
    /// <summary>
    /// Unit tests for User Service business logic
    /// Tests CRUD operations, authentication, and role management
    /// </summary>
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly JwtService _jwtService;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();

            // Use real JwtService with test configuration
            var mockConfig = new Mock<IConfiguration>();
            var mockJwtSection = new Mock<IConfigurationSection>();
            mockJwtSection.Setup(x => x["SecretKey"]).Returns("TestSecretKeyMustBeAtLeast32CharactersLong!");
            mockJwtSection.Setup(x => x["Issuer"]).Returns("TestIssuer");
            mockJwtSection.Setup(x => x["Audience"]).Returns("TestAudience");
            mockJwtSection.Setup(x => x["ExpiryHours"]).Returns("24");
            mockConfig.Setup(x => x.GetSection("JwtSettings")).Returns(mockJwtSection.Object);

            _jwtService = new JwtService(mockConfig.Object);
            _userService = new UserService(_mockUserRepository.Object, _jwtService);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingUser_ReturnsUserResponseDto()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Role = "Visitor",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("test@example.com", result.Email);
            Assert.Equal("John Doe", result.FullName);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingUser_ReturnsNull()
        {
            // Arrange
            var userId = 999;
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_WithMultipleUsers_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new() { Id = 1, Email = "user1@example.com", FirstName = "User", LastName = "One", Role = "Visitor" },
                new() { Id = 2, Email = "user2@example.com", FirstName = "User", LastName = "Two", Role = "Admin" },
                new() { Id = 3, Email = "user3@example.com", FirstName = "User", LastName = "Three", Role = "Curator" }
            };

            _mockUserRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            Assert.Equal(3, result.Count());
            Assert.Contains(result, u => u.Role == "Admin");
            Assert.Contains(result, u => u.Role == "Curator");
            Assert.Contains(result, u => u.Role == "Visitor");
        }

        [Fact]
        public async Task RegisterAsync_WithNewUser_CreatesUserAndReturnsToken()
        {
            // Arrange
            var registerDto = new RegisterRequestDto
            {
                Email = "newuser@example.com",
                Password = "SecurePass123!",
                FirstName = "New",
                LastName = "User"
            };

            var createdUser = new User
            {
                Id = 1,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PasswordHash = "hashedPassword",
                Role = "Visitor",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _mockUserRepository.Setup(r => r.EmailExistsAsync(registerDto.Email)).ReturnsAsync(false);
            _mockUserRepository.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync(createdUser);

            // Act
            var result = await _userService.RegisterAsync(registerDto);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            Assert.Equal(registerDto.Email, result.Email);
            Assert.Equal("New User", result.FullName);
            Assert.Equal("Visitor", result.Role);
            _mockUserRepository.Verify(r => r.CreateAsync(It.Is<User>(u => u.Email == registerDto.Email)), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ThrowsArgumentException()
        {
            // Arrange
            var registerDto = new RegisterRequestDto
            {
                Email = "existing@example.com",
                Password = "SecurePass123!",
                FirstName = "Test",
                LastName = "User"
            };

            _mockUserRepository.Setup(r => r.EmailExistsAsync(registerDto.Email)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _userService.RegisterAsync(registerDto));
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse()
        {
            // Arrange
            var loginDto = new LoginRequestDto
            {
                Email = "user@example.com",
                Password = "CorrectPass123!"
            };

            var hashedPassword = _jwtService.HashPassword(loginDto.Password);

            var user = new User
            {
                Id = 1,
                Email = loginDto.Email,
                PasswordHash = hashedPassword,
                FirstName = "Test",
                LastName = "User",
                Role = "Visitor",
                IsActive = true
            };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(loginDto.Email)).ReturnsAsync(user);

            // Act
            var result = await _userService.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            Assert.Equal(loginDto.Email, result.Email);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidCredentials_ReturnsNull()
        {
            // Arrange
            var loginDto = new LoginRequestDto
            {
                Email = "user@example.com",
                Password = "WrongPass123!"
            };

            var correctPassword = "CorrectPass123!";
            var hashedPassword = _jwtService.HashPassword(correctPassword);

            var user = new User
            {
                Email = loginDto.Email,
                PasswordHash = hashedPassword,
                IsActive = true
            };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(loginDto.Email)).ReturnsAsync(user);

            // Act
            var result = await _userService.LoginAsync(loginDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_WithInactiveUser_ReturnsNull()
        {
            // Arrange
            var loginDto = new LoginRequestDto
            {
                Email = "user@example.com",
                Password = "CorrectPass123!"
            };

            var hashedPassword = _jwtService.HashPassword(loginDto.Password);

            var user = new User
            {
                Email = loginDto.Email,
                PasswordHash = hashedPassword,
                IsActive = false
            };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(loginDto.Email)).ReturnsAsync(user);

            // Act
            var result = await _userService.LoginAsync(loginDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProfileAsync_WithExistingUser_ReturnsProfile()
        {
            // Arrange
            var email = "user@example.com";
            var user = new User
            {
                Id = 1,
                Email = email,
                FirstName = "John",
                LastName = "Doe",
                Role = "Visitor",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetProfileAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
            Assert.Equal("John Doe", result.FullName);
        }

        [Fact]
        public async Task GetProfileAsync_WithNonExistingUser_ReturnsNull()
        {
            // Arrange
            var email = "nonexistent@example.com";
            _mockUserRepository.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetProfileAsync(email);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("Curator")]
        [InlineData("Visitor")]
        public async Task UpdateRoleAsync_WithValidRole_UpdatesAndReturnsUser(string role)
        {
            // Arrange
            var userId = 1;
            var updatedUser = new User
            {
                Id = userId,
                Email = "user@example.com",
                FirstName = "Test",
                LastName = "User",
                Role = role,
                IsActive = true
            };

            _mockUserRepository.Setup(r => r.UpdateRoleAsync(userId, role)).ReturnsAsync(updatedUser);

            // Act
            var result = await _userService.UpdateRoleAsync(userId, role);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(role, result.Role);
        }

        [Fact]
        public async Task UpdateRoleAsync_WithInvalidRole_ThrowsArgumentException()
        {
            // Arrange
            var userId = 1;
            var invalidRole = "SuperAdmin";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _userService.UpdateRoleAsync(userId, invalidRole));
        }

        [Fact]
        public async Task UpdateStatusAsync_DeactivateUser_ReturnsDeactivatedUser()
        {
            // Arrange
            var userId = 1;
            var updatedUser = new User
            {
                Id = userId,
                Email = "user@example.com",
                FirstName = "Test",
                LastName = "User",
                Role = "Visitor",
                IsActive = false
            };

            _mockUserRepository.Setup(r => r.UpdateStatusAsync(userId, false)).ReturnsAsync(updatedUser);

            // Act
            var result = await _userService.UpdateStatusAsync(userId, false);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsActive);
        }

        [Fact]
        public async Task DeleteAsync_WithExistingUser_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            _mockUserRepository.Setup(r => r.DeleteAsync(userId)).ReturnsAsync(true);

            // Act
            var result = await _userService.DeleteAsync(userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingUser_ReturnsFalse()
        {
            // Arrange
            var userId = 999;
            _mockUserRepository.Setup(r => r.DeleteAsync(userId)).ReturnsAsync(false);

            // Act
            var result = await _userService.DeleteAsync(userId);

            // Assert
            Assert.False(result);
        }
    }
}
