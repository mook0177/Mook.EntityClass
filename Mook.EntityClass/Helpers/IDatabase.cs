using System.Collections.Generic;
using System.Data.Common;

namespace EntityClass
{
    public interface IDatabase
    {
        /// <summary>
        /// 获取所有数据库
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        List<string> GetDatabases(DbConnection conn);

        /// <summary>
        /// 获取某一数据库所有表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        List<Table> GetAllTables(DbConnection conn, string database = null);

        /// <summary>
        /// 获取某个表结构
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        List<TableColumn> GetTableColumns(DbConnection conn, Table table, string database = null);
    }
}
