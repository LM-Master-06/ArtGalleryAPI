// Data Transfer Objects for Artist Entity
using System.ComponentModel.DataAnnotations;

namespace ArtGalleryAPI.DTOs
{
    /// <summary>
    /// DTO for returning artist data in API responses
    /// </summary>
    public class ArtistResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ClanGroup { get; set; }
        public string? Region { get; set; }
        public string? LanguageGroup { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public string? Biography { get; set; }
        public bool IsDeceased { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public int ArtifactCount { get; set; }
    }

    /// <summary>
    /// DTO for creating a new artist
    /// </summary>
    public class CreateArtistRequestDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ClanGroup { get; set; }

        [StringLength(200)]
        public string? Region { get; set; }

        [StringLength(100)]
        public string? LanguageGroup { get; set; }

        public DateTime? BirthDate { get; set; }

        public DateTime? DeathDate { get; set; }

        [StringLength(4000)]
        public string? Biography { get; set; }

        public bool IsDeceased { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing artist
    /// </summary>
    public class UpdateArtistRequestDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ClanGroup { get; set; }

        [StringLength(200)]
        public string? Region { get; set; }

        [StringLength(100)]
        public string? LanguageGroup { get; set; }

        public DateTime? BirthDate { get; set; }

        public DateTime? DeathDate { get; set; }

        [StringLength(4000)]
        public string? Biography { get; set; }

        public bool IsDeceased { get; set; }
    }

    /// <summary>
    /// DTO for searching/filtering artists
    /// </summary>
    public class ArtistSearchRequestDto
    {
        public string? SearchTerm { get; set; }
        public string? Region { get; set; }
        public string? ClanGroup { get; set; }
        public string? LanguageGroup { get; set; }
        public bool? IsDeceased { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// Paginated response for artist searches
    /// </summary>
    public class ArtistSearchResponseDto
    {
        public IEnumerable<ArtistResponseDto> Items { get; set; } = new List<ArtistResponseDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
