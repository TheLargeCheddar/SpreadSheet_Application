// <copyright file="OperatorNode.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CptS321
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Superclass of all Operators to be used in the expression tree.
    /// </summary>
    public abstract class OperatorNode : BaseNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorNode"/> class.
        /// </summary>
        /// <param name="c"> operator character.</param>
        public OperatorNode(char c)
        {
            this.Operator = c;
            this.Left = this.Right = null;
        }

        /// <summary>
        /// Gets or Sets Operator for node.
        /// </summary>
        public char Operator { get; set; }

        /// <summary>
        /// Gets or sets left child.
        /// </summary>
        public BaseNode Left { get; set; }

        /// <summary>
        /// Gets or sets right child.
        /// </summary>
        public BaseNode Right { get; set; }

        /// <summary>
        /// Gets or sets determines the order of operations applied to the expression.
        /// </summary>
        public abstract ushort Precedence { get; set; }

        /// <summary>
        /// Must be overridden in subclasses to perform some evaluation on its contents and return a double result.
        /// </summary>
        /// <returns> Evaluated double.</returns>
        public abstract override double Evaluate();
    }
}
