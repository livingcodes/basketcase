using System.Collections.Generic;
using System.Data;

namespace Basketcase
{
    public class ReaderToStructList<T> : IReaderConverter<List<T>>
    {
        public List<T> Convert(IDataReader reader) {
            var list = new List<T>();
            var properties = typeof(T).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            var columns = new GetColumns().From(reader);
            while (reader.Read()) {
                var item = default(T);
                object obj = item; // box
                foreach (var property in properties) {
                    if (columns.Contains(property.Name))
                        property.SetValue(obj, reader[property.Name]);
                }
                item = (T)obj; // unbox
                list.Add(item);
            }
            return list;
        }
    }
}
