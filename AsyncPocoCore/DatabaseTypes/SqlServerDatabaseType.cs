// AsyncPoco is a fork of PetaPoco and is bound by the same licensing terms.
// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.

using System;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AsyncPoco.Internal;

namespace AsyncPoco.DatabaseTypes
{
	class SqlServerDatabaseType : DatabaseType
	{




        public override string BuildPageQuery(long skip, long take, PagingHelper.SQLParts parts, ref object[] args)
		{

            /* all sql */
            //{
            //    parts.sqlSelectRemoved = PagingHelper.rxOrderBy.Replace(parts.sqlSelectRemoved, "", 1);
            //    if (PagingHelper.rxDistinct.IsMatch(parts.sqlSelectRemoved))
            //    {
            //        parts.sqlSelectRemoved = "peta_inner.* FROM (SELECT " + parts.sqlSelectRemoved + ") peta_inner";
            //    }

            //    var sqlPage = string.Format(
            //        "SELECT * FROM (SELECT ROW_NUMBER() OVER ({0}) peta_rn, {1}) peta_paged WHERE peta_rn>@{2} AND peta_rn<=@{3}",
            //        parts.sqlOrderBy == null ? "ORDER BY (SELECT NULL)" : parts.sqlOrderBy, parts.sqlSelectRemoved,
            //        args.Length, args.Length + 1);
            //    args = args.Concat(new object[] {skip, skip + take}).ToArray();
            //    return sqlPage;
            //}

            /* sql 2012+ */
            {
                var sqlPage = parts.sql;

                if (parts.sqlOrderBy == null)
                    sqlPage = string.Format("{0}\n{1}", sqlPage, parts.sqlOrderBy == null ? "ORDER BY (SELECT NULL)" : parts.sqlOrderBy);

                sqlPage = string.Format("{0}\nOFFSET (@{1}) ROWS\nFETCH NEXT (@{2}) ROWS ONLY", sqlPage, args.Length, args.Length+1);
                args = args.Concat(new object[] {skip, take}).ToArray();
                return sqlPage;
            }

        }

		public override Task<object> ExecuteInsertAsync(Database db, DbCommand cmd, string PrimaryKeyName)
		{
		    cmd.CommandText += ";\nSELECT scope_identity() AS NewID;";
            return db.ExecuteScalarHelperAsync(cmd);
		}

		public override string GetExistsSql()
		{
			return "IF EXISTS (SELECT 1 FROM [{0}] WHERE {1}) SELECT 1 ELSE SELECT 0";
		}

		public override string GetInsertOutputClause(string primaryKeyName)
		{
			return String.Format(" OUTPUT INSERTED.[{0}]", primaryKeyName);
		}
	}

}
