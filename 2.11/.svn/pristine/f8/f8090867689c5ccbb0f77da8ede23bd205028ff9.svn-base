using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Model
{
    public class Common
    {
        /// <summary>
        /// Removes any format like ',' and '£' and returns only decimal value
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static decimal RemoveFormatForDecimal(string p)
        {
            return Convert.ToDecimal(Regex.Replace(p, @"[^-?,\.\d]", ""));
        }

        private static readonly string logFileName = AppDomain.CurrentDomain.GetPath() + "Log.txt";
        public static void LogText(string msg)
        {
            File.AppendAllText(logFileName, msg);
            File.AppendAllText(logFileName, Environment.NewLine);
        }
    }
}
