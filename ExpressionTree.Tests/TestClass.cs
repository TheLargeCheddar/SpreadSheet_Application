using System;
using System.Reflection;
using CptS321;
using NUnit.Framework;

namespace ExpressionTree.Tests
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        public void TestCreateAdditionNode()
        {
            OperatorNode testNode;

            testNode = ExpressionTreeFactory.CreateOperatorNode('+');
            Assert.That(testNode is PlusNode);
        }

        [Test]
        public void TestCreateSubtractionNode()
        {
            OperatorNode testNode;

            testNode = ExpressionTreeFactory.CreateOperatorNode('-');
            Assert.That(testNode is MinusNode);
        }

        [Test]
        public void TestCreateMultiplicationNode()
        {
            OperatorNode testNode;

            testNode = ExpressionTreeFactory.CreateOperatorNode('*');
            Assert.That(testNode is MultiplyNode);
        }

        [Test]
        public void TestCreateDivisionNode()
        {
            OperatorNode testNode;

            testNode = ExpressionTreeFactory.CreateOperatorNode('/');
            Assert.That(testNode is DivideNode);
        }

        [Test]
        public void TestPrecedenceIsCorrect()
        {
            OperatorNode addNode = ExpressionTreeFactory.CreateOperatorNode('+');
            OperatorNode subNode = ExpressionTreeFactory.CreateOperatorNode('-');
            OperatorNode mulNode = ExpressionTreeFactory.CreateOperatorNode('*');
            OperatorNode divNode = ExpressionTreeFactory.CreateOperatorNode('/');

            Assert.That(addNode.Precedence, Is.EqualTo(subNode.Precedence));
            Assert.That(mulNode.Precedence, Is.EqualTo(divNode.Precedence));
            Assert.That(mulNode.Precedence, Is.GreaterThan(addNode.Precedence));
            Assert.That(divNode.Precedence, Is.GreaterThan(subNode.Precedence));
        }

        [Test]
        public void TestAddition()
        {
            CptS321.ExpressionTree tree = new CptS321.ExpressionTree("1+2+3");
            Assert.That(tree.Evaluate() == 6);

            tree.InFixExpression = "5+10+100+10000";
            Assert.That(tree.Evaluate() == 10115);
        }

        [Test]
        public void TestSubtraction()
        {
            CptS321.ExpressionTree tree = new CptS321.ExpressionTree("10-1");
            Assert.That(tree.Evaluate() == 9);

            tree.InFixExpression = "10-1-2-3";
            Assert.That(tree.Evaluate() == 4);

            tree.InFixExpression = "10000-1000-100-10-1";
            Assert.That(tree.Evaluate() == 8889);
        }

        [Test]
        public void TestDivision()
        {
            CptS321.ExpressionTree tree = new CptS321.ExpressionTree("10/2");
            Assert.That(tree.Evaluate() == 5);

            tree.InFixExpression = "100/2/2/5";
            Assert.That(tree.Evaluate() == 5);
        }

        [Test]
        public void TestMultiplication()
        {
            CptS321.ExpressionTree tree = new CptS321.ExpressionTree("10*2");
            Assert.That(tree.Evaluate() == 20);

            tree.InFixExpression = "1*2*3*10";
            Assert.That(tree.Evaluate() == 60);
        }

        [Test]
        public void TestEvaluateExpressionsWithParenthesis()
        {
            CptS321.ExpressionTree tree = new CptS321.ExpressionTree("(3+1)*2+1*(1+4)");
            Assert.That(tree.Evaluate() == 13);

            tree.InFixExpression = "((10+1)*(3+2)+1)-1";
            Assert.That(tree.Evaluate() == 55);

            tree.InFixExpression = "((100-50)/(23+27))";
            Assert.That(tree.Evaluate() == 1);

            tree.InFixExpression = "(10/2-5+10/2-2*3)";
            Assert.That(tree.Evaluate() == -1);
        }

        [TestCase("(48+73-100)*0", 0)]
        [TestCase("(5*100) + 10*(10-5)", 550)]
        [TestCase("100/10+(80-90*1)", 0)]
        public void TestMoreExpressions(string expression, double result)
        {
            CptS321.ExpressionTree tree = new CptS321.ExpressionTree(expression);
            Assert.That(tree.Evaluate() == result);
        }

        [Test]
        public void TestEdgeCases()
        {
            CptS321.ExpressionTree tree = new CptS321.ExpressionTree("(3+1");
            Assert.Throws<FormatException>(() => { tree.Evaluate(); }); //lambda expression of the above.
        }
    }
}
