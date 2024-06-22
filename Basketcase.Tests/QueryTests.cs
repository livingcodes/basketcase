namespace Basketcase.Tests;
[tc]public class QueryTests : BaseTests
{
  [ClassInitialize]
  public static void InitializeClass(TestContext ctx) =>
    initialize();

  [TestInitialize]
  public void RunBeforeEachTest() =>
    crtPostTbl();

  [tm]public void QueryMultiple() {
    var posts = db.Sel<Post>("select * from post");
    t(posts.Count == 4);
  }

  [tm]public void QryMultWPrm() {
    var posts = db
      .Prm("@min", 2)
      .Sel<Post>("select * from post where id > @min");
    t(posts.Count == 2);
  }

  [tm]public void QryMultWPrms() {
    var posts = db.Sel<Post>("select * from post where id > @min and id < @max", 1, 4);
    t(posts.Count == 2);
    t(posts.Exists(p => p.Id == 2));
    t(posts.Exists(p => p.Id == 3));
  }

  [tm]public void QuerySql() {
    var posts = db
      .Sql("select * from post where id > @min")
      .Prm("@min", 1)
      .Sel<Post>();
    t(posts.Count > 0);
  }

  [tm]public void QryPg() {
    var posts = db.Sql("select * from post order by id")
      .Pg(2, 2)
      .Sel<Post>();
    t(posts.Count == 2);

    posts = db
      .Pg(1, 2)
      .Sel<Post>("select * from post where id > @id order by id", 1);
    t(posts.Count == 2);
    t(!posts.Exists(p => p.Id == 1));
  }

  [tm]public void QryOne() {
    var post = db.Sel1<Post>("select top 1 * from post");
    t(post != null);
  }

  [tm]public void QryOneWPrm() {
    var post = db
      .Prm("@min", 2)
      .Sel1<Post>("select top 1 * from post where id > @min");
    t(post.Id > 2);
  }

  [tm]public void QryOneWPrms() {
    var post = db.Sel1<Post>("select top 1 * from post where id > @min and id < @max", 2, 4);
    t(post.Id == 3);
  }

  [tm]public void QryOneById() {
    var post = db.SelById<Post>(2);
    t(post.Id == 2);
  }

  [tm]public void DfltIsSelAll() {
    var posts = db.Sel<Post>();
    t(posts.Count == 4);
  }

  [tm]public void SelTbl() {
    var posts = db.SelTbl("select * from post");
    t(posts.Rows.Count == 4);
    t((int)posts.Rows[0]["Id"] == 1);
    t(posts.Rows[1]["Html"].ToString().StartsWith("Post"));
  }

  [tm]public void SelTblWPrm() {
    var posts = db.SelTbl("select * from post where id > @min", 2);
    t(posts.Rows.Count == 2);
  }

  [tm]public void SelTblSql() {
    var posts = db
      .Sql("select * from post where id > @min")
      .Prm("@min", 1)
      .SelTbl();
    t(posts.Rows.Count == 3);
  }

  [tm]public void CachePost() {
    var posts = db
      .Cache("Top2Posts", 60)
      .Sel<Post>("select top 2 * from post order by id");
    t(posts.Count == 2);

    posts = db
      .Cache("Top2Posts", 60)
      .Sel<Post>("anything");
    t(posts.Count == 2);
  }

  [tm]public void CacheTbl() {
    var posts = db
      .Cache("Top2Posts", 60)
      .SelTbl("select top 2 * from post order by id");
    t(posts.Rows.Count == 2);

    posts = db
      .Cache("Top2Posts", 60)
      .SelTbl("anything");
    t(posts.Rows.Count == 2);
    
    try {
      t((int)posts.Rows[0]["Id"] == 1);
    } catch (System.Exception ex) {
      // Id column is an int but when DataTable is cached it's cached as a long
    }
    t((lng)posts.Rows[0]["Id"] == 1);
    t(System.Convert.ToInt32(posts.Rows[0]["Id"]) == 1);
    t(posts.Rows[1]["Html"].ToString().StartsWith("Post"));
  }
}