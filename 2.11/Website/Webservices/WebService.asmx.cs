using System;
using System.Configuration;
using System.Security;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.Configuration;
using System.Text;
using System.Web;
using System.Xml;

namespace Website.Webservices
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
    public class WebService : System.Web.Services.WebService
    {
        [WebMethod]
        public string Run(string storedProcedureName, string parameters)
        {
            //split save to send save/update procs to a seperate connection string
            var getPut = "";

          

            if (ConfigurationManager.AppSettings["UseLocalXML"].ToString() == "1")
            {
                // --- Get test data from static XML file
                var path = AppDomain.CurrentDomain.GetPath("App_Data\\" + storedProcedureName + ".xml");

                if (File.Exists(AppDomain.CurrentDomain.GetPath("App_Data\\" + storedProcedureName + ".xml")))
                {
                    return File.ReadAllText(path);
                }
            }
             
            if (StaticWS.version != null)
            {
                //EPOS DB may be on a different connection string to the APP, so we have a seperate conn string in the web config for this
                if (storedProcedureName.ToLower().StartsWith("epos"))
                {
                    if(WebConfigurationManager.AppSettings["SplitEPOS"].ToString() == "1")
                    {
                        //split save to send save/update procs to a seperate connection string
                        if (storedProcedureName.ToLower().Contains("save") || storedProcedureName.ToLower().Contains("update"))
                            getPut = "_PUT"; 
                    }

                    return StaticWS.Run(storedProcedureName, parameters, "EPOS_v" + StaticWS.version + getPut);
                }

                if (WebConfigurationManager.AppSettings["SplitSP"].ToString() == "1")
                {
                    //split save to send save/update procs to a seperate connection string
                    if (storedProcedureName.ToLower().Contains("save") || storedProcedureName.ToLower().Contains("update"))
                        getPut = "_PUT";
                }

                return StaticWS.Run(storedProcedureName, parameters, "ExceedraConn_v" + StaticWS.version + getPut);
            }
            else
            {
                return "<ConnectionError>Version not set in client.config: ?v=[versionMajor_minor]</ConnectionError>";
            }
            

           

        }
    }
}
