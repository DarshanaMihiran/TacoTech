# TacoTech User Sync Service

A **modular-monolithic, domain-driven .NET 9 service** for synchronizing users from a remote API into a local SQLite database.

Includes:

* Clean Architecture (DDD)
* MediatR CQRS
* EF Core 9 + SQLite
* SMTP Email Notifications
* Fully isolated Infrastructure Layer
* Enterprise-style unit tests (Package + Iterator pattern)

---

## Project Structure

```
src/
  TacoTech.UserSync.Domain/
    Users/
      User.cs
      UserId.cs
      Email.cs
      City.cs
      Interfaces/IUserRepository.cs

  TacoTech.UserSync.Application/
    Users/
      Commands/
        SyncUsersCommand.cs
        GetUsersQuery.cs
      Handlers/
        SyncUsersCommandHandler.cs
        GetUsersQueryHandler.cs
      DTOs/
        RemoteUserDto.cs
        LocalUserDto.cs
    Notifications/
      IEmailNotifier.cs

  TacoTech.UserSync.Infrastructure/
    Users/
      Persistence/
        DBContexts/
          UserSyncDbContext.cs
          UserSyncDbContextFactory.cs
        Repositories/
          UserRepository.cs
    Remote/
      RemoteUserClient.cs
    Email/
      SmtpEmailNotifier.cs

  TacoTech.UserSync.Api/
    Controllers/
      UserSyncController.cs
    Program.cs
    appsettings.json

tests/
  TacoTech.UserSync.Application.Tests/
    Users/
      SyncUsers/
        SyncUsersCommandHandlerTestSetup.cs
        SyncUsersCommandHandlerTestPackage.cs
        SyncUsersCommandHandlerTestData.cs
        SyncUsersCommandHandlerTests.cs
      UserComparisonTests.cs
```

---

## Architecture Overview (DDD + Modular Monolith)

| Layer              | Responsibility                                                 |
| ------------------ | -------------------------------------------------------------- |
| **Domain**         | Entities, value objects, business rules, repository interfaces |
| **Application**    | MediatR commands/queries, sync logic, DTOs                     |
| **Infrastructure** | EF Core, SQLite, repositories, remote client, email            |
| **API**            | Controllers, DI configuration, hosting                         |

---

## Features

### User Synchronization (Remote → Local)

* Creates new users
* Updates modified users
* Skips identical users
* Counts per-user errors
* Continues even if some fail

### Email Notification

* Sent after sync completes
* SMTP (Gmail recommended)
* Email failures **never break sync**

### GET Endpoint for Local Users

```
GET /api/usersync/users
```

### SQLite Persistence (EF Core 9)

Database stored at:

```
src/TacoTech.UserSync.Api/users.db
```

---

## API Endpoints

### Sync Remote Users

```
POST /api/usersync/users
```

Response:

```json
{
  "usersCreated": 3,
  "usersUpdated": 1,
  "usersSkipped": 5,
  "errors": 0
}
```

### Retrieve Local Users

```
GET /api/usersync/users
```

---

## SQLite + EF Core Setup

### Install EF Tools

```bash
dotnet tool update -g dotnet-ef
```

### Add Initial Migration

```bash
dotnet ef migrations add InitialCreate \
  -p src/TacoTech.UserSync.Infrastructure \
  -s src/TacoTech.UserSync.Api \
  -o Users/Persistence/Migrations
```

### Update Database

```bash
dotnet ef database update \
  -p src/TacoTech.UserSync.Infrastructure \
  -s src/TacoTech.UserSync.Api
```

SQLite file appears under:

```
src/TacoTech.UserSync.Api/users.db
```

---

## Email (Gmail SMTP) Setup

### Step 1: Enable 2FA

[https://myaccount.google.com/security](https://myaccount.google.com/security)

### Step 2: Create App Password

[https://myaccount.google.com/apppasswords](https://myaccount.google.com/apppasswords)

Choose:

```
App: Mail
Device: Other (UserSyncApp)
```

Google gives a **16-character password**.

### Step 3: Add to `appsettings.json`

```json
"Email": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "Username": "your@gmail.com",
  "Password": "your-app-password",
  "From": "your@gmail.com",
  "To": "recipient@example.com",
  "EnableSsl": true
}
```

---

## Unit Testing (Package / Iterator Pattern)

Test location:

```
tests/TacoTech.UserSync.Application.Tests/
```

### Covered Scenarios

* Create new user
* Update when data differs
* Skip identical user
* Per-user exception increments error count
* Email failure does **not** break sync
* Comparison logic correctness

### Structure (Magnet-style)

* `TestSetup` → mocks + handler creation
* `TestPackage` → scenario definition
* `TestData` → iterator of packages (xUnit ClassData)
* `Tests` → single test running all scenarios

### Running tests

```bash
dotnet test
```

---

## Running the API

```bash
cd src/TacoTech.UserSync.Api
dotnet run
```

Default URLs:

```
https://localhost:7001
http://localhost:5001
```

---

## 📝 Logging

### Console output appears automatically when running:

```bash
dotnet run
```

### Enable Serilog file logging

Install:

```
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
```

---
