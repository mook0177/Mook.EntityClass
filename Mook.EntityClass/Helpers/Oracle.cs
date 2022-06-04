using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data.Common;

namespace EntityClass
{
    public class Oracle : IDatabase
    {
        /// <summary>
        /// 获取所有数据库
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public List<string> GetDatabases(DbConnection conn)
        {
            List<string> list = new List<string>();
            return list;
        }

        /// <summary>
        /// 获取某一数据库所有表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        public List<Table> GetAllTables(DbConnection conn, string database = null)
        {
            List<Table> tables = new List<Table>();
            OracleCommand com = new OracleCommand("select table_name from all_tables where owner not like '%SYS%' order by table_name", (OracleConnection)conn);
            using (OracleDataReader dataReader = com.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    tables.Add(new Table
                    {
                        Name = dataReader["TABLE_NAME"].ToString()
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
        /// <param name="table"></param>
        /// <returns></returns>
        public List<TableColumn> GetTableColumns(DbConnection conn, Table table, string database = null)
        {
            OracleCommand com = new OracleCommand($@"select a.TABLE_NAME,a.COLUMN_NAME,a.DATA_TYPE,a.DATA_LENGTH,a.NULLABLE,b.COMMENTS,c.CONSTRAINT_TYPE from all_tab_cols a
                                           left join all_col_comments b on a.TABLE_NAME=b.TABLE_NAME and a.COLUMN_NAME=b.COLUMN_NAME
                                           left join (select a.TABLE_NAME,a.CONSTRAINT_TYPE,b.COLUMN_NAME from all_constraints a,all_cons_columns b 
                                           where a.CONSTRAINT_NAME=b.CONSTRAINT_NAME and a.TABLE_NAME='{table.Name}' and a.CONSTRAINT_TYPE='P') c on a.TABLE_NAME=c.table_name and a.COLUMN_NAME=c.column_name
                                           where a.TABLE_NAME='{table.Name}' order by a.COLUMN_ID", (OracleConnection)conn);
            List<TableColumn> tableColumn = new List<TableColumn>();
            using (OracleDataReader dataReader = com.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    tableColumn.Add(new TableColumn
                    {
                        TableName = dataReader["TABLE_NAME"].ToString(),
                        ColumnName = dataReader["COLUMN_NAME"].ToString(),
                        DataType = dataReader["DATA_TYPE"].ToString(),
                        DataLength = dataReader["DATA_LENGTH"].ToString(),
                        Nullable = dataReader["NULLABLE"].ToString(),
                        Comments = dataReader["COMMENTS"].ToString(),
                        ConstraintType = dataReader["CONSTRAINT_TYPE"].ToString()
                    });
                }
                tableColumn.ForEach(i =>
                {
                    i.CSharpName = SqlProvider.UpString(i.ColumnName);
                });
            }
            return tableColumn;
        }
    }
}
