using MySql.Data.Entity;
using System;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;

namespace FinancesSharp.Models.Migration
{
    public class BTreeMigrationSqlGenerator : MySqlMigrationSqlGenerator
    {
        public BTreeMigrationSqlGenerator() : base() { }

        /// <summary>
        /// we want BTREE because HASH is not correct for normal Keys on MySQL 8
        /// See also:
        /// https://stackoverflow.com/questions/51140727/ef6-incorrect-usage-of-spatial-fulltext-hash-index-and-explicit-index-order
        /// https://bugs.mysql.com/bug.php?id=91938
        /// </summary>
        protected override MigrationStatement Generate(CreateIndexOperation op)
        {
            MigrationStatement migrationStatement = base.Generate(op);
            System.Diagnostics.Trace.WriteLine(migrationStatement.Sql);
            string sql = migrationStatement.Sql.TrimEnd();

            if (sql.EndsWith("using HASH", StringComparison.OrdinalIgnoreCase))
            {
                migrationStatement.Sql = sql.Replace("using HASH", "using BTREE");
            }
            return migrationStatement;
        }
    }
}