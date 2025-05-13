using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.IO;
using CptS321;
using Assert = NUnit.Framework.Assert;

namespace Spreadsheet.Tests
{
    /// <summary>
    /// TDD for spreadsheet xml development.
    /// </summary>
    [TestFixture]
    public class SpreadsheetXmlTests
    {
        private CptS321.Spreadsheet spreadsheet;
        private MemoryStream memoryStream;

        [SetUp]
        public void Setup()
        {
            spreadsheet = new CptS321.Spreadsheet(50, 26); // Standard size for A-Z columns, 50 rows
            memoryStream = new MemoryStream();
        }

        [TearDown]
        public void Cleanup()
        {
            memoryStream.Dispose();
        }

        [Test]
        public void TestSaveAndLoadEmptySpreadsheet()
        {
            // Save empty spreadsheet
            spreadsheet.SaveToXML(memoryStream);

            // Reset stream position for reading
            memoryStream.Position = 0;

            // Create new spreadsheet and load
            CptS321.Spreadsheet newSheet = new CptS321.Spreadsheet(50, 26);
            newSheet.LoadFromXML(memoryStream);

            // Verify no cells have content
            for (int r = 0; r < newSheet.RowCount; r++)
            {
                for (int c = 0; c < newSheet.ColumnCount; c++)
                {
                    Assert.That(newSheet.GetCell(r, c).Text, Is.EqualTo(string.Empty));
                    Assert.That(newSheet.GetCell(r, c).BGColor, Is.EqualTo(0xFFFFFFFF));
                }
            }
        }

        [Test]
        public void TestSaveAndLoadWithBasicText()
        {
            // Set up test data
            spreadsheet.GetCell(0, 0).Text = "Test";
            spreadsheet.GetCell(1, 1).Text = "123";

            // Save to stream
            spreadsheet.SaveToXML(memoryStream);
            memoryStream.Position = 0;

            // Load into new spreadsheet
            CptS321.Spreadsheet newSheet = new CptS321.Spreadsheet(50, 26);
            newSheet.LoadFromXML(memoryStream);

            // Verify
            Assert.That(newSheet.GetCell(0, 0).Text, Is.EqualTo("Test"));
            Assert.That(newSheet.GetCell(1, 1).Text, Is.EqualTo("123"));
        }

        [Test]
        public void TestSaveAndLoadWithFormula()
        {
            // Set up test data
            spreadsheet.GetCell(0, 0).Text = "=A2+B2";
            spreadsheet.GetCell(1, 0).Text = "10"; // A2
            spreadsheet.GetCell(1, 1).Text = "20"; // B2

            // Save to stream
            spreadsheet.SaveToXML(memoryStream);
            memoryStream.Position = 0;

            // Load into new spreadsheet
            CptS321.Spreadsheet newSheet = new CptS321.Spreadsheet(50, 26);
            newSheet.LoadFromXML(memoryStream);

            // Verify formula and values
            Assert.That(newSheet.GetCell(0, 0).Text, Is.EqualTo("=A2+B2"));
            Assert.That(newSheet.GetCell(1, 0).Text, Is.EqualTo("10"));
            Assert.That(newSheet.GetCell(1, 1).Text, Is.EqualTo("20"));
            Assert.That(newSheet.GetCell(0, 0).Value, Is.EqualTo("30")); // Formula should evaluate to 30
        }

        [Test]
        public void TestSaveAndLoadWithBackgroundColor()
        {
            // Set up test data
            uint testColor = 0xFF0000FF; // Red
            spreadsheet.GetCell(0, 0).BGColor = testColor;
            spreadsheet.GetCell(0, 0).Text = "Colored Cell";

            // Save to stream
            spreadsheet.SaveToXML(memoryStream);
            memoryStream.Position = 0;

            // Load into new spreadsheet
            CptS321.Spreadsheet newSheet = new CptS321.Spreadsheet(50, 26);
            newSheet.LoadFromXML(memoryStream);

            // Verify
            Assert.That(newSheet.GetCell(0, 0).BGColor, Is.EqualTo(testColor));
            Assert.That(newSheet.GetCell(0, 0).Text, Is.EqualTo("Colored Cell"));
        }

        [Test]
        public void TestSaveAndLoadMultipleCellAttributes()
        {
            // Set up multiple cells with different attributes
            spreadsheet.GetCell(0, 0).Text = "Header";
            spreadsheet.GetCell(0, 0).BGColor = 0xFF0000FF;
            spreadsheet.GetCell(1, 0).Text = "=A1+10";
            spreadsheet.GetCell(2, 2).BGColor = 0x00FF00FF;

            // Save to stream
            spreadsheet.SaveToXML(memoryStream);
            memoryStream.Position = 0;

            // Load into new spreadsheet
            CptS321.Spreadsheet newSheet = new CptS321.Spreadsheet(50, 26);
            newSheet.LoadFromXML(memoryStream);

            // Verify all attributes were preserved
            Assert.Multiple(() =>
            {
                Assert.That(newSheet.GetCell(0, 0).Text, Is.EqualTo("Header"));
                Assert.That(newSheet.GetCell(0, 0).BGColor, Is.EqualTo(0xFF0000FF));
                Assert.That(newSheet.GetCell(1, 0).Text, Is.EqualTo("=A1+10"));
                Assert.That(newSheet.GetCell(2, 2).BGColor, Is.EqualTo(0x00FF00FF));
            });
        }
    }
}
