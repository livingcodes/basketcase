using System.Data;
using System.Data.SqlClient;

namespace Basketcase.Tests
{
    public class ConnectionFactory : IConnectionFactory
    {
        public ConnectionFactory(string connectionString) {
            this.connectionString = connectionString;
        }
        string connectionString;

        public IDbConnection Create() =>
            new SqlConnection(connectionString);
    }
}