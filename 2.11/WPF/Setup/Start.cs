using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elmah.Everywhere;
using Elmah.Everywhere.Diagnostics;
using Model.Entity.Diagnostics;

namespace WPF.Setup
{
   public class Start
    {
        public static void SetupElmahConfig(LoggingConfigData lggingConfigData)
        {
            if (lggingConfigData.IsActive && !string.IsNullOrWhiteSpace(lggingConfigData.EndPoint))
            {
                var defaults = new ExceptionDefaults
                {
                    Token = "Test-Token",
                    ApplicationName = "ExceedraSP",
                    Host =
                        string.Format("{0}-{1}-{2}", lggingConfigData.Name, lggingConfigData.ReleaseLevel, App.VersionInfo),
                    RemoteLogUri = new Uri(lggingConfigData.EndPoint)
                };


                var writer = new HttpExceptionWritter
                {
                    RequestUri = new Uri(lggingConfigData.EndPoint, UriKind.Absolute)
                };
                ExceptionHandler.Configure(writer, defaults, null);
            }
        }

       public static void SetupAzureDBConfig(AzureADData azureAdConfigData)
       {
            
       }
    }
}
