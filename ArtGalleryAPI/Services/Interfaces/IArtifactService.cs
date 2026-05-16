// Artifact Service Interface
using ArtGalleryAPI.DTOs;

namespace ArtGalleryAPI.Services.Interfaces
{
    /// <summary>
    /// Service interface for Artifact business logic
    /// </summary>
    public interface IArtifactService
    {
        Task<ArtifactResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<ArtifactResponseDto>> GetAllAsync();
        Task<ArtifactResponseDto> CreateAsync(CreateArtifactRequestDto createDto);
        Task<ArtifactResponseDto?> UpdateAsync(UpdateArtifactRequestDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<ArtifactSearchResponseDto> SearchAsync(ArtifactSearchRequestDto searchRequest);
        Task<ArtifactResponseDto?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<ArtifactResponseDto>> GetByArtistIdAsync(int artistId);
        Task<IEnumerable<ArtifactResponseDto>> GetByArtTypeIdAsync(int artTypeId);
        Task<IEnumerable<ArtifactResponseDto>> GetAvailableAsync();
        Task<bool> UpdateAvailabilityAsync(int id, bool isAvailable);
        Task<ArtifactStatisticsDto> GetStatisticsAsync();
    }
}
