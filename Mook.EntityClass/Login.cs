using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Windows.Forms;

namespace EntityClass
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            cbbDatabaseType.DataSource = Enum.GetNames(typeof(DatabaseType));
            cbbDatabaseType.SelectedIndex = 0;
            cbbDatabaseConnType.SelectedIndex = 0;
        }

        private void cbbDatabaseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbDatabaseType.Text == "Oracle")
            {
                LoadTNS();
            }
            else
            {
                DataTable dataTable = new DataTable();
                cbbServers.DataSource = dataTable;
            }
        }

        private void LoadTNS()
        {
            string[] environmentPath = Environment.GetEnvironmentVariable("Path").Split(';');
            ArrayList oraclePath = new ArrayList();
            for (int i = 0; i < environmentPath.Length; i++)
            {
                if (environmentPath[i].Contains("dbhome") && environmentPath[i].EndsWith("\\bin"))
                {
                    oraclePath.Add(environmentPath[i].Substring(0, environmentPath[i].Length - 3));
                }
            }
            string tmpPath = "network\\admin\\tnsnames.ora";
            string tnsPath = "";
            for (int i = 0; i < oraclePath.Count; i++)
            {
                if (File.Exists(oraclePath[i] + tmpPath))
                {
                    tnsPath = oraclePath[i] + tmpPath;
                }
            }
            SortedDictionary<string, string> databases = GetDatabases(tnsPath);
            cbbServers.DataSource = new BindingSource(databases, null);
            cbbServers.DisplayMember = "key";
            cbbServers.ValueMember = "value";
        }

        /// <summary>
        /// 获取TNS配置信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SortedDictionary<string, string> GetDatabases(string path)
        {
            SortedDictionary<string, string> databases = new SortedDictionary<string, string>();
            string dicKey = "";
            string dicValue = "";
            string tnsName = "";
            string fileLine;
            Stack parens = new Stack();
            StreamReader sr = new StreamReader(path);
            while ((fileLine = sr.ReadLine()) != null)
            {
                fileLine = fileLine.Trim().Replace(" ", "");
                if (!string.IsNullOrWhiteSpace(fileLine) && fileLine.Length > 0 && (fileLine.Trim().Substring(0, 1) != "#"))
                {
                    char lineChar;
                    for (int i = 0; i < fileLine.Length; i++)
                    {
                        lineChar = fileLine[i];
                        if (lineChar == '(')
                        {
                            parens.Push(lineChar);
                            dicValue += lineChar;
                        }
                        else if (lineChar == ')')
                        {
                            parens.Pop();
                            dicValue += lineChar;
                        }
                        else
                        {
                            if (parens.Count == 0)
                            {
                                if (!string.IsNullOrWhiteSpace(dicKey))
                                {
                                    databases.Add(dicKey, dicValue);
                                    dicKey = "";
                                    dicValue = "";
                                }
                                if (lineChar != '=')
                                {
                                    tnsName += lineChar;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(tnsName) && !databases.ContainsKey(tnsName))
                                {
                                    dicKey = tnsName;
                                    tnsName = "";
                                }
                                dicValue += lineChar;
                            }
                        }
                    }
                }
            }
            sr.Close();
            return databases;
        }

        private void cbbDatabaseConnType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbDatabaseConnType.Text == "连接字符串")
            {
                rtbConnectionStr.Visible = true;
            }
            else
            {
                rtbConnectionStr.Visible = false;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string conString;
            string dbName = "";
            if (cbbDatabaseConnType.Text == "连接字符串")
            {
                conString = rtbConnectionStr.Text.Trim();
                if (!string.IsNullOrWhiteSpace(conString) && conString.Contains(";"))
                {
                    string[] connStr = conString.Split(';');
                    for (int i = 0; i < connStr.Length; i++)
                    {
                        if (cbbDatabaseType.Text == "Oracle" && connStr[i].ToUpper().Contains("USER"))
                        {
                            string[] tmp = connStr[i].Split('=');
                            if (tmp.Length == 2)
                            {
                                dbName = tmp[1];
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请输入连接字符串", "提示");
                    return;
                }
            }
            else
            {
                string dataSource = cbbServers.Text.Trim();
                if (cbbDatabaseType.Text == "Oracle")
                {
                    dataSource = cbbServers.SelectedValue.ToString();
                }
                if (!string.IsNullOrWhiteSpace(dataSource) && !string.IsNullOrWhiteSpace(tbUserName.Text.Trim()) && !string.IsNullOrWhiteSpace(tbPassword.Text.Trim()))
                {
                    conString = string.Format("Data Source={0};User ID={1};Password={2}", dataSource, tbUserName.Text.Trim(), tbPassword.Text.Trim());
                    dbName = tbUserName.Text.Trim();
                }
                else
                {
                    MessageBox.Show("请输入相关信息", "提示");
                    return;
                }
            }
            try
            {
                SqlProvider.connString = conString;
                switch (cbbDatabaseType.Text)
                {
                    case "SqlServer":
                        SqlProvider.dbType = DatabaseType.SqlServer;
                        SqlProvider.db = new SqlServer();
                        break;
                    case "Oracle":
                        SqlProvider.dbType = DatabaseType.Oracle;
                        SqlProvider.db = new Oracle();
                        break;
                    case "MySql":
                        SqlProvider.dbType = DatabaseType.MySql;
                        SqlProvider.db = new MySql();
                        break;
                }
                using (DbConnection conn = SqlProvider.GetDbConnection())
                {
                    conn.Open();
                    if (cbbDatabaseType.Text == "Oracle")
                    {
                        List<Table> list = SqlProvider.db.GetAllTables(conn);
                        SqlProvider.databases.Add(dbName, list);
                    }
                    else
                    {
                        List<string> list = SqlProvider.db.GetDatabases(conn);
                        for (int i = 0; i < list.Count; i++)
                        {
                            List<Table> listTables = SqlProvider.db.GetAllTables(conn, list[i]);
                            SqlProvider.databases.Add(list[i], listTables);
                        }
                    }
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库连接失败，" + ex.Message, "提示");
            }
        }
    }
}
