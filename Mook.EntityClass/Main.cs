using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace EntityClass
{
    public partial class Main : Form
    {
        List<Table> listTables;
        public bool isExpand = true;
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Text = "实体类生成工具 by Mook V" + Application.ProductVersion;
            foreach (var item in SqlProvider.databases)
            {
                if (SqlProvider.databases.Count > 1)
                {
                    isExpand = false;
                }
                BuildTree(item.Key, item.Value);
            }
            cbbOrm.SelectedIndex = 0;
            rtbTemplate.Text = GetTemplateContent(SqlProvider.orm + ".txt");
        }

        public void BuildTree(string rootName, List<Table> list)
        {
            TreeNode rootNode = new TreeNode
            {
                Tag = rootName,
                Text = rootName
            };
            for (int i = 0; i < list.Count; i++)
            {
                TreeNode childNode = new TreeNode
                {
                    Tag = list[i].CSharpName,
                    Text = list[i].Name,
                    ToolTipText = rootName
                };
                rootNode.Nodes.Add(childNode);
            }
            tvTables.Nodes.Add(rootNode);
            if (isExpand)
                tvTables.ExpandAll();
        }

        private void tvTables_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                SetNodeCheckStatus(e.Node, e.Node.Checked);
                SetRootNodeIsCheck(e.Node);
            }
        }

        /// <summary>
        /// 表全选
        /// </summary>
        /// <param name="tn"></param>
        /// <param name="Checked"></param>
        private void SetNodeCheckStatus(TreeNode tn, bool Checked)
        {
            if (tn == null) return;
            foreach (TreeNode tnChild in tn.Nodes)
            {
                tnChild.Checked = Checked;
            }
        }

        private void SetRootNodeIsCheck(TreeNode Node)
        {
            int nNodeCount = 0;
            if (Node.Nodes.Count != 0)
            {
                foreach (TreeNode tnTemp in Node.Nodes)
                {
                    if (tnTemp.Checked == true)
                        nNodeCount++;
                }
                if (nNodeCount == Node.Nodes.Count)
                {
                    Node.Checked = true;
                }
                else
                {
                    Node.Checked = false;
                }
            }
            if (Node.Parent != null)
                SetRootNodeIsCheck(Node.Parent);
        }

        /// <summary>
        /// 获取模板文件内容
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        public static string GetTemplateContent(string templateName)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using (Stream stream = currentAssembly.GetManifestResourceStream($"{currentAssembly.GetName().Name}.Template.{templateName}"))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            return string.Empty;
        }

        private void cbbOrm_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlProvider.orm = cbbOrm.Text;
            rtbTemplate.Text = GetTemplateContent(SqlProvider.orm + ".txt");
        }

        private void btnChooseDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowDialog();
            tbPath.Text = dlg.SelectedPath;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            string nameSpace = tbNameSpace.Text.Trim();
            string savePath = tbPath.Text.Trim();
            if (string.IsNullOrWhiteSpace(nameSpace))
            {
                MessageBox.Show("请输入命名空间", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(savePath))
            {
                MessageBox.Show("请选择文件保存路径", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            listTables = new List<Table>();
            TreeViewAllNode(tvTables.Nodes);
            if (listTables.Count == 0)
            {
                MessageBox.Show("请选择表", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            backgroundWorker1.RunWorkerAsync();
            ProgressBar form = new ProgressBar(backgroundWorker1, listTables.Count);
            form.ShowDialog();
            form.Close();
        }

        /// <summary>
        /// 获取所有被选中的表
        /// </summary>
        /// <param name="parent"></param>
        public void TreeViewAllNode(TreeNodeCollection parent)
        {
            foreach (TreeNode tnChild in parent)
            {
                if (tnChild.Checked && tnChild.Parent != null)
                {
                    listTables.Add(new Table { Name = tnChild.Text, CSharpName = tnChild.Tag.ToString(), DatabaseName = tnChild.ToolTipText });
                }
                TreeViewAllNode(tnChild.Nodes);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            using (DbConnection conn = SqlProvider.GetDbConnection())
            {
                conn.Open();
                for (int i = 0; i < listTables.Count; i++)
                {
                    List<TableColumn> tableColumn = SqlProvider.db.GetTableColumns(conn, listTables[i], listTables[i].DatabaseName);
                    BuildEntityClass(listTables[i], tableColumn);
                    worker.ReportProgress(i);
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                MessageBox.Show("取消");
            }
            else
            {
                MessageBox.Show("完成");
                cbbOrm.SelectedIndex = 0;
                tbNameSpace.Text = "";
                tbPath.Text = "";
                CancelSelectedNode(tvTables.Nodes);
            }
        }

        /// <summary>
        /// 取消被选中的表
        /// </summary>
        /// <param name="parent"></param>
        public void CancelSelectedNode(TreeNodeCollection parent)
        {
            foreach (TreeNode tnChild in parent)
            {
                if (tnChild.Checked && tnChild.Parent != null)
                {
                    tnChild.Checked = false;
                }
                CancelSelectedNode(tnChild.Nodes);
            }
        }

        /// <summary>
        /// 生成属性
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static string GenerateEntityProperty(TableColumn column)
        {
            StringBuilder sb = new StringBuilder();
            string comments = column.Comments.Replace("\n", "").Replace("\r", "");
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// " + comments);
            sb.AppendLine("        /// </summary>");
            if (column.ConstraintType == "P")
            {
                switch (SqlProvider.orm)
                {
                    case "EFCore":
                        sb.AppendLine("        [Key]");
                        break;
                    case "DapperCore":
                        if(column.AutoIncrement)
                            sb.AppendLine("        [Key]");
                        else
                            sb.AppendLine("        [ExplicitKey]");
                        break;
                    case "FreeSqlCore":
                        sb.AppendLine("        [Column(IsPrimary = true)]");
                        break;
                    case "SqlSugarCore":
                        sb.AppendLine("        [SugarColumn(IsPrimary = true)]");
                        break;
                }
            }
            if (SqlProvider.orm == "EFCore")
            {
                sb.AppendLine($"        [Column(\"{column.ColumnName}\")]");
                if (column.CSharpType == "string")
                {
                    if (string.IsNullOrEmpty(comments))
                    {
                        comments = column.CSharpName;
                    }
                    sb.AppendLine($"        [StringLength({column.DataLength}, ErrorMessage = \"{comments}长度不能超出{column.DataLength}字符\")]");
                }
            }
            string colType = column.CSharpType;
            if (colType != "string" && colType != "byte[]" && column.Nullable == "Y")
            {
                colType += "?";
            }
            sb.AppendLine($"        public {colType} {column.CSharpName} " + "{ get; set; }");
            return sb.ToString();
        }

        /// <summary>
        /// 生成实体类文件
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columns"></param>
        public void BuildEntityClass(Table table, List<TableColumn> columns)
        {
            columns.ForEach(x =>
            {
                string csharpType = DataTypeMapping.dbColumnDataTypes.FirstOrDefault(t =>
                        t.DatabaseType == SqlProvider.dbType && t.ColumnTypes.Split(',').Any(p =>
                            p.Trim().Equals(x.DataType, StringComparison.OrdinalIgnoreCase)))?.CSharpType;
                if (string.IsNullOrEmpty(csharpType))
                {
                    throw new Exception($"未从字典中找到\"[{x.TableName}]表 [{x.ColumnName}]字段 {x.DataType}\"类型，对应的C#数据类型");
                }
                x.CSharpType = csharpType;
            });
            StringBuilder sb = new StringBuilder();
            foreach (TableColumn column in columns)
            {
                string tmp = GenerateEntityProperty(column);
                sb.AppendLine(tmp);
            }
            string content = GetTemplateContent(SqlProvider.orm + ".txt");
            content = content.Replace("{Namespace}", tbNameSpace.Text.Trim())
               .Replace("{ClassName}", table.CSharpName)
               .Replace("{TableName}", table.Name)
               .Replace("{Properties}", sb.ToString().TrimEnd());
            string path = tbPath.Text + "\\" + table.CSharpName + ".cs";
            WriteAndSave(path, content);
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        public static void WriteAndSave(string fileName, string content)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(content);
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }
            }
        }
    }
}
