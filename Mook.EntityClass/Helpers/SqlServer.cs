using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace EntityClass
{
    public class SqlServer : IDatabase
    {
        /// <summary>
        /// 获取所有数据库
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public List<string> GetDatabases(DbConnection conn)
        {
            List<string> databases = new List<string>();
            SqlCommand com = new SqlCommand("select name from sys.databases where database_id>4 and state_desc='online' order by name", (SqlConnection)conn);
            using (SqlDataReader dataReader = com.ExecuteReader())
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
            if (database.Contains("."))
            {
                throw new Exception(database + "数据库名称异常，该系统不支持含有 . 字符");
            }
            List<Table> tables = new List<Table>();
            SqlCommand com = new SqlCommand($"select name from {database}..sysobjects where xtype='U' order by name", (SqlConnection)conn);
            using (SqlDataReader dataReader = com.ExecuteReader())
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
            SqlCommand com = new SqlCommand($@"select a.name as table_name,b.name as column_name,b.length as data_length,case when b.isnullable=1 then 'Y' else 'N' end as nullable,b.colstat as auto_increment,
            case when c.xtype = 'PK' then 'P' else null end as constraint_type,d.name as data_type,e.value as comments from {database}..sysobjects a
            left join {database}..syscolumns b on a.id=b.id
            left join (select b.id,c.colid,a.xtype from {database}..sysobjects a inner join {database}..sysindexes b on a.name=b.name inner join {database}..sysindexkeys c on b.id=c.id and b.indid=c.indid) c on b.id=c.id and b.colid=c.colid
            left join {database}..systypes d on b.xtype=d.xusertype
            left join {database}.sys.extended_properties e on b.id=e.major_id and b.colid=e.minor_id where a.name='{table.Name}'", (SqlConnection)conn);
            using (SqlDataReader dataReader = com.ExecuteReader())
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
