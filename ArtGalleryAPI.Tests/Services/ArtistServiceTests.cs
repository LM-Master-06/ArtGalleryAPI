// Artist Service Tests - Business Logic Testing
using Microsoft.EntityFrameworkCore;
using ArtGalleryAPI.Data;
using ArtGalleryAPI.DTOs;
using ArtGalleryAPI.Models;
using ArtGalleryAPI.Repositories.Interfaces;
using ArtGalleryAPI.Services;
using Moq;

namespace ArtGalleryAPI.Tests.Services
{
    /// <summary>
    /// Unit tests for Artist Service business logic
    /// Tests CRUD operations, search, and statistics
    /// </summary>
    public class ArtistServiceTests
    {
        private readonly Mock<IArtistRepository> _mockArtistRepository;
        private readonly ArtGalleryDbContext _dbContext;
        private readonly ArtistService _artistService;

        public ArtistServiceTests()
        {
            _mockArtistRepository = new Mock<IArtistRepository>();

            // Create in-memory database for testing statistics
            var options = new DbContextOptionsBuilder<ArtGalleryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ArtGalleryDbContext(options);
            _artistService = new ArtistService(_mockArtistRepository.Object, _dbContext);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingArtist_ReturnsArtistWithArtifactCount()
        {
            // Arrange
            var artistId = 1;
            var artist = new Artist
            {
                Id = artistId,
                FirstName = "Emily",
                LastName = "Kngwarreye",
                ClanGroup = "Alhalkere",
                Region = "Utopia",
                LanguageGroup = "Anmatyerre",
                IsDeceased = true
            };

            _mockArtistRepository.Setup(r => r.GetByIdAsync(artistId)).ReturnsAsync(artist);
            _mockArtistRepository.Setup(r => r.GetArtifactCountAsync(artistId)).ReturnsAsync(5);

            // Act
            var result = await _artistService.GetByIdAsync(artistId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(artistId, result.Id);
            Assert.Equal("Emily", result.FirstName);
            Assert.Equal(5, result.ArtifactCount);
            Assert.Equal("Emily Kngwarreye", result.FullName);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingArtist_ReturnsNull()
        {
            // Arrange
            var artistId = 999;
            _mockArtistRepository.Setup(r => r.GetByIdAsync(artistId)).ReturnsAsync((Artist?)null);

            // Act
            var result = await _artistService.GetByIdAsync(artistId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_CreatesArtist()
        {
            // Arrange
            var createDto = new CreateArtistRequestDto
            {
                FirstName = "New",
                LastName = "Artist",
                ClanGroup = "Test Clan",
                Region = "Test Region",
                LanguageGroup = "Test Language",
                Biography = "Test biography",
                IsDeceased = false
            };

            var createdArtist = new Artist
            {
                Id = 1,
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                ClanGroup = createDto.ClanGroup,
                Region = createDto.Region,
                LanguageGroup = createDto.LanguageGroup,
                Biography = createDto.Biography,
                IsDeceased = createDto.IsDeceased,
                CreatedAt = DateTime.UtcNow
            };

            _mockArtistRepository.Setup(r => r.CreateAsync(It.IsAny<Artist>())).ReturnsAsync(createdArtist);

            // Act
            var result = await _artistService.CreateAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createDto.FirstName, result.FirstName);
            Assert.Equal(createDto.LastName, result.LastName);
            Assert.Equal(createDto.ClanGroup, result.ClanGroup);
            Assert.Equal(0, result.ArtifactCount); // New artist has no artifacts
        }

        [Fact]
        public async Task UpdateAsync_WithExistingArtist_UpdatesArtist()
        {
            // Arrange
            var updateDto = new UpdateArtistRequestDto
            {
                Id = 1,
                FirstName = "Updated",
                LastName = "Name",
                ClanGroup = "Updated Clan",
                Region = "Updated Region",
                LanguageGroup = "Updated Language",
                Biography = "Updated biography",
                IsDeceased = true
            };

            var existingArtist = new Artist
            {
                Id = 1,
                FirstName = "Old",
                LastName = "Name",
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            };

            var updatedArtist = new Artist
            {
                Id = updateDto.Id,
                FirstName = updateDto.FirstName,
                LastName = updateDto.LastName,
                ClanGroup = updateDto.ClanGroup,
                Region = updateDto.Region,
                LanguageGroup = updateDto.LanguageGroup,
                Biography = updateDto.Biography,
                IsDeceased = updateDto.IsDeceased,
                CreatedAt = existingArtist.CreatedAt
            };

            _mockArtistRepository.Setup(r => r.GetByIdAsync(updateDto.Id)).ReturnsAsync(existingArtist);
            _mockArtistRepository.Setup(r => r.UpdateAsync(It.IsAny<Artist>())).ReturnsAsync(updatedArtist);
            _mockArtistRepository.Setup(r => r.GetArtifactCountAsync(updateDto.Id)).ReturnsAsync(3);

            // Act
            var result = await _artistService.UpdateAsync(updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated", result.FirstName);
            Assert.Equal("Updated Clan", result.ClanGroup);
            Assert.Equal(3, result.ArtifactCount);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistingArtist_ReturnsNull()
        {
            // Arrange
            var updateDto = new UpdateArtistRequestDto
            {
                Id = 999,
                FirstName = "Test",
                LastName = "Artist"
            };

            _mockArtistRepository.Setup(r => r.GetByIdAsync(updateDto.Id)).ReturnsAsync((Artist?)null);

            // Act
            var result = await _artistService.UpdateAsync(updateDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_WithExistingArtist_ReturnsTrue()
        {
            // Arrange
            var artistId = 1;
            _mockArtistRepository.Setup(r => r.DeleteAsync(artistId)).ReturnsAsync(true);

            // Act
            var result = await _artistService.DeleteAsync(artistId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetStatisticsAsync_WithMultipleArtists_ReturnsCorrectStatistics()
        {
            // Arrange - Seed in-memory database
            var artists = new List<Artist>
            {
                new() { Id = 1, FirstName = "Artist", LastName = "One", Region = "Region A", LanguageGroup = "Group X", IsDeceased = true },
                new() { Id = 2, FirstName = "Artist", LastName = "Two", Region = "Region A", LanguageGroup = "Group Y", IsDeceased = false },
                new() { Id = 3, FirstName = "Artist", LastName = "Three", Region = "Region B", LanguageGroup = "Group X", IsDeceased = true },
                new() { Id = 4, FirstName = "Artist", LastName = "Four", Region = "Region B", LanguageGroup = "Group Y", IsDeceased = false }
            };

            await _dbContext.Artists.AddRangeAsync(artists);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _artistService.GetStatisticsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.TotalArtists);
            Assert.Equal(2, result.DeceasedArtists);
            Assert.Equal(2, result.LivingArtists);
            Assert.Equal(2, result.ArtistsByRegion["Region A"]);
            Assert.Equal(2, result.ArtistsByRegion["Region B"]);
            Assert.Equal(2, result.ArtistsByLanguageGroup["Group X"]);
            Assert.Equal(2, result.ArtistsByLanguageGroup["Group Y"]);
        }

        [Fact]
        public async Task SearchAsync_WithFilters_ReturnsPaginatedResults()
        {
            // Arrange
            var searchRequest = new ArtistSearchRequestDto
            {
                SearchTerm = "Emily",
                Region = "Utopia",
                PageNumber = 1,
                PageSize = 10
            };

            var searchResult = new ArtistSearchResponseDto
            {
                Items = new List<ArtistResponseDto>
                {
                    new() { Id = 1, FirstName = "Emily", LastName = "Kngwarreye", Region = "Utopia" }
                },
                TotalCount = 1,
                PageNumber = 1,
                PageSize = 10
            };

            _mockArtistRepository.Setup(r => r.SearchAsync(searchRequest)).ReturnsAsync(searchResult);

            // Act
            var result = await _artistService.SearchAsync(searchRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Equal(1, result.TotalPages);
            Assert.Single(result.Items);
        }

        [Theory]
        [InlineData(0, 10, 1, 10)]    // Invalid page number corrected
        [InlineData(1, 0, 1, 10)]     // Invalid page size corrected to default
        [InlineData(1, 150, 1, 10)]   // Page size capped at 100
        public async Task SearchAsync_WithInvalidPagination_CorrectsPaginationValues(
            int inputPage, int inputSize, int expectedPage, int expectedSize)
        {
            // Arrange
            var searchRequest = new ArtistSearchRequestDto
            {
                PageNumber = inputPage,
                PageSize = inputSize
            };

            var searchResult = new ArtistSearchResponseDto
            {
                Items = new List<ArtistResponseDto>(),
                TotalCount = 0,
                PageNumber = expectedPage,
                PageSize = expectedSize
            };

            _mockArtistRepository.Setup(r => r.SearchAsync(It.Is<ArtistSearchRequestDto>(
                s => s.PageNumber == expectedPage && s.PageSize == expectedSize)))
                .ReturnsAsync(searchResult);

            // Act
            var result = await _artistService.SearchAsync(searchRequest);

            // Assert
            Assert.NotNull(result);
        }
    }
}
