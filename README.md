# Pinger

Pinger is a collection of real-time session tracker endpoints built with **SignalR**, enabling clients to connect and receive live updates about session state over WebSockets.

## Tech Stack

- .NET 10 / ASP.NET Core
- SignalR
- Entity Framework Core

## Project Structure

| Project | Description |
|---|---|
| `Pinger.Api` | Host/startup project, exposes the SignalR hubs |
| `Pinger.Application` | Utilities, functions, DTOs, and other shared application logic |
| `Pinger.Infrastructure` | Data access layer, EF Core migrations |

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- EF Core CLI tools

Install the EF Core tools globally:

```bash
dotnet tool install --global dotnet-ef
```

## Getting Started

### 1. Create the initial migration

```bash
dotnet ef migrations add InitialCreate --project Pinger.Infrastructure --startup-project Pinger.Api -o Persistence/Migrations
```

### 2. Apply migrations to the database

```bash
dotnet ef database update --project Pinger.Infrastructure --startup-project Pinger.Api
```

## Managing Migrations

### Remove the last migration

```bash
dotnet ef migrations remove --project Pinger.Infrastructure --startup-project Pinger.Api
```

## License

This project is licensed under the [MIT License](LICENSE).