// Artist Repository Interface
using ArtGalleryAPI.DTOs;
using ArtGalleryAPI.Models;

namespace ArtGalleryAPI.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Artist entity operations
    /// </summary>
    public interface IArtistRepository
    {
        Task<Artist?> GetByIdAsync(int id);
        Task<IEnumerable<Artist>> GetAllAsync();
        Task<Artist> CreateAsync(Artist artist);
        Task<Artist?> UpdateAsync(Artist artist);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<ArtistSearchResponseDto> SearchAsync(ArtistSearchRequestDto searchRequest);
        Task<Artist?> GetByIdWithArtifactsAsync(int id);
        Task<int> GetArtifactCountAsync(int artistId);
    }
}
