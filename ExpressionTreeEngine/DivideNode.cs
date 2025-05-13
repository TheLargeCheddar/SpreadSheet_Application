// <copyright file="DivideNode.cs" company="PlaceholderCompany">
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
    /// Node for divding.
    /// </summary>
    public class DivideNode : OperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DivideNode"/> class.
        /// </summary>
        public DivideNode()
            : base('/')
        {
        }

        /// <summary>
        /// Gets or Sets precedence.
        /// </summary>
        public override ushort Precedence { get; set; } = 3;

        /// <summary>
        /// Divides the right child by the left child.
        /// </summary>
        /// <returns>double result of evaluation.</returns>
        public override double Evaluate()
        {
            try
            {
                return this.Right.Evaluate() / this.Left.Evaluate();
            }
            catch (Exception)
            {
                Console.WriteLine("---Error applying operator to children---");
                throw new Exception("Left or Right child was not a constant node or Value was not set.");
            }
        }
    }
}
