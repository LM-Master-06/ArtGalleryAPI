// Artifact Service Implementation
using ArtGalleryAPI.DTOs;
using ArtGalleryAPI.Models;
using ArtGalleryAPI.Repositories.Interfaces;
using ArtGalleryAPI.Services.Interfaces;

namespace ArtGalleryAPI.Services
{
    /// <summary>
    /// Service implementation for Artifact business logic
    /// </summary>
    public class ArtifactService : IArtifactService
    {
        private readonly IArtifactRepository _artifactRepository;
        private readonly IArtistRepository _artistRepository;
        private readonly IArtTypeRepository _artTypeRepository;

        public ArtifactService(
            IArtifactRepository artifactRepository,
            IArtistRepository artistRepository,
            IArtTypeRepository artTypeRepository)
        {
            _artifactRepository = artifactRepository;
            _artistRepository = artistRepository;
            _artTypeRepository = artTypeRepository;
        }

        public async Task<ArtifactResponseDto?> GetByIdAsync(int id)
        {
            var artifact = await _artifactRepository.GetByIdAsync(id);
            if (artifact == null)
                return null;

            return MapToResponseDto(artifact);
        }

        public async Task<IEnumerable<ArtifactResponseDto>> GetAllAsync()
        {
            var artifacts = await _artifactRepository.GetAllAsync();
            return artifacts.Select(MapToResponseDto);
        }

        public async Task<ArtifactResponseDto> CreateAsync(CreateArtifactRequestDto createDto)
        {
            // Validate artist exists
            var artistExists = await _artistRepository.ExistsAsync(createDto.ArtistId);
            if (!artistExists)
                throw new ArgumentException($"Artist with ID {createDto.ArtistId} does not exist");

            // Validate art type exists
            var artTypeExists = await _artTypeRepository.ExistsAsync(createDto.ArtTypeId);
            if (!artTypeExists)
                throw new ArgumentException($"Art type with ID {createDto.ArtTypeId} does not exist");

            var artifact = new Artifact
            {
                Title = createDto.Title,
                Description = createDto.Description,
                Story = createDto.Story,
                Dimensions = createDto.Dimensions,
                Medium = createDto.Medium,
                YearCreated = createDto.YearCreated,
                Price = createDto.Price,
                IsAvailable = createDto.IsAvailable,
                ImageUrl = createDto.ImageUrl,
                ArtistId = createDto.ArtistId,
                ArtTypeId = createDto.ArtTypeId
            };

            var createdArtifact = await _artifactRepository.CreateAsync(artifact);

            // Reload with includes
            var artifactWithDetails = await _artifactRepository.GetByIdWithDetailsAsync(createdArtifact.Id);
            return MapToResponseDto(artifactWithDetails!);
        }

