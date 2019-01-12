using System.Data;

namespace Basketcase
{
    public interface IReaderConverter<T>
    {
        T Convert(IDataReader reader);
    }
}
