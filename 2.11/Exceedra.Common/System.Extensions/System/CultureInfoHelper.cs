using System.Globalization;

namespace System
{
    public static class CultureInfoHelper
    {
        /// <summary>
        /// The format (culture info) that are set in the Control Panel / Clock, Language and Region / Region of a user's OS.
        /// </summary>
        public static CultureInfo RegionCultureInfo { get; set; }
    }
}