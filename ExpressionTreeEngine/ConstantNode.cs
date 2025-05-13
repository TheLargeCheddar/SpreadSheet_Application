// <copyright file="ConstantNode.cs" company="PlaceholderCompany">
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
    /// Constant node.
    /// </summary>
    public class ConstantNode : BaseNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantNode"/> class.
        /// </summary>
        /// <param name="value"> value of constant. </param>
        public ConstantNode(double value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or Sets Value.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Evaluates node.
        /// </summary>
        /// <returns> value of constant. </returns>
        public override double Evaluate()
        {
            return this.Value;
        }
    }
}
