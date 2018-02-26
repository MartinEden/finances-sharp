using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using MySql.Data.Entity;

namespace FinancesSharp.Models
{
	[DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class FinanceDb : DbContext
    {
        public FinanceDb()
        {
            Database.SetInitializer<FinanceDb>(new DropCreateDatabaseIfModelChanges<FinanceDb>());
        }

        public virtual DbSet<Card> Cards { get; set; }
        public virtual DbSet<CategorisationRule> CategorisationRules { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Exclusion> Exclusions { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<Search> Searches { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<TransactionType> TransactionTypes { get; set; }

        public IQueryable<CategorisationRule> ActiveRules
        {
            get
            {
                return CategorisationRules
                    .Include(x => x.Category)
                    .Include(x => x.Criteria)
                    .Where(x => x.Active);
            }
        }
        public IQueryable<Category> TopLevelCategories
        {
            get
            {
                return Categories.Include(x => x.Parent).Where(c => c.Parent == null);
            }
        }
        
        public void Backup()
        {
            var mostRecentTransaction = Transactions.OrderByDescending(x => x.Date).FirstOrDefault();
            if (mostRecentTransaction != null)
            {
				var backup = getBackupMechanism();
				string path = getBackupFileName(mostRecentTransaction, backup.Extension);
                if (!File.Exists(path))
                {
					backup.DoBackup(path, Database);
                }
            }
        }

        private static string getBackupFileName(Transaction mostRecentTransaction, string extension)
        {
            var today = DateTime.Today.ToString("yyyy-MM-dd");
            var mostRecent = mostRecentTransaction.Date.ToString("yyyy-MM-dd");
			string name = string.Format("{0} (Most recent transaction - {1}).{2}", today, mostRecent, extension);
            var path = Path.Combine(BackupFolder, name);
            return path;
        }

		private IBackupMechanism getBackupMechanism()
        {
			var dialect = WebConfigurationManager.AppSettings["SQLDialect"];
			if (dialect == "MySQL")
			{
				return new MySQLBackup();
			}
			else if (dialect == "MSSQL")
			{
				return new MSSQLBackup();
			}
			else
			{
				throw new Exception("Unknown SQL dialect: " + dialect);
			}
        }
        public static string BackupFolder
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["DatabaseBackupPath"];
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Category.OnModelCreating(modelBuilder);
        }
    }
}