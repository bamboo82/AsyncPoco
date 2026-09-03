// Smoke test for AsyncPocoCore on .NET 9 using in-memory SQLite.
// Exercises: connection via DbConnection ctor, DDL, InsertAsync (auto-increment PK),
// SingleAsync, FetchAsync, UpdateAsync, PageAsync, ExistsAsync, DeleteAsync,
// transactions (commit + rollback), and the .NET Core provider-factory guard.
//
// Run: dotnet run --project AsyncPoco.Smoke
// Exit code 0 => all checks passed.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using AsyncPoco;
using Microsoft.Data.Sqlite;

namespace AsyncPoco.Smoke
{
    [TableName("Person")]
    [PrimaryKey("Id", autoIncrement = true)]
    public class Person
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    internal static class Program
    {
        private static int _failures;

        private static void Check(bool condition, string what)
        {
            Console.WriteLine((condition ? "  ok   " : "  FAIL ") + what);
            if (!condition) _failures++;
        }

        private static async Task<int> Main()
        {
            Console.WriteLine("AsyncPocoCore smoke test (net9.0, Microsoft.Data.Sqlite in-memory)");

            // :memory: databases live only as long as the connection, so open it ourselves and hand it
            // to the Database(DbConnection) constructor (which never closes an external connection).
            using (var conn = new SqliteConnection("Data Source=:memory:"))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (var db = new Database(conn))
                {
                    Check(db.Connection == conn, "Database(DbConnection) keeps the external connection");

                    await db.ExecuteAsync("CREATE TABLE Person (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL, Age INTEGER NOT NULL)").ConfigureAwait(false);

                    // Insert
                    var alice = new Person { Name = "Alice", Age = 30 };
                    var newId = await db.InsertAsync(alice).ConfigureAwait(false);
                    Check(alice.Id == 1 && Convert.ToInt64(newId) == 1, "InsertAsync assigns auto-increment PK (SELECT last_insert_rowid)");

                    for (var i = 0; i < 9; i++)
                        await db.InsertAsync(new Person { Name = "P" + i, Age = 20 + i }).ConfigureAwait(false);

                    // Single / Fetch
                    var loaded = await db.SingleAsync<Person>(1L).ConfigureAwait(false);
                    Check(loaded.Name == "Alice" && loaded.Age == 30, "SingleAsync by PK maps columns");

                    var all = await db.FetchAsync<Person>("ORDER BY Id").ConfigureAwait(false);
                    Check(all.Count == 10, "FetchAsync with auto-select prefix returns all rows");

                    var byAge = await db.FetchAsync<Person>("WHERE Age >= @0", 25).ConfigureAwait(false);
                    Check(byAge.Count == 5, "FetchAsync with @0 positional parameter");

                    // Update
                    loaded.Age = 31;
                    var updated = await db.UpdateAsync(loaded).ConfigureAwait(false);
                    var reloaded = await db.SingleAsync<Person>(1L).ConfigureAwait(false);
                    Check(updated == 1 && reloaded.Age == 31, "UpdateAsync persists change");

                    // Paging
                    var page2 = await db.PageAsync<Person>(2, 3, "ORDER BY Id").ConfigureAwait(false);
                    Check(page2.TotalItems == 10 && page2.TotalPages == 4 && page2.Items.Count == 3 && page2.Items[0].Id == 4,
                        "PageAsync builds LIMIT/OFFSET paging and count query");

                    // Exists / Delete
                    Check(await db.ExistsAsync<Person>(1L).ConfigureAwait(false), "ExistsAsync by PK");
                    var deleted = await db.DeleteAsync<Person>(1L).ConfigureAwait(false);
                    Check(deleted == 1 && !await db.ExistsAsync<Person>(1L).ConfigureAwait(false), "DeleteAsync by PK");

                    // Transaction rollback (dispose without Complete)
                    using (var tx = await db.GetTransactionAsync().ConfigureAwait(false))
                    {
                        await db.InsertAsync(new Person { Name = "Rolled", Age = 99 }).ConfigureAwait(false);
                    }
                    Check(!await db.ExistsAsync<Person>("Name = @0", "Rolled").ConfigureAwait(false), "Transaction rollback on dispose");

                    // Transaction commit
                    using (var tx = await db.GetTransactionAsync().ConfigureAwait(false))
                    {
                        await db.InsertAsync(new Person { Name = "Committed", Age = 98 }).ConfigureAwait(false);
                        tx.Complete();
                    }
                    Check(await db.ExistsAsync<Person>("Name = @0", "Committed").ConfigureAwait(false), "Transaction commit");

                    // Scalar
                    var count = await db.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM Person").ConfigureAwait(false);
                    Check(count == 10, "ExecuteScalarAsync<long>");
                }
            }

            // .NET Core provider-factory guard: providerName ctor must fail with a clear message, not a bare ArgumentException.
            try
            {
                var _ = new Database("Data Source=:memory:", "Microsoft.Data.Sqlite");
                Check(false, "Database(connStr, providerName) throws when factory is not registered");
            }
            catch (InvalidOperationException ex)
            {
                Check(ex.Message.Contains("RegisterFactory"), "Database(connStr, providerName) throws InvalidOperationException mentioning RegisterFactory");
            }

            // ...and works once the host registers the factory.
            DbProviderFactories.RegisterFactory("Microsoft.Data.Sqlite", SqliteFactory.Instance);
            using (var db2 = new Database("Data Source=:memory:", "Microsoft.Data.Sqlite"))
            {
                var one = await db2.ExecuteScalarAsync<long>("SELECT 1").ConfigureAwait(false);
                Check(one == 1, "Database(connStr, providerName) works after DbProviderFactories.RegisterFactory");
            }

            Console.WriteLine();
            Console.WriteLine(_failures == 0 ? "ALL CHECKS PASSED" : _failures + " CHECK(S) FAILED");
            return _failures == 0 ? 0 : 1;
        }
    }
}
