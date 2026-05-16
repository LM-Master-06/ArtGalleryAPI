// ArtType Repository Interface
using ArtGalleryAPI.DTOs;
using ArtGalleryAPI.Models;

namespace ArtGalleryAPI.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for ArtType entity operations
    /// </summary>
    public interface IArtTypeRepository
    {
        Task<ArtType?> GetByIdAsync(int id);
        Task<IEnumerable<ArtType>> GetAllAsync();
        Task<ArtType> CreateAsync(ArtType artType);
        Task<ArtType?> UpdateAsync(ArtType artType);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<ArtTypeSearchResponseDto> SearchAsync(ArtTypeSearchRequestDto searchRequest);
        Task<ArtType?> GetByIdWithArtifactsAsync(int id);
        Task<int> GetArtifactCountAsync(int artTypeId);
        Task<bool> HasArtifactsAsync(int artTypeId);
    }
}