        public async Task<ArtifactResponseDto?> UpdateAsync(UpdateArtifactRequestDto updateDto)
        {
            var existingArtifact = await _artifactRepository.GetByIdAsync(updateDto.Id);
            if (existingArtifact == null)
                return null;

            // Validate artist exists
            var artistExists = await _artistRepository.ExistsAsync(updateDto.ArtistId);
            if (!artistExists)
                throw new ArgumentException($"Artist with ID {updateDto.ArtistId} does not exist");

            // Validate art type exists
            var artTypeExists = await _artTypeRepository.ExistsAsync(updateDto.ArtTypeId);
            if (!artTypeExists)
                throw new ArgumentException($"Art type with ID {updateDto.ArtTypeId} does not exist");

            var artifact = new Artifact
            {
                Id = updateDto.Id,
                Title = updateDto.Title,
                Description = updateDto.Description,
                Story = updateDto.Story,
                Dimensions = updateDto.Dimensions,
                Medium = updateDto.Medium,
                YearCreated = updateDto.YearCreated,
                Price = updateDto.Price,
                IsAvailable = updateDto.IsAvailable,
                ImageUrl = updateDto.ImageUrl,
                ArtistId = updateDto.ArtistId,
                ArtTypeId = updateDto.ArtTypeId,
                CreatedAt = existingArtifact.CreatedAt
            };

            var updatedArtifact = await _artifactRepository.UpdateAsync(artifact);
            if (updatedArtifact == null)
                return null;

            // Reload with includes
            var artifactWithDetails = await _artifactRepository.GetByIdWithDetailsAsync(updatedArtifact.Id);
            return MapToResponseDto(artifactWithDetails!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _artifactRepository.DeleteAsync(id);
        }

        public async Task<ArtifactSearchResponseDto> SearchAsync(ArtifactSearchRequestDto searchRequest)
        {
            // Validate pagination
            if (searchRequest.PageNumber < 1)
                searchRequest.PageNumber = 1;
            if (searchRequest.PageSize < 1 || searchRequest.PageSize > 100)
                searchRequest.PageSize = 10;

            // Validate price range
            if (searchRequest.MinPrice.HasValue && searchRequest.MaxPrice.HasValue)
            {
                if (searchRequest.MinPrice > searchRequest.MaxPrice)
                {
                    throw new ArgumentException("Minimum price cannot be greater than maximum price");
                }
            }

            // Validate year range
            if (searchRequest.YearFrom.HasValue && searchRequest.YearTo.HasValue)
            {
                if (searchRequest.YearFrom > searchRequest.YearTo)
                {
                    throw new ArgumentException("Year from cannot be greater than year to");
                }
            }

            return await _artifactRepository.SearchAsync(searchRequest);
        }

        public async Task<ArtifactResponseDto?> GetByIdWithDetailsAsync(int id)
        {
            var artifact = await _artifactRepository.GetByIdWithDetailsAsync(id);
            if (artifact == null)
                return null;

            return MapToResponseDto(artifact);
        }

        public async Task<IEnumerable<ArtifactResponseDto>> GetByArtistIdAsync(int artistId)
        {
            var artifacts = await _artifactRepository.GetByArtistIdAsync(artistId);
            return artifacts.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<ArtifactResponseDto>> GetByArtTypeIdAsync(int artTypeId)
        {
            var artifacts = await _artifactRepository.GetByArtTypeIdAsync(artTypeId);
            return artifacts.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<ArtifactResponseDto>> GetAvailableAsync()
        {
            var artifacts = await _artifactRepository.GetAvailableAsync();
            return artifacts.Select(MapToResponseDto);
        }

        public async Task<bool> UpdateAvailabilityAsync(int id, bool isAvailable)
        {
            return await _artifactRepository.UpdateAvailabilityAsync(id, isAvailable);
        }

        public async Task<ArtifactStatisticsDto> GetStatisticsAsync()
        {
            return await _artifactRepository.GetStatisticsAsync();
        }

        private static ArtifactResponseDto MapToResponseDto(Artifact artifact)
        {
            return new ArtifactResponseDto
            {
                Id = artifact.Id,
                Title = artifact.Title,
                Description = artifact.Description,
                Story = artifact.Story,
                Dimensions = artifact.Dimensions,
                Medium = artifact.Medium,
                YearCreated = artifact.YearCreated,
                Price = artifact.Price,
                IsAvailable = artifact.IsAvailable,
                ImageUrl = artifact.ImageUrl,
                CreatedAt = artifact.CreatedAt,
                Artist = artifact.Artist != null ? new ArtistSummaryDto
                {
                    Id = artifact.Artist.Id,
                    FullName = $"{artifact.Artist.FirstName} {artifact.Artist.LastName}",
                    Region = artifact.Artist.Region
                } : null,
                ArtType = artifact.ArtType != null ? new ArtTypeSummaryDto
                {
                    Id = artifact.ArtType.Id,
                    Name = artifact.ArtType.Name,
                    Technique = artifact.ArtType.Technique
                } : null
            };
        }
    }
}
