using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ArtGalleryAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Artists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    ClanGroup = table.Column<string>(type: "text", nullable: true),
                    Region = table.Column<string>(type: "text", nullable: true),
                    LanguageGroup = table.Column<string>(type: "text", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeathDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Biography = table.Column<string>(type: "text", nullable: true),
                    IsDeceased = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArtTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Region = table.Column<string>(type: "text", nullable: false),
                    Technique = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Artifacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Story = table.Column<string>(type: "text", nullable: true),
                    Dimensions = table.Column<string>(type: "text", nullable: true),
                    Medium = table.Column<string>(type: "text", nullable: true),
                    YearCreated = table.Column<int>(type: "integer", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: true),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ArtistId = table.Column<int>(type: "integer", nullable: false),
                    ArtTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artifacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Artifacts_ArtTypes_ArtTypeId",
                        column: x => x.ArtTypeId,
                        principalTable: "ArtTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Artifacts_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ArtTypes",
                columns: new[] { "Id", "Description", "Name", "Region", "Technique" },
                values: new object[,]
                {
                    { 1, "Traditional Aboriginal art using dots to create patterns and hide sacred meanings", "Dot Painting", "Central Desert", "Acrylic dots using sticks, brushes, or dowels" },
                    { 2, "Painting on eucalyptus bark using natural ochres", "Bark Painting", "Arnhem Land", "Natural pigments on prepared bark" },
                    { 3, "Ancient paintings and engravings on rock surfaces", "Rock Art", "Throughout Australia", "Ochre, charcoal, clay, and engraving" },
                    { 4, "Traditional basket weaving and fiber art", "Weaving", "Coastal and River Regions", "Pandanus, grasses, and natural fibers" }
                });

            migrationBuilder.InsertData(
                table: "Artists",
                columns: new[] { "Id", "Biography", "BirthDate", "ClanGroup", "CreatedAt", "DeathDate", "FirstName", "IsDeceased", "LanguageGroup", "LastName", "Region" },
                values: new object[,]
                {
                    { 1, "One of Australia's most significant contemporary artists, known for her abstract expressionist works.", new DateTime(1910, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Alhalkere", new DateTime(2026, 4, 1, 18, 8, 33, 985, DateTimeKind.Utc).AddTicks(6610), new DateTime(1996, 9, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Emily", true, "Anmatyerre", "Kame Kngwarreye", "Utopia, Northern Territory" },
                    { 2, "Pioneer of the contemporary Aboriginal art movement, famous for his dot paintings.", new DateTime(1932, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Anmatyerre", new DateTime(2026, 4, 1, 18, 8, 33, 985, DateTimeKind.Utc).AddTicks(6620), new DateTime(2002, 6, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Clifford", true, "Anmatyerre", "Possum Tjapaltjarri", "Napperby Station, Northern Territory" },
                    { 3, "First famous Aboriginal watercolour artist, known for his landscape paintings of Central Australia.", new DateTime(1902, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Western Arrernte", new DateTime(2026, 4, 1, 18, 8, 33, 985, DateTimeKind.Utc).AddTicks(6620), new DateTime(1959, 8, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Albert", true, "Arrernte", "Namatjira", "Hermannsburg, Northern Territory" },
                    { 4, "Known for vibrant abstract paintings representing his country and ancestral stories.", new DateTime(1935, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pitjantjatjara", new DateTime(2026, 4, 1, 18, 8, 33, 985, DateTimeKind.Utc).AddTicks(6630), new DateTime(2017, 11, 9, 0, 0, 0, 0, DateTimeKind.Utc), "Yannima", true, "Pitjantjatjara", "Tommy Watson", "Irrunytju, Western Australia" }
                });

            migrationBuilder.InsertData(
                table: "Artifacts",
                columns: new[] { "Id", "ArtTypeId", "ArtistId", "CreatedAt", "Description", "Dimensions", "ImageUrl", "IsAvailable", "Medium", "Price", "Story", "Title", "YearCreated" },
                values: new object[,]
                {
                    { 1, 1, 1, new DateTime(2026, 4, 1, 18, 8, 33, 985, DateTimeKind.Utc).AddTicks(6640), "A vibrant abstract representation of the artist's ancestral lands", "151cm x 121cm", null, false, "Acrylic on Canvas", 250000.00m, "This painting represents the Bush Yam Dreaming, a story passed down through generations of the Alhalkere women.", "Alhalkere Country", 1991 },
                    { 2, 1, 2, new DateTime(2026, 4, 1, 18, 8, 33, 985, DateTimeKind.Utc).AddTicks(6640), "A major collaborative work depicting the ancestral fire dreaming", "210cm x 560cm", null, false, "Synthetic polymer paint on canvas", 2500000.00m, "Tells the story of Lungkata, the Blue Tongue Lizard, who brought fire to the people of the Western Desert.", "Warlugulong", 1977 },
                    { 3, 3, 3, new DateTime(2026, 4, 1, 18, 8, 33, 985, DateTimeKind.Utc).AddTicks(6650), "Watercolour landscape featuring the iconic ghost gum trees", "35cm x 53cm", null, false, "Watercolour on paper", 45000.00m, "Depicts the MacDonnell Ranges near Alice Springs, painted en plein air.", "Ghost Gum", 1955 },
                    { 4, 1, 4, new DateTime(2026, 4, 1, 18, 8, 33, 985, DateTimeKind.Utc).AddTicks(6650), "Abstract dot painting in vibrant reds, oranges, and yellows", "183cm x 244cm", null, false, "Synthetic polymer paint on linen", 180000.00m, "Represents the artist's birthplace and the ancestral creation stories of the Western Desert.", "Irrunytju", 2007 },
                    { 5, 1, 1, new DateTime(2026, 4, 1, 18, 8, 33, 985, DateTimeKind.Utc).AddTicks(6650), "Contemporary dot painting exploring women's ceremonial sites", "122cm x 150cm", null, true, "Acrylic on Belgian linen", 32000.00m, "Depicts the journey of ancestral women across the desert country, gathering food and performing ceremonies.", "Mina Mina Dreaming", 2005 },
                    { 6, 2, 2, new DateTime(2026, 4, 1, 18, 8, 33, 985, DateTimeKind.Utc).AddTicks(6650), "Traditional bark painting showing traditional hunting tools", "120cm x 80cm", null, true, "Natural ochres on eucalyptus bark", 28000.00m, "Depicts the traditional woven fish traps used by the Yolngu people in Arnhem Land.", "Fish Trap", 1985 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Artifacts_ArtistId",
                table: "Artifacts",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_Artifacts_ArtTypeId",
                table: "Artifacts",
                column: "ArtTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Artifacts");

            migrationBuilder.DropTable(
                name: "ArtTypes");

            migrationBuilder.DropTable(
                name: "Artists");
        }
    }
}
