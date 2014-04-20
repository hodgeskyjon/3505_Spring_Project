using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;
using SpreadsheetGUI;
using System.Collections;

namespace SS
{
    public partial class OpenF : Form
    {
        private SS.Form1 form1;
        private bool first;
       // private SpreadsheetController spreadsheetController;

        public OpenF()
        {
            InitializeComponent();
            this.listOfSavedFiles.DataSource = null;
        }

        public OpenF(SS.Form1 form1, bool f)
        {
            InitializeComponent();
            first = f;

            //******testing********
           // form1.fileList.Add("test1");
            //form1.fileList.Add("test2");
            //form1.fileList.Add("test3");

            this.listOfSavedFiles.DataSource = form1.fileList;
            this.form1 = form1;
            this.Visible = true;
  
        }

        //*****from spreadsheet controller*********
        /*public OpenF(SpreadsheetController spreadsheetController)
        {
            InitializeComponent();
            this.spreadsheetController = spreadsheetController;
            this.listOfSavedFiles.DataSource = spreadsheetController.fileList;
        }
        */
        private void button1_Click(object sender, EventArgs e)
        {
            int selectedIndex = this.listOfSavedFiles.SelectedIndex;

            if (first == true)
            {
                form1.FirstOpen(form1.fileList[selectedIndex]);
            }

            else
            {
                if (form1.fileList != null)
                {
                    form1.Open(form1.fileList[selectedIndex]);

                    //*********from spreadsheet controller**********
                    //spreadsheetController.OpenSC(spreadsheetController.fileList[selectedIndex]);
                    //MessageBox.Show(spreadsheetController.fileList[selectedIndex], "File Selected", MessageBoxButtons.OK);
                }
            }
            Close();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new NewF(form1);
            Close();
        }
    }
}
