using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    public partial class OpenF : Form
    {
        private SS.Form1 form1;

        public OpenF()
        {
            InitializeComponent();
            this.listOfSavedFiles.DataSource = null;
        }

        public OpenF(SS.Form1 form1)
        {
            InitializeComponent();
            form1.fileList.Add("test1");
            form1.fileList.Add("test2");
            form1.fileList.Add("test3");
            this.listOfSavedFiles.DataSource = form1.fileList;
            this.form1 = form1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int selectedIndex = this.listOfSavedFiles.SelectedIndex;
            form1.Open(form1.fileList[selectedIndex]);
            //MessageBox.Show(form1.fileList[selectedIndex], "File Selected", MessageBoxButtons.OK);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
