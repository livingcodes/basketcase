namespace Basketcase.Tests;
using System;
[tc]public class DbTests : BaseTests
{
  [ClassInitialize]
  public static void InitializeClass(TestContext ctx) {
    initialize();
    crtTbl("Profile");
  }
  public class Profile
  {
    public int Id { get; set; }
    public str Html { get; set; }
    public dte DateCreated = dte.Now;
  }

  [tm]public void InsRets() {
    db.Admin.Trun("Profile");
    var (id, rows) = db.Ins(new Profile { Html = "A" });
    t(id == 1);
    t(rows == 1);

    (id, rows) = db.Ins(new Profile { Html = "B" });
    t(id == 2);
    t(rows == 1);
  }

  [tm]public void InsSelUpdDel() {
    db.Admin.Trun("Profile");

    var (id, rows) = db.Ins(new Profile { Html = "B" });
    t(id == 1);
    t(rows == 1);

    var profs = db.Sel<Profile>("select * from profile where id = 1");
    t(profs.Count == 1);
    var prof = profs[0];
    t(prof.Html == "B");
    t(prof.DateCreated > dte.Now.Date);

    prof.Html = "c";
    var newDate = new dte(2222, 2, 2);
    prof.DateCreated = newDate;
    rows = db.Upd(prof);
    t(rows == 1);
    profs = db.Sel<Profile>("select * from profile where id = 1");
    prof = profs[0];
    t(prof.Html == "c");
    t(prof.DateCreated == newDate);

    rows = db.Del<Profile>(1);
    t(rows == 1);
    profs = db.Sel<Profile>("select * from profile where id = 1");
    t(profs.Count == 0);
  }

  [tm]public void ExecuteWithParameter() {
    db.Admin.Trun("Profile");

    db.Exe("insert into profile values (@html, @date)", "<h1>hello</h1>", DateTime.Now);
    t(db.Sel<Profile>().Count == 1);
  }
}