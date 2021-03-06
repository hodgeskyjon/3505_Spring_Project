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
    public partial class Form1 : Form
    {
        private Spreadsheet ss; //spreadsheet associated with the panel
        private string filename; //name of the file to open, if any
        private SpreadsheetClientModel scm;
        public List<string> fileList;
        private LoginWindow logWin;
        private int version;
        private bool first;
        public string username;
        public int port;
        public string password;
        static int x = 200;
        static int y = 200;
        private LoginWindow loginWindow;
        //private SpreadsheetController ssController;

        /// <summary>
        /// Constructs a new default Spreadsheet
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            ss = new Spreadsheet(isValidName, s => s.ToUpper(), "Spreadsheet");
            scm = new SpreadsheetClientModel();
            fileList = new List<string>();
            version = 0;
            filename = null;
            this.Text = ss.Version;
            spreadsheetPanel1.SetSelection(0, 0);
            selectedCell.Text = "A1";
            selectedValue.Text = "";
            editCell.Text = "";

            scm.IncomingLineEvent += MessageReceived;
            UpdateCell();
        }


        /// <summary>
        /// Constructs a new Spreadsheet when you open a new spreadsheet from the server.
        /// </summary>
        /// <param name="filepath">Path to file</param>
        public Form1(string filepath, string user, int p, string pass)
        {
            InitializeComponent();
            //ss = new Spreadsheet(filepath, isValidName, s => s.ToUpper(), "Spreadsheet");
            ss = new Spreadsheet(isValidName, s => s.ToUpper(), filepath);
            fileList = new List<string>();
            first = false;

            //Will this be bad??
            scm = new SpreadsheetClientModel();
            scm.Connect(user, p, pass);
            scm.SendMessage("CREATE" + "\\e" + filepath + "\n");

            username = user;
            port = p;
            password = pass;

            version = 0;
            filename = filepath;
            this.Text = filename;
            spreadsheetPanel1.SetSelection(0, 0);
            selectedCell.Text = "A1";
            selectedValue.Text = "";
            editCell.Text = "";

            scm.IncomingLineEvent += MessageReceived;
            UpdateCell();

            //testing success
            //new OpenF(this, first);

        }
