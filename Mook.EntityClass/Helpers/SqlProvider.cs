using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

namespace EntityClass
{
    public class SqlProvider
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public static DatabaseType dbType;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string connString;

        /// <summary>
        /// 数据库集合
        /// </summary>
        public static Dictionary<string, List<Table>> databases = new Dictionary<string, List<Table>>();

        /// <summary>
        /// 数据库调用接口
        /// </summary>
        public static IDatabase db;

        /// <summary>
        /// ORM框架模板
        /// </summary>
        public static string orm = "EFCore";

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns></returns>
        public static DbConnection GetDbConnection()
        {
            DbConnection conn = null;
            switch (dbType)
            {
                case DatabaseType.SqlServer:
                    conn = new SqlConnection(connString);
                    break;
                case DatabaseType.Oracle:
                    conn = new OracleConnection(connString);
                    break;
                case DatabaseType.MySql:
                    conn = new MySqlConnection(connString);
                    break;
            }
            return conn;
        }

        /// <summary>
        /// 将首字母和带 _ 后第一个字母转换成大写
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string UpString(string tableName)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                if (tableName.Contains("_"))
                {
                    string[] splitName = tableName.Split('_');
                    foreach (string tmpStr in splitName)
                    {
                        if (tmpStr.Length > 1)
                        {
                            sb.Append(UpString(tmpStr) + "_");
                        }
                    }
                }
                else
                {
                    char[] chr = tableName.ToCharArray();
                    if (!Regex.IsMatch(chr[0].ToString(), "[A-Z]") || !Regex.IsMatch(chr[1].ToString(), "[a-z]"))
                    {
                        chr = tableName.ToLower().ToCharArray();
                        if (chr[0] >= 'a' && chr[0] <= 'z')
                        {
                            chr[0] = (char)(chr[0] - 32);
                        }
                    }
                    sb.Append(chr);
                }
            }
            return sb.ToString().TrimEnd('_');
        }
    }
}
