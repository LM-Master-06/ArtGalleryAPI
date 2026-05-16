// Artifact Repository Implementation
using Microsoft.EntityFrameworkCore;
using ArtGalleryAPI.Data;
using ArtGalleryAPI.DTOs;
using ArtGalleryAPI.Models;
using ArtGalleryAPI.Repositories.Interfaces;

namespace ArtGalleryAPI.Repositories
{
    /// <summary>
    /// Repository implementation for Artifact entity
    /// </summary>
    public class ArtifactRepository : IArtifactRepository
    {
        private readonly ArtGalleryDbContext _context;

        public ArtifactRepository(ArtGalleryDbContext context)
        {
            _context = context;
        }

        public async Task<Artifact?> GetByIdAsync(int id)
        {
            return await _context.Artifacts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Artifact>> GetAllAsync()
        {
            return await _context.Artifacts
                .AsNoTracking()
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Artifact> CreateAsync(Artifact artifact)
        {
            artifact.CreatedAt = DateTime.UtcNow;
            _context.Artifacts.Add(artifact);
            await _context.SaveChangesAsync();
            return artifact;
        }

        public async Task<Artifact?> UpdateAsync(Artifact artifact)
        {
            var existingArtifact = await _context.Artifacts.FindAsync(artifact.Id);
            if (existingArtifact == null)
                return null;

            _context.Entry(existingArtifact).CurrentValues.SetValues(artifact);
            await _context.SaveChangesAsync();
            return existingArtifact;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var artifact = await _context.Artifacts.FindAsync(id);
            if (artifact == null)
                return false;

            _context.Artifacts.Remove(artifact);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Artifacts.AnyAsync(a => a.Id == id);
        }

        public async Task<ArtifactSearchResponseDto> SearchAsync(ArtifactSearchRequestDto searchRequest)
        {
            var query = _context.Artifacts
                .Include(a => a.Artist)
                .Include(a => a.ArtType)
                .AsNoTracking()
                .AsQueryable();

            // Apply search term filter (search title, description, story)
            if (!string.IsNullOrWhiteSpace(searchRequest.SearchTerm))
            {
                var searchLower = searchRequest.SearchTerm.ToLower();
                query = query.Where(a =>
                    a.Title.ToLower().Contains(searchLower) ||
                    (a.Description != null && a.Description.ToLower().Contains(searchLower)) ||
                    (a.Story != null && a.Story.ToLower().Contains(searchLower)));
            }

            // Apply artist filter
            if (searchRequest.ArtistId.HasValue)
            {
                query = query.Where(a => a.ArtistId == searchRequest.ArtistId.Value);
            }

            // Apply art type filter
            if (searchRequest.ArtTypeId.HasValue)
            {
                query = query.Where(a => a.ArtTypeId == searchRequest.ArtTypeId.Value);
            }

            // Apply availability filter
            if (searchRequest.IsAvailable.HasValue)
            {
                query = query.Where(a => a.IsAvailable == searchRequest.IsAvailable.Value);
            }

            // Apply price range filter
            if (searchRequest.MinPrice.HasValue)
            {
                query = query.Where(a => a.Price >= searchRequest.MinPrice.Value);
            }
            if (searchRequest.MaxPrice.HasValue)
            {
                query = query.Where(a => a.Price <= searchRequest.MaxPrice.Value);
            }

            // Apply year range filter
            if (searchRequest.YearFrom.HasValue)
            {
                query = query.Where(a => a.YearCreated >= searchRequest.YearFrom.Value);
            }
            if (searchRequest.YearTo.HasValue)
            {
                query = query.Where(a => a.YearCreated <= searchRequest.YearTo.Value);
            }

            // Apply medium filter
            if (!string.IsNullOrWhiteSpace(searchRequest.Medium))
            {
                query = query.Where(a => a.Medium != null && a.Medium.Contains(searchRequest.Medium));
            }

            // Get price range
            var priceStats = await query
                .Where(a => a.Price.HasValue)
                .GroupBy(a => 1)
                .Select(g => new { Min = g.Min(a => a.Price), Max = g.Max(a => a.Price) })
                .FirstOrDefaultAsync();

            // Apply sorting
            query = searchRequest.SortBy?.ToLower() switch
            {
                "title" => searchRequest.SortDescending
                    ? query.OrderByDescending(a => a.Title)
                    : query.OrderBy(a => a.Title),
                "price" => searchRequest.SortDescending
                    ? query.OrderByDescending(a => a.Price)
                    : query.OrderBy(a => a.Price),
                "yearcreated" => searchRequest.SortDescending
                    ? query.OrderByDescending(a => a.YearCreated)
                    : query.OrderBy(a => a.YearCreated),
                _ => searchRequest.SortDescending
                    ? query.OrderByDescending(a => a.CreatedAt)
                    : query.OrderBy(a => a.CreatedAt)
            };

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var items = await query
                .Skip((searchRequest.PageNumber - 1) * searchRequest.PageSize)
                .Take(searchRequest.PageSize)
                .ToListAsync();

            return new ArtifactSearchResponseDto
            {
                Items = items.Select(MapToResponseDto),
                TotalCount = totalCount,
                PageNumber = searchRequest.PageNumber,
                PageSize = searchRequest.PageSize,
                MinPriceFound = priceStats?.Min,
                MaxPriceFound = priceStats?.Max
            };
        }

        public async Task<Artifact?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Artifacts
                .Include(a => a.Artist)
                .Include(a => a.ArtType)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Artifact>> GetByArtistIdAsync(int artistId)
        {
            return await _context.Artifacts
                .Include(a => a.ArtType)
                .AsNoTracking()
                .Where(a => a.ArtistId == artistId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Artifact>> GetByArtTypeIdAsync(int artTypeId)
        {
            return await _context.Artifacts
                .Include(a => a.Artist)
                .AsNoTracking()
                .Where(a => a.ArtTypeId == artTypeId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Artifact>> GetAvailableAsync()
        {
            return await _context.Artifacts
                .Include(a => a.Artist)
                .Include(a => a.ArtType)
                .AsNoTracking()
                .Where(a => a.IsAvailable)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateAvailabilityAsync(int id, bool isAvailable)
        {
            var artifact = await _context.Artifacts.FindAsync(id);
            if (artifact == null)
                return false;

            artifact.IsAvailable = isAvailable;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ArtifactStatisticsDto> GetStatisticsAsync()
        {
            var totalArtifacts = await _context.Artifacts.CountAsync();
            var availableArtifacts = await _context.Artifacts.CountAsync(a => a.IsAvailable);
            var soldArtifacts = totalArtifacts - availableArtifacts;
            var totalValue = await _context.Artifacts
                .Where(a => a.Price.HasValue)
                .SumAsync(a => a.Price) ?? 0;
            var averagePrice = await _context.Artifacts
                .Where(a => a.Price.HasValue)
                .AverageAsync(a => a.Price) ?? 0;

            var countByType = await _context.ArtTypes
                .Select(t => new ArtifactCountByTypeDto
                {
                    ArtTypeId = t.Id,
                    ArtTypeName = t.Name,
                    Count = t.Artifacts.Count
                })
                .ToListAsync();

            var countByArtist = await _context.Artists
                .Select(a => new ArtifactCountByArtistDto
                {
                    ArtistId = a.Id,
                    ArtistName = $"{a.FirstName} {a.LastName}",
                    Count = a.Artifacts.Count
                })
                .OrderByDescending(a => a.Count)
                .Take(10)
                .ToListAsync();

            return new ArtifactStatisticsDto
            {
                TotalArtifacts = totalArtifacts,
                AvailableArtifacts = availableArtifacts,
                SoldArtifacts = soldArtifacts,
                TotalValue = totalValue,
                AveragePrice = averagePrice,
                CountByType = countByType,
                CountByArtist = countByArtist
            };
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
