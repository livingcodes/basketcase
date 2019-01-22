using System;
using System.Collections.Generic;
using System.Data;

namespace Basketcase
{
    public partial class Db
    {
        public IDb Sproc(string name) {
            sprocName = name;
            return this;
        }
        string sprocName;

        public bool IsSproc => sprocName != null;

        public List<T> Select<T>(string sql = null, params object[] parameters) {
            // get from cache
            if (cacheKey != null) {
                var cacheValue = cache.Get<List<T>>(cacheKey);
                if (cacheValue != null)
                    return (List<T>)cacheValue;
            }

            // sql syntax
            if (sql == null || sql == "")
                sql = this.sql;

            // if sql is not set then select all
            if (sql == null || sql == "") {
                var tableName = this.tableName.Get<T>();
                sql = $"SELECT * FROM [{tableName}]";
            }

            if (!IsSproc && (
               sql.ToUpper().StartsWith("WHERE ")
            || sql.ToUpper().StartsWith("GROUP BY ")
            || sql.ToUpper().StartsWith("ORDER BY ")
            )) {
                var tableName = this.tableName.Get<T>();
                //sql = sql ?? query.Sql();
                var sqlStart = $"SELECT * FROM [{tableName}] ";
                sql = sqlStart + sql;
            }

            // parameters
            if (!IsSproc) {
                var parameterNames = getParameterNamesFromSql.Execute(sql);
                // todo: are count and length checks necessary
                if (parameterNames.Count > 0) {
                    if (parameters.Length > 0) {
                        if (parameters.Length != parameterNames.Count)
                            throw new System.Exception($"Parameter name and value counts are not equal. Parameter name count: {parameterNames.Count}, Parameter value count: {parameters.Length}");
                        for (var i = 0; i < parameters.Length; i++)
                            Parameter(parameterNames[i], parameters[i]);
                    }
                }
            }

            if (!IsSproc && hasPaging)
                sql += pagingSql;
            if (hasPaging) {
                Parameter("@PageNumber", pageNumber);
                Parameter("@PageSize", pageSize);
            }

            var list = select<T>(sql);

            setCache(list);
            setQueryToNull();
            return list;
        }

        public T SelectOne<T>(string sql = null, params object[] parameters) {
            // get from cache
            if (cacheKey != null) {
                var cacheValue = cache.Get<T>(cacheKey);
                if (cacheValue != null) {
                    setQueryToNull();
                    return (T)cacheValue;
                }
            }

            // sql syntax
            if (sql == null || sql == "")
                sql = this.sql;

            if (!IsSproc && (
               sql.ToUpper().StartsWith("WHERE ")
            || sql.ToUpper().StartsWith("GROUP BY ")
            || sql.ToUpper().StartsWith("ORDER BY ")
            )) {
                var tableName = this.tableName.Get<T>();
                //sql = sql ?? query.Sql();
                var sqlStart = $"SELECT TOP 1 * FROM [{tableName}] ";
                sql = sqlStart + sql;
            }

            if (!IsSproc && hasPaging)
                sql += pagingSql;

            // parameters
            if (!IsSproc) {
                var parameterNames = getParameterNamesFromSql.Execute(sql);
                // todo: are count and length checks necessary
                if (parameterNames.Count > 0) {
                    if (parameters.Length > 0) {
                        if (parameters.Length != parameterNames.Count)
                            throw new System.Exception($"Parameter name and value counts are not equal. Parameter name count: {parameterNames.Count}, Parameter value count: {parameters.Length}");
                        for (var i = 0; i < parameters.Length; i++)
                            Parameter(parameterNames[i], parameters[i]);
                    }
                }
                if (hasPaging) {
                    Parameter("@PageNumber", pageNumber);
                    Parameter("@PageSize", pageSize);
                }
            }

            var content = selectOne<T>(sql);

            setCache(content);
            setQueryToNull();
            return content;
        }

        public T SelectById<T>(int id) =>
            Parameter("@Id", id)
            .SelectOne<T>($"WHERE Id = @Id");

        List<T> select<T>(string sql) {
            var list = new List<T>();
            var connection = connectionFactory.Create();
            IDbCommand command = null;
            try {
                connection.Open();
                command = connection.CreateCommand();

                if (sprocName != null) {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = sprocName;
                } else {
                    command.CommandText = sql;
                }
                foreach (var parameter in parameters) {
                    var p = command.CreateParameter();
                    p.ParameterName = parameter.name;
                    p.Value = parameter.value;
                    command.Parameters.Add(p);
                }
                var reader = command.ExecuteReader();
                list = this.reader.ReadList<T>(reader);
            } finally {
                if (command != null)
                    command.Dispose();
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
            }
            return list;
        }

        T selectOne<T>(string sql) {
            var content = default(T);
            var connection = connectionFactory.Create();
            IDbCommand command = null;
            try {
                connection.Open();
                command = connection.CreateCommand();

                if (sprocName != null) {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = sprocName;
                } else {
                    command.CommandText = sql;
                }
                foreach (var parameter in parameters) {
                    var p = command.CreateParameter();
                    p.ParameterName = parameter.name;
                    p.Value = parameter.value;
                    command.Parameters.Add(p);
                }
                var reader = command.ExecuteReader();
                content = this.reader.ReadOne<T>(reader);
            } finally {
                if (command != null)
                    command.Dispose();
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
            }
            return content;
        }

        public IDb Sql(string sql) {
            this.sql = sql;
            return this;
        }
        string sql;

        public IDb Parameter(string name, object value) {
            parameters.Add((name, value));
            return this;
        }
        List<(string name, object value)> parameters;

        /// <summary>Setup paging. 
        /// Example: If 100 rows then Paging(2, 10) would return rows 11-20.
        /// SQL must contain order by statement; otherwise, exception thrown.</summary>
        /// <param name="number">Page number: only rows from this page number are returned. First page is 1 (i.e. not zero-based).</param>
        /// <param name="size">Page size (i.e. take) controls number of rows on a page.</param>
        public IDb Paging(int number, int size) {
            hasPaging = true;
            pageNumber = number;
            pageSize = size;
            pagingSql = @"
                OFFSET((@PageNumber - 1) * @PageSize) ROWS
                FETCH NEXT @PageSize ROWS ONLY";
            return this;
        }
        bool hasPaging;
        int pageNumber, pageSize;
        string pagingSql;

        void setQueryToNull() {
            sql = "";
            hasPaging = false;
            pageNumber = 0;
            pageSize = 0;
            parameters = new List<(string name, object value)>();
            cacheKey = null;
        }
        
        public IDb Cache(string key, DateTime expiration) {
            cacheKey = key;
            var duration = expiration.Subtract(DateTime.Now);
            cacheSeconds = (int)duration.TotalSeconds;
            return this;
        }
        public IDb Cache(string key, int seconds) {
            cacheKey = key;
            cacheSeconds = seconds;
            return this;
        }
        //public IDb Cache(DateTime expiration) => Cache(null, expiration);
        //public IDb Cache(int seconds) => Cache(null, seconds);
        string cacheKey;
        int cacheSeconds;

        void setCache(object obj) {
            if (cacheKey != null) {
                cache.Set(cacheKey, obj, cacheSeconds);
                cacheKey = null;
                cacheSeconds = 60;
            }
        }
    }
}
