using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace EntityClass
{
    public partial class ProgressBar : Form
    {
        private readonly BackgroundWorker bgWork;
        public ProgressBar(BackgroundWorker backgroundWorker, int maxBar)
        {
            InitializeComponent();
            bgWork = backgroundWorker;
            progressBar1.Maximum = maxBar;
            bgWork.ProgressChanged += BgWork_ProgressChanged;
            bgWork.RunWorkerCompleted += BgWork_RunWorkerCompleted;
        }

        private void BgWork_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {            
            progressBar1.Value = e.ProgressPercentage;
        }

        private void BgWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            bgWork.CancelAsync();
            BtnCancel.Enabled = false;
            Close();
        }
    }
}
