namespace Basketcase;
public class GetCols
{
  public List<str> From(IDataReader rdr) {
    var colNmLs = new List<str>();
    var cnt = rdr.FieldCount;
    for (int i = 0; i < cnt; i++) {
      var colNm = rdr.GetName(i);
      colNmLs.Add(colNm);
    }
    return colNmLs;
  }

  public List<str> From(str tblNm, IDb db, ICache cache) {
    // get from cache
    var colNmLs = cache == null
        ? null
        : cache.Get<List<str>>($"ColumnsFor{tblNm}");
    if (colNmLs == null) {
      // get from database
      colNmLs = db.Sel<str>(
        $@"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = '{tblNm}'"
      );
      if (cache != null)
        cache.Set($"ColumnsFor{tblNm}", colNmLs, 60);
    }
    return colNmLs;
  }
}