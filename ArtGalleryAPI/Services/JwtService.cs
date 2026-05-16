// JWT Authentication Service for Art Gallery API
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ArtGalleryAPI.Models;

namespace ArtGalleryAPI.Services
{
    /// <summary>
    /// Service for handling JWT token generation and validation
    /// Implements uncovered authentication approach as per assignment requirements
    /// </summary>
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Generates a JWT token for authenticated user
        /// </summary>
        /// <param name="user">The authenticated user</param>
        /// <returns>JWT token string</returns>
        public string GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var issuer = jwtSettings["Issuer"] ?? "ArtGalleryAPI";
            var audience = jwtSettings["Audience"] ?? "ArtGalleryClient";
            var expiryHours = int.Parse(jwtSettings["ExpiryHours"] ?? "24");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expiryHours),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Hashes a password using SHA256 with salt
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <returns>Hashed password with salt and checksum</returns>
        public string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Combine password and salt, then hash
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] combined = new byte[passwordBytes.Length + salt.Length];
                Buffer.BlockCopy(passwordBytes, 0, combined, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, combined, passwordBytes.Length, salt.Length);

                byte[] hash = sha256.ComputeHash(combined);

                // Create a checksum of the salt+hash for tamper detection (first 4 bytes of second hash)
                byte[] checksumInput = new byte[salt.Length + hash.Length];
                Buffer.BlockCopy(salt, 0, checksumInput, 0, salt.Length);
                Buffer.BlockCopy(hash, 0, checksumInput, salt.Length, hash.Length);
                byte[] checksum = sha256.ComputeHash(checksumInput);

                // Combine salt + hash + checksum (4 bytes) for storage
                byte[] result = new byte[salt.Length + hash.Length + 4];
                Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
                Buffer.BlockCopy(hash, 0, result, salt.Length, hash.Length);
                Buffer.BlockCopy(checksum, 0, result, salt.Length + hash.Length, 4);

                return Convert.ToBase64String(result);
            }
        }

        /// <summary>
        /// Verifies a password against stored hash
        /// </summary>
        /// <param name="password">Plain text password to verify</param>
        /// <param name="storedHash">Stored hash from database</param>
        /// <returns>True if password matches</returns>
        /// <exception cref="FormatException">Thrown when the stored hash is invalid or tampered with</exception>
        public bool VerifyPassword(string password, string storedHash)
        {
            byte[] storedBytes = Convert.FromBase64String(storedHash);

            // Validate that the stored hash has the expected length (16 bytes salt + 32 bytes SHA256 hash + 4 bytes checksum)
            if (storedBytes.Length != 52)
            {
                throw new FormatException("Invalid password hash format: hash length mismatch. The hash may have been tampered with.");
            }

            byte[] salt = new byte[16];
            byte[] hash = new byte[32];
            byte[] storedChecksum = new byte[4];
            Buffer.BlockCopy(storedBytes, 0, salt, 0, 16);
            Buffer.BlockCopy(storedBytes, 16, hash, 0, 32);
            Buffer.BlockCopy(storedBytes, 48, storedChecksum, 0, 4);

            // Verify checksum to detect tampering
            using (var sha256 = SHA256.Create())
            {
                byte[] checksumInput = new byte[48];
                Buffer.BlockCopy(salt, 0, checksumInput, 0, 16);
                Buffer.BlockCopy(hash, 0, checksumInput, 16, 32);
                byte[] computedChecksum = sha256.ComputeHash(checksumInput);

                // Compare stored checksum with computed checksum
                for (int i = 0; i < 4; i++)
                {
                    if (storedChecksum[i] != computedChecksum[i])
                    {
                        throw new FormatException("Password hash integrity check failed. The hash may have been tampered with.");
                    }
                }

                // Now verify the password
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] combined = new byte[passwordBytes.Length + salt.Length];
                Buffer.BlockCopy(passwordBytes, 0, combined, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, combined, passwordBytes.Length, salt.Length);

                byte[] computedHash = sha256.ComputeHash(combined);

                // Compare computed hash with stored hash
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (hash[i] != computedHash[i])
                        return false;
                }
                return true;
            }
        }
    }
}
