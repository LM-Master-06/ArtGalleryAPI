# Aboriginal Art Gallery API

A .NET 8 Web API for managing an Aboriginal Art Gallery with three bounded contexts:
- **Artists** - Aboriginal artist profiles
- **ArtTypes** - Categories of Aboriginal art (Dot Painting, Bark Painting, etc.)
- **Artifacts** - Art pieces connecting Artists and ArtTypes

## Technology Stack

- ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- PostgreSQL

## Prerequisites

- .NET 8 SDK
- PostgreSQL (installed locally or via Docker)
- Optional: pgAdmin or any PostgreSQL client

## Setup Instructions

### 1. Install PostgreSQL

Using Homebrew on macOS:
```bash
brew install postgresql
brew services start postgresql
```

Or using Docker:
```bash
docker run --name postgres -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=yourpassword -p 5432:5432 -d postgres
```

### 2. Create Database

```bash
createdb ArtGalleryDB
```

Or using psql:
```bash
psql -U postgres
create database ArtGalleryDB;
```

### 3. Update Connection String

Update `appsettings.json` with your PostgreSQL credentials:
```json
"ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ArtGalleryDB;Username=postgres;Password=yourpassword"
}
```

### 4. Run Migrations

```bash
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet run
```

The API will be available at:
- API: `https://localhost:5001` or `http://localhost:5000`
- Swagger UI: `https://localhost:5001/swagger`

## API Endpoints

### ArtTypes
- `GET /api/arttypes` - Get all art types
- `GET /api/arttypes/{id}` - Get art type by ID
- `GET /api/arttypes/{id}/artifacts` - Get artifacts by art type
- `POST /api/arttypes` - Create new art type
- `PUT /api/arttypes/{id}` - Update art type
- `DELETE /api/arttypes/{id}` - Delete art type

### Artists
- `GET /api/artists` - Get all artists
- `GET /api/artists/{id}` - Get artist by ID
- `GET /api/artists/{id}/artifacts` - Get artifacts by artist
- `GET /api/artists/search/by-region?region={region}` - Search artists by region
- `POST /api/artists` - Create new artist
- `PUT /api/artists/{id}` - Update artist
- `DELETE /api/artists/{id}` - Delete artist

### Artifacts
- `GET /api/artifacts` - Get all artifacts (with artist & type details)
- `GET /api/artifacts/{id}` - Get artifact by ID
- `GET /api/artifacts/available` - Get available artifacts
- `GET /api/artifacts/search/by-type?artTypeId={id}` - Search by art type
- `GET /api/artifacts/search/by-artist?artistId={id}` - Search by artist
- `POST /api/artifacts` - Create new artifact
- `PUT /api/artifacts/{id}` - Update artifact
- `PATCH /api/artifacts/{id}/availability` - Update availability status
- `DELETE /api/artifacts/{id}` - Delete artifact

## Sample Data

The database comes pre-seeded with:
- 4 Art Types (Dot Painting, Bark Painting, Rock Art, Weaving)
- 4 Famous Aboriginal Artists (Emily Kame Kngwarreye, Clifford Possum, etc.)
- 6 Sample Artifacts with Dreamtime stories

## Example POST Requests

### Create ArtType
```json
POST /api/arttypes
{
    "name": "Sculpture",
    "description": "Traditional carved wooden sculptures",
    "region": "Arnhem Land",
    "technique": "Wood carving"
}
```

### Create Artist
```json
POST /api/artists
{
    "firstName": "Sally",
    "lastName": "Gabori",
    "clanGroup": "Kaiadilt",
    "region": "Mornington Island",
    "languageGroup": "Kaiadilt",
    "isDeceased": false
}
```

### Create Artifact
```json
POST /api/artifacts
{
    "title": "My Country",
    "description": "Vibrant abstract painting",
    "story": "Depicts the artist's traditional country",
    "dimensions": "200cm x 150cm",
    "medium": "Acrylic on Canvas",
    "yearCreated": 2020,
    "price": 15000.00,
    "isAvailable": true,
    "artistId": 1,
    "artTypeId": 1
}
```

## Project Structure

```
ArtGalleryAPI/
├── Controllers/
│   ├── ArtTypesController.cs    # Art Types bounded context
│   ├── ArtistsController.cs     # Artists bounded context
│   └── ArtifactsController.cs   # Artifacts bounded context
├── Data/
│   └── ArtGalleryDbContext.cs   # Database context with seed data
├── Models/
│   ├── ArtType.cs               # ArtType entity
│   ├── Artist.cs                # Artist entity
│   └── Artifact.cs              # Artifact entity
├── Program.cs                   # App configuration
├── appsettings.json             # Configuration
└── README.md                    # This file
```

## Entity Relationships

```
Artist (1) ----< (*) Artifact (*) >---- (1) ArtType
```
- An Artist can have many Artifacts
- An ArtType can have many Artifacts
- Each Artifact belongs to one Artist and one ArtType
