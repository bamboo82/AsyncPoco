# Status

Newest first. One entry per session that changes code. Keep entries short; details go in the other docs.

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
