using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace EntityClass
{
    public class MySql : IDatabase
    {
        /// <summary>
        /// 获取所有数据库
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public List<string> GetDatabases(DbConnection conn)
        {
            List<string> databases = new List<string>();
            MySqlCommand com = new MySqlCommand("show databases;", (MySqlConnection)conn);
            using (MySqlDataReader dataReader = com.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    databases.Add(dataReader[0].ToString());
                }
                return databases;
            }
        }

        /// <summary>
        /// 获取某一数据库所有表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        public List<Table> GetAllTables(DbConnection conn, string database)
        {
            List<Table> tables = new List<Table>();
            MySqlCommand com = new MySqlCommand($"select table_name from information_schema.tables where table_schema='{database}' and table_type='base table' order by table_name;", (MySqlConnection)conn);
            using (MySqlDataReader dataReader = com.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    tables.Add(new Table
                    {
                        Name = dataReader[0].ToString(),
                        DatabaseName = database
                    });
                }
                tables.ForEach(i =>
                {
                    i.CSharpName = SqlProvider.UpString(i.Name);
                });
                return tables;
            }
        }

        /// <summary>
        /// 获取某个表结构
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public List<TableColumn> GetTableColumns(DbConnection conn, Table table, string database)
        {
            List<TableColumn> tableColumns = new List<TableColumn>();
            MySqlCommand com = new MySqlCommand($@"select table_name,column_name,data_type,column_comment as comments,case when is_nullable='YES' then 'Y' else 'N' end as nullable,
            character_maximum_length as data_length,case when column_key='PRI' then 'P' else null end as constraint_type,case when extra='auto_increment' then 1 else 0 end as auto_increment from information_schema.columns where table_schema='{database}' and table_name='{table.Name}';", (MySqlConnection)conn);
            using (MySqlDataReader dataReader = com.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    tableColumns.Add(new TableColumn
                    {
                        TableName = dataReader["TABLE_NAME"].ToString(),
                        ColumnName = dataReader["COLUMN_NAME"].ToString(),
                        DataType = dataReader["DATA_TYPE"].ToString(),
                        DataLength = dataReader["DATA_LENGTH"].ToString(),
                        Nullable = dataReader["NULLABLE"].ToString(),
                        Comments = dataReader["COMMENTS"].ToString(),
                        ConstraintType = dataReader["CONSTRAINT_TYPE"].ToString(),
                        AutoIncrement = Convert.ToBoolean(int.Parse(dataReader["AUTO_INCREMENT"].ToString()))
                    });
                }
                tableColumns.ForEach(i =>
                {
                    i.CSharpName = SqlProvider.UpString(i.ColumnName);
                });
                return tableColumns;
            }
        }
    }
}