/*
        public Form1(SpreadsheetClientModel scm)
        {
            
            InitializeComponent();
            ss = new Spreadsheet(isValidName, s => s.ToUpper(), "Spreadsheet");
            scm = new SpreadsheetClientModel();
            logWin = new LoginWindow(scm);   
            version = 0;
            filename = null;
            this.Text = ss.Version;
            spreadsheetPanel1.SetSelection(0, 0);
            selectedCell.Text = "A1";
            selectedValue.Text = "";
            editCell.Text = "";
            this.Visible = false;

            scm.IncomingLineEvent += MessageReceived;
            UpdateCell();
        }
        */
        /// <summary>
        /// The initial constructor that the AuthenicationF uses.
        /// </summary>
        /// <param name="loginWindow"></param>
        public Form1(LoginWindow loginWindow, string user, int p, string pass)
        {
            logWin = loginWindow;
            InitializeComponent();
            ss = new Spreadsheet(isValidName, s => s.ToUpper(), "Spreadsheet");
            scm = new SpreadsheetClientModel();
            fileList = new List<string>();
            username = user;
            port = p;
            password = pass;
            first = true;
            filename = null;
            this.Text = ss.Version;
            spreadsheetPanel1.SetSelection(0, 0);
            selectedCell.Text = "A1";
            selectedValue.Text = "";
            editCell.Text = "";
            this.Visible = false;

            scm.IncomingLineEvent += MessageReceived;
            UpdateCell();

            //testing success
            //new OpenF(this, first);
        }



        //*********************************************
        //*              Event Handlers               *
        //*                                           *
        //*********************************************

        //************Need to validate***********
        /// <summary>
        /// This is where we listen for messages coming from the server
        /// and handle them accordingly.
        /// </summary>
        private void MessageReceived(string s)
        {
            if (s != null)
            {
                string[] words = s.Split((char)27);

                if (words[0].Contains("FILELIST"))
                {
                    new OpenF(this, first);

                    foreach (string file in words)
                    {
                        fileList.Add(file);
                    }
                }
                else if (words[0].Contains("INVALID"))
                {
                    logWin.LoginFailed();

                }
                else if (words[0].Contains("UPDATE"))
                {
                    // File Creation Request
                    //UPDATE[esc]current_version\n

                    int serverVersion = 0;
                    int.TryParse(words[1], out serverVersion);

                    if (serverVersion != version)
                    {
                        Resync();
                    }
                    else if (words.Length > 2)
                    {
 
                    // File Open Request
                    //UPDATE[esc]current_version[esc]cell_name1[esc]cell_content1[esc]cell_name2[esc]…\n 

                    // Edit Request
                    //UPDATE[esc]current_version[esc]cell_name[esc]cell_content\n 

                    // Undo Request
                    //UPDATE[esc] current_version[esc]cell_name[esc]cell_content\n

                        int.TryParse(words[1], out version);

                        int i = 2;
                        while (i < words.Length)
                        {
                            ss.SetContentsOfCell(words[i], words[i++]);
                            UpdateCellFromServer(words[i - 1]);
                            i++;
                        }
                    }
                    version++;
                }
                else if (words[0].Contains("SAVED"))
                {
                    //Save Request
                    MessageBox.Show("The Spreadsheet has been saved.", "Spreadsheet | Success", MessageBoxButtons.OK);
                }
                else if (words[0].Contains("SYNC"))
                {
                    // Spreadsheet out of sync detected
                    //SYNC[esc] current_version[esc]cell_name[esc]cell_content…\n

                    int.TryParse(words[1], out version);

                    int i = 2;
                    while (i < words.Length)
                    {
                        ss.SetContentsOfCell(words[i], words[i++]);
                        UpdateCellFromServer(words[i - 1]);
                        i++;
                    }
                }
                else if (words[0].Contains("ERROR"))
                {
                    // All Other Errors
                    //ERROR[esc]error_message\n
                    MessageBox.Show(words[1], "Spreadsheet | Error", MessageBoxButtons.OK);

                }
                else
                {
                    DialogResult result = MessageBox.Show("The Server has disconnected." , "Spreadsheet | Error", MessageBoxButtons.OK);
                    if (result == DialogResult.OK)
                    {
                        scm.Close();
                        Close();
                    }
                    
                }
            }
        }

        /// <summary>
        /// This will be called whenever a new connection needs to be established
        /// </summary>
        public void SSconnect(string userN, int p, string passW)
        {
            port = p;
            username = userN;
            password = passW;

            scm.Connect(userN, p, passW);

        }

        /// <summary>
        /// If the client is out of sync with the server, then it will send a request to 
        /// resync with the server.
        /// </summary>
        private void Resync()
        {
            scm.SendMessage("RESYNC\n");
        }

        /// <summary>
        /// Opens a new form with a new spreadsheet panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewF newForm = new NewF(this);
            newForm.Visible = true;

            newForm.SetDesktopLocation(x, y);
            x += 30;
            y += 30;
           
        }
        
        public void newOpen(string filename)
        {
            if (fileList != null)
            {
                if (fileList.Contains(filename))
                {
                    DialogResult result = MessageBox.Show("That filename already exist.\n" + "Please choose a different filename.", "Spreadsheet | Error", MessageBoxButtons.OK);
                    if (result == DialogResult.OK)
                    {
                        NewF newForm = new NewF(this);
                        newForm.Visible = true;

                        newForm.SetDesktopLocation(x, y);
                        x += 30;
                        y += 30;
                    }
                }
                else
                {
                    //scm.SendMessage("CREATE" + "\\e" + filename + "\\e\\e\\e\n");
                    SpreadsheetApplicationContext.getAppContext().RunForm(new Form1(filename, username, port, password));

                }
            }
            
        }
         

        /// <summary>
        /// Opens a saved spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
           /* 
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string file = openFileDialog1.FileName;
                Form1 newForm = new Form1(file);

                try
                {
                    SpreadsheetApplicationContext.getAppContext().RunForm(newForm);
                    foreach (string k in newForm.ss.GetNamesOfAllNonemptyCells())
                    {
                        newForm.updateDependent(k);
                    } 
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Spreadsheet | Error", MessageBoxButtons.OK);
                }
            }
            * */
             
            OpenF openForm = new OpenF(this, first);

            openForm.SetDesktopLocation(x, y);
            x += 30;
            y += 30;

        }

        public void Open(String filename)
        {
            scm.Connect(filename, port, password);

            scm.SendMessage("OPEN" + "\\e" + filename + "\n");

            //Form1 newForm = new Form1(filename);
            this.Visible = true;
            this.Text = filename;

            try
            {
                SpreadsheetApplicationContext.getAppContext().RunForm(this);
                foreach (string k in this.ss.GetNamesOfAllNonemptyCells())
                {
                    this.updateDependent(k);
                } 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Spreadsheet | Error", MessageBoxButtons.OK);
            }

        }

        public void FirstOpen(String filename)
        {
            scm.SendMessage("OPEN" + "\\e" + filename + "\n");

            //scm.Connect(username, port, password);

            //Form1 newForm = new Form1(filename);
            this.Visible = true;
            this.Text = filename;

            try
            {
                SpreadsheetApplicationContext.getAppContext().RunForm(this);
                foreach (string k in this.ss.GetNamesOfAllNonemptyCells())
                {
                    this.updateDependent(k);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Spreadsheet | Error", MessageBoxButtons.OK);
            }

        }
   
        /// <summary>
        /// Closes the current window when selected from the menu
        /// Notifies user if changes have not been saved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ss.Changed)
            {
                var result = MessageBox.Show("The spreadsheet has been changed and not saved. Are you sure you want to exit?",
                    "Spreadsheet | Exit", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                    return;
                else
                {
                    scm.SendMessage("DISCONNECT\n");
                    scm.Close();
                    Close();
                }
            }
            else
            {
                scm.SendMessage("DISCONNECT\n");
                scm.Close();
                Close();
            }
        }

        /// <summary>
        /// Brings up the saveFileDialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scm.SendMessage("SAVE" + "\\e" + version + "\n");
           //saveFileDialog1.ShowDialog();
        }
