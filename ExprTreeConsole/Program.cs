using CptS321;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprTreeConsole
{
    internal class Program
    {
        private static ExpressionTree expressionTree;

        /// <summary>
        /// Runs the ExpressionTree program in the command line.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            expressionTree = new ExpressionTree("1+1");
            RunExpressionProgram();
        }

        /// <summary>
        /// Presents a menu to user for expression tree operations.
        /// </summary>
        public static void RunExpressionProgram()
        {
            string input = "";
            //int option;
            bool quitFlag = false;

            do
            {
                PrintMenu();
                input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        PromptEnterExpression();
                        break;
                    case "2":
                        PromptSetVariableValue();
                        break;
                    case "3":
                        Console.WriteLine("Evaluates to:   " + expressionTree.Evaluate());
                        break;
                    case "4":
                        quitFlag = true;
                        break;
                    default:
                        continue;
                }
            } while (!quitFlag);
        }

        private static void PromptEnterExpression()
        {
            Console.Write("Enter a new expression: ");
            expressionTree = new ExpressionTree(Console.ReadLine());
            expressionTree.CompileAndBuildTree();
        }

        private static void PromptSetVariableValue()
        {
            string variableName = "";
            string variableValue = "";

            Console.Write("Enter a variable name: ");
            variableName = Console.ReadLine();

            //Check if the variable exists in the expression
            if (expressionTree.Variables.Keys.Contains(variableName))
            {
                Console.Write("Enter a value for {0}: ", variableName);
                variableValue = Console.ReadLine();
                if (Double.TryParse(variableValue, out _))
                    expressionTree.SetVariable(variableName, Double.Parse(variableValue));
                else
                    Console.WriteLine("Could not cast value as Double.");
            }
            else
            {
                Console.WriteLine("Couldn't find a variable by that name.");
                return;
            }
        }

        public static void PrintMenu()
        {
            Console.WriteLine("Menu   -   Current Expression: \"{0}\"", expressionTree.InFixExpression);
            Console.WriteLine("\t1. Enter a new expression.");
            Console.WriteLine("\t2. Set a variable name");
            Console.WriteLine("\t3. Evaluate Tree");
            Console.WriteLine("\t4. Quit");
        }
    }
}