# Status

Newest first. One entry per session that changes code. Keep entries short; details go in the other docs.

## 2026-11-21 — ConfigureAwait(false) gaps closed
- Reported by the web_headline session; re-verified here with a statement-level scan of all 71 awaits under `AsyncPoco/`.
- Library: three awaits lacked it and now have it: `Database.cs` auto-increment `InsertAsync` (`_dbType.ExecuteInsertAsync`), multi-poco `FetchAsync<TRet>(Type[], ...)`, and `Core/Transaction.cs` `BeginAsync(db, isolationLevel)`. Every library await now ends in `.ConfigureAwait(false)`.
- Template: the 12 awaits in the generated `Record<T>` methods (`InsertAsync`/`UpdateAsync`/`DeleteAsync`/`SaveAsync` in `AsyncPoco.Generator.ttinclude`) had the same gap; fixed. Consumers keep their own copy of this ttinclude, so their generated code only changes if they copy the template and regenerate.
- Why: under classic ASP.NET a sync-over-async caller (`.Result`/`.Wait()`) on the request thread would deadlock on these continuations.
- Verified: both targets 0 errors, smoke 15/15. The smoke test does not cover multi-poco fetch or the isolation-level transaction.

## 2026-11-21 — Microsoft.Data.SqlClient preferred for SQL Server
- `Database.cs`: new `TryGetFactory`; `Microsoft.Data.SqlClient` resolved via `DbProviderFactories` or, failing that, reflection on the host's package. Default provider for `Database(connectionStringName)` without `providerName` is now `Microsoft.Data.SqlClient`, falling back to `System.Data.SqlClient` when the package is absent.
- `Core/DatabaseType.cs`: the dead `"System.Data.SqlClient."` full-name check replaced by short-name checks (`SqlClient*`, `SqlConnection*`). Behaviour unchanged (SQL Server was already the fallback).
- `T4 Templates/StoredProcedures.Generator.ttinclude`: generated code now has `using Microsoft.Data.SqlClient;`. Consumers that regenerate must reference that package.
- The library itself has no `using System.Data.SqlClient;` and takes no package dependency.
- Smoke test: +1 check (15 total), `Microsoft.Data.SqlClient` 7.1.2 added to `AsyncPoco.Smoke` only. Verified: both targets 0 errors, smoke 15/15, net48 DLL loaded in Windows PowerShell (explicit `System.Data.SqlClient` ok; explicit `Microsoft.Data.SqlClient` without the package throws the descriptive error).
- Not tested: a real SQL Server connection, and the defaulted-provider fallback path (needs an app.config host).

## 2026-11-20 — AsyncPoco + AsyncPocoCore merged into one multi-target project
- `AsyncPoco/AsyncPoco.csproj` now targets `net48;net10.0`. `AsyncPocoCore/` and `AsyncPocoCore.sln` deleted; no more sync step. Owner reversed the 2026-09-04 two-project decision.
- Csproj cleanup: dead FxCop/x64/GenerateTemplate groups removed; `Platform=x86` default removed (it never affected the DLL, which was AnyCPU); `CMSWebConsole-Only` configuration kept.
- New `AsyncPoco.Modern.sln` (AsyncPoco + AsyncPoco.Smoke). Legacy `AsyncPoco.sln` untouched.
- Verified: `dotnet build` 0 errors on both targets (Debug and Release), smoke 14/14, VS 2026 MSBuild 18.11 builds the new solution.
- Not working, pre-existing: `AsyncPocoPioneer` (net461) and `Bamboo.Commenter.ChatServer` (net472) cannot project-reference a net48 library; same error with the pre-merge csproj.
- New constraint: VS 2022 MSBuild cannot build this project any more (NETSDK1045, it only has SDK 9).
- Entries below that mention `AsyncPocoCore/` are history.

