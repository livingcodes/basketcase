namespace Basketcase;
// prototype
public class Table
{
  public Table(string name) {
    Name = name;
    Sql = $@"
      IF EXISTS (
          SELECT * FROM INFORMATION_SCHEMA.TABLES
          WHERE TABLE_NAME = '{name}'
      )
          drop table [{name}]
      create table [{name}] (";
  }

  public str Name, Sql;

  public Table AddCol(str colNm, SqlType sqlType, str syntax = "") {
    Sql += $"{colNm} {sqlType} {syntax},\r\n";
    return this;
  }

  public Table End() {
    var lastIndex = Sql.LastIndexOf(",\r\n");
    Sql = Sql.Remove(lastIndex, 3);
    Sql = Sql.Insert(lastIndex, "\r\n");
    Sql += ")";
    return this;
  }

  public class Syntax
  {
    public static str PrimaryKey => " primary key";

    public static str Identity(int start, int increment) =>
      $" identity({start},{increment})";

    public static str NotNull => " not null";

    public static str Default(str value) => $" default({value})";
    public static str DefaultGetDate => Default("getdate()");
  }

  public class SqlType
  {
    private SqlType(str name) => 
      Name = name;

    public str Name { get; private set; }

    public static SqlType Int = new("int");
    public static SqlType VarChar(int numberOfCharacters) =>
      new($"varchar({numberOfCharacters})");
    public static SqlType VarCharMax = new("varchar(max)");
    public static SqlType DateTime = new("datetime");

    public override str ToString() => Name;
  }

  //public void Example() {
  //  var posts = new Table("Posts")
  //    .AddColumn("Id", SqlType.Int, Syntax.Identity(1, 1))
  //    .AddColumn("Title", SqlType.VarChar(80), Syntax.NotNull)
  //    .AddColumn("Html", SqlType.VarCharMax)
  //    //.AddColumnDateCreated()
  //    //.AddColumnLastModified();
  //    .End();
  //  //posts.Sql;
  //}
}