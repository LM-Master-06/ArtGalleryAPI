// Bounded Context: Artifacts Controller - Aboriginal Art Pieces
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArtGalleryAPI.DTOs;
using ArtGalleryAPI.Services.Interfaces;

namespace ArtGalleryAPI.Controllers
{
    /// <summary>
    /// Manages Aboriginal art artifacts (art pieces) in the gallery.
    /// Provides CRUD operations with role-based authorization.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ArtifactsController : ControllerBase
    {
        private readonly IArtifactService _artifactService;
        private readonly ILogger<ArtifactsController> _logger;

        public ArtifactsController(IArtifactService artifactService, ILogger<ArtifactsController> logger)
        {
            _artifactService = artifactService;
            _logger = logger;
        }

        /// <summary>
        /// Get all artifacts with artist and art type details
        /// </summary>
        /// <remarks>Public endpoint - no authentication required</remarks>
        /// <returns>List of all artifacts</returns>
        /// <response code="200">Returns list of artifacts</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ArtifactResponseDto>>), 200)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ArtifactResponseDto>>>> GetArtifacts()
        {
            var artifacts = await _artifactService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<ArtifactResponseDto>>.SuccessResponse(artifacts));
        }

        /// <summary>
        /// Get a specific artifact by ID
        /// </summary>
        /// <param name="id">Artifact ID</param>
        /// <returns>Artifact details</returns>
        /// <response code="200">Returns the artifact</response>
        /// <response code="404">Artifact not found</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<ArtifactResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<ApiResponse<ArtifactResponseDto>>> GetArtifact(int id)
        {
            var artifact = await _artifactService.GetByIdWithDetailsAsync(id);
            if (artifact == null)
            {
                return NotFound(ApiResponse<ArtifactResponseDto>.ErrorResponse($"Artifact with ID {id} not found"));
            }

            return Ok(ApiResponse<ArtifactResponseDto>.SuccessResponse(artifact));
        }

        /// <summary>
        /// Get artifact statistics
        /// </summary>
        /// <returns>Artifact statistics</returns>
        [HttpGet("statistics")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<ArtifactStatisticsDto>), 200)]
        public async Task<ActionResult<ApiResponse<ArtifactStatisticsDto>>> GetStatistics()
        {
            var stats = await _artifactService.GetStatisticsAsync();
            return Ok(ApiResponse<ArtifactStatisticsDto>.SuccessResponse(stats));
        }

        /// <summary>
        /// Get all available artifacts (for sale)
        /// </summary>
        /// <returns>List of available artifacts</returns>
        /// <response code="200">Returns available artifacts</response>
        [HttpGet("available")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ArtifactResponseDto>>), 200)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ArtifactResponseDto>>>> GetAvailableArtifacts()
        {
            var artifacts = await _artifactService.GetAvailableAsync();
            return Ok(ApiResponse<IEnumerable<ArtifactResponseDto>>.SuccessResponse(artifacts));
        }

        /// <summary>
        /// Search artifacts with advanced filters
        /// </summary>
        /// <param name="searchRequest">Search parameters</param>
        /// <returns>Paginated list of artifacts</returns>
        [HttpGet("search")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<ArtifactSearchResponseDto>), 200)]
        public async Task<ActionResult<ApiResponse<ArtifactSearchResponseDto>>> Search([FromQuery] ArtifactSearchRequestDto searchRequest)
        {
            var result = await _artifactService.SearchAsync(searchRequest);
            return Ok(ApiResponse<ArtifactSearchResponseDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Create a new artifact (Requires Curator or Admin role)
        /// </summary>
        /// <param name="createDto">Artifact details</param>
        /// <returns>Created artifact</returns>
        /// <response code="201">Artifact created successfully</response>
        /// <response code="400">Invalid data or artist/art type not found</response>
        /// <response code="401">Not authenticated</response>
        /// <response code="403">Not authorized (not Curator or Admin)</response>
        [HttpPost]
        [Authorize(Policy = "CuratorOrAdmin")]
        [ProducesResponseType(typeof(ApiResponse<ArtifactResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<ApiResponse<ArtifactResponseDto>>> CreateArtifact(CreateArtifactRequestDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ArtifactResponseDto>.ErrorResponse("Invalid data",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            try
            {
                var createdArtifact = await _artifactService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetArtifact), new { id = createdArtifact.Id },
                    ApiResponse<ArtifactResponseDto>.SuccessResponse(createdArtifact, "Artifact created successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<ArtifactResponseDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Update an existing artifact (Requires Curator or Admin role)
        /// </summary>
        /// <param name="id">Artifact ID</param>
        /// <param name="updateDto">Updated artifact details</param>
        /// <returns>Updated artifact</returns>
        /// <response code="200">Updated successfully</response>
        /// <response code="400">Invalid data</response>
        /// <response code="401">Not authenticated</response>
        /// <response code="403">Not authorized</response>
        /// <response code="404">Artifact not found</response>
        [HttpPut("{id}")]
        [Authorize(Policy = "CuratorOrAdmin")]
        [ProducesResponseType(typeof(ApiResponse<ArtifactResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<ApiResponse<ArtifactResponseDto>>> UpdateArtifact(int id, UpdateArtifactRequestDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest(ApiResponse<ArtifactResponseDto>.ErrorResponse("ID mismatch in route and body"));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ArtifactResponseDto>.ErrorResponse("Invalid data",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            try
            {
                var updatedArtifact = await _artifactService.UpdateAsync(updateDto);
                if (updatedArtifact == null)
                {
                    return NotFound(ApiResponse<ArtifactResponseDto>.ErrorResponse($"Artifact with ID {id} not found"));
                }

                return Ok(ApiResponse<ArtifactResponseDto>.SuccessResponse(updatedArtifact, "Artifact updated successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<ArtifactResponseDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Update artifact availability status (Requires Curator or Admin role)
        /// </summary>
        /// <param name="id">Artifact ID</param>
        /// <param name="availabilityDto">New availability status</param>
        /// <returns>No content on success</returns>
        [HttpPatch("{id}/availability")]
        [Authorize(Policy = "CuratorOrAdmin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateAvailability(int id, [FromBody] UpdateArtifactAvailabilityDto availabilityDto)
        {
            var result = await _artifactService.UpdateAvailabilityAsync(id, availabilityDto.IsAvailable);
            if (!result)
            {
                return NotFound(ApiResponse.ErrorResponse($"Artifact with ID {id} not found"));
            }

            return NoContent();
        }

        /// <summary>
        /// Delete an artifact (Admin only)
        /// </summary>
        /// <param name="id">Artifact ID</param>
        /// <returns>No content on success</returns>
        /// <response code="204">Deleted successfully</response>
        /// <response code="401">Not authenticated</response>
        /// <response code="403">Not authorized (not Admin)</response>
        /// <response code="404">Artifact not found</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteArtifact(int id)
        {
            var result = await _artifactService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse.ErrorResponse($"Artifact with ID {id} not found"));
            }

            return NoContent();
        }
    }
}
