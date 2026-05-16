// JWT Service Tests - Authentication & Security
using ArtGalleryAPI.Services;
using ArtGalleryAPI.Models;
using Microsoft.Extensions.Configuration;
using Moq;

namespace ArtGalleryAPI.Tests.Services
{
    /// <summary>
    /// Unit tests for JWT authentication service
    /// Tests token generation, password hashing, and verification
    /// </summary>
    public class JwtServiceTests
    {
        private readonly JwtService _jwtService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IConfigurationSection> _mockJwtSettings;

        public JwtServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockJwtSettings = new Mock<IConfigurationSection>();

            // Setup JWT configuration
            _mockJwtSettings.Setup(x => x["SecretKey"]).Returns("YourSuperSecretKeyMustBeAtLeast32CharactersLong!");
            _mockJwtSettings.Setup(x => x["Issuer"]).Returns("ArtGalleryAPI");
            _mockJwtSettings.Setup(x => x["Audience"]).Returns("ArtGalleryClient");
            _mockJwtSettings.Setup(x => x["ExpiryHours"]).Returns("24");

            _mockConfiguration.Setup(x => x.GetSection("JwtSettings")).Returns(_mockJwtSettings.Object);

            _jwtService = new JwtService(_mockConfiguration.Object);
        }

        [Fact]
        public void GenerateToken_WithValidUser_ReturnsNonEmptyToken()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Role = "Visitor"
            };

            // Act
            var token = _jwtService.GenerateToken(user);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
            Assert.Contains(".", token); // JWT tokens have 3 parts separated by dots
        }

        [Fact]
        public void GenerateToken_WithNullSecretKey_ThrowsInvalidOperationException()
        {
            // Arrange
            _mockJwtSettings.Setup(x => x["SecretKey"]).Returns((string?)null);
            var jwtServiceWithNullKey = new JwtService(_mockConfiguration.Object);

            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Role = "Visitor"
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => jwtServiceWithNullKey.GenerateToken(user));
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("Curator")]
        [InlineData("Visitor")]
        public void GenerateToken_WithDifferentRoles_GeneratesValidToken(string role)
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Role = role
            };

            // Act
            var token = _jwtService.GenerateToken(user);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public void HashPassword_WithValidPassword_ReturnsHashedString()
        {
            // Arrange
            var password = "SecurePassword123!";

            // Act
            var hashedPassword = _jwtService.HashPassword(password);

            // Assert
            Assert.NotNull(hashedPassword);
            Assert.NotEmpty(hashedPassword);
            Assert.NotEqual(password, hashedPassword); // Should not be plaintext
            Assert.True(Convert.FromBase64String(hashedPassword).Length > 16); // Salt (16) + Hash (32)
        }

        [Fact]
        public void HashPassword_SamePasswordDifferentHashes_ReturnsDifferentHashes()
        {
            // Arrange
            var password = "SecurePassword123!";

            // Act
            var hash1 = _jwtService.HashPassword(password);
            var hash2 = _jwtService.HashPassword(password);

            // Assert
            Assert.NotEqual(hash1, hash2); // Due to random salt, same password should produce different hashes
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
        {
            // Arrange
            var password = "SecurePassword123!";
            var hashedPassword = _jwtService.HashPassword(password);

            // Act
            var result = _jwtService.VerifyPassword(password, hashedPassword);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
        {
            // Arrange
            var password = "SecurePassword123!";
            var wrongPassword = "WrongPassword123!";
            var hashedPassword = _jwtService.HashPassword(password);

            // Act
            var result = _jwtService.VerifyPassword(wrongPassword, hashedPassword);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_WithTamperedHash_ThrowsException()
        {
            // Arrange
            var password = "SecurePassword123!";
            var hashedPassword = _jwtService.HashPassword(password);
            var tamperedHash = hashedPassword.Substring(0, hashedPassword.Length - 4) + "XXXX";

            // Act & Assert - Will throw an exception (FormatException or other) due to invalid base64
            Assert.ThrowsAny<Exception>(() => _jwtService.VerifyPassword(password, tamperedHash));
        }

        [Fact]
        public void HashPassword_WithEmptyPassword_ReturnsValidHash()
        {
            // Arrange
            var password = "";

            // Act
            var hashedPassword = _jwtService.HashPassword(password);

            // Assert
            Assert.NotNull(hashedPassword);
            Assert.NotEmpty(hashedPassword);
            // Should still be able to verify
            Assert.True(_jwtService.VerifyPassword(password, hashedPassword));
        }

        [Fact]
        public void HashPassword_WithLongPassword_ReturnsValidHash()
        {
            // Arrange
            var password = new string('a', 1000); // Very long password

            // Act
            var hashedPassword = _jwtService.HashPassword(password);
            var isValid = _jwtService.VerifyPassword(password, hashedPassword);

            // Assert
            Assert.NotNull(hashedPassword);
            Assert.NotEmpty(hashedPassword);
            Assert.True(isValid);
        }
    }
}
