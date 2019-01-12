namespace Basketcase
{
    public interface ISqlBuilder
    {
        string BuildInsertSql();
        string BuildUpdateSql();
    }
}
