using System;
using System.Data.SqlTypes;
namespace Basketcase.Tests;

[tc]public class DbStructTests : BaseTests
{
  [ClassInitialize]
  public static void InitializeClass(TestContext ctx) {
    initialize();
  }

  public DbStructTests() {
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
    db.Exe(sql);

    actual = new Post() {
      Html = "abc",
      Score = 9876.12345,
      PublishDate = new DateTime(2018, 1, 1),
      AdRevenue = 80.21m,
      Length = 123456789,
      IsActive = true
    };
    db.Ins(actual);
  }
  Post actual;

  [tm]public void SelectToStructList() {
    var posts = db.Sel<Post>("select * from post");
    t(posts[0].Id == 1);
  }

  [tm]public void SelToStruct() {
    var post = db.SelOne<Post>("select top 1 * from post");
    t(post.Id == 1);
    t(post.Html == "abc");
  }

  [tm]public void SelStrLs() {
    var htmlList = db.Sel<str>("select html from post");
    t(htmlList[0] == "abc");
  }

  [tm]public void SelStr() {
    var html = db.SelOne<str>("select top 1 html from post");
    t(html == "abc");
  }

  [tm]public void SelIntLs() {
    var id = db.Sel<int>("select id from post");
    t(id[0] == 1);
  }

  [tm]public void SelInt() {
    var id = db.SelOne<int>("select top 1 id from post");
    t(id == 1);
  }

  [tm]public void SelectDateTimeList() {
    var dates = db.Sel<dte>("select publishdate from post");
    t(dates[0] == actual.PublishDate);
  }

  [tm]public void SelDte() {
    var date = db.SelOne<dte>("select top 1 publishdate from post");
    t(date == actual.PublishDate);
  }

  [tm]public void SelDblLs() {
    var numLs = db.Sel<double>("select score from post");
    t(numLs[0] == actual.Score);
  }

  [tm]public void SelDbl() {
    var num = db.SelOne<double>("select top 1 score from post");
    t(num == actual.Score);
  }

  [tm]public void SelDecLs() {
    var decLs = db.Sel<decimal>("select adrevenue from post");
    t(decLs[0] == actual.AdRevenue);
  }

  [tm]public void SelDec() {
    var adRevenue = db.SelOne<decimal>("select top 1 adrevenue from post");
    Assert.IsTrue(adRevenue == actual.AdRevenue);
  }

  [tm]public void SelLongLs() {
    var lenLs = db.Sel<long>("select [length] from post");
    t(lenLs[0] == actual.Length);
  }

  [tm]public void SelectLong() {
    var len = db.SelOne<long>("select top 1 [length] from post");
    t(len == actual.Length);
  }

  // can't save c# date that is outside sql server date range
  [tm, ExpectedException(typeof(SqlTypeException))]
  public void UpdDateOutsideRange() {
    // 0001-01-01 is the c# min
    // SQL only goes back to 1753 and to 9999
    // so c# can have values that can't be stored in sql server
    actual.Id = 1;
    actual.PublishDate = new DateTime();
    db.Upd(actual);
  }

  [tm]public void ConvertNullColToCSharpValue() {
    db.Exe("truncate table post");
    // leave publish date and isactive null
    db.Exe("insert into post (html,score,adrevenue) values ('abc',1,2)");
    // trying to select null date into non-nullable date property
    var post = db.SelById<Post>(1);
    // if bit column used for c# bool is null then it is converted to c# false
    t(post.IsActive == false);
    // if date column is dbnull then it is converted to c# minvalue
    t(post.PublishDate == DateTime.MinValue);
    // if number column is null then it is converted to zero
    t(post.Length == 0);
  }

  [tm]public void SelBlnLs() {
    var isActive = db.Sel<bool>("select isactive from post");
    t(isActive[0] == true);
  }

  [tm]public void SelBln() {
    var isActive = db.SelOne<bool>("select top 1 isactive from post");
    t(isActive == true);
  }

  [tm]public void SelFalse() {
    db.Ins(new Post() {
      Html = "a",
      AdRevenue = 1m,
      IsActive = false,
      Length = 2,
      Score = 3,
      PublishDate = new dte(2017, 1, 1)
    });
    var isActive = db.Sel<bln>("select isactive from post");
    t(isActive[1] == false);
    Assert.IsFalse(isActive[1] == true);
  }

  public struct Post
  {
    public int Id { get; set; }
    public str Html { get; set; }
    public dbl Score { get; set; }
    public dec AdRevenue { get; set; }
    public lng Length { get; set; }
    public bln IsActive { get; set; }
    public dte PublishDate { get; set; }
  }
}