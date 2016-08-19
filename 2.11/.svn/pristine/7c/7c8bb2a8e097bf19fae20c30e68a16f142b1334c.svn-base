using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.Entity.Diagnostics
{

    public static class ConfigXML
    {
        public static string GetXML()
        {
            string configUrl;
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                configUrl = ApplicationDeployment.CurrentDeployment.UpdateLocation.AbsoluteUri.ToLower().Replace("wpf.xbap",
                                                                                                       "Config/Client.config.xml");
            }
            else
            {
                configUrl = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Client.config");
            }
            return configUrl;
        }
    }


    /// <summary>
    /// Load additional looging config data from Client.Config file.
    /// </summary>
  public class LoggingConfigData
  {
      public string Name { get; set; }
      public string EndPoint { get; set; }
      public bool IsActive { get; set; }
      public string ReleaseLevel { get; set; }
      public LoggingConfigData()
        {
            var configUrl = ConfigXML.GetXML();

            var config = XElement.Load(configUrl);

            var xElement = config.Element("LoggingConfig");
            
          if (xElement != null)
            {
                try
                {
                    IsActive = xElement.Attribute("isactive").Value == "1";
                }
                catch (Exception)
                {
                    IsActive = false;
                }

                if (IsActive)
                {
                    try
                    {
                        Name = xElement.Attribute("name").Value;
                    }
                    catch (Exception)
                    {
                        Name = "Name Failed";
                    }

                    try
                    {
                        EndPoint = xElement.Attribute("endpoint").Value;
                    }
                    catch (Exception)
                    {
                        EndPoint = "";
                        IsActive = false;
                    }

                    try
                    {
                        ReleaseLevel = xElement.Attribute("releaseLevel").Value;
                    }
                    catch (Exception)
                    {
                        ReleaseLevel = "ReleaseLevel Failed";
                    }
                }
               
            }
            else
            {
                Name = "Unkown";
            }
        }
     
    }


    public class AzureADData
    {
        public string Tenant { get; set; }
        public string ClientId { get; set; }
        public string RedirectUri { get; set; }
        public string ResourceId { get; set; }
        public string AADInstance { get; set; }
        public bool IsActive { get; set; }
        public AzureADData()
        {
            var configUrl = ConfigXML.GetXML();
            var config = XElement.Load(configUrl);
            var xElement = config.Element("AzureAD");
            if (xElement != null)
            {
                try
                {
                    IsActive = xElement.Attribute("IsActive").Value == "1";
                }
                catch (Exception)
                {
                    IsActive = false;
                }

                if (IsActive)
                {
                    try
                    {
                        Tenant = xElement.Attribute("Tenant").Value;
                        ClientId = xElement.Attribute("ClientId").Value;
                        RedirectUri = xElement.Attribute("RedirectUri").Value;
                        ResourceId = xElement.Attribute("ResourceId").Value;
                        AADInstance = xElement.Attribute("AADInstance").Value;
                    }
                    catch (Exception)
                    {
                        IsActive = false;
                    }                     
                }
            }            
        }

       
    }

    public class StorageData
    {
        public string Provider { get; set; }
        public string Key { get; set; }
        public string Account { get; set; }
        public string Container { get; set; }
        public string Path { get; set; }

        public int MaxFileSizeMb { get; set; }
      
        public StorageData()
        {
            var configUrl = ConfigXML.GetXML();
            var config = XElement.Load(configUrl);
            var xElement = config.Element("StorageData");
            if (xElement != null)
            {           
                Provider = xElement.Attribute("Provider").Value;
                Key = xElement.Attribute("Key").Value;
                Account = xElement.Attribute("Account").Value;
                Container = xElement.Attribute("Container").Value;
                Path = xElement.Attribute("Path").Value;
                MaxFileSizeMb = Convert.ToInt32(xElement.Attribute("MaxFileSizeMb").Value);              
            }
        }


    }

    public class SiteData
    {
        public string BaseURL { get; set; }
        public bool UserPasswordReset { get; set; }

        public SiteData() 
        {
            var configUrl = ConfigXML.GetXML();
            var config = XElement.Load(configUrl);
            var xElement = config.Element("SiteConfig");
            if (xElement != null)
            {
                try
                {
                    BaseURL = xElement.Attribute("BaseURL").Value;
                    UserPasswordReset = true; //xElement.Attribute("UserPasswordReset").MaybeValue() == "1";
                }
                catch (Exception)
                {
                    BaseURL = "";
                }

            } 
        }
       
    }

}
