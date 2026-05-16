// ArtType Service Implementation
using ArtGalleryAPI.DTOs;
using ArtGalleryAPI.Models;
using ArtGalleryAPI.Repositories.Interfaces;
using ArtGalleryAPI.Services.Interfaces;

namespace ArtGalleryAPI.Services
{
    /// <summary>
    /// Service implementation for ArtType business logic
    /// </summary>
    public class ArtTypeService : IArtTypeService
    {
        private readonly IArtTypeRepository _artTypeRepository;

        public ArtTypeService(IArtTypeRepository artTypeRepository)
        {
            _artTypeRepository = artTypeRepository;
        }

        public async Task<ArtTypeResponseDto?> GetByIdAsync(int id)
        {
            var artType = await _artTypeRepository.GetByIdAsync(id);
            if (artType == null)
                return null;

            var dto = MapToResponseDto(artType);
            dto.ArtifactCount = await _artTypeRepository.GetArtifactCountAsync(id);
            return dto;
        }

        public async Task<IEnumerable<ArtTypeResponseDto>> GetAllAsync()
        {
            var artTypes = await _artTypeRepository.GetAllAsync();
            var result = new List<ArtTypeResponseDto>();

            foreach (var artType in artTypes)
            {
                var dto = MapToResponseDto(artType);
                dto.ArtifactCount = await _artTypeRepository.GetArtifactCountAsync(artType.Id);
                result.Add(dto);
            }

            return result;
        }

        public async Task<ArtTypeResponseDto> CreateAsync(CreateArtTypeRequestDto createDto)
        {
            var artType = new ArtType
            {
                Name = createDto.Name,
                Description = createDto.Description,
                Region = createDto.Region,
                Technique = createDto.Technique
            };

            var createdArtType = await _artTypeRepository.CreateAsync(artType);
            return MapToResponseDto(createdArtType);
        }

        public async Task<ArtTypeResponseDto?> UpdateAsync(UpdateArtTypeRequestDto updateDto)
        {
            var existingArtType = await _artTypeRepository.GetByIdAsync(updateDto.Id);
            if (existingArtType == null)
                return null;

            var artType = new ArtType
            {
                Id = updateDto.Id,
                Name = updateDto.Name,
                Description = updateDto.Description,
                Region = updateDto.Region,
                Technique = updateDto.Technique
            };

            var updatedArtType = await _artTypeRepository.UpdateAsync(artType);
            if (updatedArtType == null)
                return null;

            var dto = MapToResponseDto(updatedArtType);
            dto.ArtifactCount = await _artTypeRepository.GetArtifactCountAsync(updatedArtType.Id);
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _artTypeRepository.DeleteAsync(id);
        }

        public async Task<ArtTypeSearchResponseDto> SearchAsync(ArtTypeSearchRequestDto searchRequest)
        {
            // Validate pagination
            if (searchRequest.PageNumber < 1)
                searchRequest.PageNumber = 1;
            if (searchRequest.PageSize < 1 || searchRequest.PageSize > 100)
                searchRequest.PageSize = 10;

            return await _artTypeRepository.SearchAsync(searchRequest);
        }

        public async Task<ArtTypeResponseDto?> GetByIdWithArtifactsAsync(int id)
        {
            var artType = await _artTypeRepository.GetByIdWithArtifactsAsync(id);
            if (artType == null)
                return null;

            var dto = MapToResponseDto(artType);
            dto.ArtifactCount = artType.Artifacts?.Count ?? 0;
            return dto;
        }

        private static ArtTypeResponseDto MapToResponseDto(ArtType artType)
        {
            return new ArtTypeResponseDto
            {
                Id = artType.Id,
                Name = artType.Name,
                Description = artType.Description,
                Region = artType.Region,
                Technique = artType.Technique
            };
        }
    }
}
