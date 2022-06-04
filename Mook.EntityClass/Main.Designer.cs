namespace EntityClass
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.tvTables = new System.Windows.Forms.TreeView();
            this.rtbTemplate = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbNameSpace = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbPath = new System.Windows.Forms.TextBox();
            this.btnChooseDir = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.label3 = new System.Windows.Forms.Label();
            this.cbbOrm = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // tvTables
            // 
            this.tvTables.CheckBoxes = true;
            this.tvTables.Location = new System.Drawing.Point(15, 15);
            this.tvTables.Margin = new System.Windows.Forms.Padding(4);
            this.tvTables.Name = "tvTables";
            this.tvTables.Size = new System.Drawing.Size(226, 495);
            this.tvTables.TabIndex = 0;
            this.tvTables.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvTables_AfterCheck);
            // 
            // rtbTemplate
            // 
            this.rtbTemplate.AcceptsTab = true;
            this.rtbTemplate.Location = new System.Drawing.Point(251, 198);
            this.rtbTemplate.Name = "rtbTemplate";
            this.rtbTemplate.Size = new System.Drawing.Size(670, 312);
            this.rtbTemplate.TabIndex = 1;
            this.rtbTemplate.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(299, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 14);
            this.label1.TabIndex = 2;
            this.label1.Text = "命名空间：";
            // 
            // tbNameSpace
            // 
            this.tbNameSpace.Location = new System.Drawing.Point(382, 61);
            this.tbNameSpace.Name = "tbNameSpace";
            this.tbNameSpace.Size = new System.Drawing.Size(356, 23);
            this.tbNameSpace.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(271, 105);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 14);
            this.label2.TabIndex = 4;
            this.label2.Text = "文件存放路径：";
            // 
            // tbPath
            // 
            this.tbPath.Location = new System.Drawing.Point(382, 102);
            this.tbPath.Name = "tbPath";
            this.tbPath.ReadOnly = true;
            this.tbPath.Size = new System.Drawing.Size(356, 23);
            this.tbPath.TabIndex = 5;
            // 
            // btnChooseDir
            // 
            this.btnChooseDir.Location = new System.Drawing.Point(744, 102);
            this.btnChooseDir.Name = "btnChooseDir";
            this.btnChooseDir.Size = new System.Drawing.Size(89, 23);
            this.btnChooseDir.TabIndex = 6;
            this.btnChooseDir.Text = "选择文件夹";
            this.btnChooseDir.UseVisualStyleBackColor = true;
            this.btnChooseDir.Click += new System.EventHandler(this.btnChooseDir_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(514, 146);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(84, 34);
            this.btnCreate.TabIndex = 7;
            this.btnCreate.Text = "生  成";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(306, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 14);
            this.label3.TabIndex = 8;
            this.label3.Text = "ORM框架：";
            // 
            // cbbOrm
            // 
            this.cbbOrm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbOrm.FormattingEnabled = true;
            this.cbbOrm.Items.AddRange(new object[] {
            "EFCore",
            "DapperCore",
            "FreeSqlCore",
            "SqlSugarCore"});
            this.cbbOrm.Location = new System.Drawing.Point(382, 23);
            this.cbbOrm.Name = "cbbOrm";
            this.cbbOrm.Size = new System.Drawing.Size(355, 22);
            this.cbbOrm.TabIndex = 9;
            this.cbbOrm.SelectedIndexChanged += new System.EventHandler(this.cbbOrm_SelectedIndexChanged);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 525);
            this.Controls.Add(this.cbbOrm);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnChooseDir);
            this.Controls.Add(this.tbPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbNameSpace);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rtbTemplate);
            this.Controls.Add(this.tvTables);
            this.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvTables;
        private System.Windows.Forms.RichTextBox rtbTemplate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbNameSpace;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbPath;
        private System.Windows.Forms.Button btnChooseDir;
        private System.Windows.Forms.Button btnCreate;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbbOrm;
    }
}