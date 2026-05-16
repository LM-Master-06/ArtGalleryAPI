// Bounded Context: Artifacts - Art pieces
namespace ArtGalleryAPI.Models
{
    public class Artifact
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Story { get; set; } // Dreamtime story/cultural significance
        public string? Dimensions { get; set; } // e.g., "120cm x 80cm"
        public string? Medium { get; set; } // e.g., "Acrylic on Canvas", "Ochre on Bark"
        public int? YearCreated { get; set; }
        public decimal? Price { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public int ArtistId { get; set; }
        public int ArtTypeId { get; set; }

        // Navigation properties
        public Artist Artist { get; set; } = null!;
        public ArtType ArtType { get; set; } = null!;
    }
}
