using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Basketcase
{
    public class ReaderToClassList<T> : IReaderConverter<List<T>>
    {
        public List<T> Convert(IDataReader reader) {
            var list = new List<T>();
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var columns = new GetColumns().From(reader);
            while (reader.Read()) {
                var item = System.Activator.CreateInstance<T>();
                foreach (var property in properties) {
                    if (columns.Contains(property.Name))
                        property.SetValue(item, reader[property.Name]);
                }
                list.Add(item);
            }
            return list;
        }
    }
}
