using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancesSharp.Migrate
{
    public class SQLiteDatabase : IDisposable
    {
        private SQLiteConnection conn;
        private string dbPath;

        public SQLiteDatabase(string dbPath)
        {
            this.dbPath = dbPath;
        }

        public IEnumerable<object[]> Query(string queryString)
        {
            getConnection();
            var command = conn.CreateCommand();
            command.CommandText = queryString;
            command.CommandType = CommandType.Text;

            var list = new List<object[]>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var array = new object[reader.FieldCount];
                    reader.GetValues(array);
                    list.Add(array);
                }
            }
            return list;
        }

        private void getConnection()
        {
            if (conn == null)
            {
                conn = new SQLiteConnection(string.Format("Data Source={0}", dbPath));
                conn.Open();
            }
        }

        public void Dispose()
        {
            conn.Dispose();
        }
    }
}
