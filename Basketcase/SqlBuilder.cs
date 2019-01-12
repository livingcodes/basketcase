using System;
using System.Data.SqlClient;

namespace Basketcase
{
    public class SqlBuilder<T> : ISqlBuilder
    {
        public SqlBuilder(T instance, SqlCommand command, IDb db, ICache cache, ITableName tableName = null) {
            this.instance = instance;
            this.command = command;
            this.db = db;
            this.cache = cache;
            this.tableName = tableName ?? new TableName_ClassName();
        }
        T instance;
        SqlCommand command;
        IDb db;
        ICache cache;
        ITableName tableName;

        public string BuildInsertSql() {
            var tableName = this.tableName.Get(instance);
            var properties = typeof(T).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            var tableColumns = new GetColumns().From(tableName, db, cache);
            var columnNames = "";
            var values = "";
            
            foreach (var property in properties) {
                if (property.Name.ToUpper() == "ID")
                    continue;
                if (!tableColumns.Contains(property.Name))
                    continue;

                var value = property.GetValue(instance);
                if (value is null)
                    value = DBNull.Value;
                columnNames += property.Name + ", ";
                
                command.Parameters.AddWithValue("@" + property.Name, value);
                values += "@" + property.Name + ", ";
            }
            columnNames = columnNames.Substring(0, columnNames.Length - 2);
            values = values.Substring(0, values.Length - 2);
            var sql = $@"INSERT INTO [{tableName}] ({columnNames}) VALUES ({values})
            SELECT @@IDENTITY Id";
            return sql;
        }

        public string BuildUpdateSql() {
            var tableName = this.tableName.Get(instance);
            var properties = typeof(T).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            var tableColumns = new GetColumns().From(tableName, db, cache);
            var setters = "";
            var id = "0";
            foreach (var property in properties) {
                if (property.Name.ToUpper() == "ID") {
                    var objId = property.GetValue(instance);
                    id = objId == null ? "0" : objId.ToString();
                    //id = property.GetValue(instance).ToStringOr("0");
                    continue;
                }
                if (!tableColumns.Contains(property.Name))
                    continue;

                var value = property.GetValue(instance);
                if (value is null)
                    value = DBNull.Value;
                
                setters += $"{property.Name} = @{property.Name}, ";

                command.Parameters.AddWithValue("@" + property.Name, value);
            }
            setters = setters.Substring(0, setters.Length - 2);
            var sql = $@"update [{tableName}] set {setters} where id = {id}";
            return sql;
        }
    }
}