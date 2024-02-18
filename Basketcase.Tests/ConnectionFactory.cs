using System.Data;

namespace Basketcase.Tests
{
    public class ConnectionFactory : IConnectionFactory
    {
        public ConnectionFactory(string connectionString) {
            this.connectionString = connectionString;
        }
        string connectionString;

        public IDbConnection Create() =>
            new Microsoft.Data.SqlClient.SqlConnection(connectionString);
    }
}