## 2026-11-16 — AsyncPocoCore retargeted to net10.0
- `AsyncPocoCore/` and `AsyncPoco.Smoke/` now target `net10.0` (was net9.0) so they match Visual Studio 2026. Csproj-only change; no source edits. `AsyncPoco/` stays net48.
- Packages bumped 9.0.0 → 10.0.0: `System.Configuration.ConfigurationManager`, `Microsoft.Data.Sqlite` (smoke only).
- Verified with SDK 10.0.401: net48 build 0 errors, net10 build 0 errors, smoke 14/14. Real output is `AsyncPocoCore/bin/<Config>/net10.0/AsyncPoco.dll`.
- Open: smoke test restore warns NU1903 (GHSA-2m69-gcr7-jv3q) on transitive `SQLitePCLRaw.lib.e_sqlite3` 2.1.11. Test-only dependency; the library does not reference it.
- Older entries below say net9; read them as history.

## 2026-10-09 — comment/doc cleanup to cut future reading
- Replaced the wrong copy-pasted `<summary>` ("marks the property maxlength") on 10 attribute files (8 under `Attributes/Special/`, plus `MaxLength`, `Description`) with accurate one-liners; added summaries to `ReferenceByAttribute.cs` / `ReferenceToPassiveAttribute.cs` noting the class/file name mismatch and the swapped-arg ctor quirk. Comments only, no behaviour change. Synced to `AsyncPocoCore/`; both builds clean, smoke 14/14.
- `docs/ARCHITECTURE.md`: full ctor table for every `Attributes/Special/*` attribute (no need to open the files); corrected two false claims — the ORM never reads `MaxLength`/`Required`/Special attributes, and `ExceedMaxLengthException` is never thrown by the library.
- `CLAUDE.md` gotcha updated accordingly.

## 2026-09-11 — docs sync
- README renamed the project title to "AsyncPoco (BambooMod)" (commit 2fb6f5d) and added precaution #4 on SQL Server `IsolationLevel` reuse (commit 020970b). `CLAUDE.md` gotchas now carry the same warning.
- No code change. Both csproj targets confirmed: `AsyncPoco/` net48, `AsyncPocoCore/` net9.0. Owner reconfirmed: only `AsyncPocoCore/` targets net9.
- Stale non-TFM outputs (`AsyncPocoCore/bin/Debug/AsyncPoco.dll`, `.../Release/AsyncPoco.dll`, dated 2023, net45) are git-ignored leftovers; the real net9 output is `AsyncPocoCore/bin/<Config>/net9.0/AsyncPoco.dll`.

## 2026-09-04 — net9 port of AsyncPocoCore complete
- `AsyncPocoCore/` = net9.0, source-identical to `AsyncPoco/` (net48). Both build clean.
- `AsyncPoco.Smoke/` (in-memory SQLite) passes 14/14. Command in `CLAUDE.md`.
- Fixed in shared source: `DatabaseType.Resolve` dead case-sensitive checks; descriptive error when `DbProviderFactories` has no provider (net9).
- Docs created: `CLAUDE.md`, `docs/ARCHITECTURE.md`, `docs/PORTING-NET9.md`, this file.
- Committed as 79e6282, merged to `master` via PR #1 (`net9-port` branch).

### Open items / next steps
1. Pre-existing warnings worth a 5-minute fix: `Core/PocoData.cs:401` unreachable `break`; `Attributes/ComputedColumnAttribute.cs` hides `Name`/`ForceToUtc` (add `new` or remove the duplicates).
2. `AsyncPoco.nuspec` still targets `lib\net45`; if a net9 package is ever wanted, switch to `dotnet pack` with `PackageId`/`Version` in the csproj.
3. `OracleProvider.cs` is untested on net9.
4. Consider a T4-free way to produce the repo/`Record<T>` boilerplate for net9 consumers (T4 runs only inside VS).
5. Optional: delete the stale 2023 dlls under `AsyncPocoCore/bin/Debug` and `bin/Release` so nothing picks up a net45 build by mistake.
