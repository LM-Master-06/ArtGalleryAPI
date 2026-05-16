// Artist Service Interface
using ArtGalleryAPI.DTOs;

namespace ArtGalleryAPI.Services.Interfaces
{
    /// <summary>
    /// Service interface for Artist business logic
    /// </summary>
    public interface IArtistService
    {
        Task<ArtistResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<ArtistResponseDto>> GetAllAsync();
        Task<ArtistResponseDto> CreateAsync(CreateArtistRequestDto createDto);
        Task<ArtistResponseDto?> UpdateAsync(UpdateArtistRequestDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<ArtistSearchResponseDto> SearchAsync(ArtistSearchRequestDto searchRequest);
        Task<ArtistResponseDto?> GetByIdWithArtifactsAsync(int id);
        Task<ArtistStatisticsDto> GetStatisticsAsync();
    }

    /// <summary>
    /// Statistics for artists
    /// </summary>
    public class ArtistStatisticsDto
    {
        public int TotalArtists { get; set; }
        public int DeceasedArtists { get; set; }
        public int LivingArtists { get; set; }
        public Dictionary<string, int> ArtistsByRegion { get; set; } = new();
        public Dictionary<string, int> ArtistsByLanguageGroup { get; set; } = new();
    }
}
