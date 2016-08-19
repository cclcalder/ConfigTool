using System;
using System.Security;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Text;
using System.Xml;

namespace Exceedra.Website.Webservices
{
    using System.Xml.Linq;

    /// <summary>
    /// Summary description for WebService
    /// </summary>
    [WebService(Namespace = "http://webservice.exceedra.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService_V2_5 : System.Web.Services.WebService
    {
        private const string Salt = "$2a$10$PwtD1e0SsFl79YAmJZT2le";

  

        [WebMethod]
        public string Run(string storedProcedureName, string parameters)
        {
            // --- Get test data from static XML file
            //return File.ReadAllText(AppDomain.CurrentDomain.GetPath("App_Data\\" + storedProcedureName + ".xml"));

            if (storedProcedureName.Equals("app.Procast_AD_Login", StringComparison.InvariantCultureIgnoreCase))
            {
                var xml = XElement.Parse(parameters);
                var userElement = xml.Element("ADName");
                if (userElement == null) return "<Results><Error>No Active Directory User specified</Error></Results>";
                var hashElement = xml.Element("Hash");
                if (hashElement == null) return "<Results><Error>No Hash specified</Error></Results>";
                var hash = BCrypt.Net.BCrypt.HashPassword(userElement.Value.ToUpperInvariant(), Salt).Substring(Salt.Length);
                if (hash != hashElement.Value)
                    return "<Results><Error>UserName could not be verified</Error></Results>";
            }
            
            // --- Get real data from database
            return GetData(storedProcedureName, parameters);
        }

        /// <summary>
        /// Connects to the databse and executes intended stored procedure and returns xml result as string
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string GetData(string spName, string parameters)
        {
            try
            {
                var connStr = WebConfigurationManager.ConnectionStrings["ExceedraConn_v2_5"].ConnectionString;

                using (var conn = new SqlConnection(connStr))
                using (var cmd = new SqlCommand(spName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    const string xmlInParam = "@XML_In";
                    cmd.Parameters.Add(xmlInParam, SqlDbType.Xml);
                    cmd.Parameters[xmlInParam].Value = parameters;
                    
                    //get the timeout from the web.config, or set default to 30 secs
                    int res = 30;
                    try {
                        res = Convert.ToInt32(WebConfigurationManager.AppSettings["Timeout"].ToString());
                    }
                    catch { }

                    cmd.CommandTimeout = res;// 120;
                    conn.Open();

                    var strRes = new StringBuilder();

                    try
                    {
                        using (XmlReader reader = cmd.ExecuteXmlReader())
                        {
                            reader.Read();
                            while (reader.ReadState != ReadState.EndOfFile)
                                strRes.Append(reader.ReadOuterXml());
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        // Try to salvage the situation and get some meaningful XML.
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                        }
                    }

                    conn.Close();

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
