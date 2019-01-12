using System;
using System.Data;
using System.Reflection;

namespace Basketcase
{
    public class ReaderToStruct<T> : IReaderConverter<T>
    {
        public T Convert(IDataReader reader) {
            var item = default(T);
            object boxed = item;
            var columns = new GetColumns().From(reader);
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            while (reader.Read()) {
                foreach (var property in properties) {
                    if (columns.Contains(property.Name)) {
                        object value = reader[property.Name];
                        // if dbnull change to c# null
                        if (value == DBNull.Value)
                            value = null;
                        property.SetValue(boxed, value);
                    }
                }
                item = (T)boxed;
                break;
            }
            return item;
        }
    }
}
