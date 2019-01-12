using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Basketcase
{
    public partial class Db : IDb
    {
        public Db(
            IConnectionFactory connectionFactory,
            IRead reader,
            ICache cache,
            ITableName tableName = null
        ) {
            this.reader = reader;
            this.connectionFactory = connectionFactory;
            this.cache = cache;
            this.tableName = tableName ?? new TableName_ClassName();
            this.getParameterNamesFromSql = new GetParameterNamesFromSql();
            this.parameters = new List<(string name, object value)>();
        }
        public IConnectionFactory connectionFactory { get; private set; }
        IRead reader;
        ICache cache;
        ITableName tableName;
        GetParameterNamesFromSql getParameterNamesFromSql;

        /// <summary>Insert content. Return new ID and rows affected.</summary>
        /// <param name="content">Content to insert</param>
        /// <returns>New ID and rows affected</returns>
        public (int id, int numberRowsAffected) Insert<T>(T content) {
            var connection = connectionFactory.Create();
            SqlCommand command = null;
            int numberRowsAffected = 0;
            IDataReader reader = null;
            int id = -1;
            try {
                connection.Open();
                command = (SqlCommand)connection.CreateCommand();
                ISqlBuilder sqlBuilder = new SqlBuilder<T>(content, command, this, cache, tableName);
                var sql = sqlBuilder.BuildInsertSql();
                command.CommandText = sql;
                reader = command.ExecuteReader();
                numberRowsAffected = reader.RecordsAffected;
                id = (int)this.reader.ReadOne<decimal>(reader); // had to get @@IDENTITY as decimal and then convert to int
            } finally {
                if (command != null)
                    command.Dispose();
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
            }
            return (id, numberRowsAffected);
        }

        /// <summary>Updates content and returns number of rows affected</summary>
        public int Update<T>(T instance) {
            var connection = connectionFactory.Create();
            SqlCommand command = null;
            int rowsAffected = 0;

            try {
                connection.Open();
                command = (SqlCommand)connection.CreateCommand();
                ISqlBuilder sqlBuilder = new SqlBuilder<T>(instance, command, this, cache, tableName);
                var sql = sqlBuilder.BuildUpdateSql();
                command.CommandText = sql;
                rowsAffected = command.ExecuteNonQuery();
            } finally {
                if (command != null)
                    command.Dispose();
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
            }
            return rowsAffected;
        }

        public int Delete<T>(int id) {
            var table = tableName.Get<T>();
            int rowsAffected = 0;
            var connection = connectionFactory.Create();
            SqlCommand command = null;
            try {
                connection.Open();
                command = (SqlCommand)connection.CreateCommand();
                command.CommandText = $"delete from {table} where id = {id}";
                rowsAffected = command.ExecuteNonQuery();
            } finally {
                if (command != null)
                    command.Dispose();
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
            }
            return rowsAffected;
        }

        public int Execute(string sql, params object[] parameters) {
            var connection = connectionFactory.Create();
            int affectedRows = -1;
            IDbCommand command = null;
            try {
                connection.Open();
                command = connection.CreateCommand();
                command.CommandText = sql;
                var parameterNames = new GetParameterNamesFromSql().Execute(sql);
                if (parameters.Length != parameterNames.Count)
                    throw new Exception($"Parameter name and value counts are not equal. Parameter name count: {parameterNames.Count}, Parameter value count: {parameters.Length}");
                for (var i = 0; i < parameters.Length; i++) {
                    var p = command.CreateParameter();
                    p.ParameterName = parameterNames[i];
                    p.Value = parameters[i];
                    command.Parameters.Add(p);
                }
                affectedRows = command.ExecuteNonQuery();
            } finally {
                if (command != null)
                    command.Dispose();
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
            }
            return affectedRows;
        }

        public IAdminDb Admin { get {
            if (_admin == null)
                _admin = new AdminDb(this);
            return _admin;
        } }
        IAdminDb _admin;
    }
}