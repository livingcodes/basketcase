namespace Basketcase;
  using System;
  using System.Reflection;
// if table contains column with property name
//   then set property to column value
// if table does not contain column with property name
//   then ignore property
public class ReaderToClass<T> : IReaderConverter<T>
{
  public T Convert(IDataReader rdr) {
    var itm = default(T);
    var cols = new GetCols().From(rdr);
    var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
    var flds = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public);
    while (rdr.Read()) {
      itm = Activator.CreateInstance<T>();
      foreach (var prop in props) {
        if (cols.Contains(prop.Name)) {
          obj val = rdr[prop.Name];
          // if dbnull change to c# null
          if (val == DBNull.Value)
            val = null;
          prop.SetValue(itm, val);
        }
      }

      foreach (var field in flds) {
        if (cols.Contains(field.Name)) {
          obj val = rdr[field.Name];
          if (val == DBNull.Value)
            val = null;
          field.SetValue(itm, val);
        }
      }
      break;
    }
    return itm;
  }
}