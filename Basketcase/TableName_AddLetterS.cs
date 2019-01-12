namespace Basketcase
{
    /// <summary>Table name equals type name with letter s appended</summary>
    public class TableName_AddLetterS : ITableName
    {
        public string Get<T>() => typeof(T).Name + "s";
        public string Get(object instance) => instance.GetType().Name + "s";
    }
}