namespace Basketcase;
public interface ISqlBldr
{
  str BldInsSql(bln genGuid = false);
  str BldUpdSql();
}