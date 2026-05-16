// Bounded Context: Users - Authentication & Authorization Controller
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArtGalleryAPI.DTOs;
using ArtGalleryAPI.Services.Interfaces;

namespace ArtGalleryAPI.Controllers
{
    /// <summary>
    /// Handles user authentication and authorization for the Art Gallery API.
    /// Implements JWT-based authentication as an uncovered approach for HD requirement.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user account
        /// </summary>
        /// <param name="request">User registration details</param>
        /// <returns>Authentication token for the new user</returns>
        /// <response code="200">Returns the JWT token</response>
        /// <response code="400">Email already exists or validation failed</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid data",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            try
            {
                var response = await _userService.RegisterAsync(request);
                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "User registered successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Authenticate user and get JWT token
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>JWT token if authentication succeeds</returns>
        /// <response code="200">Returns the JWT token</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid data",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var response = await _userService.LoginAsync(request);
            if (response == null)
            {
                return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid email or password"));
            }

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "Login successful"));
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        /// <returns>Current user details</returns>
        /// <response code="200">Returns user profile</response>
        /// <response code="401">User not authenticated</response>
        [HttpGet("profile")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetProfile()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized(ApiResponse<UserProfileDto>.ErrorResponse("User not authenticated"));
            }

            var profile = await _userService.GetProfileAsync(email);
            if (profile == null)
            {
                return NotFound(ApiResponse<UserProfileDto>.ErrorResponse("User profile not found"));
            }

            return Ok(ApiResponse<UserProfileDto>.SuccessResponse(profile));
        }

        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
        /// <returns>List of all users</returns>
        /// <response code="200">Returns list of users</response>
        /// <response code="403">Not authorized (not admin)</response>
        [HttpGet("users")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserResponseDto>>), 200)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserResponseDto>>>> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<UserResponseDto>>.SuccessResponse(users));
        }

        /// <summary>
        /// Search users (Admin only)
        /// </summary>
        /// <param name="searchRequest">Search parameters</param>
        /// <returns>Paginated list of users</returns>
        [HttpGet("users/search")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ApiResponse<UserSearchResponseDto>), 200)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<ApiResponse<UserSearchResponseDto>>> SearchUsers([FromQuery] UserSearchRequestDto searchRequest)
        {
            var result = await _userService.SearchAsync(searchRequest);
            return Ok(ApiResponse<UserSearchResponseDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Assign role to user (Admin only)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="roleDto">Role assignment</param>
        /// <returns>Updated user</returns>
        [HttpPut("users/{userId}/role")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> AssignRole(int userId, [FromBody] UpdateUserRoleDto roleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<UserResponseDto>.ErrorResponse("Invalid data",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            try
            {
                var user = await _userService.UpdateRoleAsync(userId, roleDto.Role);
                if (user == null)
                {
                    return NotFound(ApiResponse<UserResponseDto>.ErrorResponse($"User with ID {userId} not found"));
                }

                return Ok(ApiResponse<UserResponseDto>.SuccessResponse(user, "Role updated successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<UserResponseDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Update user active status (Admin only)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="statusDto">Status update</param>
        /// <returns>Updated user</returns>
        [HttpPut("users/{userId}/status")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> UpdateUserStatus(int userId, [FromBody] UpdateUserStatusDto statusDto)
        {
            var user = await _userService.UpdateStatusAsync(userId, statusDto.IsActive);
            if (user == null)
            {
                return NotFound(ApiResponse<UserResponseDto>.ErrorResponse($"User with ID {userId} not found"));
            }

            var statusMessage = statusDto.IsActive ? "activated" : "deactivated";
            return Ok(ApiResponse<UserResponseDto>.SuccessResponse(user, $"User {statusMessage} successfully"));
        }

        /// <summary>
        /// Delete user (Admin only)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>No content on success</returns>
        [HttpDelete("users/{userId}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var result = await _userService.DeleteAsync(userId);
            if (!result)
            {
                return NotFound(ApiResponse.ErrorResponse($"User with ID {userId} not found"));
            }

            return NoContent();
        }
    }
}
