using System;

namespace Basketcase
{
    public interface IDb
    {
        (int id, int numberRowsAffected) Insert<T>(T content);
        int Update<T>(T content);
        int Delete<T>(int id);
        int Execute(string sql, params object[] parameters);
        List<T> Select<T>(string sql = "", params object[] parameters);
        T SelectOne<T>(string sql = "", params object[] parameters);
        T SelectById<T>(int id);
        IDb Sql(string sql);
        IDb Cache(string key, DateTime expiration);
        IDb Cache(string key, int seconds);
        //IDb Cache(DateTime expiration);
        //IDb Cache(int seconds);
        IDb Parameter(string name, object value);
        IDb Paging(int number, int size);
        IDb Sproc(string name);
        IAdminDb Admin { get; }
    }
}
