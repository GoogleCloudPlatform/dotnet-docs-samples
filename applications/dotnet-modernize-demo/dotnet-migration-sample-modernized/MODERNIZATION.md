# .NET Application Modernization Report: ContosoUniversity

## Modernization Notes

### 1. Target Framework & Project System Modernization

- **Port to .NET 8 (LTS):** Migrated `ContosoUniversity.csproj` from legacy
  `net5.0` to `net8.0`.
- **SDK & Package Cleanup:**
    - Retained the modern `Microsoft.NET.Sdk.Web` SDK project format.
    - Replaced legacy Entity Framework 6 (`EntityFramework 6.4.4`) with
      `Microsoft.EntityFrameworkCore` (`8.0.11`).
    - Added PostgreSQL provider `Npgsql.EntityFrameworkCore.PostgreSQL`
      (`8.0.11`).
    - Updated `Microsoft.AspNetCore.Mvc.NewtonsoftJson` to `8.0.11`.
    - Retained `BuildBundlerMinifier` (`3.2.449`) to compile and minify CSS
      bundles during `dotnet publish`.
    - Removed deprecated analyzers and obsolete packages
      (`System.ComponentModel.Annotations`,
      `System.Configuration.ConfigurationManager`,
      `Google.Cloud.Diagnostics.AspNetCore`, `Grpc.AspNetCore`).
    - Added
      `<EnableSourceControlManagerQueries>false</EnableSourceControlManagerQueries>`
      to ensure reproducible builds across varied container and git reftable
      environments.

### 2. Data Layer Migration (SQL Server to PostgreSQL)

- **SchoolContext Refactoring:**
    - Migrated `ContosoUniversity.DAL.SchoolContext` from
      `System.Data.Entity.DbContext` to
      `Microsoft.EntityFrameworkCore.DbContext`.
    - Configured DI options via `DbContextOptions<SchoolContext>`.
    - Mapped entity table name explicitly in `OnModelCreating`
      (`modelBuilder.Entity<Student>().ToTable("Student")`).
- **Database Initialization & Seeding:**
    - Refactored `SchoolInitializer` into `DbInitializer` implementing
      `context.Database.EnsureCreated()` and idempotent seed data population.
    - Retained `SchoolInitializer` as an alias wrapper for backward
      compatibility.
    - At startup, the application creates the database and populates seed
      students if the table is empty.
- **PostgreSQL Case Sensitivity & Raw SQL:**
    - PostgreSQL folds unquoted SQL identifiers to lowercase, whereas EF Core
      creates quoted table and column identifiers.
    - Refactored the raw SQL query in `StudentController.GetEnrollmentSummary`
      from unquoted SQL to quoted identifiers:

        ```sql
        SELECT "EnrollmentDate", COUNT(*) AS "StudentCount"
        FROM "Student" GROUP BY "EnrollmentDate"
        ```

    - Switched from EF6's `Database.SqlQuery<T>` to EF Core 8's
      `Database.SqlQueryRaw<T>`.

- **Timestamp Compatibility:**
    - Enabled
      `AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true)` in
      `Program.cs` to allow seamless persistence of `DateTime` values (with
      `Kind=Unspecified` produced by HTML form bindings) into PostgreSQL
      timestamp columns.
- **Exception Handling:**
    - In `StudentController.Create`, replaced EF6 `RetryLimitExceededException`
      with EF Core's `DbUpdateException`.

### 3. Google Cloud Run Container Contract Compliance

- **Port Binding:**
    - Dynamically binds to `http://0.0.0.0:${PORT:-8080}` as required by Cloud
      Run.
    - Configured both `ASPNETCORE_HTTP_PORTS` and `PORT` environment variables
      in the container image.
- **Structured JSON Logging:**
    - Removed all legacy or file-based logging configurations.
    - Configured `ILogger` with `logging.AddJsonConsole()` to output single-line
      structured JSON logs to stdout/console for native Google Cloud Logging
      ingestion.
- **Graceful Shutdown & Signal Handling:**
    - Registered POSIX signal handler for `SIGTERM` using
      `PosixSignalRegistration.Create(PosixSignal.SIGTERM, ...)`.
    - Registered lifecycle listener on
      `IHostApplicationLifetime.ApplicationStopping` for clean shutdown.
- **Concurrency & In-Memory State Warning:**
    - Registered `IMemoryCache` in `Startup.cs` with architectural documentation
      warning that in-memory cache is local to each container instance and does
      not persist across Cloud Run horizontal auto-scaling or scale-to-zero
      events. Cloud Memorystore (Redis) is recommended for distributed caching.

### 4. Containerization & Dockerfile Best Practices

- **Multi-Stage Build:**
    - Build stage: `mcr.microsoft.com/dotnet/sdk:8.0`.
    - Runtime stage: `mcr.microsoft.com/dotnet/aspnet:8.0`.
- **Build Checks:** Verified against Docker Build Checks with
  `docker build --check .` (0 errors, 0 warnings).
- **Static Asset Serving:** Bundler generates `wwwroot/css/styles.css` during
  publish, and `favicon.ico` is placed in `wwwroot`, serving all static assets
  with `200 OK`.

