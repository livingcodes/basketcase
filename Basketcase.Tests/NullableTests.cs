using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Basketcase.Tests
{
    [TestClass]
    public class DatabaseNullableTests : BaseTests
    {
        [ClassInitialize]
        public static void InitializeClass(TestContext context) {
            initialize();
        }

        public DatabaseNullableTests() {
            var sql = @"
                IF EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_NAME = 'Article'
                )
                    DROP TABLE Article
                CREATE TABLE Article (
	                Id INT PRIMARY KEY IDENTITY(1, 1),
	                Html VARCHAR(MAX) NOT NULL,
                    OptIn BIT,
                )";
            db.Execute(sql);
        }
        Article actual;

        [TestMethod]
        public void GetNullableTrue() {
            actual = new Article() {
                Html = "abc",
                OptIn = true
            };
            db.Insert(actual);

            var post = db.SelectById<Article>(1);
            assert(post.OptIn.Value);
        }

        [TestMethod]
        public void GetNullableFalse() {
            actual = new Article() {
                Html = "abc",
                OptIn = false
            };
            db.Insert(actual);

            var post = db.SelectById<Article>(1);
            assert(post.OptIn.Value == false);
        }

        [TestMethod]
        public void GetNullableBoolNull() {
            db.Execute("insert into article (html) values ('abc')");

            var post = db.SelectById<Article>(1);
            assert(post.OptIn.HasValue == false);
        }

        public class Article
        {
            public int Id { get; set; }
            public string Html { get; set; }
            public Nullable<bool> OptIn { get; set; }
        }
    }
}
