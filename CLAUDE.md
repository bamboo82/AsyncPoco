# AsyncPoco (BambooMod) — session guide

Read this first. Then read `docs/STATUS.md` for where things stand. Open other docs only when needed.

## What this repo is
Async fork of the PetaPoco micro-ORM, plus fork-specific attributes and T4 code generation used by the
owner's CMSWeb / mfpKeeper projects. One library project, two targets (merged 2026-11-20; the old `AsyncPocoCore/` mirror is gone):

| Project | Target | Role |
|---|---|---|
| `AsyncPoco/` | net48 + net10.0 | The library. One source tree, `<TargetFrameworks>net48;net10.0</TargetFrameworks>`. Output `bin\<Config>\<tfm>\AsyncPoco.dll`. Siblings `X:\P5\AsyncPocoPioneer` and `X:\P5\Bamboo.Commenter` project-reference this csproj. |
| `AsyncPoco.Smoke/` | net10.0 | In-memory SQLite smoke test for the net10.0 target. |

Solutions: `AsyncPoco.Modern.sln` = the two live projects (open this one in VS 2026). `AsyncPoco.sln` = legacy, includes the dead net45 projects.

`AsyncPoco.Tests/`, `AsyncPoco.DevBed/`, `csj/` are legacy net45 projects (need live DB servers / old tooling). Not built, not maintained.

## Commands
```bash
dotnet build AsyncPoco/AsyncPoco.csproj -nologo -v q   # both targets; expect 0 errors, 6 known warnings (3 per target)
dotnet run --project AsyncPoco.Smoke                   # net10 runtime check; exit 0 = pass
```
Needs .NET SDK 10+. VS 2022 / its MSBuild resolve SDK 9 and fail with NETSDK1045; use VS 2026 (`C:\Program Files\Microsoft Visual Studio8`) or the `dotnet` CLI.

## Rules / gotchas
- **Framework-specific dependencies are conditioned in the csproj** (`System.configuration` reference for net48, `System.Configuration.ConfigurationManager` package for net10.0). The source has no `#if` on framework; keep it that way if possible.
- The net48 siblings target net461 / net472 and their project reference to this net48 library already failed to build before the merge ("cannot be referenced by a project targeting v4.6.1"). Pre-existing; not fixed here.
- `.gitignore` ignores every dot-file/dir except `.gitignore`, `.gitattributes`, `.editorconfig`, `.claude/`. Adding another dot-path needs a `!` exception or `git add` silently does nothing.
- On .NET 9+, `DbProviderFactories.GetFactory(name)` has no registered providers. Hosts must `RegisterFactory` at startup or use the `Database(DbConnection)` / `Database(string, DbProviderFactory)` ctors. `Database.CommonConstruct` throws a descriptive `InvalidOperationException` otherwise.
- **SQL Server provider (2026-11-21):** `Microsoft.Data.SqlClient` is preferred. `Database.TryGetFactory` finds it by reflection (no package reference in the library, no `RegisterFactory` needed). `Database(connectionStringName)` with no `providerName` in config tries it first and falls back to `System.Data.SqlClient` if the host lacks the package. `StoredProcedures.Generator.ttinclude` emits `using Microsoft.Data.SqlClient;`, so regenerated stored-procedure code needs that package in the consumer. T4 design-time code still runs on `System.Data.SqlClient` inside VS.
- `Linq/` (old PetaPoco LINQ provider) is dead code, removed from both trees. Don't resurrect it.
- `Attributes/Special/*`, `MaxLength`, `Description`: metadata for the owner's CMS generator; **the ORM never reads them**. Full ctor table in `docs/ARCHITECTURE.md`; don't open the files. `ExceedMaxLengthException` is defined but never thrown here. Two files are misnamed: `FileTypeAttribute.cs` holds `FileTypePassiveAttribute`, `ReferenceByAttribute.cs` holds `ReferenceByPassiveAttribute`.
- Build output is Traditional Chinese (`建置成功` = build succeeded, `錯誤` = error).
- Version lives in `Properties/AssemblyInfo.cs` (both trees, same file). `GenerateAssemblyInfo` is off.
- SQL Server reuses a pooled connection's `IsolationLevel`, and connectors differ; after `GetTransactionAsync(IsolationLevel)` the level can leak into later work on that connection. README precaution #4. Consider resetting it on a fresh connection.

- `X:\P5\AsyncPoco\AsyncPoco2\` is an older copy of this repo (with an `AsyncPocoCore.backup`). Not this project; don't read or copy from it.

## Docs
- `docs/STATUS.md` — dated state + next steps. Update it at the end of every session that changes code.
- `docs/ARCHITECTURE.md` — file map and request flow. Read before touching `Database.cs` / `Core/`.
- `docs/PORTING-NET9.md` — what the net9 port changed (now retargeted to net10; see its last section), runtime differences, what was left out.
