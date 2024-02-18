namespace Basketcase;
/// <summary>Define convention database table name</summary>
public interface ITblNm
{
  /// <summary>Get table name that stores instances of the specified type</summary>
  str Get<T>();
  /// <summary>Get table name that stores instance</summary>
  str Get(obj inst);
}