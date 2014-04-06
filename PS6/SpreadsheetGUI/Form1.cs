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

namespace SS
{
    public partial class Form1 : Form
    {
        private Spreadsheet ss; //spreadsheet associated with the panel
        private string filename; //name of the file to open, if any

        /// <summary>
        /// Constructs a new default Spreadsheet
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            ss = new Spreadsheet(isValidName, s => s.ToUpper(), "Spreadsheet");
            filename = null;
            this.Text = ss.Version;
            spreadsheetPanel1.SetSelection(0, 0);
            selectedCell.Text = "A1";
            selectedValue.Text = "";
            editCell.Text = "";

            UpdateCell();
        }

        /// <summary>
        /// Constructs a new Spreadsheet with the provided filepath
        /// </summary>
        /// <param name="filepath">Path to file</param>
        public Form1(string filepath)
        {
            InitializeComponent();
            ss = new Spreadsheet(filepath, isValidName, s => s.ToUpper(), "Spreadsheet");
            filename = filepath;
            this.Text = filename;
            spreadsheetPanel1.SetSelection(0, 0);
            selectedCell.Text = "A1";
            selectedValue.Text = "";
            editCell.Text = "";

            UpdateCell();
        }



        //Event Handlers

        /// <summary>
        /// Opens a new form with a new spreadsheet panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpreadsheetApplicationContext.getAppContext().RunForm(new Form1());
        }

        /// <summary>
        /// Opens a saved spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                    Close();
            }
            else
                Close();
        }

        /// <summary>
        /// Brings up the saveFileDialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

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
            MessageBox.Show("Program created by \n Christy Bowen", "About", MessageBoxButtons.OK);
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
    }
}