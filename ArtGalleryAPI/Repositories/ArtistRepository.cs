// Artist Repository Implementation
using Microsoft.EntityFrameworkCore;
using ArtGalleryAPI.Data;
using ArtGalleryAPI.DTOs;
using ArtGalleryAPI.Models;
using ArtGalleryAPI.Repositories.Interfaces;

namespace ArtGalleryAPI.Repositories
{
    /// <summary>
    /// Repository implementation for Artist entity
    /// </summary>
    public class ArtistRepository : IArtistRepository
    {
        private readonly ArtGalleryDbContext _context;

        public ArtistRepository(ArtGalleryDbContext context)
        {
            _context = context;
        }

        public async Task<Artist?> GetByIdAsync(int id)
        {
            return await _context.Artists
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Artist>> GetAllAsync()
        {
            return await _context.Artists
                .AsNoTracking()
                .OrderBy(a => a.LastName)
                .ThenBy(a => a.FirstName)
                .ToListAsync();
        }

        public async Task<Artist> CreateAsync(Artist artist)
        {
            artist.CreatedAt = DateTime.UtcNow;
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();
            return artist;
        }

        public async Task<Artist?> UpdateAsync(Artist artist)
        {
            var existingArtist = await _context.Artists.FindAsync(artist.Id);
            if (existingArtist == null)
                return null;

            _context.Entry(existingArtist).CurrentValues.SetValues(artist);
            await _context.SaveChangesAsync();
            return existingArtist;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var artist = await _context.Artists.FindAsync(id);
            if (artist == null)
                return false;

            // Check if artist has artifacts
            var hasArtifacts = await _context.Artifacts.AnyAsync(a => a.ArtistId == id);
            if (hasArtifacts)
                throw new InvalidOperationException("Cannot delete artist with existing artifacts");

            _context.Artists.Remove(artist);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Artists.AnyAsync(a => a.Id == id);
        }

        public async Task<ArtistSearchResponseDto> SearchAsync(ArtistSearchRequestDto searchRequest)
        {
            var query = _context.Artists.AsNoTracking().AsQueryable();

            // Apply search term filter
            if (!string.IsNullOrWhiteSpace(searchRequest.SearchTerm))
            {
                var searchLower = searchRequest.SearchTerm.ToLower();
                query = query.Where(a =>
                    (a.FirstName != null && a.FirstName.ToLower().Contains(searchLower)) ||
                    (a.LastName != null && a.LastName.ToLower().Contains(searchLower)) ||
                    (a.Biography != null && a.Biography.ToLower().Contains(searchLower)));
            }

            // Apply region filter
            if (!string.IsNullOrWhiteSpace(searchRequest.Region))
            {
                query = query.Where(a => a.Region != null && a.Region.Contains(searchRequest.Region));
            }

            // Apply clan group filter
            if (!string.IsNullOrWhiteSpace(searchRequest.ClanGroup))
            {
                query = query.Where(a => a.ClanGroup != null && a.ClanGroup.Contains(searchRequest.ClanGroup));
            }

            // Apply language group filter
            if (!string.IsNullOrWhiteSpace(searchRequest.LanguageGroup))
            {
                query = query.Where(a => a.LanguageGroup != null && a.LanguageGroup.Contains(searchRequest.LanguageGroup));
            }

            // Apply deceased filter
            if (searchRequest.IsDeceased.HasValue)
            {
                query = query.Where(a => a.IsDeceased == searchRequest.IsDeceased.Value);
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var items = await query
                .OrderBy(a => a.LastName)
                .ThenBy(a => a.FirstName)
                .Skip((searchRequest.PageNumber - 1) * searchRequest.PageSize)
                .Take(searchRequest.PageSize)
                .ToListAsync();

            // Map to DTOs and get artifact counts
            var artistDtos = new List<ArtistResponseDto>();
            foreach (var artist in items)
            {
                var dto = MapToResponseDto(artist);
                dto.ArtifactCount = await GetArtifactCountAsync(artist.Id);
                artistDtos.Add(dto);
            }

            return new ArtistSearchResponseDto
            {
                Items = artistDtos,
                TotalCount = totalCount,
                PageNumber = searchRequest.PageNumber,
                PageSize = searchRequest.PageSize
            };
        }

        public async Task<Artist?> GetByIdWithArtifactsAsync(int id)
        {
            return await _context.Artists
                .Include(a => a.Artifacts)
                .ThenInclude(a => a.ArtType)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<int> GetArtifactCountAsync(int artistId)
        {
            return await _context.Artifacts
                .CountAsync(a => a.ArtistId == artistId);
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
