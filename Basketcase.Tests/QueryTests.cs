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

  [tm]public void QuyMultWPrms() {
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
    var post = db.SelOne<Post>("select top 1 * from post");
    t(post != null);
  }

  [tm]public void QryOneWPrm() {
    var post = db
      .Prm("@min", 2)
      .SelOne<Post>("select top 1 * from post where id > @min");
    t(post.Id > 2);
  }

  [tm]public void QryOneWPrms() {
    var post = db.SelOne<Post>("select top 1 * from post where id > @min and id < @max", 2, 4);
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
}