/*
        /// <summary>
        /// saves the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string name = saveFileDialog1.FileName;
            ss.Save(name);
        }
*/
        /// <summary>
        /// Handles Evaluate button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EvaluateButton_Click(object sender, EventArgs e)
        {
            Evaluate();
        }

        /// <summary>
        /// Handles the editable textbox when Enter is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editCell_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                Evaluate();
        }

        /// <summary>
        /// Handler to update text boxes that give the
        /// information of the currently selected cell
        /// </summary>
        /// <param name="sender"></param>
        private void spreadsheetPanel1_SelectionChanged(SpreadsheetPanel sender)
        {
            selectedCell.Text = getSelectedCellName();
            UpdateCell();
        }

        /// <summary>
        /// Handler for About menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAbout();
        }

        /// <summary>
        /// Handler for Get Help menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetHelp();
        }


        //Utility Methods

        /// <summary>
        /// Updates the currently selected cell
        /// </summary>
        private void UpdateCell()
        {
            string name = getSelectedCellName();
            if (!isValidName(name))
            {
                MessageBox.Show("Not a valid cell");
            }
            selectedCell.Text = name;
            object cellValue = ss.GetCellValue(name);
            object cellContents = ss.GetCellContents(name);

            if (cellValue is FormulaError)
            {
                FormulaError formula = (FormulaError)cellValue;
                MessageBox.Show(formula.Reason);
            }
            else
                selectedValue.Text = cellValue.ToString();

            if (cellContents is Formula)
            {
                editCell.Text = "=" + cellContents.ToString();
                editCell.SelectAll();
            }
            else
            {
                editCell.Text = cellContents.ToString();
                editCell.SelectAll();
            }
        }

        /// <summary>
        /// Updates the currently selected cell
        /// </summary>
        private void UpdateCellFromServer(string name)
        {
            object cellValue = ss.GetCellValue(name);
            object cellContents = ss.GetCellContents(name);

            if (cellValue is FormulaError)
            {
                FormulaError formula = (FormulaError)cellValue;
                MessageBox.Show(formula.Reason);
            }
            else
                selectedValue.Text = cellValue.ToString();

            if (cellContents is Formula)
            {
                editCell.Text = "=" + cellContents.ToString();
                editCell.SelectAll();
            }
            else
            {
                editCell.Text = cellContents.ToString();
                editCell.SelectAll();
            }
        }

        /// <summary>
        /// Gets the name of the currently selected cell
        /// </summary>
        /// <returns>Cell name</returns>
        private string getSelectedCellName()
        {
            int row, col;
            spreadsheetPanel1.GetSelection(out col, out row);
            Char c = (Char)(65 + col);
            return c.ToString() + (row + 1).ToString();            
        }

        /// <summary>
        /// Method to evaluate the currently selected cell
        /// and its dependents
        /// </summary>
        private void Evaluate()
        {
            string name = getSelectedCellName();
            ISet<string> dependents;

            try
            {
                dependents = ss.SetContentsOfCell(name, editCell.Text);
                
                // Send message to the server about the change. 
                scm.SendMessage("ENTER" + "\\e" + version + "\\e" + name + "\\e" + ss.GetCellContents(name) + "\n");
                version++;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            UpdateCell();

            if(!(ss.GetCellValue(name) is FormulaError))
                foreach(string d in dependents)
                    updateDependent(d);

        }

        /// <summary>
        /// Helper method to update the dependents of
        /// the currently selected cell
        /// </summary>
        /// <param name="d">Name of dependent cell to update</param>
        private void updateDependent(string d)
        {
            int col, row;
            GetSelectedColRow(d, out col, out row);
            object value = ss.GetCellValue(d);

            if (value is FormulaError)
            {
                FormulaError formula = (FormulaError)value;
                spreadsheetPanel1.SetValue(col, row, formula.Reason);
            }
            else
                spreadsheetPanel1.SetValue(col, row, value.ToString());
        }

        /// <summary>
        /// Get current col & row based on cell name
        /// </summary>
        /// <param name="name">Cell selected</param>
        /// <param name="col">Column number</param>
        /// <param name="row">Row number</param>
        private void GetSelectedColRow(String cellName, out int col, out int row)
        {
            int rowNum;
            int.TryParse(cellName.Substring(1), out rowNum);
            col = cellName.First<char>() - 'A';
            row = rowNum - 1;
        }

        /// <summary>
        /// Message box with info about program
        /// </summary>
        private void ShowAbout()
        {
            MessageBox.Show("Program created by \n Christy Bowen, Skyler Jones, James Sullivan, and Mike Miner", "About", MessageBoxButtons.OK);
        }

        /// <summary>
        /// Shows help information
        /// </summary>
        private void GetHelp()
        {
            StringBuilder helpText = new StringBuilder();
            helpText.Append("Spreadsheet Help \n\n");
            helpText.Append("The textbox to the right of 'Selected Cell' shows the name of the currently selected cell. \n\n");
            helpText.Append("The textbox to the right of 'Cell Value' shows the value of the currently selected cell. \n\n");
            helpText.Append("The textbox to the right of 'Edit Cell Contents' shows the contents of the cell, and allows you to edit the contents. \n\n");
            helpText.Append("To select a cell, click on it. \n\n");
            helpText.Append("Once selected, a cell can be edited with the 'Edit Cell Contents' box. \n\n");
            helpText.Append("You can hit the 'Enter' key, or click the 'Evaluate' button to update. \n\n");
            helpText.Append("When entering a formula, it must start with '=' \n\n");
            helpText.Append("When you click the Undo button it will revert to the last change on the server. \n\n");
            helpText.Append("Menu items work in the usual way");
            MessageBox.Show(helpText.ToString(), "Help", MessageBoxButtons.OK);
        }

        /// <summary>
        /// Helper method to check for valid cell names
        /// </summary>
        /// <param name="name">Name of cell to check</param>
        /// <returns></returns>
        private bool isValidName(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z]+(?: [a-zA-Z]|\d)*$");
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            version++;
            scm.SendMessage("UNDO" + "\\e" + version + "\n");
        }

    }
}
