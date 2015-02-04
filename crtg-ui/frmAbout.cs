using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CRTG.UI
{
    public partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();

            // Show version number
            this.groupBox1.Text = "CRTG v" + typeof(Program).Assembly.GetName().Version.ToString();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
