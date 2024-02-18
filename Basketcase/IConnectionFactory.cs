namespace Basketcase
{
    public interface IConnectionFactory
    {
        IDbConnection Create();
    }
}
