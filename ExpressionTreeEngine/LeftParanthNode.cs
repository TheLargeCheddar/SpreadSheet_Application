// <copyright file="LeftParanthNode.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// Jack Leeper - 11708252
namespace ExpressionTreeEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Class representing the Left parantheses operator.
    /// </summary>
    internal class LeftParanthNode : OperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LeftParanthNode"/> class.
        /// </summary>
        public LeftParanthNode()
            : base('(')
        {
        }

        /// <summary>
        /// Gets Precedence value which will be used in determining which operation to execute first.
        /// </summary>
        public int Precedence { get; } = 2;

        /// <summary>
        /// Overridden function of Evaluate, retruns node on right.
        /// </summary>
        /// <param name="left"> double value on left. </param>
        /// <param name="right"> double value on right. </param>
        /// <returns> returns double value calculated from operation. </returns>
        public override double Evaluate(double left, double right)
        {
            return right;
        }
    }
}
