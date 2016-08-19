using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exceedra.Common
{
    using System.Text.RegularExpressions;

    public static class FormatHelper
    {
        private static readonly Regex NumberRegex = new Regex("^[NCP][0-9]?$", RegexOptions.IgnoreCase);

        public static bool IsNumberFormat(string format)
        {
            return (!string.IsNullOrWhiteSpace(format)) && NumberRegex.IsMatch(format);
        }
    }
}
