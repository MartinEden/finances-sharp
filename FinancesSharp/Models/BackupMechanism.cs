using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

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
		private string executablePath
		{
			get { return System.Configuration.ConfigurationManager.AppSettings["MySQLDumpPath"]; }
		}

		public void DoBackup(string path, Database database)
		{
			var connString = ConnectionStringHelper.Get("FinanceDb");
			var args = String.Format("-h {0} -P {1} -u {2} --password={2} --no-tablespaces --complete-insert {3}",
				connString["server"], connString["port"], connString["uid"], connString["database"]);
						
			var startInfo = new ProcessStartInfo(executablePath, args)
			{
			    UseShellExecute = false,
			    RedirectStandardOutput = true,
				RedirectStandardError = true,
			};
			var proc = Process.Start(startInfo);
			using (var outStream = new StreamWriter(path))
			{
				copyToOutputStream(proc.StandardOutput, outStream);
			}
			proc.WaitForExit();

			if (proc.ExitCode != 0)
			{
				throw new Exception(string.Format("Backup failed with exit code {0}. Error output: {1}", proc.ExitCode, proc.StandardError.ReadToEnd()));
			}
		}

		private void copyToOutputStream(StreamReader inputStream, StreamWriter outputStream)
		{
			string line = null;
			while ((line = inputStream.ReadLine()) != null)
			{
				outputStream.WriteLine(line);
			}
			outputStream.Write(inputStream.ReadToEnd());
		}
	}
}
