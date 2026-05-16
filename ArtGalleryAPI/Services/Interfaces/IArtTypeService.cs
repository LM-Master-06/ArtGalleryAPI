// ArtType Service Interface
using ArtGalleryAPI.DTOs;

namespace ArtGalleryAPI.Services.Interfaces
{
    /// <summary>
    /// Service interface for ArtType business logic
    /// </summary>
    public interface IArtTypeService
    {
        Task<ArtTypeResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<ArtTypeResponseDto>> GetAllAsync();
        Task<ArtTypeResponseDto> CreateAsync(CreateArtTypeRequestDto createDto);
        Task<ArtTypeResponseDto?> UpdateAsync(UpdateArtTypeRequestDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<ArtTypeSearchResponseDto> SearchAsync(ArtTypeSearchRequestDto searchRequest);
        Task<ArtTypeResponseDto?> GetByIdWithArtifactsAsync(int id);
    }
}
