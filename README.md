# 🎨 ArtGalleryAPI

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com) 
[![C#](https://img.shields.io/badge/Language-C%23-239120?logo=csharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/) 
[![EF Core](https://img.shields.io/badge/ORM-EF%20Core%208.0-512BD4?logo=dotnet&logoColor=white)](https://docs.microsoft.com/en-us/ef/core/) 
[![PostgreSQL](https://img.shields.io/badge/Database-PostgreSQL-336791?logo=postgresql&logoColor=white)](https://www.postgresql.org) 
[![JWT](https://img.shields.io/badge/Auth-JWT-000000?logo=jsonwebtokens&logoColor=white)](https://jwt.io) 
[![Swagger](https://img.shields.io/badge/API%20Docs-Swagger%20UI-85EA2D?logo=swagger&logoColor=black)](https://swagger.io) 
[![xUnit](https://img.shields.io/badge/Testing-xUnit-5B2D8E)](https://xunit.net) 
[![Status](https://img.shields.io/badge/Status-Active%20Development-2ECC71)](https://github.com/LM-Master-06/ArtGalleryAPI) 
[![License](https://img.shields.io/badge/License-MIT-F1C40F)](LICENSE)

A **production-ready Art Gallery Management API** built with **ASP.NET Core 9.0** and **C#**. This RESTful Web API provides comprehensive artwork cataloguing, artist profile management, exhibition handling, and art type organization with a clean, scalable architecture.

---

## ✨ Features

### 🖼️ **Artwork Management**
- **CRUD Operations**: Create, read, update, delete artwork entries
- **Rich Metadata**: Store title, medium, year, dimensions, description, and price
- **Artist & Category Association**: Link artworks to artists and categorize by art type
- **Advanced Filtering**: Query artworks by various attributes
- **Timestamp Tracking**: Automatic creation and modification date tracking

### 👤 **Artist Profiles**
- **Complete Artist Records**: Biography, nationality, date of birth, contact info
- **Portfolio Management**: Track multiple artworks per artist
- **Activity Status**: Mark artists as active or inactive
- **Professional Details**: Store education and career milestones

### 🏛️ **Exhibition Management** 
- **Create & Organize Exhibitions**: Add detailed exhibition information
- **Artwork Assignment**: Assign artworks to specific exhibitions
- **Exhibition Metadata**: Track dates, themes, descriptions
- **Status Tracking**: Track exhibition lifecycle (planned, active, completed)

### 🗂️ **Category Organization**
- **Art Type Classification**: Organize artworks into categories (Painting, Sculpture, Photography, etc.)
- **Category CRUD**: Full management of art types
- **Filtering Support**: Quick filtering of artworks by category

### 🔐 **Authentication & Authorization**
- **JWT Authentication**: Secure token-based authentication
- **Role-Based Access Control (RBAC)**: Admin, Curator, and Viewer roles
- **Password Security**: Encrypted password storage with checksums
- **Token Validation**: Issuer, audience, and lifetime validation

### ✅ **Testing & Quality Assurance**
- **Dedicated Test Project**: Comprehensive unit and integration tests
- **Service Layer Testing**: Tests for all business logic
- **Controller Testing**: HTTP endpoint validation
- **Test Isolation**: In-memory database for test environment

### ⚡ **Performance & Scalability**
- **EF Core Optimizations**: Efficient database queries with lazy loading prevention
- **Layered Architecture**: Clear separation of concerns
- **DTO Pattern**: Data transfer objects for API contracts
- **Middleware Pipeline**: Custom exception handling and validation middleware
- **CORS Support**: Multi-origin request handling for frontend integration

### 📚 **Documentation**
- **Swagger UI**: Interactive API documentation at `/swagger`
- **XML Documentation**: Code-level documentation for all endpoints
- **Endpoint Examples**: Example requests and responses for all operations

---

## 🏗️ **Architecture**

The API follows a **Clean Architecture** pattern with distinct layers:

```
┌─────────────────────────────────────────────────────────┐
│                    CLIENT LAYER                         │
│          (Web Browser / Mobile App / Desktop)           │
└──────────────────────┬──────────────────────────────────┘
                       │ HTTP/HTTPS
┌──────────────────────▼──────────────────────────────────┐
│              PRESENTATION LAYER                         │
│              (Controllers / Endpoints)                  │
│   AuthController │ ArtistsController │ ArtifactsAPI     │
└──────────────────────┬──────────────────────────────────┘
                       │ Dependency Injection
┌──────────────────────▼──────────────────────────────────┐
│           BUSINESS LOGIC LAYER                          │
│              (Services)                                 │
│   UserService │ ArtistService │ ArtifactService │ etc   │
└──────────────────────┬──────────────────────────────────┘
                       │ Repository Pattern
┌──────────────────────▼──────────────────────────────────┐
│           DATA ACCESS LAYER                             │
│          (Repositories & DbContext)                     │
│   ArtistRepository │ ArtifactRepository │ etc            │
└──────────────────────┬──────────────────────────────────┘
                       │ EF Core ORM
┌──────────────────────▼──────────────────────────────────┐
│            DATA PERSISTENCE LAYER                       │
│            (PostgreSQL Database)                        │
│      Users │ Artists │ Artifacts │ ArtTypes             │
└─────────────────────────────────────────────────────────┘
```

---

## 📁 **Project Structure**

```
ArtGalleryAPI-Repository/
│
├── ArtGalleryAPI/                          # Main Web API Project
│   │
│   ├── Controllers/                        # HTTP Request Handlers
│   │   ├── AuthController.cs               # Authentication & user management
│   │   ├── ArtistsController.cs            # Artist CRUD endpoints
│   │   ├── ArtifactsController.cs          # Artwork CRUD endpoints
│   │   └── ArtTypesController.cs           # Category management endpoints
│   │
│   ├── Models/                             # Entity Models (Domain Layer)
│   │   ├── User.cs                         # User entity with authentication
│   │   ├── Artist.cs                       # Artist profile entity
│   │   ├── Artifact.cs                     # Artwork entity
│   │   └── ArtType.cs                      # Art category entity
│   │
│   ├── DTOs/                               # Data Transfer Objects
│   │   ├── UserDTOs.cs                     # User request/response objects
│   │   ├── ArtistDTOs.cs                   # Artist request/response objects
│   │   ├── ArtifactDTOs.cs                 # Artifact request/response objects
│   │   ├── ArtTypeDTOs.cs                  # ArtType request/response objects
│   │   └── ApiResponse.cs                  # Standardized API response format
│   │
│   ├── Services/                           # Business Logic Layer
│   │   ├── Interfaces/                     # Service contracts
│   │   │   ├── IUserService.cs             # User service interface
│   │   │   ├── IArtistService.cs           # Artist service interface
│   │   │   ├── IArtifactService.cs         # Artifact service interface
│   │   │   └── IArtTypeService.cs          # ArtType service interface
│   │   │
│   │   ├── UserService.cs                  # User service implementation
│   │   ├── JwtService.cs                   # JWT token generation & validation
│   │   ├── ArtistService.cs                # Artist business logic
│   │   ├── ArtifactService.cs              # Artifact business logic
│   │   └── ArtTypeService.cs               # ArtType business logic
│   │
│   ├── Repositories/                       # Data Access Layer
│   │   ├── Interfaces/                     # Repository contracts
│   │   │   ├── IUserRepository.cs          # User repository interface
│   │   │   ├── IArtistRepository.cs        # Artist repository interface
│   │   │   ├── IArtifactRepository.cs      # Artifact repository interface
│   │   │   └── IArtTypeRepository.cs       # ArtType repository interface
│   │   │
│   │   ├── UserRepository.cs               # User data access
│   │   ├── ArtistRepository.cs             # Artist data access
│   │   ├── ArtifactRepository.cs           # Artifact data access
│   │   └── ArtTypeRepository.cs            # ArtType data access
│   │
│   ├── Data/                               # EF Core Configuration
│   │   ├── ArtGalleryDbContext.cs          # DbContext with all DbSets
│   │   └── Migrations/                     # Database version control
│   │
│   ├── Middleware/                         # Custom Middleware
│   │   ├── ExceptionHandlingMiddleware.cs  # Global exception handling
│   │   └── ValidationMiddleware.cs         # Input validation
│   │
│   ├── Properties/                         # Project Properties
│   │   └── launchSettings.json             # Development server config
│   │
│   ├── appsettings.json                    # Production configuration
│   ├── appsettings.Development.json        # Development configuration
│   ├── Program.cs                          # Application entry point & DI
│   ├── seed_data.sql                       # SQL data seeding script
│   └── ArtGalleryAPI.csproj                # Project file with dependencies
│
├── ArtGalleryAPI.Tests/                    # Unit & Integration Tests
│   │
│   ├── Controllers/                        # Controller Tests
│   │   └── AuthControllerTests.cs          # Authentication endpoint tests
│   │
│   ├── Services/                           # Service Tests
│   │   ├── UserServiceTests.cs             # User service tests
│   │   ├── JwtServiceTests.cs              # JWT service tests
│   │   ├── ArtistServiceTests.cs           # Artist service tests
│   │   └── ArtTypeServiceTests.cs          # ArtType service tests
│   │
│   └── ArtGalleryAPI.Tests.csproj          # Test project file
│
├── art-gallery-frontend-development/       # React/TypeScript Frontend (Optional)
│   ├── src/                                # Source code
│   ├── public/                             # Static assets
│   ├── package.json                        # Frontend dependencies
│   └── vite.config.ts                      # Vite configuration
│
├── NewBackend.sln                          # Visual Studio Solution file
├── README.md                               # Project documentation
├── EndPoints1.png                          # API endpoint screenshots
├── EndPoints2.png                          # API endpoint screenshots
└── .gitignore                              # Git ignore file

```

---

## 🚀 **Quick Start Guide**

### Prerequisites
- **.NET 9.0 SDK** or later ([Download](https://dotnet.microsoft.com/download))
- **PostgreSQL 12+** ([Download](https://www.postgresql.org/download/))
- **Git** ([Download](https://git-scm.com/))
- **Visual Studio 2022** or **VS Code** with C# extension (Recommended)

### 1️⃣ **Clone the Repository**

```bash
git clone https://github.com/LM-Master-06/ArtGalleryAPI.git
cd ArtGalleryAPI
```

### 2️⃣ **Configure Database Connection**

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "User ID=postgres;Password=your_password;Host=localhost;Port=5432;Database=ArtGalleryDB;"
  }
}
```

### 3️⃣ **Restore Dependencies & Create Database**

```bash
# Navigate to the API project
cd ArtGalleryAPI

# Restore NuGet packages
dotnet restore

# Apply migrations to create database
dotnet ef database update
```

### 4️⃣ **Run the Application**

```bash
# From the ArtGalleryAPI directory
dotnet run
```

**Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to quit.
```

### 5️⃣ **Access the API**

- **API Base URL**: `https://localhost:5001`
- **Swagger UI**: `https://localhost:5001/swagger`
- **Admin Login**: 
  - Email: `admin@artgallery.com`
  - Password: `Admin123!`

---

## 📡 **API Endpoints**

### Authentication Endpoints

```
POST   /api/auth/register              Register a new user
POST   /api/auth/login                 Login user and get JWT token
POST   /api/auth/refresh-token         Refresh expired JWT token
POST   /api/auth/logout                Logout user (optional)
GET    /api/auth/profile               Get current user profile
PUT    /api/auth/profile               Update user profile
```

### Artist Endpoints

```
GET    /api/artists                    Get all artists (paginated)
GET    /api/artists/{id}               Get artist by ID
POST   /api/artists                    Create new artist (Admin/Curator)
PUT    /api/artists/{id}               Update artist details (Admin/Curator)
DELETE /api/artists/{id}               Delete artist (Admin only)
GET    /api/artists/{id}/artworks      Get artworks by specific artist
```

### Artifact (Artwork) Endpoints

```
GET    /api/artifacts                  Get all artworks (with filtering)
GET    /api/artifacts/{id}             Get artwork by ID
POST   /api/artifacts                  Create new artwork (Curator)
PUT    /api/artifacts/{id}             Update artwork details (Curator)
DELETE /api/artifacts/{id}             Delete artwork (Admin only)
GET    /api/artifacts/type/{typeId}    Get artworks by art type
GET    /api/artifacts/artist/{artistId} Get artworks by artist
```

### Art Type (Category) Endpoints

```
GET    /api/artTypes                   Get all art types/categories
GET    /api/artTypes/{id}              Get art type by ID
POST   /api/artTypes                   Create new art type (Admin)
PUT    /api/artTypes/{id}              Update art type (Admin)
DELETE /api/artTypes/{id}              Delete art type (Admin only)
```

### Example Requests

**Register User:**
```bash
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "SecurePass123!",
    "firstName": "John",
    "lastName": "Doe"
  }'
```

**Login:**
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "SecurePass123!"
  }'
```

**Get All Artists:**
```bash
curl https://localhost:5001/api/artists \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Create Artwork:**
```bash
curl -X POST https://localhost:5001/api/artifacts \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Starry Night",
    "medium": "Oil on Canvas",
    "year": 1889,
    "description": "A swirling night sky",
    "artistId": 1,
    "artTypeId": 1,
    "price": 50000000
  }'
```

---

## 🔐 **Authentication & Authorization**

### JWT Configuration

The API uses **JWT (JSON Web Token)** for stateless authentication.

**JWT Settings** (`appsettings.json`):
```json
{
  "JwtSettings": {
    "SecretKey": "YourSecretKeyMustBeAtLeast32CharsLong!",
    "Issuer": "ArtGalleryAPI",
    "Audience": "ArtGalleryClient",
    "ExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

### Roles & Permissions

| Role | Permissions |
|------|------------|
| **Admin** | Full system access, user management, delete operations |
| **Curator** | Create/edit artworks and artists, manage exhibitions |
| **Viewer** | Read-only access to artwork and artist information |

### Protected Endpoints

Add the JWT token to request headers:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## 🗄️ **Database Schema**

### Entity Relationship Diagram

```
┌─────────────┐         ┌──────────────┐
│    Users    │         │   Artists    │
├─────────────┤         ├──────────────┤
│ UserId (PK) │         │ ArtistId (PK)│
│ Email       │         │ Name         │
│ FirstName   │         │ Bio          │
│ LastName    │         │ Nationality  │
│ Role        │         │ DoB          │
│ IsActive    │         │ IsActive     │
│ CreatedAt   │         │ CreatedAt    │
└─────────────┘         └──────────────┘
                              │
                              │ 1:N
                              │
                        ┌─────▼────────────┐
                        │   Artifacts      │
                        ├──────────────────┤
                        │ ArtifactId (PK)  │
                        │ Title            │
                        │ Medium           │
                        │ Year             │
                        │ Description      │
                        │ Price            │
                        │ ArtistId (FK)    │
                        │ ArtTypeId (FK)   │
                        │ CreatedAt        │
                        │ UpdatedAt        │
                        └──────┬───────────┘
                               │
                               │ N:1
                               │
                        ┌──────▼─────────┐
                        │    ArtTypes    │
                        ├────────────────┤
                        │ ArtTypeId (PK) │
                        │ Name           │
                        │ Description    │
                        │ CreatedAt      │
                        └────────────────┘
```

### Key Tables

**Users Table:**
- User authentication and role management
- Password stored as hashed + salted values
- Support for multiple roles and permissions

**Artists Table:**
- Artist profile information
- Biographical data and career information
- One-to-many relationship with Artifacts

**Artifacts Table:**
- Core artwork/artwork records
- Links to Artists (Foreign Key)
- Links to ArtTypes (Foreign Key)
- Pricing and metadata information

**ArtTypes Table:**
- Art category/type definitions (Painting, Sculpture, etc.)
- Referenced by Artifacts for classification

---

## 🧪 **Testing**

### Running Tests

```bash
# Run all tests with output
dotnet test

# Run specific test file
dotnet test ArtGalleryAPI.Tests/Controllers/AuthControllerTests.cs

# Run with detailed logging
dotnet test --logger "console;verbosity=detailed"

# Generate code coverage report
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

### Test Coverage Areas

- ✅ Controller endpoint validation
- ✅ Service business logic
- ✅ Repository data access
- ✅ Authentication and authorization
- ✅ Error handling and validation
- ✅ JWT token generation and validation

### Example Test

```csharp
[Fact]
public async Task CreateArtist_WithValidData_ReturnsCreatedArtist()
{
    // Arrange
    var createArtistDto = new CreateArtistDto 
    { 
        Name = "Vincent van Gogh",
        Nationality = "Dutch"
    };
    
    // Act
    var result = await _artistService.CreateArtistAsync(createArtistDto);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("Vincent van Gogh", result.Name);
}
```

---

## ⚙️ **Configuration**

### Development Configuration

`appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "User ID=postgres;Password=password;Host=localhost;Port=5432;Database=ArtGalleryDB_Dev;"
  },
  "JwtSettings": {
    "SecretKey": "DevelopmentSecretKeyMustBeAtLeast32CharactersLong!",
    "Issuer": "ArtGalleryAPI-Dev",
    "Audience": "ArtGalleryClient-Dev",
    "ExpirationMinutes": 120,
    "RefreshTokenExpirationDays": 30
  }
}
```

### Production Configuration

Update values for production environment:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Production PostgreSQL connection string"
  },
  "JwtSettings": {
    "SecretKey": "Use strong 32+ character secret key",
    "Issuer": "ArtGalleryAPI",
    "Audience": "ArtGalleryClient",
    "ExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

### Environment Variables

```bash
# Set connection string via environment variable
export ASPNETCORE_CONNECTION_STRINGS:DEFAULTCONNECTION="your_connection_string"

# Set JWT secret
export ASPNETCORE_JWTSETTINGS:SECRETKEY="your_secret_key"

# Set environment
export ASPNETCORE_ENVIRONMENT="Production"
```

---

## 🔒 **Security Features**

### ✓ Implemented Security Measures

- **JWT Authentication**: Secure token-based authentication with configurable expiration
- **Password Hashing**: PBKDF2 algorithm with salt and iterations
- **CORS Protection**: Configurable cross-origin resource sharing
- **Input Validation**: Data annotation-based validation on all DTOs
- **SQL Injection Prevention**: Entity Framework Core parameterized queries
- **Authorization Policies**: Role-based access control on protected endpoints
- **HTTPS Enforcement**: Automatic HTTPS redirection in production
- **Secure Headers**: Custom middleware for security headers
- **Exception Handling**: Global exception handling middleware prevents information leakage
- **Rate Limiting**: Built-in support for request throttling (can be enabled)

### Security Best Practices

1. **Change Default Credentials**: Update admin password immediately after deployment
2. **Use Environment Variables**: Never commit sensitive data to version control
3. **Enable HTTPS**: Always use HTTPS in production
4. **Rotate Tokens**: Implement regular JWT token rotation
5. **Update Dependencies**: Keep NuGet packages updated for security patches
6. **Validate Input**: Always validate and sanitize user input
7. **Limit Exposure**: Use principle of least privilege for database accounts

---

## 🛠️ **Development Guide**

### Adding a New Entity

1. **Create Model** in `Models/` folder:
```csharp
public class Exhibition
{
    public int ExhibitionId { get; set; }
    public string Title { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    // Navigation properties
    public ICollection<Artifact> Artifacts { get; set; } = new List<Artifact>();
}
```

2. **Create DTOs** in `DTOs/` folder:
```csharp
public class CreateExhibitionDto
{
    [Required]
    public string Title { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
```

3. **Create Repository Interface** in `Repositories/Interfaces/`:
```csharp
public interface IExhibitionRepository
{
    Task<Exhibition> GetByIdAsync(int id);
    Task AddAsync(Exhibition exhibition);
    Task UpdateAsync(Exhibition exhibition);
    Task DeleteAsync(int id);
}
```

4. **Implement Repository** in `Repositories/`:
```csharp
public class ExhibitionRepository : IExhibitionRepository
{
    // Implementation...
}
```

5. **Create Service Interface** and **Implementation**

6. **Register in DI Container** (`Program.cs`):
```csharp
builder.Services.AddScoped<IExhibitionRepository, ExhibitionRepository>();
builder.Services.AddScoped<IExhibitionService, ExhibitionService>();
```

7. **Create Controller** in `Controllers/`

8. **Create Migration**:
```bash
dotnet ef migrations add AddExhibition
dotnet ef database update
```

---

## 📊 **Performance Optimization**

### Implemented Optimizations

- **Lazy Loading Prevention**: Using `Select()` to load only needed columns
- **Connection Pooling**: Automatic EF Core connection pooling
- **Query Caching**: Application-level caching for frequently accessed data
- **Async/Await**: All database operations use async patterns
- **Indexing**: Database indexes on frequently queried columns

### Performance Tips

```csharp
// ❌ Bad: N+1 query problem
var artists = context.Artists.ToList();
foreach (var artist in artists)
{
    var artworks = context.Artifacts.Where(a => a.ArtistId == artist.ArtistId).ToList();
}

// ✅ Good: Single query with includes
var artists = context.Artists
    .Include(a => a.Artifacts)
    .ToList();

// ✅ Better: Projection to DTOs
var artists = context.Artists
    .Select(a => new ArtistDto 
    { 
        Id = a.ArtistId,
        Name = a.Name,
        ArtworkCount = a.Artifacts.Count
    })
    .ToList();
```

---

## 🚢 **Deployment**

### Docker Deployment

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY bin/Release/net9.0/publish/ .
EXPOSE 80
ENTRYPOINT ["dotnet", "ArtGalleryAPI.dll"]
```

Build and run:
```bash
docker build -t artgalleryapi .
docker run -p 80:80 -e ASPNETCORE_CONNECTIONSTRINGS__DEFAULTCONNECTION="your_connection_string" artgalleryapi
```

### Azure Deployment

```bash
# Create resource group
az group create --name ArtGalleryRG --location eastus

# Create App Service plan
az appservice plan create --name ArtGalleryPlan --resource-group ArtGalleryRG --sku B1

# Create web app
az webapp create --resource-group ArtGalleryRG --plan ArtGalleryPlan --name artgalleryapi-prod

# Deploy
dotnet publish -c Release
```

---

## 🤝 **Contributing**

### How to Contribute

1. **Fork the repository** on GitHub
2. **Create a feature branch**:
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. **Commit changes** with clear messages:
   ```bash
   git commit -m "Add: feature description"
   ```
4. **Push to your fork**:
   ```bash
   git push origin feature/your-feature-name
   ```
5. **Create a Pull Request** with detailed description

### Code Standards

- Follow C# naming conventions (PascalCase for classes, camelCase for variables)
- Write XML documentation for public methods
- Ensure all tests pass before submitting PR
- Add tests for new features
- Keep commits atomic and focused
- Update README for significant changes

### Commit Message Format

```
[Type]: Brief description
- Detailed explanation if needed
- Additional context or related issues
```

**Types**: `Add`, `Fix`, `Refactor`, `Docs`, `Test`, `Performance`

---

## 📝 **License**

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

```
MIT License

Copyright (c) 2024 LM-Master-06

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software...
```

---

## 📞 **Support & Contact**

### Getting Help

- **Documentation**: See `/docs` folder for detailed guides
- **Issues**: Report bugs on [GitHub Issues](https://github.com/LM-Master-06/ArtGalleryAPI/issues)
- **Discussions**: Join [GitHub Discussions](https://github.com/LM-Master-06/ArtGalleryAPI/discussions)

### Project Links

- **GitHub Repository**: https://github.com/LM-Master-06/ArtGalleryAPI
- **Issue Tracker**: https://github.com/LM-Master-06/ArtGalleryAPI/issues
- **Project Wiki**: https://github.com/LM-Master-06/ArtGalleryAPI/wiki

---

## 🎯 **Roadmap**

### Current Version (v1.0)
- ✅ Core CRUD operations for artworks, artists, and categories
- ✅ JWT authentication and authorization
- ✅ Swagger/OpenAPI documentation
- ✅ Unit and integration tests
- ✅ PostgreSQL database integration

### Planned Features (v1.1)
- 🔄 Advanced search and filtering
- 🔄 Image upload support for artworks
- 🔄 Pagination and sorting
- 🔄 Audit logging
- 🔄 API rate limiting

### Future Enhancements (v2.0)
- 📋 Real-time notifications (WebSocket)
- 📋 Recommendation engine
- 📋 Analytics dashboard
- 📋 Multi-tenancy support
- 📋 GraphQL endpoint
- 📋 Caching layer (Redis)

---

## 📚 **Additional Resources**

### Official Documentation
- [Microsoft .NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [JWT.io](https://jwt.io)

### Learning Resources
- [C# Programming Guide](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [RESTful API Best Practices](https://restfulapi.net/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

## 🙏 **Acknowledgments**

- Built with ❤️ by **LM-Master-06**
- Inspired by clean architecture principles
- Special thanks to the .NET community
- Powered by modern web development standards

---

<div align="center">

**⭐ If you find this project helpful, please consider giving it a star! ⭐**

Made with ❤️ for the art community

© 2024 ArtGalleryAPI - All Rights Reserved

</div>
