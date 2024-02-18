namespace Basketcase;
  using System.Reflection;
public class ReaderToClassList<T> : IReaderConverter<List<T>>
{
  public List<T> Convert(IDataReader rdr) {
    var ls = new List<T>();
    var flds = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public);
    var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
    var cols = new GetCols().From(rdr);
    while (rdr.Read()) {
      var item = System.Activator.CreateInstance<T>();
      foreach (var prop in props)
        if (cols.Contains(prop.Name)) {
          var value = rdr[prop.Name];
          if (value != System.DBNull.Value)
            prop.SetValue(item, value);
        }

      foreach (var fld in flds)
        if (cols.Contains(fld.Name))
          if (rdr[fld.Name] != System.DBNull.Value)
            fld.SetValue(item, rdr[fld.Name]);
      ls.Add(item);
    }
    return ls;
  }
}