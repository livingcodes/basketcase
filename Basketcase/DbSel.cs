namespace Basketcase;
public partial class Db
{
  public IDb Sproc(str name) {
    sprocName = name;
    return this;
  }
  str sprocName;

  public bln IsSproc => sprocName != null;

  public List<T> Sel<T>(str sql = null, params obj[] prms) {
    // get from cache
    if (cacheKey != null) {
      var cacheVal = cache.Get<List<T>>(cacheKey);
      if (cacheVal != null)
        return (List<T>)cacheVal;
    }

    // sql syntax
    if (sql == null || sql == "")
      sql = this.sql;

    // if sql is not set then select all
    if (sql == null || sql == "") {
      var tblNm = this.tblNm.Get<T>();
      sql = $"SELECT * FROM [{tblNm}]";
    }

    if (!IsSproc && (
       sql.ToUpper().StartsWith("WHERE ")
    || sql.ToUpper().StartsWith("GROUP BY ")
    || sql.ToUpper().StartsWith("ORDER BY ")
    )) {
      var tblNm = this.tblNm.Get<T>();
      //sql = sql ?? query.Sql();
      var sqlStart = $"SELECT * FROM [{tblNm}] ";
      sql = sqlStart + sql;
    }

    // parameters
    if (!IsSproc) {
      var prmNms = getPrmNmsFrmSql.Exe(sql);
      // todo: are count and length checks necessary
      if (prmNms.Count > 0) {
        if (prms.Length > 0) {
          if (prms.Length != prmNms.Count)
            throw new Ex($"Parameter name and value counts are not equal. Parameter name count: {prmNms.Count}, Parameter value count: {prms.Length}");
          for (var i = 0; i < prms.Length; i++)
            Prm(prmNms[i], prms[i]);
        }
      }
    }

    if (!IsSproc && hasPg)
      sql += pgSql;
    if (hasPg) {
      Prm("@PageNumber", pgNum);
      Prm("@PageSize", pgSz);
    }

    var list = sel<T>(sql);

    setCache(list);
    setQryToNul();
    return list;
  }

  public T SelOne<T>(str sql = null, params obj[] prms) {
    // get from cache
    if (cacheKey != null) {
      var cacheVal = cache.Get<T>(cacheKey);
      if (cacheVal != null) {
        setQryToNul();
        return (T)cacheVal;
      }
    }

    // sql syntax
    if (sql == null || sql == "")
      sql = this.sql;

    if (!IsSproc && (
       sql.ToUpper().StartsWith("WHERE ")
    || sql.ToUpper().StartsWith("GROUP BY ")
    || sql.ToUpper().StartsWith("ORDER BY ")
    )) {
      var tblNm = this.tblNm.Get<T>();
      //sql = sql ?? query.Sql();
      var sqlStart = $"SELECT TOP 1 * FROM [{tblNm}] ";
      sql = sqlStart + sql;
    }

    if (!IsSproc && hasPg)
      sql += pgSql;

    // parameters
    if (!IsSproc) {
      var prmNms = getPrmNmsFrmSql.Exe(sql);
      // todo: are count and length checks necessary
      if (prmNms.Count > 0) {
        if (prms.Length > 0) {
          if (prms.Length != prmNms.Count)
            throw new Ex($"Parameter name and value counts are not equal. Parameter name count: {prmNms.Count}, Parameter value count: {prms.Length}");
          for (var i = 0; i < prms.Length; i++)
            Prm(prmNms[i], prms[i]);
        }
      }
      if (hasPg) {
        Prm("@PageNumber", pgNum);
        Prm("@PageSize", pgSz);
      }
    }

    var content = selOne<T>(sql);

    setCache(content);
    setQryToNul();
    return content;
  }

  public T SelById<T>(int id) =>
    Prm("@Id", id)
    .SelOne<T>($"WHERE Id = @Id");

  List<T> sel<T>(str sql) {
    var ls = new List<T>();
    var con = conFct.Crt();
    IDbCommand cmd = null;
    try {
      con.Open();
      cmd = con.CreateCommand();

      if (sprocName != null) {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = sprocName;
      } else {
        cmd.CommandText = sql;
      }
      foreach (var prm in prms) {
        var p = cmd.CreateParameter();
        p.ParameterName = prm.name;
        p.Value = prm.val;
        cmd.Parameters.Add(p);
      }
      var rdr = cmd.ExecuteReader();
      ls = rd.ReadLs<T>(rdr);
    } finally {
      if (cmd != null)
        cmd.Dispose();
      if (con.State != ConnectionState.Closed)
        con.Close();
    }
    return ls;
  }

  T selOne<T>(str sql) {
    var content = default(T);
    var con = conFct.Crt();
    IDbCommand cmd = null;
    try {
      con.Open();
      cmd = con.CreateCommand();

      if (sprocName != null) {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = sprocName;
      } else {
        cmd.CommandText = sql;
      }
      foreach (var prm in prms) {
        var p = cmd.CreateParameter();
        p.ParameterName = prm.name;
        p.Value = prm.val;
        cmd.Parameters.Add(p);
      }
      var rdr = cmd.ExecuteReader();
      content = rd.ReadOne<T>(rdr);
    } finally {
      if (cmd != null)
        cmd.Dispose();
      if (con.State != ConnectionState.Closed)
        con.Close();
    }
    return content;
  }

  public IDb Sql(str sql) {
    this.sql = sql;
    return this;
  }
  string sql;

  public IDb Prm(str name, obj val) {
    prms.Add((name, val));
    return this;
  }
  List<(str name, obj val)> prms;

  /// <summary>Setup paging. 
  /// Example: If 100 rows then Paging(2, 10) would return rows 11-20.
  /// SQL must contain order by statement; otherwise, exception thrown.</summary>
  /// <param name="num">Page number: only rows from this page number are returned. First page is 1 (i.e. not zero-based).</param>
  /// <param name="sz">Page size (i.e. take) controls number of rows on a page.</param>
  public IDb Pg(int num, int sz) {
    hasPg = true;
    pgNum = num;
    pgSz = sz;
    pgSql = @"
              OFFSET((@PageNumber - 1) * @PageSize) ROWS
              FETCH NEXT @PageSize ROWS ONLY";
    return this;
  }
  bln hasPg;
  int pgNum, pgSz;
  str pgSql;

  void setQryToNul() {
    sql = "";
    hasPg = false;
    pgNum = 0;
    pgSz = 0;
    prms = new List<(str name, obj val)>();
    cacheKey = null;
  }

  public IDb Cache(str key, dte exp) {
    cacheKey = key;
    var duration = exp.Subtract(dte.Now);
    cacheSec = (int)duration.TotalSeconds;
    return this;
  }
  public IDb Cache(str key, int sec) {
    cacheKey = key;
    cacheSec = sec;
    return this;
  }
  //public IDb Cache(DateTime expiration) => Cache(null, expiration);
  //public IDb Cache(int seconds) => Cache(null, seconds);
  str cacheKey;
  int cacheSec;

  void setCache(obj obj) {
    if (cacheKey != null) {
      cache.Set(cacheKey, obj, cacheSec);
      cacheKey = null;
      cacheSec = 60;
    }
  }
}