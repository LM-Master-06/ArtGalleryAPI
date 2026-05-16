// User Service Implementation
using ArtGalleryAPI.DTOs;
using ArtGalleryAPI.Models;
using ArtGalleryAPI.Repositories.Interfaces;
using ArtGalleryAPI.Services.Interfaces;

namespace ArtGalleryAPI.Services
{
    /// <summary>
    /// Service implementation for User business logic
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwtService;

        public UserService(IUserRepository userRepository, JwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<UserResponseDto?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return null;

            return MapToResponseDto(user);
        }

        public async Task<UserResponseDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return null;

            return MapToResponseDto(user);
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToResponseDto);
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerDto)
        {
            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(registerDto.Email))
            {
                throw new ArgumentException("Email is already registered");
            }

            var user = new User
            {
                Email = registerDto.Email,
                PasswordHash = _jwtService.HashPassword(registerDto.Password),
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Role = "Visitor", // Default role
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.CreateAsync(user);
            var token = _jwtService.GenerateToken(createdUser);

            return new AuthResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(24),
                Email = createdUser.Email,
                Role = createdUser.Role,
                FullName = $"{createdUser.FirstName} {createdUser.LastName}"
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);

            if (user == null || !user.IsActive)
            {
                return null; // Invalid credentials or inactive user
            }

            // Verify password
            if (!_jwtService.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            var token = _jwtService.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(24),
                Email = user.Email,
                Role = user.Role,
                FullName = $"{user.FirstName} {user.LastName}"
            };
        }

        public async Task<UserProfileDto?> GetProfileAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return null;

            return new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive
            };
        }

        public async Task<UserResponseDto?> UpdateRoleAsync(int userId, string role)
        {
            var validRoles = new[] { "Admin", "Curator", "Visitor" };
            if (!validRoles.Contains(role))
            {
                throw new ArgumentException("Invalid role. Must be Admin, Curator, or Visitor");
            }

            var user = await _userRepository.UpdateRoleAsync(userId, role);
            if (user == null)
                return null;

            return MapToResponseDto(user);
        }

        public async Task<UserResponseDto?> UpdateStatusAsync(int userId, bool isActive)
        {
            var user = await _userRepository.UpdateStatusAsync(userId, isActive);
            if (user == null)
                return null;

            return MapToResponseDto(user);
        }

        public async Task<UserSearchResponseDto> SearchAsync(UserSearchRequestDto searchRequest)
        {
            // Validate pagination
            if (searchRequest.PageNumber < 1)
                searchRequest.PageNumber = 1;
            if (searchRequest.PageSize < 1 || searchRequest.PageSize > 100)
                searchRequest.PageSize = 10;

            return await _userRepository.SearchAsync(searchRequest);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        private static UserResponseDto MapToResponseDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive
            };
        }
    }
}
