---
name: modernize-dotnet-cloudrun
description: >-
  Modernizes a legacy .NET application to the current .NET Long-Term Support
  (LTS) release on Cloud Run with Cloud SQL for PostgreSQL. Use when asked to
  modernize, containerize, or migrate the .NET application in the current
  working directory to Cloud Run and PostgreSQL. Don't use for Java or other
  non-.NET workloads.
---

# .NET application modernization

<!--
  Disabling markdownlint MD029 to provide explicit ordering to avoid
  confusing the LLM.
-->
<!-- markdownlint-disable MD029 -->
<!--
    Disabling markdownlint MD030 to allow the list overflowing to numbers
    >=10.
-->
<!-- markdownlint-disable MD030 -->

## Role

Act as a Senior Cloud Architect and .NET Developer. Your goal is to:

- Port a legacy .NET 5 application to .NET 8 (Core).
- Refactor an existing .NET application into a containerized application
  suitable for Cloud Run.
- Refactor the application's data layer from SQL Server to PostgreSQL.
- You MUST NOT add, remove, or change business logic. Create a faithful, 1:1
  replication of features, API contracts, and UI behavior.
- You MUST explain every action you take.
- Output your implementation plan directly in the terminal response for review
  instead of creating an artifact file.

## Prerequisites

- The .NET source code for the application in the current working directory.
- Docker (`docker` and `docker compose`) installed and running on the host.
- The Chrome DevTools Model Context Protocol (MCP) server (`chrome-devtools`)
  configured for automated browser verification. If any prerequisite is missing
  or unavailable, stop and report the missing prerequisite to the user.
  Exception: if the `chrome-devtools` MCP tools are unavailable, or the browser
  fails to start when you reach step 8, don't stop. Complete the other steps,
  skip step 8, and report the skipped step and the exact error message in the
  `Browser test report` section of `MODERNIZATION.md`. You MUST NOT substitute
  HTTP requests for the browser test, and you MUST NOT report the browser test
  as performed if you didn't run the Chrome DevTools MCP tools.

## Instructions

### 1. Code refactoring (application layer)

Refactor the .NET logic into a production-ready web service.

- **.NET refactoring**:
    - Analyze the existing codebase to understand its structure and
      dependencies.
    - Convert the project SDK to .NET 8, convert `web.config` to
      `appsettings.json`, and handle the dependency injection (DI) changes in
      `Program.cs` and `Startup.cs`.
    - Convert the `.csproj` to the modern SDK-style format targeting .NET 8.
    - Refactor `Global.asax.cs` logic into the modern `Program.cs` middleware
      pipeline and DI container.
    - Entity Framework (EF) migration: Replace `System.Data.Entity` (EF6) with
      `Microsoft.EntityFrameworkCore`.
    - Use Entity Framework Core, and not Dapper.
    - PostgreSQL provider: Use `Npgsql.EntityFrameworkCore.PostgreSQL`.
    - Schema initialization:
        - Analyze existing `DropCreateDatabaseIfModelChanges` or
          `IDatabaseInitializer` classes.
        - Refactor these into a dedicated `DbInitializer` class that uses
          `context.Database.EnsureCreated()`, applies migrations at startup, or
          generates a SQL migration script.
        - Seeding: Refactor any `Seed` methods to work with the EF Core
          `ModelBuilder` or a custom seeding service that populates data when
          the database is empty.
    - Do not delete data access layer (DAL) files unless necessary. Refactor
      them to maintain class structure where possible to minimize logic drift.
    - To keep the structure of the application as close as possible to the
      original application, you MUST refactor existing source code files, rather
      than deleting and creating new ones, whenever possible.
    - You MUST NOT delete any needed functional component when migrating to EF
      Core. If you encounter any issue due to using the old Entity Framework,
      such as `System.Data.Entity` imports, you MUST refactor those classes
      instead of deleting them, if possible.

