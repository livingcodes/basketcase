using System;
using System.Collections.Generic;
using System.Data;

namespace Basketcase
{
    public class ReaderToList<T> : IReaderConverter<List<T>>
    {
        public List<T> Convert(IDataReader reader) {
            if (typeof(T) == typeof(string)
            ||  typeof(T) == typeof(int)
            ||  typeof(T) == typeof(DateTime)
            ||  typeof(T) == typeof(double)
            ||  typeof(T) == typeof(decimal)
            ||  typeof(T) == typeof(long)
            ||  typeof(T) == typeof(bool))
                return new ReaderToValueList<T>().Convert(reader);
            else if (typeof(T).IsClass)
                return new ReaderToClassList<T>().Convert(reader);
            else
                return new ReaderToStructList<T>().Convert(reader);
        }
    }
}