# Part 2 — Data model + database

## Goal
Describe **Systems** and **Environments** as C# classes, and let **Entity Framework Core (EF Core)** turn them into database tables.

The app supports two databases with the same code:

| Where | Database | Setting |
|---|---|---|
| Local demo (laptop) | **SQLite**: a single file, `appregistry.db`, nothing to install | `appsettings.Development.json` |
| Azure (production) | **Azure SQL Database** | `appsettings.json` + App Service configuration |

## Prerequisites
- [Part 1](01-project-setup.md) complete

## Steps

### 1. Add the EF Core packages and tool
```bash
cd src/AppRegistry.Web
dotnet add package Microsoft.EntityFrameworkCore.SqlServer   # Azure SQL / SQL Server
dotnet add package Microsoft.EntityFrameworkCore.Sqlite      # local demo database
dotnet add package Microsoft.EntityFrameworkCore.Design      # needed by the dotnet-ef tool
dotnet tool install --global dotnet-ef
```

### 2. Create the models (`Models/`)
- **`EnvironmentType.cs`**: an `enum` with `Dev, Test, UAT, Staging, Production`.
- **`AppSystem.cs`**: `Name`, `Description`, `BusinessOwner`, `SystemAdmin`, `ItDeveloper`, `CreatedAt`, `UpdatedAt`, and a list of `Environments`.
- **`AppEnvironment.cs`**: `AppSystemId` (which system it belongs to), `Type`, `Label`, `Url`, `DeploymentLocation`, `Notes`.

Attributes such as `[Required]`, `[StringLength(200)]` and `[Url]` do two jobs. They set the column sizes in the database, and they become the form validation rules in later parts.

### 3. Create the database context (`Data/AppDbContext.cs`)
The `DbContext` is the bridge between C# and the database:
- `DbSet<AppSystem> Systems` creates the **Systems** table
- `DbSet<AppEnvironment> Environments` creates the **Environments** table
- `OnModelCreating` adds the rules:
  - **Unique index** on `Systems.Name` (no duplicate systems)
  - **One-to-many** relationship with **cascade delete** (deleting a system deletes its environments)
  - `Type` stored as text (`'Production'`), so it reads clearly in SQL queries
- `SaveChangesAsync` is overridden to fill in `CreatedAt` / `UpdatedAt` automatically

### 4. Configure which database to use
`appsettings.json` (production defaults, **no secrets**):
```json
"DatabaseProvider": "SqlServer",
"ConnectionStrings": { "AppRegistry": "" }
```
`appsettings.Development.json` (local demo):
```json
"DatabaseProvider": "Sqlite",
"ConnectionStrings": { "AppRegistry": "Data Source=appregistry.db" }
```

### 5. Register the database in `Program.cs`
`Program.cs` reads `DatabaseProvider` and calls either `UseSqlite(...)` or `UseSqlServer(...)`. On startup:
- **SQLite:** `EnsureCreated()` builds the tables straight from the models
- **Azure SQL:** `Migrate()` applies the migrations in `Migrations/`

### 6. Create the migration for Azure SQL
A **migration** is a C# file that describes how to create or change tables. It is generated from the models:
```bash
DatabaseProvider=SqlServer ConnectionStrings__AppRegistry="Server=design-time-only;Database=AppRegistry" \
  dotnet ef migrations add InitialCreate
```
(The placeholder connection string is never used to connect. It only tells EF which SQL dialect to generate. On Windows PowerShell, set the two variables with `$env:DatabaseProvider="SqlServer"` etc. first.)

The same schema is exported as plain SQL in [`docs/sql/create-schema.sql`](sql/create-schema.sql) so it can be reviewed or run by hand in **SSMS**:
```bash
dotnet ef migrations script --idempotent -o ../../docs/sql/create-schema.sql
```

### 7. Keep the local database out of Git
`.gitignore` includes `*.db`, so everyone's demo data stays on their own machine.

## How to verify
- [ ] `dotnet build` shows **0 Error(s)**
- [ ] `dotnet run`, then a file `appregistry.db` appears in `src/AppRegistry.Web/`
- [ ] It has both tables: `sqlite3 appregistry.db ".tables"` prints `Environments  Systems`. You can also open it with the **SQLite Viewer** extension in VS Code.
- [ ] `Migrations/` contains `..._InitialCreate.cs`

## What you learned / why
- **Code-first:** the C# classes are the source of truth, and EF Core generates the tables. Nobody edits table structure by hand, so the code and the database always match.
- **Two providers, one codebase:** EF Core hides the differences between SQLite and SQL Server. A setting chooses which one, so the demo runs anywhere and production uses Azure SQL.
- **Migrations** are a versioned history of the schema. When a field is added later, run `dotnet ef migrations add <Name>` (with the SqlServer variables) and the app applies it to Azure SQL on its next start.
- **Changed a model locally?** SQLite uses `EnsureCreated`, which does not update an existing file. Delete `appregistry.db` and run again. (This wipes your demo data.)

**Next:** [Part 3 — Systems pages](03-systems-pages.md)
