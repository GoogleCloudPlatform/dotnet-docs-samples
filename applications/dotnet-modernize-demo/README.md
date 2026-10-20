# .NET modernization demo

Sample applications for the
[Antigravity-powered .NET modernization demo](https://github.com/GoogleCloudPlatform/cloud-solutions/tree/main/projects/dotnet-modernization-demo)
in the Cloud Solutions repository. Follow that guide to run the demo.

- [`dotnet-migration-sample/`](dotnet-migration-sample/): the legacy
  application (.NET 5, Entity Framework 6, SQL Server). It is intentionally
  outdated because it is the input of the demo.
- [`dotnet-migration-sample-modernized/`](dotnet-migration-sample-modernized/):
  the modernized application (.NET 8, Entity Framework Core, PostgreSQL,
  Cloud Run). See its
  [`MODERNIZATION.md`](dotnet-migration-sample-modernized/MODERNIZATION.md).
- [`.agents/skills/modernize-dotnet-cloudrun/`](.agents/skills/modernize-dotnet-cloudrun/SKILL.md):
  the Agent Skill that the Antigravity CLI uses to perform the modernization.
