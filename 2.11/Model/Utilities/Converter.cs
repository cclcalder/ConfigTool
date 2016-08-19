using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Telerik.Pivot.Core;
using Telerik.Pivot.Core.Aggregates;

namespace Exceedra.Common.Utilities
{
    public static class Converter
    {
        /// <summary>
        /// Tries to match a given string with a type. Ignores every character except letters.
        /// If unsuccessful returns null.
        /// </summary>
        /// <param name="typeString">A string to convert to a type</param>
        /// <returns>A type converted from a string</returns>
        public static Type StringToType(string typeString)
        {
            if (string.IsNullOrEmpty(typeString)) return null;

            string typeStringToLower = typeString.ToLower();

            // remove every char except letters
            typeStringToLower = Regex.Replace(typeStringToLower, @"[^a-z]", "");

            // remove MAX if exists
            typeStringToLower = typeStringToLower.Replace("max", "");

            switch (typeStringToLower)
            {
                case "str":
                case "string":
                case "varchar":
                case "nvarchar":
                case "ntext":
                case "text":
                    return typeof(string);

                case "bit":
                case "int":
                case "integer":
                    return typeof(int);

                case "long":
                case "bigint":
                    return typeof(long);

                case "float":
                case "double":
                    return typeof(double);

                case "dec":
                case "decimal":
                    return typeof(decimal);

                case "dt":
                case "date":
                case "time":
                case "datetime":
                    return typeof(DateTime);


                default: return null;
            }
        }

        /// <summary>
        /// Tries to match a given string with any existing pivot grid aggregate function types
        /// </summary>
        /// <param name="str">A string to convert to an aggregate function type</param>
        /// <returns>
        /// An aggregate function type converted from a string.
        /// By default - Sum.
        /// </returns>
        public static AggregateFunction StringToAggregateFunctionType(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;

            switch (str.ToLower())
            {
                case "sum":
                    return AggregateFunctions.Sum;

                case "avg":
                case "average":
                    return AggregateFunctions.Average;

                case "count":
                    return AggregateFunctions.Count;

                case "max":
                    return AggregateFunctions.Max;

                case "min":
                    return AggregateFunctions.Min;

                default: return AggregateFunctions.Sum;
            }
        }

        /// <summary>
        /// Tries to match a given string with any existing pivot grid sort order.
        /// If unsuccessful returns SortOrder.None.
        /// </summary>
        /// <param name="str">>A string to convert to a sort order type</param>
        /// <returns>
        /// A sort order type converted from a string.
        /// By default - none.
        /// </returns>
        public static SortOrder StringToSortOrder(string str)
        {
            if (string.IsNullOrEmpty(str)) return SortOrder.None;

            switch (str.ToLower())
            {
                case "asc":
                case "ascending":
                    return SortOrder.Ascending;
                    
                case "desc":
                case "descending":
                    return SortOrder.Descending;

                case "none":
                default:
                    return SortOrder.None;
            }
        }
    }
}
