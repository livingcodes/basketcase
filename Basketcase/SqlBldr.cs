namespace Basketcase;
  using System;
  using System.Reflection;
public class SqlBldr<T> : ISqlBldr
{
  public SqlBldr(T inst, SqlCommand cmd, IDb db, ICache cache, ITblNm tblNm = null) {
    this.inst = inst;
    this.cmd = cmd;
    this.db = db;
    this.cache = cache;
    this.tblNm = tblNm ?? new TblNm_ClsNm();
  }
  T inst;
  SqlCommand cmd;
  IDb db;
  ICache cache;
  ITblNm tblNm;

  public str BldInsSql() {
    var tblNm = this.tblNm.Get(inst);
    var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
    var tblCols = new GetCols().From(tblNm, db, cache);
    var colNs = "";
    var vals = "";

    foreach (var prop in props) {
      if (prop.Name.ToUpper() == "ID")
        continue;
      if (!tblCols.Contains(prop.Name))
        continue;

      var val = prop.GetValue(inst);
      if (val is null)
        val = DBNull.Value;
      colNs += prop.Name + ", ";

      cmd.Parameters.AddWithValue("@" + prop.Name, val);
      vals += "@" + prop.Name + ", ";
    }

    var flds = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public);
    foreach (var fld in flds) {
      if (fld.Name.ToUpper() == "ID")
        continue;
      if (!tblCols.Contains(fld.Name))
        continue;
      var value = fld.GetValue(inst);
      if (value is null)
        value = DBNull.Value;
      colNs += fld.Name + ", ";

      cmd.Parameters.AddWithValue("@" + fld.Name, value);
      vals += "@" + fld.Name + ", ";
    }

    colNs = colNs.Substring(0, colNs.Length - 2);
    vals = vals.Substring(0, vals.Length - 2);
    str sql = $@"INSERT INTO [{tblNm}] ({colNs}) VALUES ({vals})
            SELECT @@IDENTITY Id";
    return sql;
  }

  public str BldUpdSql() {
    var tblNm = this.tblNm.Get(inst);
    var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
    var flds = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public);
    var tblCols = new GetCols().From(tblNm, db, cache);
    var setters = "";
    var id = "0";
    foreach (var prop in props) {
      if (prop.Name.ToUpper() == "ID") {
        var objId = prop.GetValue(inst);
        if (objId == null)
          throw new Ex($"Cannot update {tblNm} because ID is null");
        id = objId.ToString();
        //id = property.GetValue(instance).ToStringOr("0");
        cmd.Parameters.AddWithValue("@Id", id);
        continue;
      }
      if (!tblCols.Contains(prop.Name))
        continue;

      var val = prop.GetValue(inst);
      if (val is null)
        val = DBNull.Value;

      setters += $"{prop.Name} = @{prop.Name}, ";

      cmd.Parameters.AddWithValue("@" + prop.Name, val);
    }
    foreach (var fld in flds) {
      if (fld.Name.ToUpper() == "ID") {
        var objId = fld.GetValue(inst);
        if (objId == null)
          throw new Ex($"Cannot update {tblNm} because ID is null");
        id = objId.ToString();
        cmd.Parameters.AddWithValue("@Id", id);
        continue;
      }
      if (!tblCols.Contains(fld.Name))
        continue;
      var val = fld.GetValue(inst);
      if (val == null)
        val = DBNull.Value;
      setters += $"{fld.Name} = @{fld.Name}, ";
      cmd.Parameters.AddWithValue("@" + fld.Name, val);
    }
    setters = setters.Substring(0, setters.Length - 2);
    var sql = $@"UPDATE [{tblNm}] SET {setters} WHERE Id = @Id";
    return sql;
  }
}