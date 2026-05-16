// Artifact Repository Interface
using ArtGalleryAPI.DTOs;
using ArtGalleryAPI.Models;

namespace ArtGalleryAPI.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Artifact entity operations
    /// </summary>
    public interface IArtifactRepository
    {
        Task<Artifact?> GetByIdAsync(int id);
        Task<IEnumerable<Artifact>> GetAllAsync();
        Task<Artifact> CreateAsync(Artifact artifact);
        Task<Artifact?> UpdateAsync(Artifact artifact);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<ArtifactSearchResponseDto> SearchAsync(ArtifactSearchRequestDto searchRequest);
        Task<Artifact?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Artifact>> GetByArtistIdAsync(int artistId);
        Task<IEnumerable<Artifact>> GetByArtTypeIdAsync(int artTypeId);
        Task<IEnumerable<Artifact>> GetAvailableAsync();
        Task<bool> UpdateAvailabilityAsync(int id, bool isAvailable);
        Task<ArtifactStatisticsDto> GetStatisticsAsync();
    }
}
