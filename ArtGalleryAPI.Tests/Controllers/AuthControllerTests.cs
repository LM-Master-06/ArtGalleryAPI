// Auth Controller Integration Tests
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ArtGalleryAPI.Controllers;
using ArtGalleryAPI.DTOs;
using ArtGalleryAPI.Services.Interfaces;
using Moq;

namespace ArtGalleryAPI.Tests.Controllers
{
    /// <summary>
    /// Integration tests for Auth Controller
    /// Tests authentication endpoints and role-based access
    /// </summary>
    public class AuthControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _authController = new AuthController(_mockUserService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Register_WithValidData_ReturnsOkWithToken()
        {
            // Arrange
            var registerRequest = new RegisterRequestDto
            {
                Email = "newuser@example.com",
                Password = "SecurePass123!",
                FirstName = "New",
                LastName = "User"
            };

            var authResponse = new AuthResponseDto
            {
                Token = "jwt-token-123",
                Expiration = DateTime.UtcNow.AddHours(24),
                Email = registerRequest.Email,
                Role = "Visitor",
                FullName = "New User"
            };

            _mockUserService.Setup(s => s.RegisterAsync(registerRequest)).ReturnsAsync(authResponse);

            // Act
            var result = await _authController.Register(registerRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);

            var apiResponse = Assert.IsType<ApiResponse<AuthResponseDto>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("jwt-token-123", apiResponse.Data.Token);
        }

        [Fact]
        public async Task Register_WithExistingEmail_ReturnsBadRequest()
        {
            // Arrange
            var registerRequest = new RegisterRequestDto
            {
                Email = "existing@example.com",
                Password = "SecurePass123!",
                FirstName = "Test",
                LastName = "User"
            };

            _mockUserService.Setup(s => s.RegisterAsync(registerRequest))
                .ThrowsAsync(new ArgumentException("Email is already registered"));

            // Act
            var result = await _authController.Register(registerRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Email = "user@example.com",
                Password = "CorrectPass123!"
            };

            var authResponse = new AuthResponseDto
            {
                Token = "jwt-token-456",
                Expiration = DateTime.UtcNow.AddHours(24),
                Email = loginRequest.Email,
                Role = "Visitor",
                FullName = "Test User"
            };

            _mockUserService.Setup(s => s.LoginAsync(loginRequest)).ReturnsAsync(authResponse);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);

            var apiResponse = Assert.IsType<ApiResponse<AuthResponseDto>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Email = "user@example.com",
                Password = "WrongPass123!"
            };

            _mockUserService.Setup(s => s.LoginAsync(loginRequest)).ReturnsAsync((AuthResponseDto?)null);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }

        [Fact]
        public async Task GetProfile_WithAuthenticatedUser_ReturnsProfile()
        {
            // Arrange
            var email = "user@example.com";
            var profile = new UserProfileDto
            {
                Id = 1,
                Email = email,
                FirstName = "Test",
                LastName = "User",
                Role = "Visitor",
                IsActive = true
            };

            _mockUserService.Setup(s => s.GetProfileAsync(email)).ReturnsAsync(profile);

            // Setup User.Identity
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, email)
            }, "TestAuthentication"));

            _authController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            // Act
            var result = await _authController.GetProfile();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task GetProfile_WithUnauthenticatedUser_ReturnsUnauthorized()
        {
            // Arrange
            _authController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            // Act
            var result = await _authController.GetProfile();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }

        [Fact]
        public async Task GetAllUsers_WithAdminAccess_ReturnsUsersList()
        {
            // Arrange
            var users = new List<UserResponseDto>
            {
                new() { Id = 1, Email = "user1@example.com", Role = "Visitor" },
                new() { Id = 2, Email = "user2@example.com", Role = "Admin" }
            };

            _mockUserService.Setup(s => s.GetAllAsync()).ReturnsAsync(users);

            // Act
            var result = await _authController.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);

            var apiResponse = Assert.IsType<ApiResponse<IEnumerable<UserResponseDto>>>(okResult.Value);
            Assert.Equal(2, apiResponse.Data.Count());
        }

        [Fact]
        public async Task SearchUsers_WithValidRequest_ReturnsPaginatedResults()
        {
            // Arrange
            var searchRequest = new UserSearchRequestDto
            {
                SearchTerm = "test",
                PageNumber = 1,
                PageSize = 10
            };

            var searchResult = new UserSearchResponseDto
            {
                Items = new List<UserResponseDto>
                {
                    new() { Id = 1, Email = "test1@example.com" },
                    new() { Id = 2, Email = "test2@example.com" }
                },
                TotalCount = 2,
                PageNumber = 1,
                PageSize = 10
            };

            _mockUserService.Setup(s => s.SearchAsync(searchRequest)).ReturnsAsync(searchResult);

            // Act
            var result = await _authController.SearchUsers(searchRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult);
        }

        [Fact]
        public async Task AssignRole_WithValidRole_ReturnsUpdatedUser()
        {
            // Arrange
            var userId = 1;
            var roleDto = new UpdateUserRoleDto { Role = "Curator" };
            var updatedUser = new UserResponseDto
            {
                Id = userId,
                Email = "user@example.com",
                Role = "Curator"
            };

            _mockUserService.Setup(s => s.UpdateRoleAsync(userId, roleDto.Role)).ReturnsAsync(updatedUser);

            // Act
            var result = await _authController.AssignRole(userId, roleDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);

            var apiResponse = Assert.IsType<ApiResponse<UserResponseDto>>(okResult.Value);
            Assert.Equal("Curator", apiResponse.Data.Role);
        }

        [Fact]
        public async Task AssignRole_WithInvalidRole_ReturnsBadRequest()
        {
            // Arrange
            var userId = 1;
            var roleDto = new UpdateUserRoleDto { Role = "InvalidRole" };

            _mockUserService.Setup(s => s.UpdateRoleAsync(userId, roleDto.Role))
                .ThrowsAsync(new ArgumentException("Invalid role"));

            // Act
            var result = await _authController.AssignRole(userId, roleDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult);
        }

        [Fact]
        public async Task DeleteUser_WithExistingUser_ReturnsNoContent()
        {
            // Arrange
            var userId = 1;
            _mockUserService.Setup(s => s.DeleteAsync(userId)).ReturnsAsync(true);

            // Act
            var result = await _authController.DeleteUser(userId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUser_WithNonExistingUser_ReturnsNotFound()
        {
            // Arrange
            var userId = 999;
            _mockUserService.Setup(s => s.DeleteAsync(userId)).ReturnsAsync(false);

            // Act
            var result = await _authController.DeleteUser(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
