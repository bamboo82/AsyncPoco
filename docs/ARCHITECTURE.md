# Architecture

One source tree, `AsyncPoco/`, compiled for net48 and net10.0.
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
| `Attributes/*.cs` | | Standard PetaPoco attributes: `TableName`, `PrimaryKey`, `Column`, `ResultColumn`, `ComputedColumn`, `Ignore`, `ExplicitColumns`. Fork additions `MaxLength(string)`, `Description(string)` are emitted by the T4 generator from the DB schema and **never read by the ORM**. |
| `Attributes/Special/*.cs` | | **Fork-only UI/metadata attributes** (see below). |
| `Exceptions/ExceedMaxLengthException.cs` | | `(fieldName, maxLength)`. Defined only; **never thrown by this library**. Reserved for consumers that validate `[MaxLength]` themselves. |
| `OracleProvider.cs` | | Reflection-based Oracle factory shim (`Assembly.Load`). Framework-era; untested on net9/net10. |
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
- **`Attributes/Special/*`**: admin-UI metadata for the owner's CMS generator (and `Bamboo.Commenter`, which reflects on `Required`/`Memo`). **The ORM never reads any of them.** Each file is a plain data-holder attribute; the table below is complete, so there is no need to open the files. `×n` = `AllowMultiple`. `Enum_*` are nested sealed "enum classes" whose `NONE`/`DEFAULT_NONE` member is `null`.

| Attribute | Namespace | Targets | Ctor / data |
|---|---|---|---|
| `Combo` | `AsyncPoco` | Property | none; drop-down whose options are the `ComboOption`s |
| `ComboOption` ×n | `AsyncPoco` | Property | `(object key, string value[, bool isDefault])` |
| `CustomizedType` | `AsyncPoco` | Property | `(string type)` → `TypeName` |
| `DateType` | `AsyncPoco` | Property | `([bool isDateType = true])` |
| `FileTypePassive` (file `FileTypeAttribute.cs`) | `AsyncPoco` | Property | `(lookupGroup, recieveAsFieldName, redirectToFieldName, tag[, isRequired, isReadonly, isVisible, shouldSave])` |
| `Memo` ×n | `AsyncPoco` | Property | `(string tag, string text)` |
| `Readonly`, `Required` | `AsyncPoco` | Property | `([bool = true])` → `IsReadonly` / `IsRequired` |
| `Orderable`, `DefaultOrderable` | `AsyncPoco.Attributes.Special` | Property, Class | `(Enum_FieldRole DEFAULT_NONE/DEFAULT_ASC/DEFAULT_DESC)`; `DefaultOrderable` adds `Value` = `"asc"`/`"desc"` |
| `MultiReferenceTo` | same | Property | `(tableName, fieldName)`; `FieldRole` (`COMMA_SEPERATED`) settable |
| `ReferenceTo` | same | Property | `(foreignTable, foreignField[, lookupGroup, int behavior])`; behavior 1 = `SINGLE_LOOKUP` (default), 2 = `MULTI_LOOKUP` |
| `ReferenceToPassive` ×n | same | Property | `(lookupGroup, recieveAsFieldName, redirectToFieldName, tag[, isRequired, isReadonly, isVisible, shouldSave, isLookupable])` |
| `ReferenceByPassive` ×n (file `ReferenceByAttribute.cs`) | same | Property | `(lookupGroup, fromFieldName, asFieldName, tag)` |
| `SearchableGroup` ×n, `QuickSearchableGroup` | same | Property | `(groupName, groupOrderIndex, fieldOrderIndex, description[, Enum_FieldRole])`; only `NONE` exists |
| `Suggestable` | same | Property | `(groupName, Enum_FieldRole NONE/AS_KEY/AS_UNIQUE_KEY)` |
| `TableShowAll`, `TableHideAll` ×n | same | Property, Class | none |
| `TableShowField`, `TableHideField` ×n | same | Property, Class | `(string fieldName)` |

  Consumers (reflect on these attributes): `CMSWebConsole/Program.cs` in `X:\P5\CMS`, `X:\P5\Bob\House\web`, `X:\P5\Inject\Panel`; `X:\P5\Wall\WallWebConsole/Program.cs`; `Bamboo.Commenter.ChatServer/CustomSocket/Base/Base_Params.cs` (`Required`).

  Quirk, do not "fix": in `FileTypePassive` and `ReferenceToPassive` the 4-arg ctor forwards `recieveAs`/`redirectTo` swapped into the long ctor. Consumers may depend on it.
- **`.ConfigureAwait(false)`** on every internal await (commit ee09632). Keep it that way when adding awaits.

## T4 templates (`T4 Templates/`)
`Database.tt` (entry; sets `ConnectionStringName`, `Namespace`, `RepoName`, flags) → includes `AsyncPoco.Core.ttinclude` (schema readers for SQL Server / MySQL / PostgreSQL / Oracle, run inside VS) → `AsyncPoco.Generator.ttinclude` (emits repo + POCOs). `StoredProcedures.tt` / `.Generator.ttinclude` emit stored-procedure wrappers. `Database.tt` currently contains owner-project-specific table tweaks (Chinese view names); treat it as an example, not library code.
