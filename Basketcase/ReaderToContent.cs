namespace Basketcase;
public class ReaderToItem<T> : IReaderConverter<T>
{
  public T Convert(IDataReader rdr) {
    if (typeof(T) == typeof(str)
    || typeof(T) == typeof(int)
    || typeof(T) == typeof(dte)
    || typeof(T) == typeof(double)
    || typeof(T) == typeof(dec)
    || typeof(T) == typeof(long)
    || typeof(T) == typeof(bln))
      return new ReaderToValue<T>().Convert(rdr);
    if (typeof(T).IsClass)
      return new ReaderToClass<T>().Convert(rdr);
    else
      return new ReaderToStruct<T>().Convert(rdr);
  }
}