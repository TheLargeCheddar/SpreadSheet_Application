// <copyright file="Form1.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CptS321
{
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

    /// <summary>
    /// Form.
    /// </summary>
    public partial class Form1 : Form
    {
        private Spreadsheet spreadsheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        public Form1()
        {
            this.InitializeComponent();

            this.Load += new EventHandler(this.Form1_Load);

            this.spreadsheet = new Spreadsheet(50, 26);
            this.spreadsheet.PropertyChanged += this.CellChangedHandler;
            this.undoToolStripMenuItem.Enabled = false;
            this.redoToolStripMenuItem.Enabled = false;

            this.undoToolStripMenuItem.Click += new System.EventHandler(this.UndoToolStripMenuItem_Click);
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.RedoToolStripMenuItem_Click);
            this.changeColorOfSelectedCellsToolStripMenuItem.Click += new System.EventHandler(this.ChangeColorOfSelectedCellsToolStripMenuItem_Click);
            this.loadFromXMLToolStripMenuItem.Click += new System.EventHandler(this.LoadFromXMLToolStripMenuItem_Click);
            this.saveToXMLToolStripMenuItem.Click += new System.EventHandler(this.SaveToXMLToolStripMenuItem_Click);
        }

        /// <summary>
        /// Load method.
        /// </summary>
        /// <param name="sender"> sender. </param>
        /// <param name="e"> e. </param>
        private void Form1_Load(object sender, EventArgs e)
        {
            this.SetupDataGridView();
        }

        /// <summary>
        /// Sets up the DataGridView.
        /// </summary>
        private void SetupDataGridView()
        {
            this.spreadSheetGrid.Columns.Clear();
            this.spreadSheetGrid.Rows.Clear();
            this.spreadSheetGrid.AutoGenerateColumns = false;
            this.spreadSheetGrid.AutoResizeColumns();

            this.spreadSheetGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            this.spreadSheetGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            this.spreadSheetGrid.ColumnHeadersDefaultCellStyle.Font =
                                    new Font(this.spreadSheetGrid.Font, FontStyle.Bold);

            int asciiForA = 65;
            for (int column = 1; column <= 26; column++)
            {
                string columnName = ((char)(column - 1 + asciiForA)).ToString();
                string headerText = ((char)(column - 1 + asciiForA)).ToString();
                this.spreadSheetGrid.Columns.Add(columnName, headerText);
            }

            object[] defaultRow = new object[this.spreadSheetGrid.ColumnCount];

            for (int row = 1; row <= 50; row++)
            {
                DataGridViewRow newDefaultRow = new DataGridViewRow();
                this.spreadSheetGrid.Rows.Add(newDefaultRow);
                this.spreadSheetGrid.Rows[row - 1].HeaderCell.Value = row.ToString();
            }
        }

        /// <summary>
        /// Event handler for CellBeginEdit, changes the DataGridView's respective Cell value to match the Cell Text property.
        /// </summary>
        /// <param name="sender"> sender. </param>
        /// <param name="e"> e.</param>
        private void SpreadSheetGrid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            this.spreadSheetGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = this.spreadsheet.SpreadsheetArray[e.RowIndex, e.ColumnIndex].Text;
        }

        /// <summary>
        /// Event handler for CellEndEdit, changes the DataGridView's Value to match the Cell's evaluated Value.
        /// </summary>
        /// <param name="sender"> sender. </param>
        /// <param name="e"> e. </param>
        private void SpreadSheetGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Console.WriteLine("Finished Editing Cell at ({0}, {1})",
            // e.RowIndex, e.ColumnIndex);
            var prevCell = this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex).CreateCopy();
            this.spreadsheet.AddUndo(prevCell, "Text change");

            this.undoToolStripMenuItem.Enabled = true;
            this.undoToolStripMenuItem.Text = "Undo " + this.spreadsheet.PeekUndoPropName();

            if (this.spreadSheetGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                this.spreadsheet.SpreadsheetArray[e.RowIndex, e.ColumnIndex].Text = this.spreadSheetGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                this.spreadSheetGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = this.spreadsheet.SpreadsheetArray[e.RowIndex, e.ColumnIndex].Value;
            }
        }

        /// <summary>
        /// Event handler to update the visible value of the DataGridView cell to match the updated Cell's evaluated value.
        /// </summary>
        /// <param name="sender"> sender. </param>
        /// <param name="e"> e. </param>
        private void CellChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.CompareTo("Text") == 0)
            {
                this.spreadSheetGrid.Rows[((Cell)sender).RowIndex].Cells[((Cell)sender).ColumnIndex].Value = this.spreadsheet.SpreadsheetArray[((Cell)sender).RowIndex, ((Cell)sender).ColumnIndex].Value;
            }
            else if (e.PropertyName.CompareTo("BGColor") == 0)
            {
                this.spreadSheetGrid.Rows[((Cell)sender).RowIndex].Cells[((Cell)sender).ColumnIndex].Style.BackColor = Color.FromArgb((int)this.spreadsheet.SpreadsheetArray[((Cell)sender).RowIndex, ((Cell)sender).ColumnIndex].BGColor);
            }
        }

        /// <summary>
        /// handles clicking change color button.
        /// </summary>
        /// <param name="sender"> sender. </param>
        /// <param name="e"> e. </param>
        private void ChangeColorOfSelectedCellsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.spreadSheetGrid.SelectedCells.Count >= 1)
            {
                ColorDialog myDialog = new ColorDialog();

                // Keeps the user from selecting a custom color.
                myDialog.AllowFullOpen = false;

                // Allows the user to get help. (The default is false.)
                myDialog.ShowHelp = true;

                // Sets the initial color select to the current text color.
                myDialog.Color = this.spreadSheetGrid.SelectedCells[0].Style.BackColor;

                // Update the text box color if the user clicks OK
                if (myDialog.ShowDialog() == DialogResult.OK)
                {
                    // Need to get selected Cells in order to change their color.
                    for (int index = 0; index < this.spreadSheetGrid.SelectedCells.Count; index++)
                    {
                        int rowIndex = this.spreadSheetGrid.SelectedCells[index].RowIndex;
                        int columnIndex = this.spreadSheetGrid.SelectedCells[index].ColumnIndex;

                        // Add each cell to the undoStack in Spreadsheet.
                        var prevCell = this.spreadsheet.GetCell(rowIndex, columnIndex).CreateCopy();
                        this.spreadsheet.AddUndo(prevCell, "Color change");

                        this.undoToolStripMenuItem.Enabled = true;
                        this.undoToolStripMenuItem.Text = "Undo " + this.spreadsheet.PeekUndoPropName();

                        this.spreadsheet.SpreadsheetArray[rowIndex, columnIndex].BGColor =
                            (uint)((myDialog.Color.A << 24)
                            | (myDialog.Color.R << 16)
                            | (myDialog.Color.G << 8)
                            | (myDialog.Color.B << 0));
                    }
                }
            }
        }

        /// <summary>
        /// handles Undobutton being clicked.
        /// </summary>
        /// <param name="sender">sender.</param>
        /// <param name="e">e.</param>
        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var undoneCell = this.spreadsheet.ExecuteUndo();
            if (undoneCell != null)
            {
                this.spreadsheet.AddRedo(undoneCell);
                this.redoToolStripMenuItem.Enabled = true;
                this.redoToolStripMenuItem.Text = "Redo " + this.spreadsheet.PeekRedoPropName();
            }

            if (this.spreadsheet.GetUndoStackSize() == 0)
            {
                this.undoToolStripMenuItem.Enabled = false;
            }
            else
            {
                this.undoToolStripMenuItem.Enabled = true;
            }
        }

        /// <summary>
        /// handles redobutton being clicked.
        /// </summary>
        /// <param name="sender"> sender. </param>
        /// <param name="e"> e. </param>
        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.spreadsheet.ExecuteRedo();
            if (this.spreadsheet.GetRedoStackSize() == 0)
            {
                this.redoToolStripMenuItem.Enabled = false;
            }
            else
            {
                this.redoToolStripMenuItem.Enabled = true;
            }
        }

        /// <summary>
        /// handles button click of load.
        /// </summary>
        /// <param name="sender"> sender.</param>
        /// <param name="e">e.</param>
        private void LoadFromXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = AppContext.BaseDirectory;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Get the path of specified file
                string filePath = openFileDialog.FileName;

                // Read the contents of the file into a stream
                var fileStream = openFileDialog.OpenFile();

                // Before loading, clear the sheet.
                this.spreadsheet.ClearSheet();

                this.spreadsheet.LoadFromXML(fileStream);
            }
        }

        /// <summary>
        /// handles clicking of button savetoxml.
        /// </summary>
        /// <param name="sender">sender.</param>
        /// <param name="e">e.</param>
        private void SaveToXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream fileStream;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = AppContext.BaseDirectory;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if ((fileStream = saveFileDialog.OpenFile()) != null)
                {
                    // Code to write the stream goes here.
                    this.spreadsheet.SaveToXML(fileStream);
                    fileStream.Close();
                }
            }

            string filePath = saveFileDialog.FileName;
        }

        /// <summary>
        /// Run the Demo.
        /// </summary>
        /// <param name="sender"> sender. </param>
        /// <param name="e"> e. </param>
        private void Button1_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            int row, column;
            for (int x = 0; x < 50; x++)
            {
                row = rnd.Next(50);
                column = rnd.Next(26);
                this.spreadSheetGrid.Rows[row].Cells[column].Value = "Hello World!";
            }

            for (row = 0; row < this.spreadSheetGrid.RowCount; row++)
            {
                this.spreadSheetGrid.Rows[row].Cells[1].Value = "This is cell B" + (row + 1);
            }

            for (row = 0; row < this.spreadSheetGrid.RowCount; row++)
            {
                this.spreadSheetGrid.Rows[row].Cells[0].Value = "=B" + (row + 1);
            }
        }
    }
}
