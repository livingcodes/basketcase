namespace Basketcase.Tests;
[tc]public class CacheTests : BaseTests
{
  [ClassInitialize]
  public static void InitializeClass(TestContext ctx) {
    initialize();
  }

  [TestInitialize]
  public void RunBeforeEachTest() =>
    crtPostTbl();

  [tm]public void CacheBySeconds() {
    var posts = db
      .Cache(key: "two", sec: 120)
      .Sel<Post>("select * from post where id > 1 and id < 3");
    t(posts.Count == 1);
    t(posts[0].Id == 2);

    // doesn't require sql
    posts = db
      .Cache("two", 120)
      .Sel<Post>();
    t(posts[0].Id == 2);

    // sql can be anything
    posts = db
      .Cache("two", 120)
      .Sel<Post>("anything");
    t(posts[0].Id == 2);
  }

  [tm]public void CacheByDate() {
    var posts = db
      .Cache("two", dte.Now.AddSeconds(120))
      .Sel<Post>("select * from post where id > 1 and id < 3");
    t(posts[0].Id == 2);

    posts = db
      .Cache("two", 120)
      .Sel<Post>();
    t(posts[0].Id == 2);
  }

  [tm]public void CacheOne() {
    var post = db
      .Cache("CacheSelectOne", 60)
      .Sel1<Post>("select * from post where id = 2");
    t(post.Id == 2);

    var cached = db.Cache("CacheSelectOne", 60)
      .Sel1<Post>("anything");
    t(cached.Id == 2);
  }

  [tm]public void CacheOneById() {
    var post = db
      .Cache("CacheOneById", 60)
      .SelById<Post>(2);
    t(post.Id == 2);

    var cached = db.Cache("CacheOneById", 60)
      .Sel1<Post>("anything");
    t(post.Id == 2);
  }
}