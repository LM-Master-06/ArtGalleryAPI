// Data Transfer Objects for ArtType Entity
using System.ComponentModel.DataAnnotations;

namespace ArtGalleryAPI.DTOs
{
    /// <summary>
    /// DTO for returning art type data in API responses
    /// </summary>
    public class ArtTypeResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Technique { get; set; } = string.Empty;
        public int ArtifactCount { get; set; }
    }

    /// <summary>
    /// DTO for creating a new art type
    /// </summary>
    public class CreateArtTypeRequestDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Region { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Technique { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for updating an existing art type
    /// </summary>
    public class UpdateArtTypeRequestDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Region { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Technique { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for searching/filtering art types
    /// </summary>
    public class ArtTypeSearchRequestDto
    {
        public string? SearchTerm { get; set; }
        public string? Region { get; set; }
        public string? Technique { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// Paginated response for art type searches
    /// </summary>
    public class ArtTypeSearchResponseDto
    {
        public IEnumerable<ArtTypeResponseDto> Items { get; set; } = new List<ArtTypeResponseDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
