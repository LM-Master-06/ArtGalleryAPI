// User Service Interface
using ArtGalleryAPI.DTOs;

namespace ArtGalleryAPI.Services.Interfaces
{
    /// <summary>
    /// Service interface for User business logic
    /// </summary>
    public interface IUserService
    {
        Task<UserResponseDto?> GetByIdAsync(int id);
        Task<UserResponseDto?> GetByEmailAsync(string email);
        Task<IEnumerable<UserResponseDto>> GetAllAsync();
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerDto);
        Task<AuthResponseDto?> LoginAsync(LoginRequestDto loginDto);
        Task<UserProfileDto?> GetProfileAsync(string email);
        Task<UserResponseDto?> UpdateRoleAsync(int userId, string role);
        Task<UserResponseDto?> UpdateStatusAsync(int userId, bool isActive);
        Task<UserSearchResponseDto> SearchAsync(UserSearchRequestDto searchRequest);
        Task<bool> DeleteAsync(int id);
    }
}
