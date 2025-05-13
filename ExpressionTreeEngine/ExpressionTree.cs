// <copyright file="ExpressionTree.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// Jack Leeper - 11708252.
namespace CptS321
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// ExpressionTree class.
    /// </summary>
    public class ExpressionTree
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTree"/> class.
        /// </summary>
        /// <param name="expression"> expression for tree. </param>
        public ExpressionTree(string expression)
        {
            this.InFixExpression = expression;
            this.Variables = new Dictionary<string, double>();
            this.ExpressionTreeRoot = null;
        }

        /// <summary>
        /// Gets or sets english arithmetic expression, with assumed no spaces or parenthesis.
        /// </summary>
        public string InFixExpression { get; set; }

        /// <summary>
        /// Gets or sets postfix expression calculated from infix expression.
        /// </summary>
        public string PostFixExpression { get; set; }

        /// <summary>
        /// Gets or sets dictionary of. <string Name, double value> pairs to store variables.
        /// </summary>
        public Dictionary<string, double> Variables { get; set; }

        private BaseNode ExpressionTreeRoot { get; set; }

        /// <summary>
        /// Method to Compile and build the ExpressionTree.
        /// </summary>
        public void CompileAndBuildTree()
        {
            this.Compile();
            this.BuildTree();
        }

        /// <summary>
        /// Sets the value of the variable name to a double value passed in parameter.
        /// </summary>
        /// <param name="variableName"> name of variable. </param>
        /// <param name="variableValue"> value of variable. </param>
        public void SetVariable(string variableName, double variableValue)
        {
            this.Variables[variableName] = variableValue;
        }

        /// <summary>
        /// First needs to build the expression tree based on the infix expression, then evaluate the expression tree from the root.
        /// </summary>
        /// <returns>Double result of the expression tree's evaluation.</returns>
        public double Evaluate()
        {
            double result = 0.0;

            // Compile the infix to a postfix expression, then build the expression tree.
            this.BuildTree();

            // Root of the tree should be an operator. (Unless there is only one variable in the expression)
            if (this.ExpressionTreeRoot != null)
            {
                try
                {
                    BaseNode root = this.ExpressionTreeRoot;
                    result = root.Evaluate();
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException("Root of Expression Tree was not an OperatorNode.");
                }
                catch (Exception)
                {
                    // Which is the proper method? I think we want to throw the exception here and test for that.
                    throw new Exception("Error occured in Evaluate(). Exiting.");
                }
            }

            // Nothing to evaluate
            else
            {
                throw new FormatException("Formatting could not be interpreted.");
            }

            return result;
        }

        /// <summary>
        /// Looks for known variable names.
        /// </summary>
        /// <returns> returns list of variable names.</returns>
        public List<string> GetVariableNames()
        {
            List<string> variableNames = new List<string>();

            var tokens = this.InFixExpression.Split(ExpressionTreeFactory.Operators.Keys.ToArray<char>());
            int asciiFor_A = 65;
            int asciiFor_Z = 90;

            foreach (var token in tokens)
            {
                var tokTemp = token.ToUpper();
                if ((int)tokTemp[0] >= asciiFor_A && (int)tokTemp[0] <= asciiFor_Z)
                {
                    if (int.TryParse(tokTemp.Substring(1), out _))
                    {
                        variableNames.Add(token);
                    }
                }
            }

            return variableNames;
        }

        /// <summary>
        /// Checks if the symbol is a valid operator.
        /// </summary>
        /// <param name="symbol">operator symbol.</param>
        /// <returns>True if symbol is an operator, false otherwise.</returns>
        private static bool IsOperator(char symbol)
        {
            if (ExpressionTreeFactory.CreateOperatorNode(symbol) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Uses the postfix expression to create the expression tree, which can then be evaluated on the root.
        /// </summary>
        private void BuildTree()
        {
            BaseNode newNode;
            Stack<BaseNode> stack = new Stack<BaseNode>();

            // Compile the postfix expression,
            this.Compile();

            // then build the expression tree.
            foreach (var symbol in this.PostFixExpression.Split(' '))
            {
                // token is a constant value
                if (symbol != string.Empty)
                {
                    if (double.TryParse(symbol, out _))
                    {
                        newNode = new ConstantNode(double.Parse(symbol));
                    }

                    // token is a variable name
                    else if (this.Variables.Keys.Contains(symbol))
                    {
                        newNode = new VariableNode(symbol, this.Variables[symbol]);
                    }

                    // token is an operator
                    else if (ExpressionTreeFactory.Operators.ContainsKey(symbol[0]))
                    {
                        newNode = ExpressionTreeFactory.CreateOperatorNode(symbol[0]);
                        ((OperatorNode)newNode).Left = stack.Pop();
                        ((OperatorNode)newNode).Right = stack.Pop();
                    }
                    else
                    {
                        newNode = null;
                    }

                    stack.Push(newNode);
                }
            }

            // Let the root point to the operator at the top of the stack.
            try
            {
                this.ExpressionTreeRoot = stack.Pop();
            }
            catch (Exception)
            {
                throw new Exception("Top of stack was not an operator.");
            }
        }

        /// <summary>
        /// Compiles the string infix-expression into postfix using Dijkstra's Shunting Yard algorithm, which will be used in the construction of the expression tree.
        ///     Inputs: InFixExpression must have a real string expression stored.
        ///     Outputs: a string PostFixExpression, which is the converted infix expression.
        /// </summary>
        private void Compile()
        {
            var stack = new Stack<string>();
            var output = new List<string>(); // Will be joined by spaces and saved in PostFixExpression.
            char symbol;

            // Iterate over the infix expression and use Dijkstra's Shunting Yard algorithm to construct the postfix expression.
            for (int index = 0; index < this.InFixExpression.Length; index++)
            {
                symbol = this.InFixExpression[index];

                // symbol is a constant/operand, add it to the output.
                if (char.IsDigit(symbol))
                {
                    string digitBuilder = string.Empty;

                    // This allows support of numbers greater than one digit long..
                    while (char.IsDigit(symbol) && index < this.InFixExpression.Length)
                    {
                        digitBuilder += symbol;
                        index++;
                        if (index < this.InFixExpression.Length)
                        {
                            symbol = this.InFixExpression[index];
                        }
                    }

                    output.Add(digitBuilder);
                    index--;
                }
                else if (symbol == '(')
                {
                    stack.Push(symbol.ToString());
                }
                else if (symbol == ')')
                {
                    string top = string.Empty;
                    while (stack.Count > 0 && (top = stack.Pop()) != "(")
                    {
                        output.Add(top);
                    }

                    if (top != "(")
                    {
                        throw new ArgumentException("No matching left parenthesis.");
                    }
                }

                // symbol is an operator.
                else if (IsOperator(symbol))
                {
                    OperatorNode thisNode = ExpressionTreeFactory.CreateOperatorNode(symbol);
                    while (stack.Count > 0 && IsOperator(stack.Peek()[0]))
                    {
                        OperatorNode otherNode = ExpressionTreeFactory.CreateOperatorNode(stack.Peek()[0]);

                        // We aren't worried about associativity yet, so just compare precedence.
                        if (otherNode.Precedence >= thisNode.Precedence)
                        {
                            output.Add(stack.Pop());
                        }
                        else
                        {
                            break;
                        }
                    }

                    stack.Push(symbol.ToString());
                }

                // symbol is a variable name, add it to the variable dictionary
                else
                {
                    string variableNameBuilder = string.Empty;
                    while (!ExpressionTreeFactory.Operators.ContainsKey(symbol) && symbol != '(' && symbol != ')' && index < this.InFixExpression.Length)
                    {
                        variableNameBuilder += symbol;
                        index++;
                        if (index < this.InFixExpression.Length)
                        {
                            symbol = this.InFixExpression[index];
                        }
                    }

                    output.Add(variableNameBuilder);
                    if (!this.Variables.ContainsKey(variableNameBuilder))
                    {
                        this.Variables.Add(variableNameBuilder, 0);
                    }

                    index--;
                }
            }

            // Add the rest of the elements to the postfix expression output.
            while (stack.Count > 0)
            {
                var top = stack.Pop();
                output.Add(top);
            }

            this.PostFixExpression = string.Join(" ", output);

            // Console.WriteLine("Created the postfix expression: {0} based on infix expression: {1}", this.PostFixExpression, this.InFixExpression);
        }
    }
}
