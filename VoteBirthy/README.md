# VoteBirthy - Birthday Gift Voting Application

VoteBirthy is an ASP.NET Core MVC application that allows employees to create and participate in votes for birthday gifts for their colleagues.

## Features

- User registration and authentication
- Create birthday gift votes for colleagues
- Vote for preferred gift options
- View active and completed votes
- Manage the gift catalog
- Track voting results and winners

## Prerequisites

- .NET 8.0 SDK or higher
- MySQL Server (for production) or SQLite (for development)

## Getting Started

### Development Setup (with SQLite)

1. Clone the repository
2. Navigate to the project directory
3. Run the application:

```
dotnet run
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`.

### Production Setup (with MySQL)

1. Update the connection string in `appsettings.json` with your MySQL server details:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=your_server;Database=VoteBirthy;User=your_user;Password=your_password;Port=3306;"
}
```

2. Run database migrations:

```
dotnet ef database update
```

3. Build and run the application:

```
dotnet publish -c Release
dotnet ./bin/Release/net8.0/VoteBirthy.dll
```

## Default Accounts

The application is seeded with the following test accounts:

- Username: `john.doe`, Password: `Password123!`
- Username: `jane.smith`, Password: `Password123!`
- Username: `bob.johnson`, Password: `Password123!`

## Architecture

- ASP.NET Core 8.0 MVC
- Entity Framework Core with MySQL/SQLite
- Cookie-based Authentication
- Razor Views

## License

This project is licensed under the MIT License. 