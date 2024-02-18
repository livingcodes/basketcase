namespace Basketcase.Tests;
  using System.Data;
public class ConFct : IConFct
{
  public ConFct(str conStr) {
    this.conStr = conStr;
  }
  str conStr;

  public IDbConnection Crt() =>
    new Microsoft.Data.SqlClient.SqlConnection(conStr);
}