using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using static Basketcase.Table;
namespace Basketcase.Tests;
[tc]public class BaseTests
{
  protected static IDb db;
  protected static ICache cache;

  protected static void initialize() {
    //this.configuration = new ConfigurationBuilder()
    //    .AddJsonFile(@"c:\code\secrets\Tent\settings.json")
    //    .Build();
    //string connectionString = configuration["connectionString"];

    // i wasn't able to figure out how to construct distributed memory cache
    // the IOptions in particular
    // so i used the service provider to build it
    IServiceCollection svcs = new ServiceCollection();
    svcs.AddDistributedMemoryCache();
    var svcPrvdr = svcs.BuildServiceProvider();
    var distCache = svcPrvdr.GetService<IDistributedCache>();
    cache = new SerializedCache(distCache);

    var conStr = "server=(LocalDb)\\MSSQLLocalDB; database=Basketcase; trusted_connection=true;";

    db = new Db(
      new ConFct(conStr),
      new Reader(),
      cache,
      new TblNm_ClsNm()
    );
  }

  protected static void crtTbl(str tblNm) {
    var sql = new Table(tblNm)
      .AddCol("Id", SqlType.Int, Syntax.Identity(1, 1))
      .AddCol("Html", SqlType.VarChar(200))
      .AddCol("DateCreated", SqlType.DateTime)
      .End()
      .Sql;
    db.Exe(sql);
  }

  protected static void crtPostTbl() {
    crtTbl("Post");
    for (var i = 0; i < 4; i++)
      db.Ins(new Post { Html = $"Post {i}" });
  }

  protected void t(bln condition) {
    Assert.IsTrue(condition);
  }

  public class Post
  {
    public int Id { get; set; }
    public str Html { get; set; }
    public dte DateCreated = dte.Now;
  }
}