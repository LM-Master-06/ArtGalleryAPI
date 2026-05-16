// Bounded Context: Users - Authentication & User Management
using System.ComponentModel.DataAnnotations;

namespace ArtGalleryAPI.Models
{
    /// <summary>
    /// Represents a user in the Art Gallery system with authentication credentials
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique identifier for the user
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Email address used for login (must be unique)
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Hashed password for authentication
        /// </summary>
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// User's first name
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// User's last name
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// User role for authorization (Admin, Curator, Visitor)
        /// </summary>
        public string Role { get; set; } = "Visitor";

        /// <summary>
        /// When the user account was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Whether the account is active
        /// </summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Data transfer object for user registration
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// User's email address
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password (min 6 characters)
        /// </summary>
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// User's first name
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// User's last name
        /// </summary>
        public string LastName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data transfer object for user login
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// User's email address
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's password
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response object containing JWT token after successful authentication
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// JWT access token
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Token expiration time
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// User's email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's role
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// User's full name
        /// </summary>
        public string FullName { get; set; } = string.Empty;
    }
}
