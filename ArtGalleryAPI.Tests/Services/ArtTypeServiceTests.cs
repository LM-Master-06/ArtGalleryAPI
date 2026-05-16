// ArtType Service Tests - Business Logic Testing
using ArtGalleryAPI.DTOs;
using ArtGalleryAPI.Models;
using ArtGalleryAPI.Repositories.Interfaces;
using ArtGalleryAPI.Services;
using Moq;

namespace ArtGalleryAPI.Tests.Services
{
    /// <summary>
    /// Unit tests for ArtType Service business logic
    /// Tests CRUD operations and search functionality
    /// </summary>
    public class ArtTypeServiceTests
    {
        private readonly Mock<IArtTypeRepository> _mockArtTypeRepository;
        private readonly ArtTypeService _artTypeService;

        public ArtTypeServiceTests()
        {
            _mockArtTypeRepository = new Mock<IArtTypeRepository>();
            _artTypeService = new ArtTypeService(_mockArtTypeRepository.Object);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingArtType_ReturnsArtTypeWithArtifactCount()
        {
            // Arrange
            var artTypeId = 1;
            var artType = new ArtType
            {
                Id = artTypeId,
                Name = "Dot Painting",
                Description = "Traditional Aboriginal art using dots",
                Region = "Central Desert",
                Technique = "Acrylic dots"
            };

            _mockArtTypeRepository.Setup(r => r.GetByIdAsync(artTypeId)).ReturnsAsync(artType);
            _mockArtTypeRepository.Setup(r => r.GetArtifactCountAsync(artTypeId)).ReturnsAsync(10);

            // Act
            var result = await _artTypeService.GetByIdAsync(artTypeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(artTypeId, result.Id);
            Assert.Equal("Dot Painting", result.Name);
            Assert.Equal(10, result.ArtifactCount);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingArtType_ReturnsNull()
        {
            // Arrange
            var artTypeId = 999;
            _mockArtTypeRepository.Setup(r => r.GetByIdAsync(artTypeId)).ReturnsAsync((ArtType?)null);

            // Act
            var result = await _artTypeService.GetByIdAsync(artTypeId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_WithMultipleArtTypes_ReturnsAllWithArtifactCounts()
        {
            // Arrange
            var artTypes = new List<ArtType>
            {
                new() { Id = 1, Name = "Dot Painting", Description = "Description 1" },
                new() { Id = 2, Name = "Bark Painting", Description = "Description 2" },
                new() { Id = 3, Name = "Rock Art", Description = "Description 3" }
            };

            _mockArtTypeRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(artTypes);
            _mockArtTypeRepository.Setup(r => r.GetArtifactCountAsync(1)).ReturnsAsync(5);
            _mockArtTypeRepository.Setup(r => r.GetArtifactCountAsync(2)).ReturnsAsync(3);
            _mockArtTypeRepository.Setup(r => r.GetArtifactCountAsync(3)).ReturnsAsync(8);

            // Act
            var result = await _artTypeService.GetAllAsync();

            // Assert
            Assert.Equal(3, result.Count());
            Assert.Equal(5, result.First().ArtifactCount);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_CreatesArtType()
        {
            // Arrange
            var createDto = new CreateArtTypeRequestDto
            {
                Name = "Sculpture",
                Description = "Traditional carved wooden sculptures",
                Region = "Arnhem Land",
                Technique = "Wood carving"
            };

            var createdArtType = new ArtType
            {
                Id = 1,
                Name = createDto.Name,
                Description = createDto.Description,
                Region = createDto.Region,
                Technique = createDto.Technique
            };

            _mockArtTypeRepository.Setup(r => r.CreateAsync(It.IsAny<ArtType>())).ReturnsAsync(createdArtType);

            // Act
            var result = await _artTypeService.CreateAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createDto.Name, result.Name);
            Assert.Equal(createDto.Description, result.Description);
            Assert.Equal(createDto.Region, result.Region);
            Assert.Equal(createDto.Technique, result.Technique);
            Assert.Equal(0, result.ArtifactCount);
        }

        [Fact]
        public async Task UpdateAsync_WithExistingArtType_UpdatesArtType()
        {
            // Arrange
            var updateDto = new UpdateArtTypeRequestDto
            {
                Id = 1,
                Name = "Updated Name",
                Description = "Updated Description",
                Region = "Updated Region",
                Technique = "Updated Technique"
            };

            var existingArtType = new ArtType
            {
                Id = 1,
                Name = "Old Name",
                Description = "Old Description",
                Region = "Old Region",
                Technique = "Old Technique"
            };

            var updatedArtType = new ArtType
            {
                Id = updateDto.Id,
                Name = updateDto.Name,
                Description = updateDto.Description,
                Region = updateDto.Region,
                Technique = updateDto.Technique
            };

            _mockArtTypeRepository.Setup(r => r.GetByIdAsync(updateDto.Id)).ReturnsAsync(existingArtType);
            _mockArtTypeRepository.Setup(r => r.UpdateAsync(It.IsAny<ArtType>())).ReturnsAsync(updatedArtType);
            _mockArtTypeRepository.Setup(r => r.GetArtifactCountAsync(updateDto.Id)).ReturnsAsync(5);

            // Act
            var result = await _artTypeService.UpdateAsync(updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.Name);
            Assert.Equal("Updated Description", result.Description);
            Assert.Equal(5, result.ArtifactCount);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistingArtType_ReturnsNull()
        {
            // Arrange
            var updateDto = new UpdateArtTypeRequestDto
            {
                Id = 999,
                Name = "Test",
                Description = "Test"
            };

            _mockArtTypeRepository.Setup(r => r.GetByIdAsync(updateDto.Id)).ReturnsAsync((ArtType?)null);

            // Act
            var result = await _artTypeService.UpdateAsync(updateDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_WithExistingArtType_ReturnsTrue()
        {
            // Arrange
            var artTypeId = 1;
            _mockArtTypeRepository.Setup(r => r.DeleteAsync(artTypeId)).ReturnsAsync(true);

            // Act
            var result = await _artTypeService.DeleteAsync(artTypeId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingArtType_ReturnsFalse()
        {
            // Arrange
            var artTypeId = 999;
            _mockArtTypeRepository.Setup(r => r.DeleteAsync(artTypeId)).ReturnsAsync(false);

            // Act
            var result = await _artTypeService.DeleteAsync(artTypeId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetByIdWithArtifactsAsync_WithArtifacts_ReturnsWithCount()
        {
            // Arrange
            var artTypeId = 1;
            var artType = new ArtType
            {
                Id = artTypeId,
                Name = "Dot Painting",
                Description = "Description",
                Artifacts = new List<Artifact>
                {
                    new() { Id = 1, Title = "Art 1" },
                    new() { Id = 2, Title = "Art 2" },
                    new() { Id = 3, Title = "Art 3" }
                }
            };

            _mockArtTypeRepository.Setup(r => r.GetByIdWithArtifactsAsync(artTypeId)).ReturnsAsync(artType);

            // Act
            var result = await _artTypeService.GetByIdWithArtifactsAsync(artTypeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.ArtifactCount);
        }

        [Fact]
        public async Task GetByIdWithArtifactsAsync_WithNoArtifacts_ReturnsZeroCount()
        {
            // Arrange
            var artTypeId = 1;
            var artType = new ArtType
            {
                Id = artTypeId,
                Name = "Dot Painting",
                Description = "Description",
                Artifacts = null
            };

            _mockArtTypeRepository.Setup(r => r.GetByIdWithArtifactsAsync(artTypeId)).ReturnsAsync(artType);

            // Act
            var result = await _artTypeService.GetByIdWithArtifactsAsync(artTypeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.ArtifactCount);
        }

        [Fact]
        public async Task SearchAsync_WithFilters_ReturnsPaginatedResults()
        {
            // Arrange
            var searchRequest = new ArtTypeSearchRequestDto
            {
                SearchTerm = "Painting",
                Region = "Central Desert",
                PageNumber = 1,
                PageSize = 5
            };

            var searchResult = new ArtTypeSearchResponseDto
            {
                Items = new List<ArtTypeResponseDto>
                {
                    new() { Id = 1, Name = "Dot Painting", Region = "Central Desert" },
                    new() { Id = 2, Name = "Bark Painting", Region = "Arnhem Land" }
                },
                TotalCount = 2,
                PageNumber = 1,
                PageSize = 5
            };

            _mockArtTypeRepository.Setup(r => r.SearchAsync(searchRequest)).ReturnsAsync(searchResult);

            // Act
            var result = await _artTypeService.SearchAsync(searchRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal(1, result.TotalPages);
        }

        [Theory]
        [InlineData(0, 10, 1, 10)]
        [InlineData(-1, 10, 1, 10)]
        [InlineData(1, 0, 1, 10)]
        [InlineData(1, 150, 1, 10)]
        public async Task SearchAsync_WithInvalidPagination_CorrectsValues(
            int inputPage, int inputSize, int expectedPage, int expectedSize)
        {
            // Arrange
            var searchRequest = new ArtTypeSearchRequestDto
            {
                PageNumber = inputPage,
                PageSize = inputSize
            };

            var searchResult = new ArtTypeSearchResponseDto
            {
                Items = new List<ArtTypeResponseDto>(),
                TotalCount = 0,
                PageNumber = expectedPage,
                PageSize = expectedSize
            };

            _mockArtTypeRepository.Setup(r => r.SearchAsync(It.Is<ArtTypeSearchRequestDto>(
                s => s.PageNumber == expectedPage && s.PageSize == expectedSize)))
                .ReturnsAsync(searchResult);

            // Act
            var result = await _artTypeService.SearchAsync(searchRequest);

            // Assert
            Assert.NotNull(result);
        }
    }
}
