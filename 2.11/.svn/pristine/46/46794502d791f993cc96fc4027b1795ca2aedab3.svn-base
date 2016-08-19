using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Exceedra.DynamicGrid.Models
{
    public class RowSumElement
    {
        private const string RowSum = "ROWSUM";
        private const string Where = "WHERE";
        private const string In = "IN";
        private static readonly char[] OperatorTypes = { '-', '+', '*', '/' };

        public string ColumnCode { get; set; }
        public string[] RowRange { get; set; }
        public char? Operator { get; set; }
        public decimal Result { get; set; }

        public RowSumElement(string input)
        {
            ColumnCode = input.Substring(0, input.IndexOf(Where, StringComparison.Ordinal));

            if (OperatorTypes.Contains(input.Last()))
            {
                //If we have an operator then grab it, and remove it from the string
                Operator = input.Last();
                input = input.Remove(input.LastIndexOf(input.Last()));
            }

            RowRange = input.Substring(input.IndexOf(In, StringComparison.Ordinal) + In.Length).Split(',');
        }

        public static IEnumerable<RowSumElement> ConvertToRowSumElements(string equation)
        {
            var formattedEquation = equation.Replace("(", "").Replace(")", "").Replace(" ", "");

            var sumComponents = formattedEquation.Split(new[] { RowSum }, StringSplitOptions.None).ToList();
            sumComponents.RemoveAt(0);

            var elements = sumComponents.Select(com => new RowSumElement(com));

            return elements;
        }

        public static double Evaluate(string expression)
        {
            DataTable table = new DataTable();
            table.Columns.Add("expression", typeof(string), expression);
            DataRow row = table.NewRow();
            table.Rows.Add(row);
            return Double.Parse((string)row["expression"]);
        }
    }

}