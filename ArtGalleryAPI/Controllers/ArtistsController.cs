// Bounded Context: Artists Controller - Aboriginal Artists Management
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArtGalleryAPI.DTOs;
using ArtGalleryAPI.Services.Interfaces;

namespace ArtGalleryAPI.Controllers
{
    /// <summary>
    /// Manages Aboriginal artist profiles in the gallery.
    /// Provides CRUD operations with role-based authorization.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ArtistsController : ControllerBase
    {
        private readonly IArtistService _artistService;
        private readonly ILogger<ArtistsController> _logger;

        public ArtistsController(IArtistService artistService, ILogger<ArtistsController> logger)
        {
            _artistService = artistService;
            _logger = logger;
        }

        /// <summary>
        /// Get all artists
        /// </summary>
        /// <remarks>Public endpoint - no authentication required</remarks>
        /// <returns>List of all artists</returns>
        /// <response code="200">Returns list of artists</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ArtistResponseDto>>), 200)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ArtistResponseDto>>>> GetArtists()
        {
            var artists = await _artistService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<ArtistResponseDto>>.SuccessResponse(artists));
        }

        /// <summary>
        /// Get a specific artist by ID
        /// </summary>
        /// <param name="id">Artist ID</param>
        /// <returns>Artist details</returns>
        /// <response code="200">Returns the artist</response>
        /// <response code="404">Artist not found</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<ArtistResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<ApiResponse<ArtistResponseDto>>> GetArtist(int id)
        {
            var artist = await _artistService.GetByIdAsync(id);
            if (artist == null)
            {
                return NotFound(ApiResponse<ArtistResponseDto>.ErrorResponse($"Artist with ID {id} not found"));
            }

            return Ok(ApiResponse<ArtistResponseDto>.SuccessResponse(artist));
        }

        /// <summary>
        /// Get artist statistics
        /// </summary>
        /// <returns>Artist statistics</returns>
        [HttpGet("statistics")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<ArtistStatisticsDto>), 200)]
        public async Task<ActionResult<ApiResponse<ArtistStatisticsDto>>> GetStatistics()
        {
            var stats = await _artistService.GetStatisticsAsync();
            return Ok(ApiResponse<ArtistStatisticsDto>.SuccessResponse(stats));
        }

        /// <summary>
        /// Search artists with filters
        /// </summary>
        /// <param name="searchRequest">Search parameters</param>
        /// <returns>Paginated list of artists</returns>
        [HttpGet("search")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<ArtistSearchResponseDto>), 200)]
        public async Task<ActionResult<ApiResponse<ArtistSearchResponseDto>>> Search([FromQuery] ArtistSearchRequestDto searchRequest)
        {
            var result = await _artistService.SearchAsync(searchRequest);
            return Ok(ApiResponse<ArtistSearchResponseDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Create a new artist (Requires Curator or Admin role)
        /// </summary>
        /// <param name="createDto">Artist details</param>
        /// <returns>Created artist</returns>
        /// <response code="201">Artist created successfully</response>
        /// <response code="400">Invalid data</response>
        /// <response code="401">Not authenticated</response>
        /// <response code="403">Not authorized (not Curator or Admin)</response>
        [HttpPost]
        [Authorize(Policy = "CuratorOrAdmin")]
        [ProducesResponseType(typeof(ApiResponse<ArtistResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<ApiResponse<ArtistResponseDto>>> CreateArtist(CreateArtistRequestDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ArtistResponseDto>.ErrorResponse("Invalid data",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var createdArtist = await _artistService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetArtist), new { id = createdArtist.Id },
                ApiResponse<ArtistResponseDto>.SuccessResponse(createdArtist, "Artist created successfully"));
        }

        /// <summary>
        /// Update an existing artist (Requires Curator or Admin role)
        /// </summary>
        /// <param name="id">Artist ID</param>
        /// <param name="updateDto">Updated artist details</param>
        /// <returns>No content on success</returns>
        /// <response code="200">Updated successfully</response>
        /// <response code="400">Invalid data</response>
        /// <response code="401">Not authenticated</response>
        /// <response code="403">Not authorized</response>
        /// <response code="404">Artist not found</response>
        [HttpPut("{id}")]
        [Authorize(Policy = "CuratorOrAdmin")]
        [ProducesResponseType(typeof(ApiResponse<ArtistResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<ApiResponse<ArtistResponseDto>>> UpdateArtist(int id, UpdateArtistRequestDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest(ApiResponse<ArtistResponseDto>.ErrorResponse("ID mismatch in route and body"));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ArtistResponseDto>.ErrorResponse("Invalid data",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var updatedArtist = await _artistService.UpdateAsync(updateDto);
            if (updatedArtist == null)
            {
                return NotFound(ApiResponse<ArtistResponseDto>.ErrorResponse($"Artist with ID {id} not found"));
            }

            return Ok(ApiResponse<ArtistResponseDto>.SuccessResponse(updatedArtist, "Artist updated successfully"));
        }

        /// <summary>
        /// Delete an artist (Admin only)
        /// </summary>
        /// <param name="id">Artist ID</param>
        /// <returns>No content on success</returns>
        /// <response code="204">Deleted successfully</response>
        /// <response code="401">Not authenticated</response>
        /// <response code="403">Not authorized (not Admin)</response>
        /// <response code="404">Artist not found</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteArtist(int id)
        {
            var result = await _artistService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse.ErrorResponse($"Artist with ID {id} not found"));
            }

            return NoContent();
        }
    }
}
