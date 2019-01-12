using System.Collections.Generic;
using System.Data;

namespace Basketcase
{
    public class ReaderToValueList<T> : IReaderConverter<List<T>>
    {
        public List<T> Convert(IDataReader reader) {
            var list = new List<T>();
            while (reader.Read()) {
                list.Add((T)reader[0]);
            }
            return list;
        }
    }
}
