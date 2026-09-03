# AsyncPoco (Bamboo fork) — session guide

Read this first. Then read `docs/STATUS.md` for where things stand. Open other docs only when needed.

## What this repo is
Async fork of the PetaPoco micro-ORM, plus fork-specific attributes and T4 code generation used by the
owner's CMSWeb / mfpKeeper projects. Two library projects with **identical source**, different targets:

| Project | Target | Role |
|---|---|---|
| `AsyncPoco/` | net48 | **Canonical.** Edit code here. Consumed by siblings `X:\P5\AsyncPocoPioneer` and `X:\P5\Bamboo.Commenter` (net4x, project-reference this csproj). |
| `AsyncPocoCore/` | net9.0 | Mirror of `AsyncPoco/` `.cs` + T4 files. Never edit directly; sync (see below). |
| `AsyncPoco.Smoke/` | net9.0 | In-memory SQLite smoke test for the net9 build. |

`AsyncPoco.Tests/`, `AsyncPoco.DevBed/`, `csj/` are legacy net45 projects (need live DB servers / old tooling). Not built, not maintained.

## Commands
```bash
dotnet build AsyncPoco/AsyncPoco.csproj -nologo -v q          # net48 lib
dotnet build AsyncPocoCore/AsyncPocoCore.csproj -nologo -v q  # net9 lib
dotnet run --project AsyncPoco.Smoke                          # net9 runtime check; exit 0 = pass
```
Sync Core from canonical after any code change (copies `.cs`, `.tt`, `.ttinclude`; never the csproj):
```bash
cd AsyncPoco && find . -type f \( -name '*.cs' -o -name '*.tt' -o -name '*.ttinclude' \) -not -path './bin/*' -not -path './obj/*' -not -path './.vs/*' | while read f; do mkdir -p "../AsyncPocoCore/$(dirname "$f")"; cp "$f" "../AsyncPocoCore/$f"; done
```
Verify parity: `diff -rq AsyncPoco AsyncPocoCore -x bin -x obj -x .vs` should list only the csproj files and nuspec.

## Rules / gotchas
- **Change code in `AsyncPoco/` only, then sync.** Drift between the two trees was the cause of the 2024–2026 mess.
- **Do not add net9.0 to `AsyncPoco.csproj`.** Owner decision (2026-09-04): keep two separate projects.
- `.gitignore` ignores every dot-file/dir except `.gitignore`, `.gitattributes`, `.editorconfig`, `.claude/`. Adding another dot-path needs a `!` exception or `git add` silently does nothing.
- On .NET 9, `DbProviderFactories.GetFactory(name)` has no registered providers. Hosts must `RegisterFactory` at startup or use the `Database(DbConnection)` / `Database(string, DbProviderFactory)` ctors. `Database.CommonConstruct` throws a descriptive `InvalidOperationException` otherwise.
- `Linq/` (old PetaPoco LINQ provider) is dead code, removed from both trees. Don't resurrect it.
- The `<summary>` comments on most `Attributes/Special/*` are copy-paste garbage ("marks the property maxlength"). Read the class body, not the summary.
- Build output is Traditional Chinese (`建置成功` = build succeeded, `錯誤` = error).
- Version lives in `Properties/AssemblyInfo.cs` (both trees, same file). `GenerateAssemblyInfo` is off.

## Docs
- `docs/STATUS.md` — dated state + next steps. Update it at the end of every session that changes code.
- `docs/ARCHITECTURE.md` — file map and request flow. Read before touching `Database.cs` / `Core/`.
- `docs/PORTING-NET9.md` — what the net9 port changed, runtime differences, what was left out.
