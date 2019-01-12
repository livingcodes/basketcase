using System.Collections.Generic;

namespace Basketcase
{
    public class GetParameterNamesFromSql
    {
        public List<string> Execute(string sql) {
            var parameters = new Dictionary<string, int>();
            var index = sql.IndexOf('@');
            while (index > -1) {
                var endIndex = sql.IndexOfAny(new char[] { ' ', ',', ')', '\r', '\n' }, index);
                if (endIndex == -1)
                    endIndex = sql.Length - 1;
                else
                    endIndex -= 1;

                var parameterName = sql.Substring(index, endIndex - index + 1);
                if (!parameters.ContainsKey(parameterName))
                    parameters.Add(parameterName, index);
                index = sql.IndexOf('@', index + 1);
            };
            var keys = new List<string>();
            foreach (var key in parameters.Keys)
                keys.Add(key);
            return keys;
        }
    }
}
