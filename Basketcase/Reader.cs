namespace Basketcase;
public class Reader : IRead
{
  public T ReadOne<T>(IDataReader rdr) =>
    new ReaderToItem<T>().Convert(rdr);

  public List<T> ReadLs<T>(IDataReader rdr) =>
    new ReaderToList<T>().Convert(rdr);
}