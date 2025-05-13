// <copyright file="MinusNode.cs" company="PlaceholderCompany">
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
    /// Minus Node.
    /// </summary>
    public class MinusNode : OperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MinusNode"/> class.
        /// </summary>
        public MinusNode()
            : base('-')
        {
        }

        /// <summary>
        /// Gets or sets Precedence.
        /// </summary>
        public override ushort Precedence { get; set; } = 2;

        /// <summary>
        /// Subtracts the left child from the right child.
        /// </summary>
        /// <returns>double result of evaluation.</returns>
        public override double Evaluate()
        {
            try
            {
                return this.Right.Evaluate() - this.Left.Evaluate();
            }
            catch (Exception e)
            {
                Console.WriteLine("---Error applying operator to children---");
                throw new Exception("Left or Right child was not a constant node or Value was not set.");
            }
        }
    }
}
