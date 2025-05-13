// <copyright file="Spreadsheet.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CptS321
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Spreadsheet class.
    /// </summary>
    public class Spreadsheet : INotifyPropertyChanged
    {
        /// <summary>
        /// Cell array that contains all cells in the spreadsheet.
        /// </summary>
        public Cell[,] SpreadsheetArray;

        private PropertyChangedEventHandler propertyChangedEventHandler;

        private Stack<UndoRedo> undoStack;

        private Stack<UndoRedo> redoStack;

        private HashSet<string> currentEvaluationChain = new HashSet<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Spreadsheet"/> class.
        /// </summary>
        /// <param name="rows"> nm of rows. </param>
        /// <param name="columns">num of columns. </param>
        public Spreadsheet(int rows, int columns)
        {
            this.SpreadsheetArray = new Cell[rows, columns];
            this.propertyChangedEventHandler = this.CellPropertyChanged;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    this.SpreadsheetArray[r, c] = new SsCell();
                    this.SpreadsheetArray[r, c].ColumnIndex = c;
                    this.SpreadsheetArray[r, c].RowIndex = r;
                    this.SpreadsheetArray[r, c].Text = string.Empty;
                    this.SpreadsheetArray[r, c].PropertyChanged += this.CellPropertyChanged;
                }
            }

            this.undoStack = new Stack<UndoRedo>();
            this.redoStack = new Stack<UndoRedo>();
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets number of rows in the SpreadsheetArray.
        /// </summary>
        public int RowCount
        {
            get
            {
                // GetLength(0) returns number of elements in 0th dimension (rows)
                return this.SpreadsheetArray.GetLength(0);
            }
        }

        /// <summary>
        /// Gets Number of columns in the SpreadsheetArray.
        /// </summary>
        public int ColumnCount
        {
            get
            {
                // All rows have same number of dimensions (columns), so return length of 1st dimension (columns).
                return this.SpreadsheetArray.GetLength(1);
            }
        }

        /// <summary>
        /// peek at undo stack.
        /// </summary>
        /// <returns> returns string of stack item.</returns>
        public string PeekUndoPropName()
        {
            if (this.undoStack.Count > 0)
            {
                return this.undoStack.Peek().ChangedPropertyName;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// peek at prop name of redo stack.
        /// </summary>
        /// <returns> returns string. </returns>
        public string PeekRedoPropName()
        {
            if (this.redoStack.Count > 0)
            {
                return this.redoStack.Peek().ChangedPropertyName;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// get stack size.
        /// </summary>
        /// <returns> num of elements in stack. </returns>
        public int GetUndoStackSize()
        {
            return this.undoStack.Count;
        }

        /// <summary>
        /// get stack size.
        /// </summary>
        /// <returns> returns num of elements in stack. </returns>
        public int GetRedoStackSize()
        {
            return this.redoStack.Count;
        }

        /// <summary>
        /// Gets/Returns the cell at the specified row and column index, or null if out of bounds.
        /// </summary>
        /// <param name="row"> row of cell. </param>
        /// <param name="column"> column of cell. </param>
        /// <returns> returns selected cell.</returns>
        public Cell GetCell(int row, int column)
        {
            if (row < this.RowCount && row >= 0 && column < this.ColumnCount && column >= 0)
            {
                return this.SpreadsheetArray[row, column];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get cell by name.
        /// </summary>
        /// <param name="cellName"> name of cell. </param>
        /// <returns> returns cell. </returns>
        public Cell GetCell(string cellName)
        {
            if (cellName.Length == 0)
            {
                return null;
            }

            int rowIndex;
            if (int.TryParse(cellName.Substring(1), out rowIndex))
            {
                int colIndex = this.ConvertCharToColumnIndex(cellName[0]);
                return this.GetCell(rowIndex - 1, colIndex);
            }

            // index out of bounds or other error.
            return null;
        }

        /// <summary>
        /// Adds a cell to the undoStack so the user can undo this operation.
        /// </summary>
        /// <param name="undoRedoCell"> cell changed. </param>
        /// <param name="whatChanged"> property changed. </param>
        public void AddUndo(Cell undoRedoCell, string whatChanged)
        {
            this.undoStack.Push(new UndoRedo(undoRedoCell, whatChanged));
        }

        /// <summary>
        /// Adds a cell to the redoStack so the user can redo this operation.
        /// </summary>
        /// <param name="undoRedoCell"> cell changed. </param>
        public void AddRedo(UndoRedo undoRedoCell)
        {
            this.redoStack.Push(undoRedoCell);
        }

        /// <summary>
        /// Executes an undo from the top of the undoStack, then returns an UndoRedo object
        /// that should be pushed onto the redo stack.
        /// </summary>
        /// <returns> undoredo obj. </returns>
        public UndoRedo ExecuteUndo()
        {
            if (this.undoStack.Count > 0)
            {
                var undoCell = this.undoStack.Pop();
                var coordinates = undoCell.GetCoordinates();

                var redoObj = new UndoRedo(this.SpreadsheetArray[coordinates.Item1, coordinates.Item2].CreateCopy(), undoCell.ChangedPropertyName);
                undoCell.Evaluate(ref this.SpreadsheetArray[coordinates.Item1, coordinates.Item2]);

                return redoObj;
            }

            return null;
        }

        /// <summary>
        /// performs redo.
        /// </summary>
        public void ExecuteRedo()
        {
            if (this.redoStack.Count > 0)
            {
                var redoCell = this.redoStack.Pop();
                var coordinates = redoCell.GetCoordinates();
                redoCell.Evaluate(ref this.SpreadsheetArray[coordinates.Item1, coordinates.Item2]);
            }
        }

        /// <summary>
        /// Clears sheet, to be used before load.
        /// </summary>
        public void ClearSheet()
        {
            int rows = this.SpreadsheetArray.GetLength(0);
            int columns = this.SpreadsheetArray.GetLength(1);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    this.SpreadsheetArray[r, c].Text = string.Empty;
                    this.SpreadsheetArray[r, c].Value = string.Empty;
                    this.SpreadsheetArray[r, c].BGColor = 0xFFFFFFFF;
                }
            }
        }

        /// <summary>
        /// Load spreadsheet from xml file.
        /// </summary>
        /// <param name="stream"> stream. </param>
        public void LoadFromXML(Stream stream)
        {
            string filename = "SavedSpreadsheet.xml";
            string path = Path.Combine(AppContext.BaseDirectory, filename);

            this.undoStack.Clear();
            this.redoStack.Clear();
            for (int r = 0; r < this.RowCount; r++)
            {
                for (int c = 0; c < this.ColumnCount; c++)
                {
                    this.SpreadsheetArray[r, c] = new SsCell();
                    this.SpreadsheetArray[r, c].ColumnIndex = c;
                    this.SpreadsheetArray[r, c].RowIndex = r;
                    this.SpreadsheetArray[r, c].Text = string.Empty;
                    this.SpreadsheetArray[r, c].PropertyChanged += this.CellPropertyChanged;
                }
            }

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = false;

            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            Console.WriteLine("Start Element {0}", reader.Name);
                            if (reader.Name == "cell")
                            {
                                string cellName = reader.GetAttribute("cellName");
                                Cell newCell = this.CreateCellFromName(cellName);
                                newCell.Text = reader.GetAttribute("text");
                                newCell.BGColor = Convert.ToUInt32(reader.GetAttribute("bgColor"));
                                this.CopyOverCellContents(ref this.SpreadsheetArray[newCell.RowIndex, newCell.ColumnIndex], newCell);
                                this.RaiseCellPropertyChanged(this.SpreadsheetArray[newCell.RowIndex, newCell.ColumnIndex], "Cell");
                            }

                            break;
                        case XmlNodeType.Text:
                            Console.WriteLine("Text Node: {0}", reader.Value);
                            break;
                        case XmlNodeType.EndElement:
                            Console.WriteLine("End Element {0}", reader.Name);
                            break;
                        default:
                            Console.WriteLine(
                                "Other node {0} with value {1}", reader.NodeType, reader.Value);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Save spreadsheet to xml file.
        /// </summary>
        /// <param name="stream"> stream. </param>
        public void SaveToXML(Stream stream)
        {
            XmlWriterSettings settings = new XmlWriterSettings();

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                writer.WriteStartElement("MySpreadsheet");
                foreach (var cell in this.SpreadsheetArray)
                {
                    if (cell.HasAttributes())
                    {
                        int row = cell.RowIndex;
                        int col = cell.ColumnIndex + 'A';
                        string cellName = ((char)col).ToString() + (row + 1).ToString();
                        Console.WriteLine("Writing a cell to XML with name : {0}", cellName);
                        writer.WriteStartElement("cell");
                        writer.WriteAttributeString("cellName", cellName);
                        writer.WriteAttributeString("text", cell.Text);
                        writer.WriteAttributeString("bgColor", cell.BGColor.ToString());
                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// checks if references self.
        /// </summary>
        /// <param name="formula"> formula. </param>
        /// <param name="cellReference"> cell reference.</param>
        /// <returns>bool.</returns>
        private bool IsSelfReference(string formula, string cellReference)
        {
            // Remove the '=' from the start of the formula
            string formulaWithoutEquals = formula.Substring(1);

            // Split the formula into tokens (operators and cell references)
            string[] operators = { "+", "-", "*", "/" };
            string[] tokens = formulaWithoutEquals.Split(operators, StringSplitOptions.RemoveEmptyEntries);

            // Check each token after trimming whitespace
            foreach (string token in tokens)
            {
                if (token.Trim() == cellReference)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// returns cell refence.
        /// </summary>
        /// <param name="row"> row. </param>
        /// <param name="column"> col. </param>
        /// <returns> string. </returns>
        private string GetCellReference(int row, int column)
        {
            char columnLetter = (char)('A' + column);
            return $"{columnLetter}{row + 1}";
        }

        /// <summary>
        /// Checks cell for incorrect reference.
        /// </summary>
        /// <param name="cellName"> name of cell. </param>
        /// <returns> true or false. </returns>
        private bool ValidateCellReference(string cellName)
        {
            // check if cell name is simply a constant.
            int i = 0;
            bool result = int.TryParse(cellName, out i);
            if (result)
            {
                return true;
            }

            // Check if the cell name format is valid (e.g., "A1", "B2", etc.)
            if (cellName.Length < 2)
            {
                return false;
            }

            char column = char.ToUpper(cellName[0]);
            if (column < 'A' || column > 'Z')
            {
                return false;
            }

            if (!int.TryParse(cellName.Substring(1), out int row))
            {
                return false;
            }

            // Check if the reference is within spreadsheet bounds
            int colIndex = column - 'A';
            int rowIndex = row - 1;

            return rowIndex >= 0 && rowIndex < this.RowCount &&
                   colIndex >= 0 && colIndex < this.ColumnCount;
        }

        private bool CheckReferencesForInvalid(string formula, string cellReference)
        {
            // Remove the '=' from the start of the formula
            string formulaWithoutEquals = formula.Substring(1);

            // Split the formula into tokens (operators and cell references)
            string[] operators = { "+", "-", "*", "/" };
            string[] tokens = formulaWithoutEquals.Split(operators, StringSplitOptions.RemoveEmptyEntries);

            // Check each token for valid references after trimming whitespace
            foreach (string token in tokens)
            {
                if (!this.ValidateCellReference(token.Trim())) // invalid reference found, return true for reference being invalid.
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines references from fromula.
        /// </summary>
        /// <param name="formula"> expression.</param>
        /// <returns>list of references.</returns>
        private List<string> GetCellReferencesFromFormula(string formula)
        {
            List<string> references = new List<string>();
            var tokens = formula.Split(
                new[] { '+', '-', '*', '/', '(', ')' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var token in tokens)
            {
                var trimmedToken = token.Trim();
                if (trimmedToken.Length >= 2 &&
                    char.IsLetter(trimmedToken[0]) &&
                    trimmedToken.Substring(1).All(char.IsDigit))
                {
                    references.Add(trimmedToken);
                }
            }

            return references;
        }

        /// <summary>
        /// Detects if cell makes reference error.
        /// </summary>
        /// <param name="cell"> cell. </param>
        /// <param name="formula"> expression. </param>
        /// <returns>bool.</returns>
        private bool DetectCircularReference(Cell cell, string formula)
        {
            string cellName = this.GetCellReference(cell.RowIndex, cell.ColumnIndex);

            // Check for self-reference
            if (formula.Contains(cellName))
            {
                return true;
            }

            // If we've already seen this cell in the current evaluation chain, we have a circular reference
            if (!this.currentEvaluationChain.Add(cellName))
            {
                return true;
            }

            // Get all cell references from the formula
            var references = this.GetCellReferencesFromFormula(formula);

            foreach (var reference in references)
            {
                if (!this.ValidateCellReference(reference))
                {
                    cell.Value = "(bad reference)";
                    return true;
                }

                var referencedCell = this.GetCell(reference);
                if (referencedCell?.Text.StartsWith("=") == true)
                {
                    if (this.DetectCircularReference(referencedCell, referencedCell.Text.Substring(1)))
                    {
                        return true;
                    }
                }
            }

            this.currentEvaluationChain.Remove(cellName);
            return false;
        }

        /// <summary>
        /// Event Handler anytime a cell's text is changed. Expression tree gets set up if needed,
        /// then the sender Cell subscribes to each variable's PropertyChanged events from the expression.
        /// </summary>
        /// <param name="sender"> sender.</param>
        /// <param name="e"> e. </param>
        private void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Cell senderAsCell;
            int changedRow = ((Cell)sender).RowIndex;
            int changedColumn = ((Cell)sender).ColumnIndex;
            string newTextValue = ((Cell)sender).Text;

            if (sender is Cell)
            {
                senderAsCell = this.GetCell(changedRow, changedColumn);

                if (e.PropertyName.ToString().CompareTo("Text") == 0)
                {
                    // Only evaluate the cell if first char is '=', else just copy Text over.
                    if (senderAsCell != null && senderAsCell.Text.StartsWith("="))
                    {
                        // Reference validation...
                        this.currentEvaluationChain.Clear();
                        string formula = senderAsCell.Text.Substring(1);

                        // Check for references errors
                        if (this.IsSelfReference(senderAsCell.Text, this.GetCellReference(changedRow, changedColumn)))
                        {
                            senderAsCell.Value = "(self reference)";
                            return;
                        }
                        else if (this.CheckReferencesForInvalid(senderAsCell.Text, this.GetCellReference(changedRow, changedColumn)))
                        {
                            senderAsCell.Value = "(bad reference)";
                            return;
                        }
                        else if (this.DetectCircularReference(senderAsCell, formula))
                        {
                            senderAsCell.Value = "(circular reference)";
                            return;
                        }

                        try
                        {
                            if (senderAsCell.MyExpressionTree != null)
                            {
                                List<string> oldVariables = senderAsCell.MyExpressionTree.GetVariableNames();
                                foreach (var varName in oldVariables)
                                {
                                    var otherCell = this.GetCell(varName);
                                    if (otherCell != null)
                                    {
                                        otherCell.PropertyChanged -= senderAsCell.CellPropertyChanged;
                                    }
                                }
                            }

                            senderAsCell.MyExpressionTree = new ExpressionTree(senderAsCell.Text.Substring(1));
                            senderAsCell.MyExpressionTree.CompileAndBuildTree();
                            List<string> variables = senderAsCell.MyExpressionTree.GetVariableNames();

                            foreach (var varName in variables)
                            {
                                // Unsubscribe from old events here.
                                string varValue = this.GetVariableValue(varName);
                                double varValueAsDouble;

                                double.TryParse(varValue, out varValueAsDouble);

                                // Need to do this because strings are not handled by ExpressionTree.
                                if (double.TryParse(varValue, out varValueAsDouble))
                                {
                                    senderAsCell.MyExpressionTree.SetVariable(varName, varValueAsDouble);
                                    senderAsCell.MyExpressionTree.CompileAndBuildTree();
                                    this.SpreadsheetArray[changedRow, changedColumn].Value = senderAsCell.MyExpressionTree.Evaluate().ToString();
                                }
                                else
                                {
                                    this.SpreadsheetArray[changedRow, changedColumn].Value = varValue;
                                }

                                // Subscribe to new events based on variables in expression (Text).
                                var otherCell = this.GetCell(varName);
                                if (otherCell != null)
                                {
                                    otherCell.PropertyChanged += senderAsCell.CellPropertyChanged;
                                }
                            }
                        }
                        catch (MemberAccessException)
                        {
                            Console.WriteLine("MemberAccessException thrown, this should not have been thrown since I am the Spreadsheet!");
                            return;
                        }
                        catch (InvalidCastException exception)
                        {
                            throw new InvalidCastException("Illegal cast in CellPropertyChanged.", exception);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Unhandled Exception in [Spreadsheet] CellPropertyChanged");
                            return;
                        }
                    }
                    else
                    {
                        // Text doesn't have a formula, so don't need an ExpressionTree
                        senderAsCell.MyExpressionTree = null;
                    }

                    this.RaiseCellPropertyChanged(senderAsCell, "Text");
                }
                else if (e.PropertyName.ToString().CompareTo("BGColor") == 0)
                {
                    this.RaiseCellPropertyChanged(senderAsCell, "BGColor");
                }
                else
                {
                    // Console.WriteLine("[Spreadsheet] CellPropertyChanged - another property was changed.");
                }
            }
            else
            {
                throw new Exception("Sender was not type Cell.");
            }
        }

        /// <summary>
        /// Gets the string value of the cell referred to by varName.
        /// </summary>
        /// <param name="varName">Cell name of form: "A1" or "D10". </param>
        /// <returns>Value of cell as string.</returns>
        private string GetVariableValue(string varName)
        {
            int colIndex = this.ConvertCharToColumnIndex(varName[0]);
            int rowIndex;
            if (int.TryParse(varName.Substring(1), out rowIndex))
            {
                rowIndex--;

                // Console.WriteLine("In GetVariableValue() | (rowIndex, colIndex) = ({0}, {1})", rowIndex, colIndex);
                string valueAtVarName = this.SpreadsheetArray[rowIndex, colIndex].Value;

                // If the cell is empty or contains only whitespace, return "0"
                if (string.IsNullOrWhiteSpace(valueAtVarName) || valueAtVarName.Equals(string.Empty))
                {
                    return "0";
                }

                // Console.WriteLine("Grabbed this value from cell's contents " + valueAtVarName);
                return valueAtVarName;
            }
            else
            {
                // Column index was not recognized as an integer.
                return "Errored";
            }
        }

        /// <summary>
        /// Raise Property Changed event when a cell changes.
        /// </summary>
        /// <param name="changedCell"> cell being changed. </param>
        private void RaiseCellPropertyChanged(Cell changedCell, string propertyName)
        {
            this.PropertyChanged?.Invoke(changedCell, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raise Property Changed event when a cell changes.
        /// </summary>
        /// <param name="changedCell"> cell being changed. </param>
        private void RaiseCellPropertyChanged(Cell changedCell)
        {
            this.PropertyChanged?.Invoke(changedCell, new PropertyChangedEventArgs("Cell"));
        }

        /// <summary>
        /// Converts a character to an integer based on ascii values in order to
        /// return column index.
        /// </summary>
        /// <param name="character"> char of column. </param>
        /// <returns>integer column index of character.</returns>
        private int ConvertCharToColumnIndex(char character)
        {
            int asciiForA = 65;
            return (int)character - asciiForA;
        }

        /// <summary>
        /// Copies content of cell.
        /// </summary>
        /// <param name="cell"> old cell. </param>
        /// <param name="newCell"> new cell. </param>
        private void CopyOverCellContents(ref Cell cell, Cell newCell)
        {
            cell.Text = newCell.Text;
            cell.BGColor = newCell.BGColor;
        }

        /// <summary>
        /// Creates cell from entered cell name.
        /// </summary>
        /// <param name="cellName"> name of cell. </param>
        /// <returns> Cell obj. </returns>
        private Cell CreateCellFromName(string cellName)
        {
            if (cellName.Length == 0)
            {
                return null;
            }

            int rowIndex;
            if (int.TryParse(cellName.Substring(1), out rowIndex))
            {
                int colIndex = this.ConvertCharToColumnIndex(cellName[0]);
                SsCell newCell = new SsCell();
                newCell.RowIndex = rowIndex - 1;
                newCell.ColumnIndex = colIndex;
                return newCell;
            }

            // index out of bounds or other error.
            return null;
        }
    }
}
