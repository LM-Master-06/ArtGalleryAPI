// Bounded Context: ArtTypes Controller - Categories of Aboriginal Art
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArtGalleryAPI.DTOs;
using ArtGalleryAPI.Services.Interfaces;

namespace ArtGalleryAPI.Controllers
{
    /// <summary>
    /// Manages art type categories (Dot Painting, Bark Painting, etc.)
    /// Provides CRUD operations with role-based authorization.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ArtTypesController : ControllerBase
    {
        private readonly IArtTypeService _artTypeService;
        private readonly ILogger<ArtTypesController> _logger;

        public ArtTypesController(IArtTypeService artTypeService, ILogger<ArtTypesController> logger)
        {
            _artTypeService = artTypeService;
            _logger = logger;
        }

        /// <summary>
        /// Get all art types/categories
        /// </summary>
        /// <remarks>Public endpoint - no authentication required</remarks>
        /// <returns>List of all art types</returns>
        /// <response code="200">Returns list of art types</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ArtTypeResponseDto>>), 200)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ArtTypeResponseDto>>>> GetArtTypes()
        {
            var artTypes = await _artTypeService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<ArtTypeResponseDto>>.SuccessResponse(artTypes));
        }

        /// <summary>
        /// Get a specific art type by ID
        /// </summary>
        /// <param name="id">Art type ID</param>
        /// <returns>Art type details</returns>
        /// <response code="200">Returns the art type</response>
        /// <response code="404">Art type not found</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<ArtTypeResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<ApiResponse<ArtTypeResponseDto>>> GetArtType(int id)
        {
            var artType = await _artTypeService.GetByIdAsync(id);
            if (artType == null)
            {
                return NotFound(ApiResponse<ArtTypeResponseDto>.ErrorResponse($"Art type with ID {id} not found"));
            }

            return Ok(ApiResponse<ArtTypeResponseDto>.SuccessResponse(artType));
        }

        /// <summary>
        /// Get all artifacts of a specific art type
        /// </summary>
        /// <param name="id">Art type ID</param>
        /// <returns>Artifacts of this type with artist info</returns>
        /// <response code="200">Returns list of artifacts</response>
        /// <response code="404">Art type not found</response>
        [HttpGet("{id}/artifacts")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<ArtTypeResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<ApiResponse<ArtTypeResponseDto>>> GetArtifactsByType(int id)
        {
            var artType = await _artTypeService.GetByIdWithArtifactsAsync(id);
            if (artType == null)
            {
                return NotFound(ApiResponse<ArtTypeResponseDto>.ErrorResponse($"Art type with ID {id} not found"));
            }

            return Ok(ApiResponse<ArtTypeResponseDto>.SuccessResponse(artType));
        }

        /// <summary>
        /// Search art types with filters
        /// </summary>
        /// <param name="searchRequest">Search parameters</param>
        /// <returns>Paginated list of art types</returns>
        [HttpGet("search")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<ArtTypeSearchResponseDto>), 200)]
        public async Task<ActionResult<ApiResponse<ArtTypeSearchResponseDto>>> Search([FromQuery] ArtTypeSearchRequestDto searchRequest)
        {
            var result = await _artTypeService.SearchAsync(searchRequest);
            return Ok(ApiResponse<ArtTypeSearchResponseDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Create a new art type (Requires Curator or Admin role)
        /// </summary>
        /// <param name="createDto">Art type details</param>
        /// <returns>Created art type</returns>
        /// <response code="201">Art type created successfully</response>
        /// <response code="400">Invalid data</response>
        /// <response code="401">Not authenticated</response>
        /// <response code="403">Not authorized (not Curator or Admin)</response>
        [HttpPost]
        [Authorize(Policy = "CuratorOrAdmin")]
        [ProducesResponseType(typeof(ApiResponse<ArtTypeResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<ApiResponse<ArtTypeResponseDto>>> CreateArtType(CreateArtTypeRequestDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ArtTypeResponseDto>.ErrorResponse("Invalid data",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var createdArtType = await _artTypeService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetArtType), new { id = createdArtType.Id },
                ApiResponse<ArtTypeResponseDto>.SuccessResponse(createdArtType, "Art type created successfully"));
        }

        /// <summary>
        /// Update an existing art type (Requires Curator or Admin role)
        /// </summary>
        /// <param name="id">Art type ID</param>
        /// <param name="updateDto">Updated art type details</param>
        /// <returns>No content on success</returns>
        /// <response code="200">Updated successfully</response>
        /// <response code="400">Invalid data</response>
        /// <response code="401">Not authenticated</response>
        /// <response code="403">Not authorized</response>
        /// <response code="404">Art type not found</response>
        [HttpPut("{id}")]
        [Authorize(Policy = "CuratorOrAdmin")]
        [ProducesResponseType(typeof(ApiResponse<ArtTypeResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<ApiResponse<ArtTypeResponseDto>>> UpdateArtType(int id, UpdateArtTypeRequestDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest(ApiResponse<ArtTypeResponseDto>.ErrorResponse("ID mismatch in route and body"));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ArtTypeResponseDto>.ErrorResponse("Invalid data",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var updatedArtType = await _artTypeService.UpdateAsync(updateDto);
            if (updatedArtType == null)
            {
                return NotFound(ApiResponse<ArtTypeResponseDto>.ErrorResponse($"Art type with ID {id} not found"));
            }

            return Ok(ApiResponse<ArtTypeResponseDto>.SuccessResponse(updatedArtType, "Art type updated successfully"));
        }

        /// <summary>
        /// Delete an art type (Admin only)
        /// </summary>
        /// <param name="id">Art type ID</param>
        /// <returns>No content on success</returns>
        /// <response code="204">Deleted successfully</response>
        /// <response code="401">Not authenticated</response>
        /// <response code="403">Not authorized (not Admin)</response>
        /// <response code="404">Art type not found</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteArtType(int id)
        {
            var result = await _artTypeService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse.ErrorResponse($"Art type with ID {id} not found"));
            }

            return NoContent();
        }
    }
}
