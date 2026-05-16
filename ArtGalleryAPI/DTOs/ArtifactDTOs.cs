// Data Transfer Objects for Artifact Entity
using System.ComponentModel.DataAnnotations;

namespace ArtGalleryAPI.DTOs
{
    /// <summary>
    /// DTO for returning artifact data in API responses
    /// </summary>
    public class ArtifactResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Story { get; set; }
        public string? Dimensions { get; set; }
        public string? Medium { get; set; }
        public int? YearCreated { get; set; }
        public decimal? Price { get; set; }
        public bool IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        // Nested DTOs for related entities
        public ArtistSummaryDto? Artist { get; set; }
        public ArtTypeSummaryDto? ArtType { get; set; }
    }

    /// <summary>
    /// Summary DTO for Artist (used in nested responses)
    /// </summary>
    public class ArtistSummaryDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Region { get; set; }
    }

    /// <summary>
    /// Summary DTO for ArtType (used in nested responses)
    /// </summary>
    public class ArtTypeSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Technique { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for creating a new artifact
    /// </summary>
    public class CreateArtifactRequestDto
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [StringLength(4000)]
        public string? Story { get; set; }

        [StringLength(50)]
        public string? Dimensions { get; set; }

        [StringLength(100)]
        public string? Medium { get; set; }

        [Range(1800, 2100)]
        public int? YearCreated { get; set; }

        [Range(0, 100000000)]
        public decimal? Price { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Url]
        [StringLength(500)]
        public string? ImageUrl { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int ArtistId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int ArtTypeId { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing artifact
    /// </summary>
    public class UpdateArtifactRequestDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [StringLength(4000)]
        public string? Story { get; set; }

        [StringLength(50)]
        public string? Dimensions { get; set; }

        [StringLength(100)]
        public string? Medium { get; set; }

        [Range(1800, 2100)]
        public int? YearCreated { get; set; }

        [Range(0, 100000000)]
        public decimal? Price { get; set; }

        public bool IsAvailable { get; set; }

        [Url]
        [StringLength(500)]
        public string? ImageUrl { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int ArtistId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int ArtTypeId { get; set; }
    }

    /// <summary>
    /// DTO for updating artifact availability status
    /// </summary>
    public class UpdateArtifactAvailabilityDto
    {
        [Required]
        public bool IsAvailable { get; set; }
    }

    /// <summary>
    /// DTO for searching/filtering artifacts
    /// </summary>
    public class ArtifactSearchRequestDto
    {
        public string? SearchTerm { get; set; }
        public int? ArtistId { get; set; }
        public int? ArtTypeId { get; set; }
        public bool? IsAvailable { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public string? Medium { get; set; }
        public string? SortBy { get; set; } = "CreatedAt"; // CreatedAt, Title, Price, YearCreated
        public bool SortDescending { get; set; } = true;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// Paginated response for artifact searches
    /// </summary>
    public class ArtifactSearchResponseDto
    {
        public IEnumerable<ArtifactResponseDto> Items { get; set; } = new List<ArtifactResponseDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public decimal? MinPriceFound { get; set; }
        public decimal? MaxPriceFound { get; set; }
    }

    /// <summary>
    /// DTO for artifact statistics
    /// </summary>
    public class ArtifactStatisticsDto
    {
        public int TotalArtifacts { get; set; }
        public int AvailableArtifacts { get; set; }
        public int SoldArtifacts { get; set; }
        public decimal? TotalValue { get; set; }
        public decimal? AveragePrice { get; set; }
        public List<ArtifactCountByTypeDto> CountByType { get; set; } = new();
        public List<ArtifactCountByArtistDto> CountByArtist { get; set; } = new();
    }

    public class ArtifactCountByTypeDto
    {
        public int ArtTypeId { get; set; }
        public string ArtTypeName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class ArtifactCountByArtistDto
    {
        public int ArtistId { get; set; }
        public string ArtistName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
