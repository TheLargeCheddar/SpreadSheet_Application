// <copyright file="PlusNode.cs" company="PlaceholderCompany">
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
    /// Node denoting addition.
    /// </summary>
    public class PlusNode : OperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlusNode"/> class.
        /// </summary>
        public PlusNode()
            : base('+')
        {
        }

        /// <inheritdoc/>
        public override ushort Precedence { get; set; } = 2;

        /// <summary>
        /// Adds the right child to the left child.
        /// </summary>
        /// <returns>double result of evaluation.</returns>
        public override double Evaluate()
        {
            try
            {
                return this.Left.Evaluate() + this.Right.Evaluate();
            }
            catch (Exception)
            {
                Console.WriteLine("---Error applying operator to children---");
                throw new Exception("Left or Right child was not a constant node or Value was not set.");
            }
        }
    }
}
