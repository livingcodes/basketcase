namespace Basketcase;
  using System;
  using System.Reflection;
public class ReaderToStruct<T> : IReaderConverter<T>
{
  public T Convert(IDataReader rdr) {
    var itm = default(T);
    obj boxed = itm;
    var cols = new GetCols().From(rdr);
    var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
    while (rdr.Read()) {
      foreach (var prop in props) {
        if (cols.Contains(prop.Name)) {
          obj val = rdr[prop.Name];
          // if dbnull change to c# null
          if (val == DBNull.Value)
            val = null;
          prop.SetValue(boxed, val);
        }
      }
      itm = (T)boxed;
      break;
    }
    return itm;
  }
}