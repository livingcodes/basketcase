namespace Basketcase;
public class ReaderToStructList<T> : IReaderConverter<List<T>>
{
  public List<T> Convert(IDataReader rdr) {
    var ls = new List<T>();
    var props = typeof(T).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
    var cols = new GetCols().From(rdr);
    while (rdr.Read()) {
      var itm = default(T);
      obj obj = itm; // box
      foreach (var prop in props) {
        if (cols.Contains(prop.Name))
          prop.SetValue(obj, rdr[prop.Name]);
      }
      itm = (T)obj; // unbox
      ls.Add(itm);
    }
    return ls;
  }
}