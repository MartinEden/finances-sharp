using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using MySql.Data.Entity;

namespace FinancesSharp.Models
{
	[DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class FinanceDb : DbContext
    {
        public FinanceDb()
			: base("Server=localhost;Port=3306;Database=finances-sharp;Uid=finances-sharp")
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
                string path = getBackupFileName(mostRecentTransaction);
                if (!File.Exists(path))
                {
                    Backup(path);
                }
            }
        }

        private static string getBackupFileName(Transaction mostRecentTransaction)
        {
            var today = DateTime.Today.ToString("yyyy-MM-dd");
            var mostRecent = mostRecentTransaction.Date.ToString("yyyy-MM-dd");
            string name = string.Format("{0} (Most recent transaction - {1}).bak", today, mostRecent);
            var path = Path.Combine(BackupFolder, name);
            return path;
        }

        public void Backup(string path)
        {
            var connection = Database.Connection;
            connection.Open();
            try
            {
                var databaseName = new SqlConnectionStringBuilder(connection.ConnectionString).InitialCatalog;
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format("BACKUP DATABASE [{0}] TO DISK = '{1}'", databaseName, path);
                    command.ExecuteNonQuery();      
                }
            }
            finally
            {
                connection.Close();
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