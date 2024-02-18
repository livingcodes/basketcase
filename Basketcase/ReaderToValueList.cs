namespace Basketcase;
public class ReaderToValueList<T> : IReaderConverter<List<T>>
{
  public List<T> Convert(IDataReader rdr) {
    var ls = new List<T>();
    while (rdr.Read()) {
      ls.Add((T)rdr[0]);
    }
    return ls;
  }
}