- **Cloud Run container contract compliance**
  (https://docs.cloud.google.com/run/docs/container-contract):
    - **Port binding:** Configure the application to listen on `0.0.0.0` on the
      port defined by the `PORT` environment variable, defaulting to `8080` if
      undefined.
    - **Logging:**
        - Remove any file-based logging, such as Log4Net file appenders.
        - Configure `ILogger` to write structured JSON logs to `Console`
          (`stdout`) for Cloud Logging ingestion.

- **Concurrency and safety**:
    - Identify any usage of `HttpRuntime.Cache` or static lists. Replace them
      with `IMemoryCache` and add comments warning about scalability issues in a
      stateless serverless environment.
    - Implement a signal handler for `SIGTERM` to allow graceful shutdown.

- **Running tools**:
    - You MUST NOT run `dotnet` tools on the host. Run them in a Docker
      container, if necessary.
    - Whenever you run a Docker container, add the `--rm` option to the
      `docker run` command.

### 2. Dockerfile generation

Write a `Dockerfile` following these best practices:

- **Base image:**
    - Multi-stage build: Use the official `mcr.microsoft.com/dotnet/sdk:8.0` for
      building and `mcr.microsoft.com/dotnet/aspnet:8.0` for the runtime.
- **Optimization:** Clean up caches and `apt-get` lists to minimize image size.
- **Build checks:** Ensure that the `Dockerfile` passes all Docker build checks.
  Read about Docker build checks
  (https://docs.docker.com/reference/build-checks/) and ensure that the
  `Dockerfile` passes all the checks. Follow the instructions to run Docker
  build checks without building the image
  (https://docs.docker.com/build/checks/#check-a-build-without-building) by
  running: `docker build --check .`

### 3. Docker Compose generation

Write a Docker Compose file named `compose.yaml` to test the refactored
application locally. Configure the compose file to:

- Run a PostgreSQL 16 instance for the application to use. Do not create the
  database, because the .NET application creates it.
- Run the .NET application.
- Omit the `version` attribute because newer Compose specifications deprecate
  it.
- Omit the `POSTGRES_DB` environment variable in the service definition that
  runs the PostgreSQL database container, so the .NET application triggers the
  database initialization.
- Include a healthcheck for the PostgreSQL container to check if the database is
  ready before starting the .NET application container. You MUST NOT depend on
  the `contosouniversity` database being present when you build the healthcheck.
  Use the `pg_isready` command to build the healthcheck.
- Pass the connection string to the app service using environment variables.

### 4. Documentation

Create a modernization report in the `MODERNIZATION.md` file with:

- **Modernization notes:** A summary of changes, specifically pointing out the
  `MockContext` implementation.
- **Local testing:** Specific `docker build`, `docker run`, and `docker compose`
  commands.
    - Ensure that all `docker run` commands have the `--rm` flag.
    - Add the `--detach` and `--build` options to the `docker compose up`
      command.
    - Explain how to run `docker compose logs` to get logs of the running
      application and database.
- **Endpoints:** A table containing all the HTTP endpoints that the application
  supports, and a test report table listing each tested endpoint, HTTP method,
  test command, and command output.
- **Browser test report:** The result of step 8. This section is mandatory,
  even when you skip the browser test. List the pages you visited, the CRUD
  operations you performed and their results, the resources that returned
  `404 Not Found`, and the JavaScript errors you found. If you skipped the
  browser test, write `Status: skipped` followed by the reason and the exact
  error message, if any.

### 5. Build the container image

Build the container image by running the `docker build` command from the
`MODERNIZATION.md` file.

Fix any error you encounter when building the container image.

### 6. Try running the .NET application container

Run the .NET application container by doing the following:

1.  Run the Docker Compose application: `docker compose up --build --detach`
2.  Get Docker Compose logs:
    1.  Run the `docker compose logs` command.
    2.  If the output of the `docker compose logs` command is the same as the
        last time you ran it, wait for 15 seconds.
    3.  Run the `docker compose logs` command again.
    4.  If the output of the `docker compose logs` command is the same as the
        last time you ran it, stop the Docker Compose application by running
        `docker compose stop`.

3.  If Docker Compose exits with an error, examine the error. Fix any error you
    encounter when running the application. If you see an error connecting to
    the `contosouniversity` database, but the application starts, ignore this
    error. .NET uses this check to determine whether to create the database.
4.  If you edit the `Dockerfile` or the .NET application source, rebuild the
    container image by running the `docker build` command from the
    `MODERNIZATION.md` file.
5.  After each run, whether it succeeds or fails, run the following commands:
    1.  Stop Docker Compose: `docker compose stop`
    2.  Remove containers: `docker compose rm --force`

Show the output of the commands you run.

### 7. Try sending HTTP requests

Test the HTTP endpoints of the .NET application:

1.  List all the HTTP endpoints, not just the `GET` endpoints, that the
    application supports and show the output.
2.  Run the `docker compose up --build --detach` command.
3.  Wait for the .NET application to initialize the database.
4.  You MUST test ALL the HTTP endpoints, including `GET`, `POST`, and `DELETE`,
    not just the `GET` endpoints. For each HTTP endpoint, do the following:
    1.  Send a properly crafted request. Load anti-forgery tokens and cookies if
        needed, and store them in text files.
    2.  Get Docker Compose logs by running the `docker compose logs` command.
    3.  If you encounter an error, stop the application and fix the error.
    4.  If you edit the `Dockerfile` or any file of the .NET application,
        rebuild the container image by running the `docker build` command from
        the `MODERNIZATION.md` file, and run the Docker Compose application
        again with the `docker compose up --build --detach` command.
    5.  Wait for the .NET application to initialize the database.
    6.  Test the endpoint again by sending a `GET` request.

When you finish testing, do the following:

1.  Stop Docker Compose: `docker compose stop`
2.  Remove containers: `docker compose rm --force`
3.  Update `MODERNIZATION.md` with the endpoint test results described in
    Section 4.
4.  Delete the files you created to store cookies and anti-forgery tokens.

Show the output of the commands you run.

### 8. Test the .NET application in a browser

Perform an automated UI smoke test using the Chrome DevTools MCP tools, such as
`new_page`, `click`, and `take_screenshot`, to validate the frontend. Start by
calling the `new_page` tool of the `chrome-devtools` MCP server with the
application root URL, such as `http://localhost:8080`. If the `chrome-devtools`
tools aren't available in your session, or the call fails, don't work around
it with HTTP requests: record `Status: skipped` and the error in the
`Browser test report` section of `MODERNIZATION.md`, then continue with the
cleanup.

The smoke test validates the following:

- Follow all links to views.
- Run create, read, update, and delete (CRUD) tests in views.
- Validate pagination and sorting functionality.
- Make sure there are no missing resources that result in 404 errors, such as
  CSS files, JavaScript files, images, and any other media file.

1.  **Startup:** Ensure the application is running using the
    `docker compose up --build --detach` command.
2.  **Asset integrity check:**
    1.  Navigate to the application root, such as `http://localhost:8080`.
    2.  Inspect the **Network** activity. Verify that all static resources,
        including CSS, JavaScript, and images, load with `200 OK` status codes.
    3.  **Critical:** If you detect `404 Not Found` for assets, fix the static
        file middleware configuration in `Program.cs` or adjust the `Dockerfile`
        copy paths.
3.  **Functional crawl:**
    1.  **Navigation:** Identify and follow links to all major views.
    2.  **Data grids:** If a table is present, test **Pagination** across next
        and previous pages and **Sorting** by clicking column headers to ensure
        EF Core translation is accurate.
    3.  **CRUD operations:** Perform a full CRUD cycle on a main entity, such as
        Students or Courses, to verify the PostgreSQL data layer.
4.  **Console health:** Check the browser console for any JavaScript errors and
    report or fix them.
5.  **Browser test report:** Add a `Browser test report` section to
    `MODERNIZATION.md` that lists the pages you visited, the CRUD operations you
    performed and their results, the resources that returned `404 Not Found`,
    and the JavaScript errors you found. If you couldn't run the browser test,
    state that you skipped it and include the exact error message.
6.  **Cleanup:** When you finish testing, do the following:
    1.  Stop Docker Compose: `docker compose stop`
    2.  Remove containers: `docker compose rm --force`

### 9. Completion checklist

End your final response with a checklist of steps 1 to 8 of these
instructions. For each step, write `done` or `skipped`, and the reason when
skipped. Don't mark step 8 as `done` unless you called the Chrome DevTools MCP
tools. The checklist and the `Browser test report` section of
`MODERNIZATION.md` MUST agree.
