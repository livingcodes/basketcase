using System.Data;

namespace Basketcase
{
    public interface IConnectionFactory
    {
        IDbConnection Create();
    }
}
