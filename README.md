# ArtGalleryAPI

A RESTful Web API designed to manage art gallery operations including artwork cataloguing, artist profiles, exhibition management, and category organisation. Built using modern .NET practices with a focus on clean architecture, testability, and scalability.

# 🎨 Art Gallery Management API

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com) [![C#](https://img.shields.io/badge/Language-C%23-239120?logo=csharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/) [![EF Core](https://img.shields.io/badge/ORM-EF%20Core-512BD4?logo=dotnet&logoColor=white)](https://docs.microsoft.com/en-us/ef/core/) [![Swagger](https://img.shields.io/badge/API-Swagger-85EA2D?logo=swagger&logoColor=black)](https://swagger.io) [![xUnit](https://img.shields.io/badge/Tests-xUnit-5B2D8E)](https://xunit.net) [![Status](https://img.shields.io/badge/Status-Active-2ECC71)](https://github.com/LM-Master-06/ArtGalleryAPI) [![License](https://img.shields.io/badge/License-MIT-F1C40F)](LICENSE)

A **clean-architecture Art Gallery Management API** built using ASP.NET Core Web API and C#.  
This application is designed to manage gallery resources — artworks, artists, exhibitions, and categories — with full CRUD support and an interactive Swagger interface.

---

# 🚀 Features

## 🖼️ Artwork Management

- Add / Update / Delete artworks
- Catalogue entries with title, medium, year, and description
- Link artworks to artists and categories
- Filter and retrieve by various attributes

## 👤 Artist Profiles

- Register and manage artist records
- Store biography, nationality, and date of birth
- Associate multiple artworks per artist

## 🏛️ Exhibition Management

- Create and manage gallery exhibitions
- Assign artworks to exhibitions
- Track exhibition dates and descriptions

## 🗂️ Category Organisation

- Organise artworks into named categories (e.g. Painting, Sculpture, Photography)
- Add / Update / Delete categories
- Filter artworks by category

## ✅ Testing

- Dedicated test project (`ArtGalleryAPI.Tests`)
- Unit and integration tests for controllers and services
- Ensures API correctness across all resource endpoints

## ⚡ Performance & Scalability

- EF Core optimised queries
- Layered architecture (Controller → Service → Data)
- DTO-based data transfer
- Clean separation of concerns

---

# 🏗️ Tech Stack

## Backend

- ASP.NET Core Web API (.NET 8)
- C#
- Entity Framework Core
- SQLite / SQL Server
- Swagger (OpenAPI)

## Testing

- xUnit / MSTest
- In-memory database for test isolation

## Tooling

- Visual Studio 2022
- dotnet CLI

---

# 🧱 Architecture

```
Client (Swagger / HTTP)
↓
Controllers (API Layer)
↓
Services (Business Logic)
↓
Repository / DbContext (EF Core)
↓
Database (SQLite / SQL Server)
```

---

# 📂 Project Structure

```
ArtGalleryAPI/
├── ArtGalleryAPI/                  # Main Web API project
│   ├── Controllers/                # Route handlers (Artworks, Artists, etc.)
│   ├── Models/                     # Domain models
│   ├── DTOs/                       # Data transfer objects
│   ├── Data/                       # DbContext & EF Core config
│   ├── Services/                   # Business logic layer
│   └── Program.cs                  # App entry point & service registration
├── ArtGalleryAPI.Tests/            # Unit & integration test project
│   └── ArtGalleryAPI.Tests.csproj
└── NewBackend.sln                  # Visual Studio 2022 solution file
```

---

# ⚙️ Installation & Setup

## 🔧 Backend

```bash
git clone https://github.com/LM-Master-06/ArtGalleryAPI.git
cd ArtGalleryAPI
dotnet restore NewBackend.sln
dotnet ef database update --project ArtGalleryAPI
dotnet run --project ArtGalleryAPI
```

Runs on: http://localhost:5000  
Swagger UI: http://localhost:5000/swagger

---

## 🧪 Run Tests

```bash
dotnet test NewBackend.sln
```

With verbose output:

```bash
dotnet test NewBackend.sln --logger "console;verbosity=detailed"
```

---

# 📡 API Documentation

Swagger UI: http://localhost:5000/swagger

---

## 📌 Endpoints

### Artworks

```
GET    /api/artworks
GET    /api/artworks/{id}
POST   /api/artworks
PUT    /api/artworks/{id}
DELETE /api/artworks/{id}
```

### Artists

```
GET    /api/artists
GET    /api/artists/{id}
POST   /api/artists
PUT    /api/artists/{id}
DELETE /api/artists/{id}
```

### Exhibitions

```
GET    /api/exhibitions
GET    /api/exhibitions/{id}
POST   /api/exhibitions
PUT    /api/exhibitions/{id}
DELETE /api/exhibitions/{id}
```

### Categories

```
GET    /api/categories
GET    /api/categories/{id}
POST   /api/categories
PUT    /api/categories/{id}
DELETE /api/categories/{id}
```

---

# 🔒 Security

- Input validation via Data Annotations
- ORM-based SQL injection protection (EF Core)
- DTO pattern prevents direct model exposure
- Separation of concerns limits attack surface

---

# 📌 Future Enhancements

- JWT Authentication & Role-based Access Control
- Image upload support for artwork photos
- Docker containerisation & CI/CD pipeline
- Pagination and advanced filtering
- Frontend client (React.js)

---

# 🤝 Contribution

Fork → Create Branch → Commit → Push → PR

---

# 👨‍💻 Author

LM-Master-06