---

## Local Testing

### Container Build

Build the container image using Docker:

```bash
docker build -t contosouniversity:latest .
```

To run Docker build checks without building:

```bash
docker build --check .
```

### Direct Container Run

Run the container directly (ensure an accessible PostgreSQL instance is
running):

```bash
docker run --rm \
  -p 8080:8080 \
  -e PORT=8080 \
  -e ConnectionStrings__SchoolContext="Host=host.docker.internal;Port=5432;Database=contosouniversity;Username=postgres;Password=postgres" \
  contosouniversity:latest
```

_(Note: all `docker run` commands use the `--rm` flag to remove the container
upon exit)._

### Docker Compose

Start the application and PostgreSQL database with build and detach options:

```bash
docker compose up --build --detach
```

Wait for the application to become ready:

```bash
for i in $(seq 60); do curl -sf -o /dev/null http://localhost:8080/ && break; sleep 2; done; docker compose logs --tail 50
```

### Viewing Logs

View aggregate or service-specific logs:

```bash
# View combined logs
docker compose logs

# Tail last 50 lines of logs
docker compose logs --tail 50

# Follow live logs in real time
docker compose logs -f

# View application logs only
docker compose logs web

# View database logs only
docker compose logs db
```

### Stopping and Cleaning Up

```bash
# Stop running containers
docker compose stop

# Remove stopped containers
docker compose rm --force
```

---

## Endpoints

The application exposes the following HTTP endpoints:

| Endpoint          | HTTP Method | Description                                                                                                                                               |
| ----------------- | ----------- | --------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `/`               | GET         | Default application route (maps to `StudentController.Index`). Displays the student roster table, student creation form, and enrollment date summary.     |
| `/Student`        | GET         | Student controller root view (maps to `StudentController.Index`).                                                                                         |
| `/Student/Index`  | GET         | Explicit student index view.                                                                                                                              |
| `/Student/Create` | POST        | Handles new student creation (`LastName`, `FirstMidName`, `EnrollmentDate`). Protected with ASP.NET Core Anti-Forgery Token. Redirects to `/` on success. |
| `/css/styles.css` | GET         | Bundled and minified CSS stylesheet containing Bootstrap and application styles.                                                                          |
| `/favicon.ico`    | GET         | Application favicon.                                                                                                                                      |

---

## Tested Endpoints Report

I tested all supported HTTP endpoints and main user flows against the running
Docker Compose environment using `curl`.

| Endpoint                                      | HTTP Method | Command Used to Test                                                                                                                                                                                                                                                                               | Command Output / Verification Result                                                                                                                                                           |
| --------------------------------------------- | ----------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `/`                                           | GET         | `curl -s -i http://localhost:8080/`                                                                                                                                                                                                                                                                | `HTTP/1.1 200 OK`; `Content-Type: text/html; charset=utf-8`; HTML rendered containing student table, creation form, and enrollment date summary.                                               |
| `/Student`                                    | GET         | `curl -s -i http://localhost:8080/Student`                                                                                                                                                                                                                                                         | `HTTP/1.1 200 OK`; `Content-Type: text/html; charset=utf-8`; HTML rendered with anti-forgery token and initial seed data (8 students).                                                         |
| `/Student/Index`                              | GET         | `curl -s -i http://localhost:8080/Student/Index`                                                                                                                                                                                                                                                   | `HTTP/1.1 200 OK`; `Content-Type: text/html; charset=utf-8`; Full student index view matching default route.                                                                                   |
| `/css/styles.css`                             | GET         | `curl -s -i http://localhost:8080/css/styles.css`                                                                                                                                                                                                                                                  | `HTTP/1.1 200 OK`; `Content-Type: text/css`; `Content-Length: 122329`; CSS stylesheet successfully served.                                                                                     |
| `/favicon.ico`                                | GET         | `curl -s -i http://localhost:8080/favicon.ico`                                                                                                                                                                                                                                                     | `HTTP/1.1 200 OK`; `Content-Type: image/x-icon`; `Content-Length: 32038`; Favicon successfully served.                                                                                         |
| `/Student/Create`                             | POST        | `curl -s -i -c cookies.txt -b cookies.txt --data-urlencode "__RequestVerificationToken=$TOKEN" --data-urlencode "NewStudent.LastName=Curie" --data-urlencode "NewStudent.FirstMidName=Marie" --data-urlencode "NewStudent.EnrollmentDate=2024-09-01" -X POST http://localhost:8080/Student/Create` | `HTTP/1.1 302 Found`; `Location: /`; Form submission processed successfully with anti-forgery validation and redirected to `/`.                                                                |
| `/Student/Index` (Post-Creation Verification) | GET         | `curl -s -b cookies.txt http://localhost:8080/Student/Index`                                                                                                                                                                                                                                       | `HTTP/1.1 200 OK`; Verified that new student **"Curie, Marie"** (Enrollment Date: `2024-09-01`) is persisted in PostgreSQL and rendered in both the student table and enrollment date summary. |
