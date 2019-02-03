using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlTypes;

namespace Basketcase.Tests
{
    [TestClass]
    public class DatabaseStructTests : BaseTests
    {
        [ClassInitialize]
        public static void InitializeClass(TestContext context) {
            initialize();
        }

        public DatabaseStructTests() {
            var sql = @"
                IF EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_NAME = 'Post'
                )
                    DROP TABLE Post
                CREATE TABLE Post (
	                Id INT PRIMARY KEY IDENTITY(1, 1),
	                Html VARCHAR(MAX) NOT NULL,
                    Score FLOAT,
                    AdRevenue DECIMAL(13, 3),
                    Length BIGINT,
                    IsActive BIT,
	                PublishDate DATETIME
                )";
            db.Execute(sql);

            actual = new Post() {
                Html = "abc",
                Score = 9876.12345,
                PublishDate = new DateTime(2018, 1, 1),
                AdRevenue = 80.21m,
                Length = 123456789,
                IsActive = true
            };
            db.Insert(actual);
        }
        Post actual;

        [TestMethod]
        public void SelectToStructList() {
            var posts = db.Select<Post>("select * from post");
            Assert.IsTrue(posts[0].Id == 1);
        }

        [TestMethod]
        public void SelectToStruct() {
            var post = db.SelectOne<Post>("select top 1 * from post");
            Assert.IsTrue(post.Id == 1);
            Assert.IsTrue(post.Html == "abc");
        }

        [TestMethod]
        public void SelectStringList() {
            var htmlList = db.Select<string>("select html from post");
            Assert.IsTrue(htmlList[0] == "abc");
        }

        [TestMethod]
        public void SelectString() {
            var html = db.SelectOne<string>("select top 1 html from post");
            Assert.IsTrue(html == "abc");
        }

        [TestMethod]
        public void SelectIntList() {
            var id = db.Select<int>("select id from post");
            Assert.IsTrue(id[0] == 1);
        }

        [TestMethod]
        public void SelectInt() {
            var id = db.SelectOne<int>("select top 1 id from post");
            Assert.IsTrue(id == 1);
        }

        [TestMethod]
        public void SelectDateTimeList() {
            var dates = db.Select<DateTime>("select publishdate from post");
            Assert.IsTrue(dates[0] == actual.PublishDate);
        }

        [TestMethod]
        public void SelectDateTime() {
            var date = db.SelectOne<DateTime>("select top 1 publishdate from post");
            Assert.IsTrue(date == actual.PublishDate);
        }

        [TestMethod]
        public void SelectDoubleList() {
            var numberList = db.Select<double>("select score from post");
            Assert.IsTrue(numberList[0] == actual.Score);
        }

        [TestMethod]
        public void SelectDouble() {
            var number = db.SelectOne<double>("select top 1 score from post");
            Assert.IsTrue(number == actual.Score);
        }

        [TestMethod]
        public void SelectDecimalList() {
            var decimalList = db.Select<decimal>("select adrevenue from post");
            Assert.IsTrue(decimalList[0] == actual.AdRevenue);
        }

        [TestMethod]
        public void SelectDecimal() {
            var adRevenue = db.SelectOne<decimal>("select top 1 adrevenue from post");
            Assert.IsTrue(adRevenue == actual.AdRevenue);
        }

        [TestMethod]
        public void SelectLongList() {
            var lengthList = db.Select<long>("select [length] from post");
            Assert.IsTrue(lengthList[0] == actual.Length);
        }

        [TestMethod]
        public void SelectLong() {
            var length = db.SelectOne<long>("select top 1 [length] from post");
            Assert.IsTrue(length == actual.Length);
        }

        // can't save c# date that is outside sql server date range
        [TestMethod, ExpectedException(typeof(SqlTypeException))]
        public void UpdateDateOutsideRange() {
            // 0001-01-01 is the c# min
            // SQL only goes back to 1753 and to 9999
            // so c# can have values that can't be stored in sql server
            actual.Id = 1;
            actual.PublishDate = new DateTime();
            db.Update(actual);
        }

        [TestMethod]
        public void ConvertNullColumnToCSharpValue() {
            db.Execute("truncate table post");
            // leave publish date and isactive null
            db.Execute("insert into post (html,score,adrevenue) values ('abc',1,2)");
            // trying to select null date into non-nullable date property
            var post = db.SelectById<Post>(1);
            // if bit column used for c# bool is null then it is converted to c# false
            assert(post.IsActive == false);
            // if date column is dbnull then it is converted to c# minvalue
            assert(post.PublishDate == DateTime.MinValue);
            // if number column is null then it is converted to zero
            assert(post.Length == 0);
        }

        [TestMethod]
        public void SelectBoolList() {
            var isActive = db.Select<bool>("select isactive from post");
            Assert.IsTrue(isActive[0] == true);
        }

        [TestMethod]
        public void SelectBool() {
            var isActive = db.SelectOne<bool>("select top 1 isactive from post");
            Assert.IsTrue(isActive == true);
        }

        [TestMethod]
        public void SelectFalse() {
            db.Insert(new Post() {
                Html = "a",
                AdRevenue = 1m,
                IsActive = false,
                Length = 2,
                Score = 3,
                PublishDate = new DateTime(2017, 1, 1)
            });
            var isActive = db.Select<bool>("select isactive from post");
            Assert.IsTrue(isActive[1] == false);
            Assert.IsFalse(isActive[1] == true);
        }

        public struct Post
        {
            public int Id { get; set; }
            public string Html { get; set; }
            public double Score { get; set; }
            public decimal AdRevenue { get; set; }
            public long Length { get; set; }
            public bool IsActive { get; set; }
            public DateTime PublishDate { get; set; }
        }
    }
}
