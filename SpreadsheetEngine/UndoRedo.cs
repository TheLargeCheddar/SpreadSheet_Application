// <copyright file="UndoRedo.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CptS321
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Class responsible for handling undo/redo actions for spreadsheet.
    /// </summary>
    public class UndoRedo
    {
        /// <summary>
        /// Name of property change.
        /// </summary>
        public string ChangedPropertyName;

        private Cell oldCell;

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoRedo"/> class.
        /// </summary>
        /// <param name="undoRedoCell"> cell being selected for undo/redo. </param>
        /// <param name="whatChanged"> name of property being changed. </param>
        public UndoRedo(Cell undoRedoCell, string whatChanged)
        {
            this.oldCell = undoRedoCell;
            this.ChangedPropertyName = whatChanged;
        }

        /// <summary>
        /// sets proper cell value.
        /// </summary>
        /// <param name="senderCell"> cell being changed. </param>
        public void Evaluate(ref Cell senderCell)
        {
            senderCell.Text = this.oldCell.Text;
            senderCell.BGColor = this.oldCell.BGColor;
        }

        /// <summary>
        /// Get location of cell being changed.
        /// </summary>
        /// <returns> row and col index of cell. </returns>
        public Tuple<int, int> GetCoordinates()
        {
            return new Tuple<int, int>(this.oldCell.RowIndex, this.oldCell.ColumnIndex);
        }
    }
}
