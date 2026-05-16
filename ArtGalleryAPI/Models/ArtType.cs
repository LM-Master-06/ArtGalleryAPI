// Bounded Context: ArtTypes - Categories of Aboriginal Art
namespace ArtGalleryAPI.Models
{
    public class ArtType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty; // e.g., "Central Desert", "Arnhem Land"
        public string Technique { get; set; } = string.Empty; // e.g., "Dot Painting", "Cross-hatching"

        // Navigation property
        public ICollection<Artifact> Artifacts { get; set; } = new List<Artifact>();
    }
}
