// User Repository Implementation
using Microsoft.EntityFrameworkCore;
using ArtGalleryAPI.Data;
using ArtGalleryAPI.DTOs;
using ArtGalleryAPI.Models;
using ArtGalleryAPI.Repositories.Interfaces;

namespace ArtGalleryAPI.Repositories
{
    /// <summary>
    /// Repository implementation for User entity
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly ArtGalleryDbContext _context;

        public UserRepository(ArtGalleryDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .OrderBy(u => u.Email)
                .ToListAsync();
        }

        public async Task<User> CreateAsync(User user)
        {
            user.CreatedAt = DateTime.UtcNow;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateAsync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
                return null;

            _context.Entry(existingUser).CurrentValues.SetValues(user);
            await _context.SaveChangesAsync();
            return existingUser;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<UserSearchResponseDto> SearchAsync(UserSearchRequestDto searchRequest)
        {
            var query = _context.Users.AsNoTracking().AsQueryable();

            // Apply search term filter
            if (!string.IsNullOrWhiteSpace(searchRequest.SearchTerm))
            {
                var searchLower = searchRequest.SearchTerm.ToLower();
                query = query.Where(u =>
                    u.Email.ToLower().Contains(searchLower) ||
                    u.FirstName.ToLower().Contains(searchLower) ||
                    u.LastName.ToLower().Contains(searchLower));
            }

            // Apply role filter
            if (!string.IsNullOrWhiteSpace(searchRequest.Role))
            {
                query = query.Where(u => u.Role == searchRequest.Role);
            }

            // Apply active status filter
            if (searchRequest.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == searchRequest.IsActive.Value);
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var items = await query
                .OrderBy(u => u.Email)
                .Skip((searchRequest.PageNumber - 1) * searchRequest.PageSize)
                .Take(searchRequest.PageSize)
                .ToListAsync();

            return new UserSearchResponseDto
            {
                Items = items.Select(MapToResponseDto),
                TotalCount = totalCount,
                PageNumber = searchRequest.PageNumber,
                PageSize = searchRequest.PageSize
            };
        }

        public async Task<User?> UpdateRoleAsync(int userId, string role)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return null;

            user.Role = role;
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateStatusAsync(int userId, bool isActive)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return null;

            user.IsActive = isActive;
            await _context.SaveChangesAsync();
            return user;
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
