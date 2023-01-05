using FinancesSharp.Models;
using FinancesSharp.Models.Migration;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace FinancesSharp.Models.Migration
{
    public sealed class FixedMigrationInitializer : DbMigrationsConfiguration<FinanceDb>
    {
        public FixedMigrationInitializer()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            SetSqlGenerator("MySql.Data.MySqlClient", new BTreeMigrationSqlGenerator());
        }
    }
}