namespace Basketcase;
public class ReaderToList<T> : IReaderConverter<List<T>>
{
  public List<T> Convert(IDataReader rdr) {
    if (typeof(T) == typeof(str)
    || typeof(T) == typeof(int)
    || typeof(T) == typeof(dte)
    || typeof(T) == typeof(double)
    || typeof(T) == typeof(dec)
    || typeof(T) == typeof(long)
    || typeof(T) == typeof(bool))
      return new ReaderToValueList<T>().Convert(rdr);
    else if (typeof(T).IsClass)
      return new ReaderToClassList<T>().Convert(rdr);
    else
      return new ReaderToStructList<T>().Convert(rdr);
  }
}