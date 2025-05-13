// <copyright file="BaseNode.cs" company="PlaceholderCompany">
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
    /// Base node.
    /// </summary>
    public abstract class BaseNode
    {
        /// <summary>
        /// Evaluate node.
        /// </summary>
        /// <returns> returns result of evalutaion. </returns>
        public abstract double Evaluate();
    }
}
