// <copyright file="MultiplyNode.cs" company="PlaceholderCompany">
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
    /// Multiply Node.
    /// </summary>
    public class MultiplyNode : OperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiplyNode"/> class.
        /// </summary>
        public MultiplyNode()
            : base('*')
        {
        }

        /// <summary>
        /// Gets or Sets precedence.
        /// </summary>
        public override ushort Precedence { get; set; } = 3;

        /// <summary>
        /// Multiplies the right child by the left child.
        /// </summary>
        /// <returns>double result of evaluation.</returns>
        public override double Evaluate()
        {
            try
            {
                return this.Right.Evaluate() * this.Left.Evaluate();
            }
            catch (Exception e)
            {
                Console.WriteLine("---Error applying operator to children---");
                throw new Exception("Left or Right child was not a constant node or Value was not set.");
            }
        }
    }
}
