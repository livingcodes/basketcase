namespace Basketcase
{
    /// <summary>Define convention database table name</summary>
    public interface ITableName
    {
        /// <summary>Get table name that stores instances of the specified type</summary>
        string Get<T>();
        /// <summary>Get table name that stores instance</summary>
        string Get(object instance);
    }
}
