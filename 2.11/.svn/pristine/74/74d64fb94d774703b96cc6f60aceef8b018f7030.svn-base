using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.Controls.DynamicRow.Models;

namespace Exceedra.DynamicGrid.Models
{
    public static class SharedMethods
    {
        public static void ReplacePortionWithValue(ref string equation, string columnCodeToFind, string currentColumnCode, List<Property> gridProps = null, List<RowProperty> vertGridProps = null)
        {
            columnCodeToFind = columnCodeToFind.Replace("(", "").Replace(")", "");

            //Check we're not calculating with our own value (this did actually happen!)
            if (currentColumnCode.ToLowerInvariant() == columnCodeToFind.ToLowerInvariant())
                MessageBox.Show("Circular Dependency: " + currentColumnCode, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            var cell = gridProps != null
                ? (object) GetProperty(gridProps, columnCodeToFind)
                : GetProperty(vertGridProps, columnCodeToFind);

            if (cell == null)
                MessageBox.Show("Could not find ColumnCode '" + columnCodeToFind + "'", "Internal Error");

            var value = cell.GetType().GetProperty("Value").GetValue(cell, null).ToString();
            var format = cell.GetType().GetProperty("StringFormat").GetValue(cell, null).ToString();

            var val = value.Replace(CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
            
            decimal d;
            decimal.TryParse(val, NumberStyles.Any, CultureInfo.CurrentCulture, out d);

            if (val.Contains("%") || format.Contains("P"))
            {
               // d = (d / 100);
            }

            equation = equation.Replace(columnCodeToFind, d.ToString(CultureInfo.CurrentCulture));
        }

        public static List<string> ExtractColumnCodes(string equation)
        {
            if (string.IsNullOrEmpty(equation))
                return new List<string>();

            var invalidColCodeChars = new[] { "+", "-", "*", "/", ".", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "<", ">" };
            var formattedEquation = equation;

            foreach (var invalidCharacter in invalidColCodeChars)
                formattedEquation = formattedEquation.Replace(invalidCharacter, " ");

            var propertiesCodes = formattedEquation
                .Trim().Replace("(", "").Replace(")", "").Split(' ')
                .Where(equationPart => !string.IsNullOrEmpty(equationPart))
                .ToList();

            return propertiesCodes;
        }

        private static Property GetProperty(List<Property> gridProps, string columnCodeToFind)
        {
            return gridProps.SingleOrDefault(t => t.ColumnCode == columnCodeToFind);
        }

        private static RowProperty GetProperty(List<RowProperty> vertGridProps, string columnCodeToFind)
        {
            return vertGridProps.SingleOrDefault(t => t.ColumnCode == columnCodeToFind);
        }
    }
}