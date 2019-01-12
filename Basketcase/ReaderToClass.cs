using System;
using System.Data;
using System.Reflection;

namespace Basketcase
{
    // if table contains column with property name
    //   then set property to column value
    // if table does not contain column with property name
    //   then ignore property
    public class ReaderToClass<T> : IReaderConverter<T>
    {
        public T Convert(IDataReader reader) {
            var item = default(T);
            var columns = new GetColumns().From(reader);
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            while (reader.Read()) {
                item = Activator.CreateInstance<T>();
                foreach (var property in properties) {
                    if (columns.Contains(property.Name)) {
                        object value = reader[property.Name];
                        // if dbnull change to c# null
                        if (value == DBNull.Value)
                            value = null;
                        property.SetValue(item, value);
                    }
                }
                break;
            }
            return item;
        }
    }
}
