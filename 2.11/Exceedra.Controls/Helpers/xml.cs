using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Exceedra.Controls.Helpers
{
   public class xml
    {

        public static string FixNull(string In, string Default)
        {
            if (string.IsNullOrWhiteSpace(In))
            {
                In = Default;
            }

            return In;
        }

        public static bool IsNull(XElement In)
        {
            return In != null && !string.IsNullOrWhiteSpace(In.Value);
        }

       public static bool FixBool(XElement In, bool Default)
        {
            if (In == null || string.IsNullOrWhiteSpace(In.Value))
            {
                return Default;
            }

            return In.Value == "1" || In.Value.ToLower()=="true";
        }

       public static bool FixBool(XAttribute In, bool Default = false)
       {
           if (In == null || string.IsNullOrWhiteSpace(In.Value))
           {
               return Default;
           }

           return In.Value == "1" || In.Value.ToLower() == "true";
       }

       public static int FixInt(XElement In, int Default = 0)
       {
           int intValue;
           if (In == null || string.IsNullOrWhiteSpace(In.Value) || !int.TryParse(In.Value, out intValue))
           {
               return Default;
           }

           return intValue;
       }

       public static int FixInt(XAttribute In, int Default = 0)
       {
           int intValue;
           if (In == null || string.IsNullOrWhiteSpace(In.Value) || !int.TryParse(In.Value, out intValue))
           {
               return Default;
           }

           return intValue;
       }

       public static int? FixIntNullable(XElement In, int? Default = null)
       {
           int intValue;
           if (In == null || string.IsNullOrWhiteSpace(In.Value) || !int.TryParse(In.Value, out intValue))
           {
               return Default;
           }

           return intValue;
       }

       public static int? FixIntNullable(XAttribute In, int? Default = null)
       {
           int intValue;
           if (In == null || string.IsNullOrWhiteSpace(In.Value) || !int.TryParse(In.Value, out intValue))
           {
               return Default;
           }

           return intValue;
       }

        public static string FixNullInline(XElement In, string Default)
        {
            if (In == null || string.IsNullOrWhiteSpace(In.Value))
            {
                return Default;
            }

            return In.Value;
        }

        public static string FixNullInline(XAttribute In, string Default = "")
        {
            if (In == null || string.IsNullOrWhiteSpace(In.Value))
            {
                return Default;
            }

            return In.Value;
        }

        public static string FixNullInlineString(XElement In, string Default)
        {
            if (In == null || string.IsNullOrWhiteSpace(In.ToString()))
            {
                return Default;
            }

            return In.ToString();
        }
    }
}
