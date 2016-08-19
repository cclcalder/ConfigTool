using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exceedra.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Utilities;

namespace WPF.Test.Utilities
{
    [TestClass]
    public class IOServiceTests
    {
        [TestMethod]
        public void ReadingLinesFromCsv()
        {
            // Arrange
            string fileName = TestFiles.SubCustomersCsv;

            List<string> expected = new List<string>
            {
                "800005",
                "800006",
                "800007"
            };

            // Act
            var actual = IOService.GetLinesFromFile(fileName);

            // Assert
            Assert.AreEqual(expected.Count, actual.Count, "too much lines read" );

            for (int i = 0; i < actual.Count; i++)
                Assert.AreEqual(expected[i], actual.ElementAt(i), "read line doesn't match the one from the file");
        }

        [TestMethod]
        public void ReadingLinesFromEmptyFileName()
        {
            // Arrange
            string fileName = string.Empty;

            List<string> expected = null;

            // Act
            var actual = IOService.GetLinesFromFile(fileName);

            // Assert
            Assert.AreEqual(expected, actual, "tried to read lines from empty file name");
        }

        [TestMethod]
        public void TryingToReadLinesFromCsvFileWithCorruptedData()
        {
            // Arrange
            string fileName = TestFiles.SubCustomersCsvWithCorruptedData;

            List<string> expected = new List<string>
            {
                "800005",
                "800006",
                "800007"
            };

            // Act
            var actual = IOService.GetLinesFromFile(fileName);

            // Assert
            Assert.AreEqual(expected.Count, actual.Count, "at least one line with disallowed characters wasn't rejected");

            for (int i = 0; i < actual.Count; i++)
                Assert.AreEqual(expected[i], actual.ElementAt(i), "read line doesn't match the one from the file");
        }

        [TestMethod]
        public void TryingToReadLinesFromNonExistingFile()
        {
            // Arrange
            string fileName = "nonExistingFile";

            List<string> expected = null;

            // Act
            var actual = IOService.GetLinesFromFile(fileName);

            // Assert
            Assert.AreEqual(expected, actual, "tried to read lines from non existing file");
        }
    }

    public static class TestFiles
    {
        // test files folder
        private const string TestFilesLocation = @"..\..\Resources\";

        // file names
        private const string SubCustomersCsvFileName = @"SubCustomersCsv.csv";
        private const string SubCustomersCsvWithCorruptedDataFileName = @"SubCustomersCsvWithCorruptedData.csv";

        // full file paths
        public static string SubCustomersCsv
        {
            get { return TestFilesLocation + @"\" + SubCustomersCsvFileName; } 
        }

        public static string SubCustomersCsvWithCorruptedData
        {
            get { return TestFilesLocation + @"\" + SubCustomersCsvWithCorruptedDataFileName; }
        }

    }
}
