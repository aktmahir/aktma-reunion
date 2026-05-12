# aktma-reunion

This workspace now includes a C# ASP.NET Core MVC application called `SchoolSocialApp`.

## Project Features

- User authentication with ASP.NET Core Identity
- Private class groups where classmates interact only with users from the same class
- Class-specific privacy settings and posting controls
- Admin dashboard for managing classes and class settings

## Running the project

1. Open `SchoolSocialApp` in Visual Studio or VS Code.
2. Restore NuGet packages.
3. Run the application.

Default seed accounts:

- Admin: `admin@schoolapp.local` / `Admin123!`
- Student: `student@schoolapp.local` / `Student123!`

## Notes

The app uses SQLite storage in `schoolsocialapp.db` by default.
