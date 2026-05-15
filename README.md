# aktma-reunion

`aktma-reunion` contains a C# ASP.NET Core MVC application named `SchoolSocialApp`.
The application is designed for school communities and enables secure, class-based social interactions with a focus on privacy and administrative control.

## Project Overview

`SchoolSocialApp` includes:
- ASP.NET Core MVC web application structure
- ASP.NET Core Identity for user authentication and authorization
- Class-based membership, where users belong to a school class and see only class-specific content
- Admin tools for managing classes, class settings, and general application configuration
- A simple post system for classroom announcements and discussion

## Key Features

- User registration and login via Identity
- Class groups to restrict access to posts and content by class membership
- Admin pages to manage classes and school settings
- Seed data for instant local development and testing
- SQLite persistence for easy local setup

## Repository Structure

- `SchoolSocialApp/Program.cs` - application startup and middleware configuration
- `SchoolSocialApp/appsettings.json` - app configuration and database settings
- `SchoolSocialApp/Controllers/` - MVC controllers for home, classes, and admin flows
- `SchoolSocialApp/Data/` - data context and seed data classes
- `SchoolSocialApp/Models/` - domain models and view models
- `SchoolSocialApp/Views/` - Razor pages for UI rendering
- `SchoolSocialApp/wwwroot/` - static assets, including CSS and JS

## Getting Started

### Prerequisites

- .NET 8 SDK (or the version targeted by the project)
- Visual Studio, Visual Studio Code, or another compatible editor

### Run Locally

1. Open the repository in your editor.
2. Open the `SchoolSocialApp` folder or the `.csproj` file.
3. Restore NuGet packages.
4. Build the solution.
5. Run the project.

Alternatively, from a terminal inside `SchoolSocialApp`:

```powershell
cd SchoolSocialApp
dotnet restore
dotnet build
dotnet run
```

### Seed Accounts

The application includes seeded accounts for local testing:

- Admin: `admin@schoolapp.local` / `Admin123!`
- Student: `student@schoolapp.local` / `Student123!`

## Database

The default local database is SQLite. The file is created automatically as `schoolsocialapp.db` when the app runs.

## Notes

- The application is intended for local development and school community demonstrations.
- Adjust class and identity settings in `appsettings.json` and the seed data patterns as needed.
