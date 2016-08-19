using System;
using System.Deployment.Application;
using System.IO;
using System.Windows;
using System.Xml.Linq;

namespace Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App2 : Application
    {
        public static string xbap
        {
            get {

                var res = GetConfigUrl();
                 
                    return SiteData(res);
           
            }
        }
    private static string GetConfigUrl()
        {
            string configUrl;
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                configUrl = ApplicationDeployment.CurrentDeployment.UpdateLocation.AbsoluteUri.ToLower().Replace("exceedra.desktop.application",
                                                                                                       "Config/Client.config.xml");
            }
            else
            {
                configUrl = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Client.config");
            }
            return configUrl;
        }

        private static string BaseURL;
        public static string SiteData(string configUrl)
        {  
            var config = XElement.Load(configUrl);
            var xElement = config.Element("SiteConfig");
            if (xElement != null)
            {
                try
                {
                    BaseURL = xElement.Attribute("BaseURL").Value;
                }
                catch (Exception)
                {
                    BaseURL = "";
                }

            }

            return BaseURL;
        }

        public App2()
        {

            //var res = GetConfigUrl();
            //if (File.Exists(res))
            //{
            //    xbap = SiteData(res);
            //}
            //try
            //{
            //    xbap = GetConfigUrl();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.InnerException.ToString(), "error");
            //}

        }


      //public static  string GetConfigUrl()
      //{
      //    var u = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Client.config.xml";
      //      var xbap = XElement.Load(u);

      //      return xbap.Element("xbap").Value;
      //  }
    }
}
