// <copyright file="VariableNode.cs" company="PlaceholderCompany">
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
    /// Node for variables.
    /// </summary>
    public class VariableNode : BaseNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariableNode"/> class.
        /// </summary>
        /// <param name="name"> name of variable. </param>
        /// <param name="value"> value of variable. </param>
        public VariableNode(string name, double value = 0)
        {
            this.Name = name;
            this.Value = value; // default value = 0.
        }

        /// <summary>
        /// Gets or Sets Name of variable, will be replaced by value in the expression.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets double Value that the variable holds.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Evaluation simply returns the Value.
        /// </summary>
        /// <returns>double Value</returns>
        public override double Evaluate()
        {
            return this.Value;
        }
    }
}
