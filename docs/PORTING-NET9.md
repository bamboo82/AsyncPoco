# Porting AsyncPocoCore to .NET 9

Done 2026-09-04. Result: `AsyncPocoCore/` targets `net9.0`, builds with 0 errors, and passes `AsyncPoco.Smoke` (14 runtime checks on in-memory SQLite).

## What changed
1. **Source parity.** `AsyncPocoCore/` was a March-2024 snapshot missing three later commits (ConfigureAwait pass, `DefaultOrderableAttribute`, `FetchAsync<T1,TRet>(cb, Sql)`, StoredProcedures T4). All `.cs`/`.tt`/`.ttinclude` were copied from `AsyncPoco/`. Deleted from Core: `AsyncPoco.cs` (4,500-line pre-split monolith), `Linq/` (dead LINQ provider), `UpgradeReport.sarif`, `upgrade-assistant.clef`.
2. **`AsyncPocoCore.csproj` rewritten.** Removed: five dead project configurations, FxCop paths, `Compile Remove` list, Upgrade-Assistant analyzer, `System.Reflection.Emit*` 4.7.0 packages (inbox on net9), `Platform=x86` default. Kept: `GenerateAssemblyInfo=false` (version in `Properties/AssemblyInfo.cs`), `AssemblyName=AsyncPoco`. Only package: `System.Configuration.ConfigurationManager` 9.0.0. `DefaultItemExcludes` guards against `Linq\**` and `AsyncPoco.cs` ever being compiled again.
3. **Provider-factory guard** (`Database.CommonConstruct`, both trees). `DbProviderFactories.GetFactory(name)` throws `ArgumentException` on .NET Core when nothing is registered. Now wrapped to throw `InvalidOperationException` explaining the fix. No behaviour change on net48 where the factory is found.
4. **`DatabaseType.Resolve` bug fix** (both trees). Each dialect check was `if (StartsWith("X")) if (StartsWith("X", OrdinalIgnoreCase))` — the outer case-sensitive test made the inner one dead. Collapsed to the case-insensitive form. Practical effect: `Microsoft.Data.Sqlite` (`SqliteConnection`, lowercase "lite") now resolves to `SQLiteDatabaseType` instead of falling through to SQL Server (`scope_identity()`).
5. **`AsyncPoco.Smoke/`** added and registered in `AsyncPocoCore.sln`.
6. `AsyncPoco.csproj` (net48) untouched except code fixes 3 and 4 flowing through the shared source. It stays net48-only by owner decision.

## Runtime differences hosts must know (net9)
| Topic | net48 | net9 |
|---|---|---|
| `new Database(connStr, providerName)` | works via machine.config provider list | throws unless host called `DbProviderFactories.RegisterFactory(providerName, XxxFactory.Instance)` first |
| `new Database(connectionStringName)` | reads app/web.config | reads `<app>.dll.config` via `System.Configuration.ConfigurationManager`; `""` (use first) has no machine.config `LocalSqlServer` fallback |
| `new Database(DbConnection)` / `new Database(connStr, DbProviderFactory)` | works | works — **preferred on net9** |
| SQL Server provider | `System.Data.SqlClient` | use `Microsoft.Data.SqlClient` (type names still `SqlConnection`/`SqlClientFactory`, resolves via the SQL Server fallback) |
| SQLite | `System.Data.SQLite` | `Microsoft.Data.Sqlite` verified by the smoke test |
| Oracle | `OracleProvider.cs` reflection shim | untested; prefer `Oracle.ManagedDataAccess.Core` + `DbProviderFactory` ctor |
| `Reflection.Emit` (PocoData factories) | works | works (inbox) |

## Verification
```bash
dotnet build AsyncPocoCore/AsyncPocoCore.csproj -nologo -v q   # expect: 0 errors, 3 pre-existing warnings (CS0162 PocoData.cs:401, CS0108 ComputedColumnAttribute x2)
dotnet run --project AsyncPoco.Smoke                            # expect: ALL CHECKS PASSED, exit 0
```

## Deliberately not done
- `AsyncPoco.Tests` (net45, old-style csproj, PetaTest, needs SQL Server / MySQL / PostgreSQL / SQL CE instances) — not ported. The smoke test is the net9 stand-in.
- `AsyncPoco.DevBed`, `csj` — legacy, untouched.
- No multi-targeting; two projects remain by owner decision. Keep them in sync with the command in `CLAUDE.md`.
- No NuGet packaging for net9 (`AsyncPoco.nuspec` still says `lib\net45`).
