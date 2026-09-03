# Status

Newest first. One entry per session that changes code. Keep entries short; details go in the other docs.

## 2026-09-04 — net9 port of AsyncPocoCore complete
- `AsyncPocoCore/` = net9.0, source-identical to `AsyncPoco/` (net48). Both build clean.
- `AsyncPoco.Smoke/` (in-memory SQLite) passes 14/14. Command in `CLAUDE.md`.
- Fixed in shared source: `DatabaseType.Resolve` dead case-sensitive checks; descriptive error when `DbProviderFactories` has no provider (net9).
- Docs created: `CLAUDE.md`, `docs/ARCHITECTURE.md`, `docs/PORTING-NET9.md`, this file.
- Nothing committed yet by the session; owner reviews and commits.

### Open items / next steps
1. **Commit** the port (all of `AsyncPocoCore/`, `AsyncPoco.Smoke/`, `AsyncPocoCore.sln`, docs, and the two shared-source fixes in `AsyncPoco/`).
2. Pre-existing warnings worth a 5-minute fix: `Core/PocoData.cs:401` unreachable `break`; `Attributes/ComputedColumnAttribute.cs` hides `Name`/`ForceToUtc` (add `new` or remove the duplicates).
3. `AsyncPoco.nuspec` still targets `lib\net45`; if a net9 package is ever wanted, switch to `dotnet pack` with `PackageId`/`Version` in the csproj.
4. `OracleProvider.cs` is untested on net9.
5. Consider a T4-free way to produce the repo/`Record<T>` boilerplate for net9 consumers (T4 runs only inside VS).
