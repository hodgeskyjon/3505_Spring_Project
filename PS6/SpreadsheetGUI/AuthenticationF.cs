﻿using System;
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
    public partial class LoginWindow : Form
    {
       // private SpreadsheetClientModel scm;
        private SS.Form1 form1;

        public LoginWindow()
        {
            InitializeComponent();
        }
    /*
        public LoginWindow(SpreadsheetClientModel scm)
        {
            InitializeComponent();
            this.scm = scm;
        }
        */
        private void LoginButton_Click(object sender, EventArgs e)
        {
            int port = 0;
            int.TryParse(portTextBox.Text, out port);

            //scm.Connect(UsernameTextBox.Text, port, PasswordTextBox.Text);
            form1 = new Form1(this, UsernameTextBox.Text, port, PasswordTextBox.Text);
            form1.SSconnect(UsernameTextBox.Text, port, PasswordTextBox.Text);
        }

        public void LoginFailed()
        {
           MessageBox.Show("Invalid Password!", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error); 
        }

        private void PasswordTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                int port = 0;
                int.TryParse(portTextBox.Text, out port);

                //scm.Connect(UsernameTextBox.Text, port, PasswordTextBox.Text);
                form1 = new Form1(this, UsernameTextBox.Text, port, PasswordTextBox.Text);
                form1.SSconnect(UsernameTextBox.Text, port, PasswordTextBox.Text);
            }
        }

    }
}
