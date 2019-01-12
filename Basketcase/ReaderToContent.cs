using System;
using System.Data;

namespace Basketcase
{
    public class ReaderToItem<T> : IReaderConverter<T>
    {
        public T Convert(IDataReader reader) {
            if (typeof(T) == typeof(string)
            || typeof(T) == typeof(int)
            || typeof(T) == typeof(DateTime)
            || typeof(T) == typeof(double)
            || typeof(T) == typeof(decimal)
            || typeof(T) == typeof(long)
            || typeof(T) == typeof(bool))
                return new ReaderToValue<T>().Convert(reader);
            if (typeof(T).IsClass)
                return new ReaderToClass<T>().Convert(reader);
            else
                return new ReaderToStruct<T>().Convert(reader);
        }
    }
}
