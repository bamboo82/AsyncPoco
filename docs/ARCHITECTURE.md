# Architecture

Applies to both `AsyncPoco/` (net48) and `AsyncPocoCore/` (net9.0); the source is identical.
Namespace is `AsyncPoco` (public) and `AsyncPoco.Internal` / `AsyncPoco.DatabaseTypes` (internals).

## File map
| Path | Lines | What |
|---|---|---|
| `Database.cs` | ~2480 | The whole public API: ctors, connection/transaction management, `ExecuteAsync`, `FetchAsync`, `QueryAsync`, `PageAsync`, `SkipTakeAsync`, `Single*/First*`, `ExistsAsync`, `InsertAsync`, `UpdateAsync`, `DeleteAsync`, `SaveAsync`, multi-poco `FetchAsync<T1..T8,TRet>`, `QueryMultipleAsync`. Partial class; T4 output extends it. |
| `Core/DatabaseType.cs` | 180 | Base class for dialect differences + `Resolve(typeName, providerName)` picking the dialect from the connection/factory type name (case-insensitive), then provider name, else SQL Server. |
| `DatabaseTypes/*.cs` | small | Dialect overrides: SqlServer, SqlServerCE, MySql, PostgreSQL, Oracle, SQLite (paging SQL, insert-identity SQL, `EXISTS` SQL, param prefix, value mapping). |
| `Core/PocoData.cs` | 445 | Reflection + `Reflection.Emit` factory: builds and caches a `DbDataReader -> T` delegate per (type, sql, columns). Handles enums, nullable, `ValueType` conversions, dynamic/Expando. |
| `Core/TableInfo.cs`, `Core/ColumnInfo.cs`, `Core/PocoColumn.cs` | | Per-class metadata read from attributes (table name, PK, auto-increment, sequence, combo PK, result/computed columns). |
| `Core/IMapper.cs`, `Core/Mappers.cs`, `Core/StandardMapper.cs` | | Pluggable mapping (`Mappers.Register(assembly|type, mapper)`). |
| `Core/MultiPocoFactory.cs` | 332 | Splits one result set into several POCOs for the multi-poco `FetchAsync`/`QueryAsync` overloads. |
| `Core/Sql.cs` | 244 | The `Sql` builder (`Sql.Builder.Select().From().Where()...`), plus `Sql.SQL`/`Arguments`. |
| `Core/Page.cs` | | `Page<T>` (`CurrentPage`, `TotalPages`, `TotalItems`, `ItemsPerPage`, `Items`, `Context`). |
| `Core/Transaction.cs` | | `ITransaction` returned by `GetTransactionAsync()`; `Complete()` or dispose-to-rollback. |
| `MultiResultSet/GridReader.cs` | 282 | `QueryMultipleAsync` reader. |
| `Utilities/PagingHelper.cs` | | Regex split of a SELECT into parts for count/page query generation. |
| `Utilities/ParametersHelper.cs` | | `@0`, `@name`, `@@literal` parameter expansion; `IEnumerable` args expand to lists. |
| `Utilities/Cache.cs`, `ArrayKey.cs`, `Singleton.cs`, `EnumMapper.cs`, `AutoSelectHelper.cs` | | Caches and helpers. |
| `Attributes/*.cs` | | Standard PetaPoco attributes: `TableName`, `PrimaryKey`, `Column`, `ResultColumn`, `ComputedColumn`, `Ignore`, `ExplicitColumns`, plus fork additions `MaxLength`, `Description`. |
| `Attributes/Special/*.cs` | | **Fork-only UI/metadata attributes** (see below). |
| `Exceptions/ExceedMaxLengthException.cs` | | Thrown on insert/update when a `[MaxLength]` string is too long. |
| `OracleProvider.cs` | | Reflection-based Oracle factory shim (`Assembly.Load`). Framework-era; untested on net9. |
| `T4 Templates/` | | Design-time code generation (see below). Not compiled. |
| `HashCodeCombiner.cs` | | Hash helper used by combo-PK keys. |

## Request flow (e.g. `FetchAsync<T>(sql, args)`)
1. `AddSelectClause<T>` prepends `SELECT cols FROM table` if `sql` starts with `WHERE`/`ORDER`… (`EnableAutoSelect`).
2. `OpenSharedConnectionAsync` — ref-counted; creates a connection from `_factory` unless one was supplied via `Database(DbConnection)` (then depth starts at 2 and it is never closed by the lib).
3. `CreateCommand` — `ParametersHelper.ProcessParams` rewrites `@0`/`@name` into provider params via `_dbType.MapParameterValue`.
4. `cmd.ExecuteReaderAsync()` → `PocoData.ForType(T).GetFactory(...)` emits/caches a reader-to-object delegate → rows materialised.
5. `CloseSharedConnection`.
Insert goes through `_dbType.ExecuteInsertAsync` (dialect-specific identity retrieval); paging through `_dbType.BuildPageQuery` after `PagingHelper.SplitSQL`.

## Fork-specific pieces
- **`entity.SaveAsync()` / `Repo.GetInstance()` pattern** described in README lives in *generated* code (`T4 Templates/AsyncPoco.Generator.ttinclude`), not in this library: the T4 emits `partial class <RepoName> : Database` with a static `GetInstance()` factory, a nested `Record<T>` base with `SaveAsync()`/`SaveAsync(Database)`, and a `partial class` per table/view.
- **Combo (composite) primary keys**: `PrimaryKeyAttribute` accepts comma-separated columns; `Core/TableInfo` + `ArrayKey`/`HashCodeCombiner` handle them. README warns support is weak.
- **`Attributes/Special/*`**: `Combo`, `ComboOption`, `CustomizedType`, `DateType`, `FileType`, `Memo`, `MultiReferenceTo`, `Orderable`, `DefaultOrderable`, `QuickSearchableGroup`, `Readonly`, `ReferenceBy`, `ReferenceTo`, `ReferenceToPassive`, `Required`, `SearchableGroup`, `Suggestable`, `TableHideAll/Field`, `TableShowAll/Field`. These carry admin-UI metadata for the owner's CMS generator; the ORM itself only reads `MaxLength`/`Required`-style checks in insert/update. Their `<summary>` XML comments are mostly wrong copy-paste; read the class body.
- **`.ConfigureAwait(false)`** on every internal await (commit ee09632). Keep it that way when adding awaits.

## T4 templates (`T4 Templates/`)
`Database.tt` (entry; sets `ConnectionStringName`, `Namespace`, `RepoName`, flags) → includes `AsyncPoco.Core.ttinclude` (schema readers for SQL Server / MySQL / PostgreSQL / Oracle, run inside VS) → `AsyncPoco.Generator.ttinclude` (emits repo + POCOs). `StoredProcedures.tt` / `.Generator.ttinclude` emit stored-procedure wrappers. `Database.tt` currently contains owner-project-specific table tweaks (Chinese view names); treat it as an example, not library code.
