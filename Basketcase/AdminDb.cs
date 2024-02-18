namespace Basketcase;
public class AdminDb : IAdminDb
{
  public AdminDb(IDb db) {
    this.db = db;
  }
  IDb db;

  public void DropTbl(str tblNm) =>
    db.Sel<int>($"DROP TABLE {tblNm}");

  public void Trun(str tblNm) =>
    db.Exe($"TRUNCATE TABLE {tblNm}");

  public int ExeRaw(str sql) {
    var con = ((Db)db).conFct.Crt();
    int rowCnt = -1;
    IDbCommand cmd = null;
    try {
      con.Open();
      cmd = con.CreateCommand();
      cmd.CommandText = sql;
      rowCnt = cmd.ExecuteNonQuery();
    } finally {
      if (cmd != null)
        cmd.Dispose();
      if (con.State != ConnectionState.Closed)
        con.Close();
    }
    return rowCnt;
  }
}