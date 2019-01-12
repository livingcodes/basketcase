using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Basketcase
{
    public class AdminDb : IAdminDb
    {
        public AdminDb(IDb db) {
            this.db = db;
        }
        IDb db;

        public void DropTable(string tableName) =>
            db.Select<int>($"drop table {tableName}");

        public void Truncate(string tableName) =>
            db.Execute($"truncate table {tableName}");

        public int ExecuteRaw(string sql) {
            var connection = ((Db)db).connectionFactory.Create();
            int affectedRows = -1;
            IDbCommand command = null;
            try {
                connection.Open();
                command = connection.CreateCommand();
                command.CommandText = sql;
                affectedRows = command.ExecuteNonQuery();
            } finally {
                if (command != null)
                    command.Dispose();
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
            }
            return affectedRows;
        }
    }
}
