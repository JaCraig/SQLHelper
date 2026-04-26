# AGENTS

This file helps coding agents become productive quickly in this repository.

## Scope

- Repository: SQLHelper
- Main solution: [SQLHelper.sln](SQLHelper.sln)
- Primary package project: [src/SQLHelper.DB/SQLHelper.DB.csproj](src/SQLHelper.DB/SQLHelper.DB.csproj)

## Canonical Commands

Run from repository root.

- Restore: `dotnet restore SQLHelper.sln`
- Build (Debug): `dotnet build SQLHelper.sln --no-restore --configuration Debug`
- Build (Release): `dotnet build SQLHelper.sln --no-restore --configuration Release`
- Test: `dotnet test SQLHelper.sln --no-build --verbosity normal`
- Update dependencies task (VS Code task): `.Net: Update Dependencies`

Reference CI command flow in:

- [.github/workflows/dotnet-test.yml](.github/workflows/dotnet-test.yml)
- [.github/workflows/dotnet-publish.yml](.github/workflows/dotnet-publish.yml)

## Local Test Prerequisites

Tests require SQL Server access.

- Default local connection is localhost with integrated security.
- Optional env vars used by tests and CI:
    - `SQLHELPER_SQL_PASSWORD`
    - `SQLHELPER_SQL_SERVER` (defaults to `127.0.0.1,1433` when password is provided)
- Test databases expected by CI/local integration scenarios:
    - `TestDatabase`
    - `TestDatabase2`
    - `MockDatabase`
    - `MockDatabaseForMockMapping`

References:

- [test/SQLHelper.Tests/Utils/TestConnectionStrings.cs](test/SQLHelper.Tests/Utils/TestConnectionStrings.cs)
- [test/SQLHelper.Tests/appsettings.json](test/SQLHelper.Tests/appsettings.json)
- [.github/workflows/dotnet-test.yml](.github/workflows/dotnet-test.yml)

## Repo Map

- `src/SQLHelper.DB/`: core library.
    - `SQLHelper.cs`: main entry point.
    - `HelperClasses/`: batch, command, connection, parameters.
    - `ExtensionMethods/`: DB command/data record helpers.
    - `CanisterModules/` and `Registration/`: DI registration.
- `test/SQLHelper.Tests/`: xUnit test suite for library behavior.
- `SQLHelper.SpeedTests/`: BenchmarkDotNet performance tests.
- `SQLHelper.Example/` and `TestApp/`: usage/demo apps.
- `docfx_project/`: docs generation project.

## Conventions To Preserve

- Follow project style from [.editorconfig](.editorconfig) (4 spaces, CRLF, C# style settings).
- Keep public API documentation current (package project enables XML docs generation).
- Prefer async APIs for I/O paths and keep `Async` suffixes where applicable.
- Respect nullable annotations and avoid introducing new warnings.

References:

- [CONTRIBUTING.md](CONTRIBUTING.md)
- [README.md](README.md)
- [.editorconfig](.editorconfig)

## Agent Working Notes

- Prefer minimal, targeted changes; avoid broad refactors unless requested.
- Do not commit generated `bin/` or `obj/` artifacts.
- For behavior changes, update or add tests in [test/SQLHelper.Tests](test/SQLHelper.Tests).
- For performance-sensitive changes, validate in [SQLHelper.SpeedTests](SQLHelper.SpeedTests).

## Useful Setup

If local tools are missing, see [setup.bat](setup.bat) for baseline setup used by this repo.
