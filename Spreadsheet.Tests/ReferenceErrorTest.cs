using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.IO;
using CptS321;
using Assert = NUnit.Framework.Assert;

namespace Spreadsheet.Tests
{
    [TestFixture]
    internal class ReferenceErrorTest
    {
        // TDD: Testing reference errors.
        private CptS321.Spreadsheet spreadsheet;

        [SetUp]
        public void Setup()
        {
            spreadsheet = new CptS321.Spreadsheet(50, 26); // Standard size for A-Z columns, 50 rows
        }

        [Test]
        public void TestSelfReference_Direct()
        {
            // Test direct self-reference (A1 = "=A1")
            spreadsheet.GetCell(0, 0).Text = "=A1";
            Assert.That(spreadsheet.GetCell(0, 0).Value, Is.EqualTo("(self reference)"));
        }

        [Test]
        public void TestSelfReference_Complex()
        {
            // Test complex self-reference (A1 = "=B1+A1*2")
            spreadsheet.GetCell(0, 0).Text = "=B1+A1*2";
            Assert.That(spreadsheet.GetCell(0, 0).Value, Is.EqualTo("(self reference)"));
        }

        [Test]
        public void TestCircularReference_Simple()
        {
            // Test simple circular reference (A1 references B1, B1 references A1)
            spreadsheet.GetCell(0, 0).Text = "=B1"; // A1
            spreadsheet.GetCell(0, 1).Text = "=A1"; // B1

            Assert.That(spreadsheet.GetCell(0, 0).Value, Is.EqualTo("(circular reference)"));
        }

        [Test]
        public void TestCircularReference_Complex()
        {
            // Test complex circular reference chain
            spreadsheet.GetCell(0, 0).Text = "=B1*2"; // A1
            spreadsheet.GetCell(0, 1).Text = "=B2*3"; // B1
            spreadsheet.GetCell(1, 1).Text = "=A2*4"; // B2
            spreadsheet.GetCell(1, 0).Text = "=A1*5"; // A2

            // At least one cell should show circular reference error
            bool hasCircularReferenceError = false;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (spreadsheet.GetCell(i, j).Value == "(circular reference)")
                    {
                        hasCircularReferenceError = true;
                        break;
                    }
                }
            }

            Assert.That(hasCircularReferenceError, Is.True, "No circular reference error was detected");
        }

        [Test]
        public void TestNonExistentReference_OutOfBounds()
        {
            // Test reference to cell outside spreadsheet bounds
            spreadsheet.GetCell(0, 0).Text = "=Z99999";
            Assert.That(spreadsheet.GetCell(0, 0).Value, Is.EqualTo("(bad reference)"));
        }

        [Test]
        public void TestNonExistentReference_InvalidFormat()
        {
            // Test reference with invalid format
            spreadsheet.GetCell(0, 0).Text = "=AA12";
            Assert.That(spreadsheet.GetCell(0, 0).Value, Is.EqualTo("(bad reference)"));
        }

        [Test]
        public void TestValidReference_EmptyCell()
        {
            // Test reference to valid but empty cell
            spreadsheet.GetCell(0, 0).Text = "=B1";
            // Empty cell should be treated as 0
            Assert.That(spreadsheet.GetCell(0, 0).Value, Is.EqualTo("0"));
        }

        [Test]
        public void TestCircularReferenceResolution()
        {
            // Create circular reference
            spreadsheet.GetCell(0, 0).Text = "=B1"; // A1
            spreadsheet.GetCell(0, 1).Text = "=A1"; // B1

            // Fix circular reference
            spreadsheet.GetCell(0, 1).Text = "5"; // B1

            // Check if A1 now evaluates correctly
            Assert.That(spreadsheet.GetCell(0, 0).Value, Is.EqualTo("5"));
        }

        [Test]
        public void TestMultipleReferences_Valid()
        {
            // Set up a chain of valid references
            spreadsheet.GetCell(0, 1).Text = "10"; // B1
            spreadsheet.GetCell(0, 2).Text = "20"; // C1
            spreadsheet.GetCell(0, 0).Text = "=B1+C1"; // A1

            Assert.That(spreadsheet.GetCell(0, 0).Value, Is.EqualTo("30"));
        }

        [Test]
        public void TestReferenceUpdate_PropagatesCorrectly()
        {
            // Set up dependent cells
            spreadsheet.GetCell(0, 1).Text = "10"; // B1
            spreadsheet.GetCell(0, 0).Text = "=B1*2"; // A1

            // Initial check
            Assert.That(spreadsheet.GetCell(0, 0).Value, Is.EqualTo("20"));

            // Update referenced cell
            spreadsheet.GetCell(0, 1).Text = "20"; // B1

            // Check if dependent cell updated
            Assert.That(spreadsheet.GetCell(0, 0).Value, Is.EqualTo("40"));
        }
    }
}
