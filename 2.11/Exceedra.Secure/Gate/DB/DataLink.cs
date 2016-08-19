using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exceedra.Secure.Gate.DB
{
   //public static class DataLink
   // {
   //     //public static string Run(string storedProcedureName, string parameters, string connKey)
   //     //{  // --- Get test data from static XML file
   //     //   //return File.ReadAllText(AppDomain.CurrentDomain.GetPath("App_Data\\" + storedProcedureName + ".xml"));

   //     //    if (storedProcedureName.Equals("app.Procast_SP_LOGIN_User_Login",
   //     //        StringComparison.InvariantCultureIgnoreCase))
   //     //    {
   //     //        var xml = XElement.Parse(parameters);
   //     //        //get all password nodes
   //     //        var passElement = xml.Element("Password");
   //     //        var saltedPassElement = xml.Element("SaltedPassword");
   //     //        //if both are null, run away
   //     //        if (passElement == null && saltedPassElement == null) return "<Results><Error>No password specified</Error></Results>";

   //     //        if (saltedPassElement != null) storedProcedureName += "_Salt";

   //     //        //same goes for username, its required
   //     //        var userElement = xml.Element("Username");
   //     //        if (userElement == null) return "<Results><Error>No User specified</Error></Results>";

   //     //        return GetData(storedProcedureName, parameters, connKey);


   //     //    }

   //     //    return GetData(storedProcedureName, parameters, connKey);
   //     //}
   // }
}
