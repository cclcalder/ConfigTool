using System;
using System.Security;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Text;
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
    public class WebService_V2_9 : System.Web.Services.WebService
    {
        [WebMethod]
        public string Run(string storedProcedureName, string parameters)
        {
            // --- Get test data from static XML file
            //return File.ReadAllText(AppDomain.CurrentDomain.GetPath("App_Data\\" + storedProcedureName + ".xml"));

            return StaticWS.Run(storedProcedureName, parameters, "ExceedraConn_v2_9");

        }
         
    }
}
