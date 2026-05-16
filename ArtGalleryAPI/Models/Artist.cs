// Bounded Context: Artists - Aboriginal Artists
namespace ArtGalleryAPI.Models
{
    public class Artist
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ClanGroup { get; set; } // Aboriginal clan/skin group
        public string? Region { get; set; } // Traditional country/region
        public string? LanguageGroup { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public string? Biography { get; set; }
        public bool IsDeceased { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<Artifact> Artifacts { get; set; } = new List<Artifact>();
    }
}
