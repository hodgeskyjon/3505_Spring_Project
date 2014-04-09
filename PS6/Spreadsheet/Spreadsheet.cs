using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;
using System.Xml;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<string, Cell> cells; //represents the cells of the spreadsheet
        private DependencyGraph dependencyGraph; //Keeps track of the dependencies of the cells
        private bool hasChanged = false;

        /// <summary>
        /// Contructs a new spreadsheet, (0 arguments from user)
        /// </summary>
        public Spreadsheet()
            : base(s => true, s => s, "ps6")
        {
            cells = new Dictionary<string, Cell>();
            dependencyGraph = new DependencyGraph();
        }

        /// <summary>
        /// Contructs a new spreadsheet, (3 arguments from user)
        /// </summary>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string>normalize, string version) 
            : base(isValid, normalize, version)
        {
            cells = new Dictionary<string, Cell>();
            dependencyGraph = new DependencyGraph();
        }

        /// <summary>
        /// Contructs a new spreadsheet, (4 arguments from user)
        /// </summary>
        public Spreadsheet(string path, Func<string, bool> isValid, Func<string, string> normalize, string version) 
            : base(isValid, normalize, version)
        {
            cells = new Dictionary<string, Cell>();
            dependencyGraph = new DependencyGraph();
            GetSavedVersion(path);
        }

        /// <summary>
        /// returns all the cells that have had contents added to them
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return cells.Keys;
        }

        /// <summary>
        /// Method to retrieve the contents of a cell
        /// </summary>
        /// <param name="name">The cell we want the contents of</param>
        /// <returns></returns>
        public override object GetCellContents(string name)
        {
            name = Normalize(name);
            if (name == null || !isValidName(name))
                throw new InvalidNameException();
            if (!cells.ContainsKey(name))
            {
                return "";
            }
            return cells[name].CellContents;
        }

        /// <summary>
        /// Sets the contents of the specified cell to the number specified
        /// </summary>
        /// <param name="name">The cell we want to set the contents of</param>
        /// <param name="number">The contents we want the cell to hold</param>
        /// <returns>The other cells whose value depends on this cell</returns>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            if (name == null || !isValidName(name))
                throw new InvalidNameException();
            if (cells.ContainsKey(name))
            {
                try
                {
                    if (!(cells[name].CellContents is double))
                    {
                        Formula oldContents = (Formula)cells[name].CellContents;
                        if (oldContents is Formula)
                        {
                            foreach (string s in oldContents.GetVariables())
                            {
                                dependencyGraph.RemoveDependency(s, name);
                            }
                        }
                    }
                }
                catch { }
                cells.Remove(name);
            }
            cells.Add(name, new Cell(number));
            
            HashSet<string> dependentCells = new HashSet<string>(GetCellsToRecalculate(name));
            foreach (string s in dependentCells)
            {
                if (cells.ContainsKey(s))
                {
                    if (cells[s].CellContents is Formula)
                    {
                        SetCellContents(s, (Formula)cells[s].CellContents);
                    }
                }
            }
            hasChanged = true;
            return dependentCells;
        }

        /// <summary>
        /// Sets the contents of the specified cell to the text specified
        /// </summary>
        /// <param name="name">The cell we want to set the contents of</param>
        /// <param name="text">The contents we want the cell to hold</param>
        /// <returns>The other cells whose value depends on this cell</returns>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            if (name == null || !isValidName(name))
                throw new InvalidNameException();
            if (text == null)
                throw new ArgumentNullException();
            if (cells.ContainsKey(name))
            {
                try
                {
                    Formula oldContents = (Formula)cells[name].CellContents;
                    foreach (string s in oldContents.GetVariables())
                    {
                        dependencyGraph.RemoveDependency(s, name);
                    }     
                }
                catch { }
                cells.Remove(name);
            }
            if (text != "")
            {
                cells.Add(name, new Cell(text));
            }
            HashSet<string> dependentCells = new HashSet<string>(GetCellsToRecalculate(name));
            foreach (string s in dependentCells)
            {
                if (cells.ContainsKey(s))
                {
                    if (cells[s].CellContents is Formula)
                    {
                        SetCellContents(s, (Formula)cells[s].CellContents);
                    }
                }
            } 
            hasChanged = true;
            return dependentCells;
        }

        /// <summary>
        /// Sets the contents of the specified cell to the Formula specified
        /// </summary>
        /// <param name="name">The cell we want to set the contents of</param>
        /// <param name="formula">The contents we want the cell to hold</param>
        /// <returns>The other cells whose value depends on this cell</returns>
        protected override ISet<string> SetCellContents(string name, SpreadsheetUtilities.Formula formula)
        {
            if (formula == null)
                throw new ArgumentNullException();
            if (name == null || !isValidName(name))
                throw new InvalidNameException();
            if (cells.ContainsKey(name))
            {
                try
                {
                    if (!(cells[name].CellContents is double))
                    {
                        Formula oldValue = (Formula)cells[name].CellContents;
                        foreach (string s in oldValue.GetVariables())
                        {
                            dependencyGraph.RemoveDependency(s, name);
                        }
                    }
                }
                catch {}
            }
            cells.Remove(name);
            cells.Add(name, new Cell(formula, lookup));

            foreach (string v in formula.GetVariables())
            {
                dependencyGraph.AddDependency(v, name);
            }

            HashSet<string> dependentCells = new HashSet<string>(GetCellsToRecalculate(name));
            foreach (string s in dependentCells)
            {
                if (cells.ContainsKey(s))
                {
                    if (cells[s].CellContents is Formula)
                    {
                        Formula oldValue = (Formula)cells[s].CellContents;
                        cells.Remove(s);
                        cells.Add(s, new Cell(oldValue, lookup));
                    }
                }
            }
            hasChanged = true;
            return dependentCells;
        }

        /// <summary>
        /// Helper method to get the dependents of a specified cell
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            name = Normalize(name);
            if (name == null)
                throw new ArgumentNullException();
            if (!isValidName(name) || !IsValid(name))
                throw new InvalidNameException();
            return dependencyGraph.GetDependents(name);
        }

        /// <summary>
        /// Helper method to check for valid cell names
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool isValidName(string name)
        {
            name = Normalize(name);
            return Regex.IsMatch(name, @"^[a-zA-Z]+(?: [a-zA-Z]|\d)*$");
        }

        /// <summary>
        /// Property to keep track of changes in spreadsheet
        /// </summary>
        public override bool Changed
        {
            get
            {
                return hasChanged;
            }
            protected set
            {
            }
        }

        /// <summary>
        /// Loads the saved XML file
        /// </summary>
        /// <param name="filename">Name of file to be loaded and read</param>
        /// <returns></returns>
        public override string GetSavedVersion(string filename)
        {
            string savedVersion = "";
            string cellName = "";
            string cellContents = "";
            bool hasValue;

            try
            {
                XmlTextReader reader = new XmlTextReader(filename);
                while (reader.Read())
                {
                    XmlNodeType nType = reader.NodeType;
                    if (nType == XmlNodeType.Element && reader.Name.ToLower() == "spreadsheet")
                    {
                        savedVersion = reader.GetAttribute(0);
                    }
                    if (nType == XmlNodeType.Element && reader.Name.ToLower() == "name")
                    {

                        reader.Read();
                        hasValue = reader.HasValue;
                        cellName = reader.Value;
                        reader.Read();
                        reader.Read();
                        reader.Read();
                        nType = reader.NodeType;
                    }
                    if (nType == XmlNodeType.Element && reader.Name.ToLower() == "contents")
                    {
                        reader.Read();
                        cellContents = reader.Value;
                        SetContentsOfCell(cellName, cellContents);
                    }
                }
            }
            catch
            {
                throw new SpreadsheetReadWriteException("could not read file");
            }
            return savedVersion;
        }

        /// <summary>
        /// Saves the spreadsheet with the given file name
        /// </summary>
        /// <param name="filename">Name to save file as</param>
        public override void Save(string filename)
        {
            try
            {
                XmlTextWriter writer = new XmlTextWriter(filename, null);
                writer.WriteStartDocument();
                writer.WriteStartElement("Spreadsheet");
                writer.WriteAttributeString("Version", Version);
                writer.WriteString("\n\n");
                foreach(KeyValuePair<string, Cell> c in cells)
                {
                    writer.WriteStartElement("Cell");
                    writer.WriteString("\n");
                    writer.WriteElementString("Name", "\n" + c.Key + "\n");
                    writer.WriteString("\n");
                    if (c.Value.CellContents is string)
                    {
                        writer.WriteElementString("Contents", "\n" + (string)c.Value.CellContents + "\n");
                        writer.WriteString("\n");
                    }
                    if (c.Value.CellContents is double)
                    {
                        writer.WriteElementString("Contents", "\n" + c.Value.CellContents.ToString() + "\n");
                        writer.WriteString("\n");
                    }
                    if (c.Value.CellContents is Formula)
                    {
                        writer.WriteElementString("Contents", "\n=" + c.Value.CellContents.ToString() + "\n");
                        writer.WriteString("\n");
                    }
                    writer.WriteEndElement();
                    writer.WriteString("\n\n");
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();

                hasChanged = false;
            }
            catch
            {
                throw new SpreadsheetReadWriteException("could not write to file");
            }
        }

        /// <summary>
        /// Looks up the value of the given cell
        /// </summary>
        /// <param name="name">cell whose value is wanted</param>
        /// <returns></returns>
        public override object GetCellValue(string name)
        {
            if (name == null || !IsValid(name) || !isValidName(name))
                throw new InvalidNameException();
            name = Normalize(name);
            if (IsValid(name))
            {
                if (!cells.ContainsKey(name))
                    return "";
                return cells[name].CellValue;
            }
            else
                throw new InvalidNameException();
        }

        /// <summary>
        /// Sets the contents of the cell. Determines whether the
        /// contents are a string, double, or formula and sets each
        /// appropriately
        /// </summary>
        /// <param name="name">Name of cell whose contents are to be set</param>
        /// <param name="content"></param>
        /// <returns></returns>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            if (content == null)
                throw new ArgumentNullException();
            if (name == null || !IsValid(name))
                throw new InvalidNameException();
            name = removeNewLines(name);
            content = removeNewLines(content);
           
            name = Normalize(name);
            if(IsValid(name))
            {
                double doubleValue = 0.0;
                bool isDouble = double.TryParse(content, out doubleValue);
                HashSet<string> cellsToRecalc;

                if (isDouble)
                    cellsToRecalc = (HashSet<string>)SetCellContents(name, doubleValue);
                else if (Regex.IsMatch(content, @"[\+\-\*\/=]"))
                {
                    char[] splitFormula = content.ToCharArray();
                    string newFormula = "";
                    for (int i = 0; i < splitFormula.Length; i++)
                    {
                        if (splitFormula[i] != '=')
                            newFormula += splitFormula[i];
                    }
                    newFormula = Normalize(newFormula);
                    Formula f = new Formula(newFormula);
                    foreach (string s in f.GetVariables())
                    {
                        if (!IsValid(s))
                            throw new FormulaFormatException("One or more variables is not valid");
                    }
                    cellsToRecalc = (HashSet<string>)SetCellContents(name, f);
                }
                else
                    cellsToRecalc = (HashSet<string>)SetCellContents(name, content);
                return cellsToRecalc;
            }
            else
                throw new InvalidNameException();
        }

        /// <summary>
        /// Helper function to look up the value of the given variable
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        private double lookup(string variable)
        {
            object cellValue = GetCellValue(variable);
            try
            {
                double doubleValue = 0.0;
                bool isDouble = double.TryParse(cellValue.ToString(), out doubleValue);
                cellValue = doubleValue;
            }
            catch
            {
                cellValue = 0;
            }
            return (double)cellValue;
        }

        /// <summary>
        /// Helper function to remove new line breaks when reading
        /// in a saved spreadsheet
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string removeNewLines(string s)
        {
            char[] newName = s.ToCharArray();
            s = "";
            for (int i = 0; i < newName.Length; i++)
            {
                if (newName[i] != '\n')
                    s += newName[i];
            }
            return s;
        }
    }

    class Cell
    {

        public Cell(string s)
        {
            CellContents = s;
            CellValue = s;
        }

        public Cell(double d)
        {
            CellContents = d;
            CellValue = d;
        }

        public Cell(Formula f, Func<string,double> lookup)
        {
            CellContents = f;
            CellValue = f.Evaluate(lookup);
        }

        public object CellContents { get; set; }

        public object CellValue { get; set; }
    }
}
