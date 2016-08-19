using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;


namespace Exceedra.Common.Utilities
{
    public static class IOService
    {
        /// <summary>
        /// Returns lines from the csv file selected with open file dialog.
        /// If the file is not selected (open file dialog is cancelled) returns null.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> ReadCsvFile()
        {
            IEnumerable<string> fileExtensions = new List<string> { "csv" };

            string fileName = GetFilePath(fileExtensions);
            return GetLinesFromFile(fileName);
        }

        /// <summary>
        /// Returns lines from the file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static ICollection<string> GetLinesFromFile(string filePath)
        {
            if (!IsFileCorrect(filePath)) return null;

            List<string> fileLines = new List<string>();

            using (StreamReader streamReader = new StreamReader(filePath))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    if (line == null) continue;
                    var values = line.Trim().ToLower().Split(',');
                    fileLines.AddRange(values.Where(ContainsOnlyAllowedCharacters));
                }
            };

            return fileLines;
        }

        /// <summary>
        /// Returns path of the file selected through open file dialog
        /// </summary>
        /// <param name="fileExtensions">file extentions used in open file dialog filter</param>
        /// <returns></returns>
        private static string GetFilePath(IEnumerable<string> fileExtensions)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";

            foreach (var fileExtension in fileExtensions)
            {
                string fileExtensionWithoutDot = fileExtension;
                if (fileExtension.StartsWith(".")) fileExtensionWithoutDot = fileExtension.Substring(1);

                openFileDialog.Filter += fileExtensionWithoutDot + " files (*." + fileExtensionWithoutDot + ")|*." + fileExtensionWithoutDot + ";";
            }

            bool? result = openFileDialog.ShowDialog();
            if (result != true) return string.Empty;
            return openFileDialog.FileName;
        }

        private static bool IsFileCorrect(string filePath)
        {
            if (String.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return false;
            return true;
        }
        private static bool ContainsOnlyAllowedCharacters(string text)
        {
            return Regex.IsMatch(text, "^[a-zA-Z0-9\\x20\\s()]*$");
        }
    }
}