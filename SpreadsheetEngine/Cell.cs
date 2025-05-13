// <copyright file="Cell.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// Jack Leeper -11708252
namespace CptS321
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Cell class to populate spreadsheet.
    /// </summary>
    public abstract class Cell : INotifyPropertyChanged
    {
        /// <summary>
        /// Expression for cell.
        /// </summary>
        public ExpressionTree MyExpressionTree;

        /// <summary>
        /// text.
        /// </summary>
        protected string text;

        /// <summary>
        /// value.
        /// </summary>
        protected string value;

        /// <summary>
        /// color of cell.
        /// </summary>
        protected uint bgColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        public Cell()
        {
            this.text = string.Empty;
            this.value = string.Empty;
            this.RowIndex = 0;
            this.ColumnIndex = 0;
            this.BGColor = 0xFFFFFFFF;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets property for the actual text that is typed in the cell.
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                if (this.text.CompareTo(value) != 0)
                {
                    this.text = value;
                    this.OnPropertyChanged("Text");
                }
            }
        }

        /// <summary>
        /// Gets or sets property for the result of the evaluated text in the cell.
        /// (This is just Text, unless the first character is '=')
        /// Allow Spreadsheet Class to set value, but no other class can.
        /// </summary>
        public string Value
        {
            get
            {
                if (this.text.Length > 0)
                {
                    if (this.text[0] == '=')
                    {
                        return this.value;
                    }
                    else
                    {
                        return this.text;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (Environment.StackTrace.Contains("Spreadsheet"))
                {
                    this.value = value;
                    this.OnPropertyChanged("Value");
                }
                else
                {
                    throw new MemberAccessException("Only the Spreadsheet class should be able to access this property.");
                }
            }
        }

        /// <summary>
        /// Gets or Sets Background color of cell.
        /// </summary>
        public uint BGColor
        {
            get
            {
                return this.bgColor;
            }

            set
            {
                if (this.bgColor != value)
                {
                    this.bgColor = value;
                    this.OnPropertyChanged("BGColor");
                }
            }
        }

        /// <summary>
        /// Gets or Sets Rowindex.
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// Gets or Sets Column Index.
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        /// Creates copy of cell.
        /// </summary>
        /// <returns> returns cell. </returns>
        public Cell CreateCopy()
        {
            Cell newCell = new SsCell
            {
                Text = this.Text,
                BGColor = this.BGColor,
                RowIndex = this.RowIndex,
                ColumnIndex = this.ColumnIndex,
            };

            return newCell;
        }

        /// <summary>
        /// Event Handler anytime a cell's text is changed.
        /// </summary>
        /// <param name="sender"> sender. </param>
        /// <param name="e"> e. </param>
        public void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.PropertyName));
        }

        /// <summary>
        /// checks cell for changed attributes.
        /// </summary>
        /// <returns> returns bool. </returns>
        internal bool HasAttributes()
        {
            return this.text.Length > 0 || this.bgColor != 0xFFFFFFFF;
        }

        /// <summary>
        /// Raises PropertyChanged event when a Cell's property changes (Text or Value).
        /// </summary>
        /// <param name="name"> name of property. </param>
        protected void OnPropertyChanged(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
