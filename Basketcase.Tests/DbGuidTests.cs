using static Basketcase.Table;
namespace Basketcase.Tests;
[tc]public class DbGuidTests:BaseTests
{
  [ClassInitialize]
  public static void InitializeClass(TestContext ctx) {
    initialize();
    var sql = new Table("Photo")
      .AddCol("Id", SqlType.VarChar(36))
      .AddCol("Name", SqlType.VarChar(62))
      .End()
      .Sql;
    db.Exe(sql);
  }
  [tm]public void InsUpdSelGuid() {
    Photo photo = new() { Name = "Photo 1" };
    (photo.Id, int rowCt) = db.Ins<Photo, str>(photo);
    t(photo.Id.Length == 36);
    t(rowCt == 1);
    t(photo.Name == "Photo 1");

    photo.Name = "Photo 2";
    rowCt = db.Upd(photo);
    t(rowCt == 1);

    photo = db.SelById<Photo>(photo.Id);
    t(photo.Name == "Photo 2");
  }
  public class Photo {
    public str Id;
    public str Name {get;set;}
  }
}