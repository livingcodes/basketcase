namespace Basketcase;
/// <summary>Table name equals type name with letter s appended</summary>
public class TblNm_AddLetterS : ITblNm
{
  public str Get<T>() => typeof(T).Name + "s";
  public str Get(obj inst) => inst.GetType().Name + "s";
}