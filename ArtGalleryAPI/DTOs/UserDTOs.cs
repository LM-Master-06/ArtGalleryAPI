// Data Transfer Objects for User Entity
using System.ComponentModel.DataAnnotations;

namespace ArtGalleryAPI.DTOs
{
    /// <summary>
    /// DTO for returning user data in API responses (admin view)
    /// </summary>
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for returning current user profile
    /// </summary>
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for user registration
    /// </summary>
    public class RegisterRequestDto
    {
        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for user login
    /// </summary>
    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response DTO after successful authentication
    /// </summary>
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for updating user role (admin only)
    /// </summary>
    public class UpdateUserRoleDto
    {
        [Required]
        [RegularExpression("^(Admin|Curator|Visitor)$", ErrorMessage = "Role must be Admin, Curator, or Visitor")]
        public string Role { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for changing user active status
    /// </summary>
    public class UpdateUserStatusDto
    {
        [Required]
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for searching users (admin only)
    /// </summary>
    public class UserSearchRequestDto
    {
        public string? SearchTerm { get; set; }
        public string? Role { get; set; }
        public bool? IsActive { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// Paginated response for user searches
    /// </summary>
    public class UserSearchResponseDto
    {
        public IEnumerable<UserResponseDto> Items { get; set; } = new List<UserResponseDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
