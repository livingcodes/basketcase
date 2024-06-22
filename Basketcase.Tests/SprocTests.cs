using System;
using static Basketcase.Table;
namespace Basketcase.Tests;

[tc]public class SprocTests : BaseTests
{
  [ClassInitialize]
  public static void InitializeClass(TestContext ctx) {
    initialize();
  }

  public SprocTests() : base() {
    var tblNm = "Game";
    db.Exe(
      new Table(tblNm)
        .AddCol("Id", SqlType.Int, Syntax.Identity(1, 1))
        .AddCol("Title", SqlType.VarChar(100), Syntax.NotNull)
        .AddCol("Html", SqlType.VarCharMax, Syntax.NotNull)
        .AddCol("DateCreated", SqlType.DateTime, Syntax.NotNull)
        .End().Sql
    );

    var admin = new AdminDb(db);
    admin.ExeRaw($@"DROP PROCEDURE IF EXISTS GetGames");
    admin.ExeRaw($"CREATE PROCEDURE GetGames AS SELECT * FROM {tblNm}");

    admin.ExeRaw($@"DROP PROCEDURE IF EXISTS GetGamesByDateCreated");
    admin.ExeRaw($@"CREATE PROCEDURE GetGamesByDateCreated(@DateCreated DATETIME) AS 
                SELECT * FROM {tblNm} WHERE DateCreated > @DateCreated");

    admin.ExeRaw($@"DROP PROCEDURE IF EXISTS GetGameById");
    admin.ExeRaw(
        $@"CREATE PROCEDURE GetGameById(@Id INT) AS BEGIN
                    SELECT * FROM {tblNm} WHERE Id = @Id
                END");

    db.Ins(new Game() {
      Title = "A",
      Html = "B"
    });
  }

  [tm]public void QrySproc() {
    var games = db.Sproc("GetGames").Sel<Game>();
    t(games.Count > 0);
  }

  [tm]public void QrySprocWPrm() {
    var posts = db.Sproc("GetGamesByDateCreated")
      .Prm("@DateCreated", DateTime.Now.AddDays(-10))
      .Sel<Game>();
    t(posts.Count > 0);
  }

  [tm]public void QrySprocWPrmRetSingle() {
    var post = db.Sproc("GetGameById")
      .Prm("@Id", 1)
      .Sel1<Game>();
    t(post.Id >= 1);
  }

  [tm]public void CacheSproc() {
    var postLs = db.Sproc("GetGamesByDateCreated")
      .Cache("cached", 60 * 5)
      .Prm("@DateCreated", dte.Now.AddDays(-10))
      .Sel<Game>();
    t(postLs.Count > 0);
  }

  class Game
  {
    public Game() {
      DateCreated = dte.Now;
    }
    public int Id { get; set; }
    public str Title { get; set; }
    public str Html { get; set; }
    public dte DateCreated { get; set; }
  }
}