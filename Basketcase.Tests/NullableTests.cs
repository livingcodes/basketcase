using System;
namespace Basketcase.Tests;

[tc]public class DbNullableTests : BaseTests
{
  [ClassInitialize]
  public static void InitializeClass(TestContext ctx) {
    initialize();
  }

  public DbNullableTests() {
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
    db.Exe(sql);
  }
  Article actual;

  [tm]public void GetNullableTrue() {
    actual = new Article() {
      Html = "abc",
      OptIn = true
    };
    db.Ins(actual);

    var post = db.SelById<Article>(1);
    t(post.OptIn.Value);
  }

  [tm]public void GetNullableFalse() {
    actual = new Article() {
      Html = "abc",
      OptIn = false
    };
    db.Ins(actual);

    var post = db.SelById<Article>(1);
    t(post.OptIn.Value == false);
  }

  [tm]public void GetNullableBoolNull() {
    db.Exe("insert into article (html) values ('abc')");

    var post = db.SelById<Article>(1);
    t(post.OptIn.HasValue == false);
  }

  public class Article
  {
    public int Id { get; set; }
    public string Html { get; set; }
    public Nullable<bln> OptIn { get; set; }
  }
}