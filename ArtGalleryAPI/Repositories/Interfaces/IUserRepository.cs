// User Repository Interface
using ArtGalleryAPI.DTOs;
using ArtGalleryAPI.Models;

namespace ArtGalleryAPI.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for User entity operations
    /// </summary>
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> CreateAsync(User user);
        Task<User?> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task<UserSearchResponseDto> SearchAsync(UserSearchRequestDto searchRequest);
        Task<User?> UpdateRoleAsync(int userId, string role);
        Task<User?> UpdateStatusAsync(int userId, bool isActive);
    }
}
