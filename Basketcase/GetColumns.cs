using System.Collections.Generic;
using System.Data;

namespace Basketcase
{
    public class GetColumns
    {
        public List<string> From(IDataReader reader) {
            var columnNameList = new List<string>();
            var count = reader.FieldCount;
            for (var i = 0; i < count; i++) {
                var columnName = reader.GetName(i);
                columnNameList.Add(columnName);
            }
            return columnNameList;
        }

        public List<string> From(string tableName, IDb db, ICache cache) {
            // get from cache
            var columnNameList = cache == null
                ? null
                : cache.Get<List<string>>($"ColumnsFor{tableName}");
            if (columnNameList == null) {
                // get from database
                columnNameList = db.Select<string>(
                    $@"SELECT COLUMN_NAME FROM Tent.INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = '{tableName}'"
                );
                if (cache != null)
                    cache.Set($"ColumnsFor{tableName}", columnNameList, 60);
            }
            return columnNameList;
        }
    }
}
