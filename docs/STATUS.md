# Status

Newest first. One entry per session that changes code. Keep entries short; details go in the other docs.

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
