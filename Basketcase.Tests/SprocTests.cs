using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static Basketcase.Table;

namespace Basketcase.Tests
{
    [TestClass]
    public class SprocTests : BaseTests
    {
        [ClassInitialize]
        public static void InitializeClass(TestContext context) {
            initialize();
        }

        public SprocTests() : base() {
            var tableName = "Game";
            db.Execute(
                new Table(tableName)
                    .AddColumn("Id", SqlType.Int, Syntax.Identity(1, 1))
                    .AddColumn("Title", SqlType.VarChar(100), Syntax.NotNull)
                    .AddColumn("Html", SqlType.VarCharMax, Syntax.NotNull)
                    .AddColumn("DateCreated", SqlType.DateTime, Syntax.NotNull)
                    .End().Sql
            );

            var admin = new AdminDb(db);
            admin.ExecuteRaw($@"DROP PROCEDURE IF EXISTS GetGames");
            admin.ExecuteRaw($"CREATE PROCEDURE GetGames AS SELECT * FROM {tableName}");

            admin.ExecuteRaw($@"DROP PROCEDURE IF EXISTS GetGamesByDateCreated");
            admin.ExecuteRaw($@"CREATE PROCEDURE GetGamesByDateCreated(@DateCreated DATETIME) AS 
                SELECT * FROM {tableName} WHERE DateCreated > @DateCreated");

            admin.ExecuteRaw($@"DROP PROCEDURE IF EXISTS GetGameById");
            admin.ExecuteRaw(
                $@"CREATE PROCEDURE GetGameById(@Id INT) AS BEGIN
                    SELECT * FROM {tableName} WHERE Id = @Id
                END");

            db.Insert(new Game() {
                Title = "A", Html = "B"
            });
        }

        [TestMethod]
        public void QuerySproc() {
            var games = db.Sproc("GetGames").Select<Game>();
            Assert.IsTrue(games.Count > 0);
        }

        [TestMethod]
        public void QuerySprocWithParameter() {
            var posts = db.Sproc("GetGamesByDateCreated")
                .Parameter("@DateCreated", DateTime.Now.AddDays(-10))
                .Select<Game>();
            Assert.IsTrue(posts.Count > 0);
        }

        [TestMethod]
        public void QuerySprocWithParameterReturnSingle() {
            var post = db.Sproc("GetGameById")
                .Parameter("@Id", 1)
                .SelectOne<Game>();
            Assert.IsTrue(post.Id >= 1);
        }

        [TestMethod]
        public void CacheSproc() {
            var postList = db.Sproc("GetGamesByDateCreated")
                .Cache("cached", 60 * 5)
                .Parameter("@DateCreated", DateTime.Now.AddDays(-10))
                .Select<Game>();
            Assert.IsTrue(postList.Count > 0);
        }

        class Game
        {
            public Game() {
                DateCreated = DateTime.Now;
            }
            public int Id { get; set; }
            public string Title { get; set; }
            public string Html { get; set; }
            public DateTime DateCreated { get; set; }
        }
    }
}
