namespace Basketcase;
public partial class Db : IDb
{
  public Db(
    IConFct conFct,
    IRead rd,
    ICache cache,
    ITblNm tblNm = null
  ) {
    this.rd = rd;
    this.conFct = conFct;
    this.cache = cache;
    this.tblNm = tblNm ?? new TblNm_ClsNm();
    this.getPrmNmsFrmSql = new GetPrmNmsFrmSql();
    this.prms = new List<(str name, obj val)>();
  }
  public IConFct conFct { get; private set; }
  IRead rd;
  ICache cache;
  ITblNm tblNm;
  GetPrmNmsFrmSql getPrmNmsFrmSql;

  /// <summary>Insert content. Return new ID and rows affected.</summary>
  /// <param name="inst">Content to insert</param>
  /// <returns>New ID and rows affected</returns>
  public (int id, int rowCnt) Ins<T>(T inst) {
    var con = conFct.Crt();
    SqlCommand cmd = null;
    int rowCnt = 0;
    IDataReader rdr = null;
    int id = -1;
    try {
      con.Open();
      cmd = (SqlCommand)con.CreateCommand();
      var sqlBldr = new SqlBldr<T>(inst, cmd, this, cache, tblNm);
      var sql = sqlBldr.BldInsSql();
      cmd.CommandText = sql;
      rdr = cmd.ExecuteReader();
      rowCnt = rdr.RecordsAffected;
      id = (int)this.rd.ReadOne<dec>(rdr); // had to get @@IDENTITY as decimal and then convert to int
    } finally {
      if (cmd != null)
        cmd.Dispose();
      if (con.State != ConnectionState.Closed)
        con.Close();
    }
    return (id, rowCnt);
  }

  /// <summary>Updates content and returns number of rows affected</summary>
  public int Upd<T>(T inst) {
    var con = conFct.Crt();
    SqlCommand cmd = null;
    int rowCnt = 0;

    try {
      con.Open();
      cmd = (SqlCommand)con.CreateCommand();
      ISqlBldr sqlBldr = new SqlBldr<T>(inst, cmd, this, cache, tblNm);
      var sql = sqlBldr.BldUpdSql();
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

  public int Del<T>(int id) {
    str tbl = tblNm.Get<T>();
    int rowCnt = 0;
    var con = conFct.Crt();
    SqlCommand cmd = null;
    try {
      con.Open();
      cmd = (SqlCommand)con.CreateCommand();
      cmd.CommandText = $"DELETE FROM {tbl} WHERE id = {id}";
      rowCnt = cmd.ExecuteNonQuery();
    } finally {
      if (cmd != null)
        cmd.Dispose();
      if (con.State != ConnectionState.Closed)
        con.Close();
    }
    return rowCnt;
  }

  public int Exe(str sql, params obj[] prms) {
    var con = conFct.Crt();
    int rowCnt = -1;
    IDbCommand cmd = null;
    try {
      con.Open();
      cmd = con.CreateCommand();
      cmd.CommandText = sql;
      var prmNm = new GetPrmNmsFrmSql().Exe(sql);
      if (prms.Length != prmNm.Count)
        throw new Ex($"Parameter name and value counts are not equal. Parameter name count: {prmNm.Count}, Parameter value count: {prms.Length}");
      for (int i = 0; i < prms.Length; i++) {
        var prm = cmd.CreateParameter();
        prm.ParameterName = prmNm[i];
        prm.Value = prms[i];
        cmd.Parameters.Add(prm);
      }
      rowCnt = cmd.ExecuteNonQuery();
    } finally {
      if (cmd != null)
        cmd.Dispose();
      if (con.State != ConnectionState.Closed)
        con.Close();
    }
    return rowCnt;
  }

  public IAdminDb Admin { get {
    if (_admin == null)
      _admin = new AdminDb(this);
    return _admin;
  } }
  IAdminDb _admin;
}