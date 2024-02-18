namespace Basketcase;
public class GetPrmNmsFrmSql
{
  public List<string> Exe(str sql) {
    var prms = new Dictionary<str, int>();
    var idx = sql.IndexOf('@');
    while (idx > -1) {
      var endIdx = sql.IndexOfAny(new char[] { ' ', ',', ')', '\r', '\n' }, idx);
      if (endIdx == -1)
        endIdx = sql.Length - 1;
      else
        endIdx -= 1;

      var prmNm = sql.Substring(idx, endIdx - idx + 1);
      if (!prms.ContainsKey(prmNm))
        prms.Add(prmNm, idx);
      idx = sql.IndexOf('@', idx + 1);
    };
    var keys = new List<str>();
    foreach (var key in prms.Keys)
      keys.Add(key);
    return keys;
  }
}