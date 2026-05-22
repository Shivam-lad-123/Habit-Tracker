# Habit Tracker

A comprehensive ASP.NET Core REST API for tracking daily and weekly habits with streak management, progress monitoring, and archival capabilities.

## Overview

Habit Tracker is a backend API designed to help users build and maintain habits. It provides a complete habit management system with features including:

- **Habit Management**: Create, read, update, and delete habits
- **Habit Completion Tracking**: Mark habits as complete and automatically track streaks
- **Streak Management**: Track current and longest streaks for each habit
- **Progress Summary**: Get quick insights on total habits, today's completions, and best streaks
- **Due Today**: View habits that need attention today
- **Habit Archival**: Archive and unarchive completed or abandoned habits
- **Habit Frequencies**: Support for both daily and weekly habits with optional target goals

## Tech Stack

- **Framework**: ASP.NET Core 10.0
- **Database**: SQL Server with Entity Framework Core
- **Validation**: FluentValidation
- **API Documentation**: OpenAPI/Swagger with Scalar UI
- **Language**: C# 13 with nullable reference types and implicit usings

## Prerequisites

Before you begin, ensure you have the following installed:

- **.NET 10.0 SDK** ([Download](https://dotnet.microsoft.com/download/dotnet/10.0))
- **SQL Server** (Local instance or Docker)
- **Visual Studio 2022** (optional, recommended for development)

### SQL Server Setup

You can set up SQL Server locally using one of these methods:

#### Option 1: SQL Server 2022 Local Installation
Download and install [SQL Server 2022 Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads).

#### Option 2: Docker (Recommended)
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Simform@123" \
  -p 1433:1433 \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/Shivam-lad-123/Habit-Tracker.git
cd Habit-Tracker
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Configure the Database Connection

Edit `appsettings.json` and update the `DefaultConnection` string if needed:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=HabitTracker;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Common connection string examples:**

- **Local SQL Server (Windows Authentication)**:
  ```
  Server=.;Database=HabitTracker;Trusted_Connection=True;TrustServerCertificate=True;
  ```

- **Docker SQL Server**:
  ```
  Server=host.docker.internal;Database=HabitTracker;User ID=sa;Password=Simform@123;Encrypt=True;TrustServerCertificate=True;
  ```

- **Named Instance**:
  ```
  Server=COMPUTERNAME\SQLEXPRESS;Database=HabitTracker;Trusted_Connection=True;TrustServerCertificate=True;
  ```

### 4. Run Database Migrations

Migrations are automatically applied on application startup, but you can also run them manually:

```bash
dotnet ef database update
```

### 5. Build the Project

```bash
dotnet build
```

### 6. Run the Application

```bash
dotnet run
```

The API will be available at `https://localhost:5001` (or `http://localhost:5000`).

## API Documentation

Once the application is running, access the interactive API documentation:

- **Scalar UI** (Modern, recommended): `https://localhost:5001/scalar/v1`
- **Swagger UI**: `https://localhost:5001/swagger`

### API Endpoints

All endpoints are prefixed with `/habits`.

#### Get All Habits
```
GET /habits?includeArchived=false
```
- Query parameter `includeArchived` (optional, default: false): Include archived habits in results

#### Get Habit Summary
```
GET /habits/summary
```
Returns: Total habits, completed today, and current longest streak

#### Get Habits Due Today
```
GET /habits/due-today
```
Returns habits that are due for completion today

#### Get Habit by ID
```
GET /habits/{id}
```

#### Create a New Habit
```
POST /habits
Content-Type: application/json

{
  "name": "Morning Run",
  "description": "30-minute morning jog",
  "frequency": 0,
  "targetDays": null
}
```

**Frequency values:**
- `0` = Daily
- `1` = Weekly

#### Update a Habit
```
PUT /habits/{id}
Content-Type: application/json

{
  "name": "Morning Run",
  "description": "Updated description",
  "frequency": 0,
  "targetDays": null
}
```

#### Delete a Habit
```
DELETE /habits/{id}
```

#### Mark Habit as Complete
```
PATCH /habits/{id}/complete
```
Automatically updates the current streak and last completed date

#### Archive a Habit
```
PATCH /habits/{id}/archive
```
Archives a habit (removes it from default list view)

#### Unarchive a Habit
```
PATCH /habits/{id}/unarchive
```
Restores an archived habit to active view

### Example Request/Response

**Create Habit Request:**
```json
{
  "name": "Read 30 minutes",
  "description": "Daily reading before bed",
  "frequency": 0,
  "targetDays": null
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "name": "Read 30 minutes",
  "description": "Daily reading before bed",
  "frequency": 0,
  "targetDays": null,
  "currentStreak": 0,
  "longestStreak": 0,
  "lastCompletedDate": null,
  "isArchived": false,
  "createdAt": "2026-05-22T05:46:16"
}
```

## Error Handling

The API returns standard HTTP status codes with detailed error messages:

- **200 OK**: Successful request
- **201 Created**: Resource successfully created
- **204 No Content**: Resource successfully deleted or action completed
- **400 Bad Request**: Validation failed (includes field errors)
- **404 Not Found**: Resource not found
- **409 Conflict**: Business logic constraint violated (e.g., invalid state transition)
- **500 Internal Server Error**: Unexpected server error

### Example Error Response:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["'Name' must not be empty."],
    "Frequency": ["'Frequency' must be a valid enum value."]
  }
}
```

## Project Structure

```
Habit-Tracker/
├── DTOs/                      # Data Transfer Objects
│   ├── CreateHabitRequest.cs
│   ├── UpdateHabitRequest.cs
│   ├── HabitResponse.cs
│   └── HabitSummaryResponse.cs
├── Models/                    # Domain Models
│   ├── Habit.cs
│   └── HabitFrequency.cs
├── Services/                  # Business Logic
│   ├── IHabitService.cs
│   ├── HabitService.cs
│   ├── IStreakService.cs
│   └── StreakService.cs
├── Endpoints/                 # API Endpoint Mapping
│   └── HabitEndpoints.cs
├── Data/                      # Data Access Layer
│   ├── HabitTrackerDbContext.cs
│   ├── HabitSeeder.cs
│   └── Migrations/
├── Validators/                # FluentValidation Rules
│   ├── CreateHabitRequestValidator.cs
│   └── UpdateHabitRequestValidator.cs
├── appsettings.json          # Configuration
├── Program.cs                # Application Setup & Dependency Injection
└── HabitTracker.csproj       # Project File
```

## Database Schema

### Habits Table

| Column | Type | Description |
|--------|------|-------------|
| Id | INT (PK) | Primary key |
| Name | NVARCHAR(MAX) | Habit name |
| Description | NVARCHAR(MAX) | Optional habit description |
| Frequency | INT | 0 = Daily, 1 = Weekly |
| TargetDays | INT (nullable) | Optional target days for the habit |
| CurrentStreak | INT | Current consecutive streak count |
| LongestStreak | INT | Longest streak achieved |
| LastCompletedDate | DATE (nullable) | Last date habit was marked complete |
| IsArchived | BIT | Archive status |
| CreatedAt | DATETIME2 | Creation timestamp |

## Development

### Building

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Running with Watch Mode

For development with automatic recompilation:

```bash
dotnet watch run
```

### Code Style

This project follows standard C# conventions:
- PascalCase for public members
- camelCase for private members and parameters
- Nullable reference types enabled
- Implicit usings enabled for cleaner code

## Configuration

### Logging

Configure logging levels in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Database Connection String

The application supports multiple database configurations. Update `appsettings.json` or use environment variables:

```bash
export ConnectionStrings__DefaultConnection="your-connection-string"
```

## Contributing

We welcome contributions! Here's how to get started:

### 1. Create a Feature Branch
```bash
git checkout -b feature/your-feature-name
```

### 2. Make Your Changes
- Follow the existing code style and conventions
- Ensure your changes don't break existing functionality
- Add appropriate validation or error handling
- Update documentation if needed

### 3. Commit Your Changes
```bash
git commit -m "feat: add your feature description"
```

### 4. Push to Your Branch
```bash
git push origin feature/your-feature-name
```

### 5. Create a Pull Request
- Provide a clear description of your changes
- Reference any related issues
- Ensure all tests pass

## Common Issues

### Connection String Issues
- Ensure SQL Server is running and accessible
- Verify the server name in the connection string
- Check credentials (if using SQL authentication)

### Migration Issues
- Delete the database and re-run migrations
- Ensure the user has permission to create databases
- Check Entity Framework Core is properly configured

### Port Already in Use
- Change the port in `Properties/launchSettings.json`
- Or use: `dotnet run -- --urls "https://localhost:5002"`

## License

[Add appropriate license information here]

## Support

For issues, questions, or suggestions, please [open an issue](https://github.com/Shivam-lad-123/Habit-Tracker/issues) on the repository.