using System.Collections.Generic;
using System.Data;

namespace Basketcase
{
    public interface IRead
    {
        T ReadOne<T>(IDataReader reader);
        List<T> ReadList<T>(IDataReader reader);
    }
}