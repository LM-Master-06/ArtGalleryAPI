// Artist Service Implementation
using Microsoft.EntityFrameworkCore;
using ArtGalleryAPI.Data;
using ArtGalleryAPI.DTOs;
using ArtGalleryAPI.Models;
using ArtGalleryAPI.Repositories.Interfaces;
using ArtGalleryAPI.Services.Interfaces;

namespace ArtGalleryAPI.Services
{
    /// <summary>
    /// Service implementation for Artist business logic
    /// </summary>
    public class ArtistService : IArtistService
    {
        private readonly IArtistRepository _artistRepository;
        private readonly ArtGalleryDbContext _context;

        public ArtistService(IArtistRepository artistRepository, ArtGalleryDbContext context)
        {
            _artistRepository = artistRepository;
            _context = context;
        }

        public async Task<ArtistResponseDto?> GetByIdAsync(int id)
        {
            var artist = await _artistRepository.GetByIdAsync(id);
            if (artist == null)
                return null;

            var dto = MapToResponseDto(artist);
            dto.ArtifactCount = await _artistRepository.GetArtifactCountAsync(id);
            return dto;
        }

        public async Task<IEnumerable<ArtistResponseDto>> GetAllAsync()
        {
            var artists = await _artistRepository.GetAllAsync();
            var result = new List<ArtistResponseDto>();

            foreach (var artist in artists)
            {
                var dto = MapToResponseDto(artist);
                dto.ArtifactCount = await _artistRepository.GetArtifactCountAsync(artist.Id);
                result.Add(dto);
            }

            return result;
        }

        public async Task<ArtistResponseDto> CreateAsync(CreateArtistRequestDto createDto)
        {
            var artist = new Artist
            {
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                ClanGroup = createDto.ClanGroup,
                Region = createDto.Region,
                LanguageGroup = createDto.LanguageGroup,
                BirthDate = createDto.BirthDate,
                DeathDate = createDto.DeathDate,
                Biography = createDto.Biography,
                IsDeceased = createDto.IsDeceased
            };

            var createdArtist = await _artistRepository.CreateAsync(artist);
            return MapToResponseDto(createdArtist);
        }

        public async Task<ArtistResponseDto?> UpdateAsync(UpdateArtistRequestDto updateDto)
        {
            var existingArtist = await _artistRepository.GetByIdAsync(updateDto.Id);
            if (existingArtist == null)
                return null;

            var artist = new Artist
            {
                Id = updateDto.Id,
                FirstName = updateDto.FirstName,
                LastName = updateDto.LastName,
                ClanGroup = updateDto.ClanGroup,
                Region = updateDto.Region,
                LanguageGroup = updateDto.LanguageGroup,
                BirthDate = updateDto.BirthDate,
                DeathDate = updateDto.DeathDate,
                Biography = updateDto.Biography,
                IsDeceased = updateDto.IsDeceased,
                CreatedAt = existingArtist.CreatedAt
            };

            var updatedArtist = await _artistRepository.UpdateAsync(artist);
            if (updatedArtist == null)
                return null;

            var dto = MapToResponseDto(updatedArtist);
            dto.ArtifactCount = await _artistRepository.GetArtifactCountAsync(updatedArtist.Id);
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _artistRepository.DeleteAsync(id);
        }

        public async Task<ArtistSearchResponseDto> SearchAsync(ArtistSearchRequestDto searchRequest)
        {
            // Validate pagination
            if (searchRequest.PageNumber < 1)
                searchRequest.PageNumber = 1;
            if (searchRequest.PageSize < 1 || searchRequest.PageSize > 100)
                searchRequest.PageSize = 10;

            return await _artistRepository.SearchAsync(searchRequest);
        }

        public async Task<ArtistResponseDto?> GetByIdWithArtifactsAsync(int id)
        {
            var artist = await _artistRepository.GetByIdWithArtifactsAsync(id);
            if (artist == null)
                return null;

            return MapToResponseDto(artist);
        }

        public async Task<ArtistStatisticsDto> GetStatisticsAsync()
        {
            var totalArtists = await _context.Artists.CountAsync();
            var deceasedArtists = await _context.Artists.CountAsync(a => a.IsDeceased);
            var livingArtists = totalArtists - deceasedArtists;

            var artistsByRegion = await _context.Artists
                .Where(a => a.Region != null)
                .GroupBy(a => a.Region!)
                .Select(g => new { Region = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Region, x => x.Count);

            var artistsByLanguageGroup = await _context.Artists
                .Where(a => a.LanguageGroup != null)
                .GroupBy(a => a.LanguageGroup!)
                .Select(g => new { LanguageGroup = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.LanguageGroup, x => x.Count);

            return new ArtistStatisticsDto
            {
                TotalArtists = totalArtists,
                DeceasedArtists = deceasedArtists,
                LivingArtists = livingArtists,
                ArtistsByRegion = artistsByRegion,
                ArtistsByLanguageGroup = artistsByLanguageGroup
            };
        }

        private static ArtistResponseDto MapToResponseDto(Artist artist)
        {
            return new ArtistResponseDto
            {
                Id = artist.Id,
                FirstName = artist.FirstName,
                LastName = artist.LastName,
                ClanGroup = artist.ClanGroup,
                Region = artist.Region,
                LanguageGroup = artist.LanguageGroup,
                BirthDate = artist.BirthDate,
                DeathDate = artist.DeathDate,
                Biography = artist.Biography,
                IsDeceased = artist.IsDeceased,
                CreatedAt = artist.CreatedAt
            };
        }
    }
}
