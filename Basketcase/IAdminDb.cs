namespace Basketcase;
public interface IAdminDb
{
  void Trun(str table);
  void DropTbl(str table);
  int ExeRaw(str sql); // won't add parameters
}
