namespace Basketcase;
public class ReaderToValue<T> : IReaderConverter<T>
{
  public T Convert(IDataReader rdr) {
    var val = default(T);
    while (rdr.Read()) {
      val = (T)rdr[0];
      break;
    }
    return val;
  }
}