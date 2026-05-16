using Microsoft.EntityFrameworkCore;
using ArtGalleryAPI.Models;

namespace ArtGalleryAPI.Data
{
    public class ArtGalleryDbContext : DbContext
    {
        public ArtGalleryDbContext(DbContextOptions<ArtGalleryDbContext> options)
            : base(options)
        {
        }

        public DbSet<ArtType> ArtTypes { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Artifact> Artifacts { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity with unique email constraint
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Complex database structure with indexes for performance (HD Requirement)
            // Index on Artifacts.ArtistId for fast lookups
            modelBuilder.Entity<Artifact>()
                .HasIndex(a => a.ArtistId)
                .HasDatabaseName("IX_Artifacts_ArtistId");

            // Index on Artifacts.ArtTypeId for filtering
            modelBuilder.Entity<Artifact>()
                .HasIndex(a => a.ArtTypeId)
                .HasDatabaseName("IX_Artifacts_ArtTypeId");

            // Composite index for availability searches
            modelBuilder.Entity<Artifact>()
                .HasIndex(a => new { a.IsAvailable, a.ArtTypeId })
                .HasDatabaseName("IX_Artifacts_Available_Type");

            // Index on Artists.Region for search queries
            modelBuilder.Entity<Artist>()
                .HasIndex(a => a.Region)
                .HasDatabaseName("IX_Artists_Region");

            // Index on Artifacts.YearCreated for chronological queries
            modelBuilder.Entity<Artifact>()
                .HasIndex(a => a.YearCreated)
                .HasDatabaseName("IX_Artifacts_YearCreated");

            // Full-text search index on Artifact.Title (PostgreSQL specific)
            modelBuilder.Entity<Artifact>()
                .HasIndex(a => a.Title)
                .HasDatabaseName("IX_Artifacts_Title");

            // Seed Art Types (Aboriginal Art Categories)
            modelBuilder.Entity<ArtType>().HasData(
                new ArtType
                {
                    Id = 1,
                    Name = "Dot Painting",
                    Description = "Traditional Aboriginal art using dots to create patterns and hide sacred meanings",
                    Region = "Central Desert",
                    Technique = "Acrylic dots using sticks, brushes, or dowels"
                },
                new ArtType
                {
                    Id = 2,
                    Name = "Bark Painting",
                    Description = "Painting on eucalyptus bark using natural ochres",
                    Region = "Arnhem Land",
                    Technique = "Natural pigments on prepared bark"
                },
                new ArtType
                {
                    Id = 3,
                    Name = "Rock Art",
                    Description = "Ancient paintings and engravings on rock surfaces",
                    Region = "Throughout Australia",
                    Technique = "Ochre, charcoal, clay, and engraving"
                },
                new ArtType
                {
                    Id = 4,
                    Name = "Weaving",
                    Description = "Traditional basket weaving and fiber art",
                    Region = "Coastal and River Regions",
                    Technique = "Pandanus, grasses, and natural fibers"
                }
            );

            // Seed Artists (Famous Aboriginal Artists)
            modelBuilder.Entity<Artist>().HasData(
                new Artist
                {
                    Id = 1,
                    FirstName = "Emily",
                    LastName = "Kame Kngwarreye",
                    ClanGroup = "Alhalkere",
                    Region = "Utopia, Northern Territory",
                    LanguageGroup = "Anmatyerre",
                    BirthDate = DateTime.SpecifyKind(new DateTime(1910, 1, 1), DateTimeKind.Utc),
                    DeathDate = DateTime.SpecifyKind(new DateTime(1996, 9, 2), DateTimeKind.Utc),
                    Biography = "One of Australia's most significant contemporary artists, known for her abstract expressionist works.",
                    IsDeceased = true
                },
                new Artist
                {
                    Id = 2,
                    FirstName = "Clifford",
                    LastName = "Possum Tjapaltjarri",
                    ClanGroup = "Anmatyerre",
                    Region = "Napperby Station, Northern Territory",
                    LanguageGroup = "Anmatyerre",
                    BirthDate = DateTime.SpecifyKind(new DateTime(1932, 1, 1), DateTimeKind.Utc),
                    DeathDate = DateTime.SpecifyKind(new DateTime(2002, 6, 21), DateTimeKind.Utc),
                    Biography = "Pioneer of the contemporary Aboriginal art movement, famous for his dot paintings.",
                    IsDeceased = true
                },
                new Artist
                {
                    Id = 3,
                    FirstName = "Albert",
                    LastName = "Namatjira",
                    ClanGroup = "Western Arrernte",
                    Region = "Hermannsburg, Northern Territory",
                    LanguageGroup = "Arrernte",
                    BirthDate = DateTime.SpecifyKind(new DateTime(1902, 7, 28), DateTimeKind.Utc),
                    DeathDate = DateTime.SpecifyKind(new DateTime(1959, 8, 8), DateTimeKind.Utc),
                    Biography = "First famous Aboriginal watercolour artist, known for his landscape paintings of Central Australia.",
                    IsDeceased = true
                },
                new Artist
                {
                    Id = 4,
                    FirstName = "Yannima",
                    LastName = "Tommy Watson",
                    ClanGroup = "Pitjantjatjara",
                    Region = "Irrunytju, Western Australia",
                    LanguageGroup = "Pitjantjatjara",
                    BirthDate = DateTime.SpecifyKind(new DateTime(1935, 1, 1), DateTimeKind.Utc),
                    DeathDate = DateTime.SpecifyKind(new DateTime(2017, 11, 9), DateTimeKind.Utc),
                    Biography = "Known for vibrant abstract paintings representing his country and ancestral stories.",
                    IsDeceased = true
                }
            );

            // Seed Artifacts (Artworks)
            modelBuilder.Entity<Artifact>().HasData(
                new Artifact
                {
                    Id = 1,
                    Title = "Alhalkere Country",
                    Description = "A vibrant abstract representation of the artist's ancestral lands",
                    Story = "This painting represents the Bush Yam Dreaming, a story passed down through generations of the Alhalkere women.",
                    Dimensions = "151cm x 121cm",
                    Medium = "Acrylic on Canvas",
                    YearCreated = 1991,
                    Price = 250000.00m,
                    IsAvailable = false,
                    ArtistId = 1,
                    ArtTypeId = 1
                },
                new Artifact
                {
                    Id = 2,
                    Title = "Warlugulong",
                    Description = "A major collaborative work depicting the ancestral fire dreaming",
                    Story = "Tells the story of Lungkata, the Blue Tongue Lizard, who brought fire to the people of the Western Desert.",
                    Dimensions = "210cm x 560cm",
                    Medium = "Synthetic polymer paint on canvas",
                    YearCreated = 1977,
                    Price = 2500000.00m,
                    IsAvailable = false,
                    ArtistId = 2,
                    ArtTypeId = 1
                },
                new Artifact
                {
                    Id = 3,
                    Title = "Ghost Gum",
                    Description = "Watercolour landscape featuring the iconic ghost gum trees",
                    Story = "Depicts the MacDonnell Ranges near Alice Springs, painted en plein air.",
                    Dimensions = "35cm x 53cm",
                    Medium = "Watercolour on paper",
                    YearCreated = 1955,
                    Price = 45000.00m,
                    IsAvailable = false,
                    ArtistId = 3,
                    ArtTypeId = 3
                },
                new Artifact
                {
                    Id = 4,
                    Title = "Irrunytju",
                    Description = "Abstract dot painting in vibrant reds, oranges, and yellows",
                    Story = "Represents the artist's birthplace and the ancestral creation stories of the Western Desert.",
                    Dimensions = "183cm x 244cm",
                    Medium = "Synthetic polymer paint on linen",
                    YearCreated = 2007,
                    Price = 180000.00m,
                    IsAvailable = false,
                    ArtistId = 4,
                    ArtTypeId = 1
                },
                new Artifact
                {
                    Id = 5,
                    Title = "Mina Mina Dreaming",
                    Description = "Contemporary dot painting exploring women's ceremonial sites",
                    Story = "Depicts the journey of ancestral women across the desert country, gathering food and performing ceremonies.",
                    Dimensions = "122cm x 150cm",
                    Medium = "Acrylic on Belgian linen",
                    YearCreated = 2005,
                    Price = 32000.00m,
                    IsAvailable = true,
                    ArtistId = 1,
                    ArtTypeId = 1
                },
                new Artifact
                {
                    Id = 6,
                    Title = "Fish Trap",
                    Description = "Traditional bark painting showing traditional hunting tools",
                    Story = "Depicts the traditional woven fish traps used by the Yolngu people in Arnhem Land.",
                    Dimensions = "120cm x 80cm",
                    Medium = "Natural ochres on eucalyptus bark",
                    YearCreated = 1985,
                    Price = 28000.00m,
                    IsAvailable = true,
                    ArtistId = 2,
                    ArtTypeId = 2
                }
            );
        }
    }
}
