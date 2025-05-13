// <copyright file="ExpressionTreeFactory.cs" company="PlaceholderCompany">
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
    /// Factory that can create individual OperatorNodes.
    /// </summary>
    public class ExpressionTreeFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTreeFactory"/> class.
        /// </summary>
        public ExpressionTreeFactory()
        {
        }

        /// <summary>
        /// Types of Operators.
        /// </summary>
        public static Dictionary<char, Type> Operators = new Dictionary<char, Type>
        {
            { '+', typeof(PlusNode) },
            { '-', typeof(MinusNode) },
            { '*', typeof(MultiplyNode) },
            { '/', typeof(DivideNode) },
        };

        /// <summary>
        /// Creates and returns an OperatorNode based on the provided character.
        /// </summary>
        /// <param name="c">Expected to be an operator.</param>
        /// <returns>A subclass Node of the OperatorNode base class.</returns>
        public static OperatorNode CreateOperatorNode(char c)
        {
            if (Operators.ContainsKey(c))
            {
                object operatorNodeObject = System.Activator.CreateInstance(Operators[c]);
                if (operatorNodeObject is OperatorNode)
                {
                    return (OperatorNode)operatorNodeObject;
                }

                throw new Exception("Unhandled exception");
            }

            return null;
        }
    }
}
