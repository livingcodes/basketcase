namespace Basketcase;
public interface IDb
{
  (int id, int rowCt) Ins<T>(T content);
  (R id, int rowCt) Ins<T,R>(T content);
  int Upd<T>(T content);
  int Del<T>(int id);
  int Exe(str sql, params obj[] prms);
  List<T> Sel<T>(str sql = "", params obj[] prms);
  T Sel1<T>(str sql = "", params obj[] prms);
  T SelById<T>(int id);
  T SelById<T>(str id);
  IDb Sql(str sql);
  IDb Cache(str key, dte exp);
  IDb Cache(str key, int sec);
  //IDb Cache(DateTime expiration);
  //IDb Cache(int seconds);
  IDb Prm(str name, obj val);
  IDb Pg(int num, int sz);
  IDb Sproc(str name);
  IAdminDb Admin { get; }
}