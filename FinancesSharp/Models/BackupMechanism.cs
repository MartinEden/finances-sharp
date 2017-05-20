using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;

namespace FinancesSharp
{
	public interface IBackupMechanism
	{
		string Extension { get; }
		void DoBackup(string path, Database database);
	}

	public class MSSQLBackup : IBackupMechanism
	{
		public string Extension
		{
			get { return "bak"; }
		}

		public void DoBackup(string path, Database database)
		{
				var connection = database.Connection;
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
	}

	public class MySQLBackup : IBackupMechanism
	{
		public string Extension
		{
			get { return "sql"; }
		}

		public void DoBackup(string path, Database database)
		{
			/*var connString = ConnectionStringHelper.Get("FinancesSharpContext");
			var args = String.Format("-h {0} -P {1} -u {2} --skip-extended-insert {3} > {4}",
				connString["server"], connString["port"], connString["uid"], connString["database"], path);
			var startInfo = new ProcessStartInfo("/usr/bin/mysqldump", args);
			startInfo.UseShellExecute = false;
			var proc = Process.Start(startInfo);
			proc.WaitForExit();
			if (proc.ExitCode != 0)
			{
				throw new Exception("Backup failed with exit code " + proc.ExitCode);
			}*/
		}
	}
}
