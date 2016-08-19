using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Xml;
using System.Xml.Linq;

namespace SalesPlannerWeb.Webservices
{
    public class StaticWS
    {
        private const string Salt = "$2a$10$PwtD1e0SsFl79YAmJZT2le";

        public static string version { get; set; }
        
        public static string Run(string storedProcedureName, string parameters, string connKey)
        {  // --- Get test data from static XML file
            //return File.ReadAllText(AppDomain.CurrentDomain.GetPath("App_Data\\" + storedProcedureName + ".xml"));
            
            if (storedProcedureName.Equals("app.Procast_SP_LOGIN_User_Login",
                StringComparison.InvariantCultureIgnoreCase))
            {
                var xml = XElement.Parse(parameters);
                //get all password nodes
                var passElement = xml.Element("Password");
                var saltedPassElement = xml.Element("SaltedPassword");
                //if both are null, run away
                if (passElement == null && saltedPassElement == null) return "<Results><Error>No password specified</Error></Results>";

                if (saltedPassElement != null) storedProcedureName += "_Salt";

                //same goes for username, its required
                var userElement = xml.Element("Username");
                if (userElement == null) return "<Results><Error>No User specified</Error></Results>";

                return GetData(storedProcedureName, parameters, connKey);

             
            }

            return GetData(storedProcedureName, parameters, connKey);
        }

        private static string GetData(string spName, string parameters, string connKey)
        {
            try
            {
                
                var connStr = WebConfigurationManager.ConnectionStrings[connKey].ConnectionString;//"ExceedraConn_v2_0"

                using (var conn = new SqlConnection(connStr))
                {
                    var strRes = new StringBuilder();
                    using (var cmd = new SqlCommand(spName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        const string xmlInParam = "@XML_In";
                        cmd.Parameters.Add(xmlInParam, SqlDbType.Xml);
                        cmd.Parameters[xmlInParam].Value = parameters;

                        //get the timeout from the web.config, or set default to 30 secs
                        int res = 30;
                        try
                        {
                            res = Convert.ToInt32(WebConfigurationManager.AppSettings["Timeout"].ToString());
                        }
                        catch { }

                        cmd.CommandTimeout = res;// 120;
                        conn.Open();                        

                        try
                        {
                            using (XmlReader reader = cmd.ExecuteXmlReader())
                            {
                                reader.Read();
                                while (reader.ReadState != ReadState.EndOfFile)
                                    strRes.Append(reader.ReadOuterXml());
                            }
                        }
                        catch (InvalidOperationException ex)
                        {
                            var e = ex;
                            // Try to salvage the situation and get some meaningful XML.
                            using (var reader = cmd.ExecuteReader())
                            {
                                reader.Read();
                            }
                        }

                        //conn.Close();
                        
                    }
                    return strRes.ToString();
                }
            }
            catch (Exception ex)
            {
                const string errorResults = "<Results><Error>The procedure {0} failed.{1}Parameters: {2}{1}Details: {3}</Error></Results>";
                return errorResults.FormatWith(spName, Environment.NewLine, SecurityElement.Escape(parameters), ex.GetBaseException().Message);
            }
        }

    }
}