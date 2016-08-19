using System.Xml.Linq;
using Model.DataAccess;
using SalesPlannerWeb.Webservices;

namespace SalesPlannerWeb.Accessors
{
    public class MvcWebServiceProxy : WebServiceProxy
    {
        private static string DbName => "ExceedraConn_v" +
                                        (StaticWS.version ?? System.Configuration.ConfigurationManager.AppSettings["ActiveDBVersion"]);


        public static XElement ParseXml(string method, XElement arguments)
        {
            return ParseXml(method, arguments, StaticWS.Run(method, arguments.ToString(), DbName));
        }
    }
}