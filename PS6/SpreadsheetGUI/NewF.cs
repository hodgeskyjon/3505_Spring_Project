using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
    public partial class NewF : Form
    {
        public string filename;
        private SS.Form1 form1;

        public NewF()
        {
            InitializeComponent();
        }

        public NewF(SS.Form1 form1)
        {
            InitializeComponent();
            this.form1 = form1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            filename = NewTextBox.Text;
            //form1.newOpen(filename);
            Close();
        }
    }
}
