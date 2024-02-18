namespace Basketcase;
public interface IRead
{
  T ReadOne<T>(IDataReader rdr);
  List<T> ReadLs<T>(IDataReader rdr);
}