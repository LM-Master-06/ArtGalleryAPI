// ArtType Repository Implementation
using Microsoft.EntityFrameworkCore;
using ArtGalleryAPI.Data;
using ArtGalleryAPI.DTOs;
using ArtGalleryAPI.Models;
using ArtGalleryAPI.Repositories.Interfaces;

namespace ArtGalleryAPI.Repositories
{
    /// <summary>
    /// Repository implementation for ArtType entity
    /// </summary>
    public class ArtTypeRepository : IArtTypeRepository
    {
        private readonly ArtGalleryDbContext _context;

        public ArtTypeRepository(ArtGalleryDbContext context)
        {
            _context = context;
        }

        public async Task<ArtType?> GetByIdAsync(int id)
        {
            return await _context.ArtTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<ArtType>> GetAllAsync()
        {
            return await _context.ArtTypes
                .AsNoTracking()
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<ArtType> CreateAsync(ArtType artType)
        {
            _context.ArtTypes.Add(artType);
            await _context.SaveChangesAsync();
            return artType;
        }

        public async Task<ArtType?> UpdateAsync(ArtType artType)
        {
            var existingArtType = await _context.ArtTypes.FindAsync(artType.Id);
            if (existingArtType == null)
                return null;

            _context.Entry(existingArtType).CurrentValues.SetValues(artType);
            await _context.SaveChangesAsync();
            return existingArtType;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var artType = await _context.ArtTypes.FindAsync(id);
            if (artType == null)
                return false;

            // Check if art type has artifacts
            var hasArtifacts = await _context.Artifacts.AnyAsync(a => a.ArtTypeId == id);
            if (hasArtifacts)
                throw new InvalidOperationException("Cannot delete art type with existing artifacts");

            _context.ArtTypes.Remove(artType);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.ArtTypes.AnyAsync(a => a.Id == id);
        }

        public async Task<ArtTypeSearchResponseDto> SearchAsync(ArtTypeSearchRequestDto searchRequest)
        {
            var query = _context.ArtTypes.AsNoTracking().AsQueryable();

            // Apply search term filter
            if (!string.IsNullOrWhiteSpace(searchRequest.SearchTerm))
            {
                var searchLower = searchRequest.SearchTerm.ToLower();
                query = query.Where(a =>
                    a.Name.ToLower().Contains(searchLower) ||
                    a.Description.ToLower().Contains(searchLower));
            }

            // Apply region filter
            if (!string.IsNullOrWhiteSpace(searchRequest.Region))
            {
                query = query.Where(a => a.Region.Contains(searchRequest.Region));
            }

            // Apply technique filter
            if (!string.IsNullOrWhiteSpace(searchRequest.Technique))
            {
                query = query.Where(a => a.Technique.Contains(searchRequest.Technique));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var items = await query
                .OrderBy(a => a.Name)
                .Skip((searchRequest.PageNumber - 1) * searchRequest.PageSize)
                .Take(searchRequest.PageSize)
                .ToListAsync();

            // Map to DTOs and get artifact counts
            var artTypeDtos = new List<ArtTypeResponseDto>();
            foreach (var artType in items)
            {
                var dto = MapToResponseDto(artType);
                dto.ArtifactCount = await GetArtifactCountAsync(artType.Id);
                artTypeDtos.Add(dto);
            }

            return new ArtTypeSearchResponseDto
            {
                Items = artTypeDtos,
                TotalCount = totalCount,
                PageNumber = searchRequest.PageNumber,
                PageSize = searchRequest.PageSize
            };
        }

        public async Task<ArtType?> GetByIdWithArtifactsAsync(int id)
        {
            return await _context.ArtTypes
                .Include(a => a.Artifacts)
                .ThenInclude(a => a.Artist)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<int> GetArtifactCountAsync(int artTypeId)
        {
            return await _context.Artifacts
                .CountAsync(a => a.ArtTypeId == artTypeId);
        }

        public async Task<bool> HasArtifactsAsync(int artTypeId)
        {
            return await _context.Artifacts.AnyAsync(a => a.ArtTypeId == artTypeId);
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
