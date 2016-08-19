using System;
using System.Collections.Concurrent;
using System.Deployment.Application;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Linq;
using System.Xml;
using System.Linq;
using System.Web.Services.Protocols;
using System.IO;
using Exceedra.Common;
using Exceedra.Common.Logging;
using Exceedra.Common.Mvvm;

namespace Model.DataAccess
{
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    public class WebServiceProxy
    {
       public static ConcurrentDictionary<string, string> cachedRequests = new ConcurrentDictionary<string, string>();

        public const string RequestedActionYieldedNoResult = "Requested action yielded no result!";
        private static readonly Regex Whitespace = new Regex(@"\s+");
        private static readonly ConcurrentDictionary<string, EndpointAddress> Endpoints =
            new ConcurrentDictionary<string, EndpointAddress>();

        private static EndpointAddress GetEndpointAddress(string contract)
        {
            return Endpoints.GetOrAdd(contract, GetEndpointAddressImpl);
        }

        private static EndpointAddress GetEndpointAddressImpl(string contract)
        {
            var configUrl = GetConfigUrl();
            var config = XElement.Load(configUrl);

            var endpointElement = config.Elements("endpoint").First(x => x.Attribute("contract").Value == contract);
            return new EndpointAddress(endpointElement.Attribute("address").Value);
        }

        private static string GetConfigUrl()
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

        public static XElement Call(string method, string args, DisplayErrors displayErrors = DisplayErrors.Yes)
        {
            return Call(method, XElement.Parse(args), displayErrors);
        }

        public static XElement Call(string method, XElement arguments, DisplayErrors displayErrors = DisplayErrors.Yes, bool cacheMe = false)
        {
            if (cacheMe == true)
            {
                string content;
                if (cachedRequests.TryGetValue(method + arguments.ToString(), out content))
                {
                    var tt =  ParseXml(method, arguments, content) ;
                    return tt;
                }
            }

            Trace.TraceInformation("{0}: {1}", method, arguments.ToString(SaveOptions.DisableFormatting));

            if (User.CurrentUser == null || User.CurrentUser.Logging)
                StorageBase.LogMessageToFile("Service Call (std)", method, arguments.ToString(), (User.CurrentUser != null ? User.CurrentUser.ID : ""));
            
            StorageBase.LogCallToFile(method, arguments.ToString(SaveOptions.DisableFormatting));

            var endpointAddress = GetEndpointAddress("ServiceReference1.WebServiceSoap");
            using (var client = new ServiceReference1.WebServiceSoapClient(new BasicHttpBinding("WebServiceSoap"), endpointAddress))
            {
           
                try
                {
                    var xml = client.Run(method, arguments.ToString());

                    //Store for later
                    if (cacheMe == true)
                    {
                        cachedRequests.TryAdd(method + arguments.ToString(), xml);
                    }

                    return ParseXml(method, arguments, xml, displayErrors);
                }
                catch (XmlException ex)
                {
                    LogXmlException(displayErrors, ex);
                    throw;
                }
                catch (SoapException ex)
                {
                    LogSoapException(displayErrors, ex);
                    throw;
                }
                catch (Exception ex)
                {
                    LogException(displayErrors, ex);
                    throw;
                }
            }
        }

        public static Task<XElement> CallAsync(string method, XElement arguments, DisplayErrors displayErrors = DisplayErrors.Yes, 
                                        bool throwOnError = true, bool cacheMe = false)
        {
            var argumentString = string.Empty;
            if (arguments != null)
            {
                argumentString = arguments.ToString(SaveOptions.DisableFormatting);
            }

            if (cacheMe == true)
            {
                string content;
                if (cachedRequests.TryGetValue(method + argumentString, out content))
                {
                    var tt = Task.Factory.FromResult(ParseXml(method, arguments, content));
                    return tt;
                }
            }
             
            Trace.TraceInformation("{0}: {1}", method, argumentString);

            if (User.CurrentUser == null || User.CurrentUser.Logging)
                StorageBase.LogMessageToFile("Service Call (Async)", method, arguments.ToString(), (User.CurrentUser != null ? User.CurrentUser.ID : ""));

            StorageBase.LogCallToFile(method, arguments.ToString(SaveOptions.DisableFormatting));

            var endpointAddress = GetEndpointAddress("ServiceReference1.WebServiceSoap")  ;
            var client = new ServiceReference1.WebServiceSoapClient(new BasicHttpBinding("WebServiceSoap"), endpointAddress);

            var task = Task.Factory.FromAsync<string>(
                    (callback, state) => client.BeginRun(method, argumentString, callback, state), client.EndRun,
                    null);

            return task.ContinueWith(t =>
                                  {
                                      if (t.IsFaulted)
                                      {
                                          HandleException(t.Exception, displayErrors);
                                          return null;
                                      }
                                      if (!throwOnError)
                                      {
                                          try
                                          {
                                             // return XElement.Parse(t.Result);


                                              var result = t.Result;
                                              //Store for later
                                              if (cacheMe == true)
                                              {
                                                  cachedRequests.TryAdd(method + argumentString, t.Result);
                                              }

                                              return ParseXml(method, arguments, result);

                                          }
                                          catch (Exception ex)
                                          {
                                              Trace.TraceError(ex.Message);
                                              throw new ExceedraDataException("Invalid data received from server.", ex);
                                          }
                                      }
                                      try
                                      {
                                          var result = t.Result;
                                          //Store for later
                                          if (cacheMe == true)
                                          {
                                              cachedRequests.TryAdd(method + argumentString, t.Result);
                                          }

                                          return ParseXml(method, arguments, result);
                                      }
                                      catch (Exception ex)
                                      {
                                          Trace.TraceError(ex.Message);
                                          if (displayErrors == DisplayErrors.Yes)
                                          {
                                             Messages.Instance.PutError(ex.Message);
                                          
                                          }
                                          return null;
                                      }
                                  });
        }

        public static XElement ParseXml(string method, XElement arguments, string xml, DisplayErrors displayErrors = DisplayErrors.Yes)
        {
               
            CheckForNull(xml);

            if (xml.Contains("DataServiceProvider")) {
                return XElement.Parse(xml);
            }
            var data = XDocument.Parse(xml).GetElement("Results");

            if (User.CurrentUser != null && User.CurrentUser.Logging)
                StorageBase.LogMessageToFile("Service Response", method, data.ToString(), (User.CurrentUser != null ? User.CurrentUser.ID : "")); 
         
            if(displayErrors == DisplayErrors.Yes)
                CheckForError(data);

            return data;
        }

        private static void HandleException(AggregateException exception, DisplayErrors displayErrors)
        {
            try
            {
                throw exception.GetBaseException();
            }
            catch (XmlException ex)
            {
              LogXmlException(displayErrors, ex);
            }
            catch (FaultException<XmlException> ex)
            {
              LogXmlException(displayErrors, ex.Detail);
            }
            catch (SoapException ex)
            {
              LogSoapException(displayErrors, ex);
            }
            catch (FaultException<SoapException> ex)
            {
              LogSoapException(displayErrors, ex.Detail);
            }
            catch (FaultException ex)
            {
                LogFaultException(displayErrors, ex);
            }
            catch (Exception ex)
            {
              LogException(displayErrors, ex);
            }
        }

        private static void LogFaultException(DisplayErrors displayErrors, FaultException ex)
        {
            if (User.CurrentUser != null && User.CurrentUser.Logging)
                StorageBase.LogMessageToFile("Error","Fault Exception",ex.Reason.Translations[0].Text, (User.CurrentUser != null ? User.CurrentUser.ID : ""));

            const string text = "A fatal error occurred.  Details:{0}{1}";
            if (displayErrors == DisplayErrors.Yes)
                Messages.Instance.Put(new ErrorMessage(text.FormatWith(Environment.NewLine, ex.Message)));

            if (displayErrors == DisplayErrors.Message)
                Messages.Instance.Put(new InformationMessage(ex.Message.Replace("Error -", "Fault info -")));
        }

        public static void LogException(DisplayErrors displayErrors, Exception ex)
        {
            if (User.CurrentUser != null && User.CurrentUser.Logging)
                StorageBase.LogMessageToFile("Error", "Generic exception", ex.Message, (User.CurrentUser != null ? User.CurrentUser.ID : ""));

            const string text = "A fatal error occurred.  Details:{0}{1}";
            if (displayErrors == DisplayErrors.Yes)
                Messages.Instance.Put(new ErrorMessage(text.FormatWith(Environment.NewLine, ex.Message)));

            if (displayErrors == DisplayErrors.Message)
                Messages.Instance.Put(new InformationMessage(ex.Message.Replace("Error -","")));
        }

        private static void LogSoapException(DisplayErrors displayErrors, SoapException ex)
        {
            var msg = "A database related error has been caught with following details:" + Environment.NewLine + ex.Message;

            if (User.CurrentUser != null && User.CurrentUser.Logging)
                StorageBase.LogMessageToFile("Error","SOAP exception",msg, (User.CurrentUser != null ? User.CurrentUser.ID : ""));

            if (displayErrors == DisplayErrors.Yes)
                Messages.Instance.Put(new ErrorMessage(msg));


            if (displayErrors == DisplayErrors.Message)
                Messages.Instance.Put(new InformationMessage(ex.Message.Replace("Error -", "Databse info -")));
        }

        private static void LogXmlException(DisplayErrors displayErrors, XmlException ex)
        {
            var msg = "An XML data format related error has been caught with following details:" + Environment.NewLine +
                      ex.Message;

            if (User.CurrentUser != null && User.CurrentUser.Logging)
                StorageBase.LogMessageToFile("Error","XML exception",msg, (User.CurrentUser != null ? User.CurrentUser.ID : ""));

            if (displayErrors == DisplayErrors.Yes)
                Messages.Instance.Put(new ErrorMessage(msg));

            if (displayErrors == DisplayErrors.Message)
                Messages.Instance.Put(new InformationMessage(ex.Message.Replace("Error -", "XML data info -")));
        }

        public static void CheckForError(XElement data)
        {
            const string errorElement = "Error";
            if (data.GetValue<string>(errorElement) != null)
            {
                if (User.CurrentUser != null && User.CurrentUser.Logging)
                    StorageBase.LogMessageToFile("Error", "XML error", data.ToString(), (User.CurrentUser != null ? User.CurrentUser.ID : ""));

                throw new ExceedraDataException(data.GetValue<string>(errorElement));
            }
        }

       

        public static void CheckForNull(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                if (User.CurrentUser != null && User.CurrentUser.Logging)
                    StorageBase.LogMessageToFile("Error","XML null", RequestedActionYieldedNoResult, (User.CurrentUser != null ? User.CurrentUser.ID : ""));

                throw new ExceedraDataException(RequestedActionYieldedNoResult);
            }
        }

        


    
    }

    public enum DisplayErrors
    {
        Yes,
        No,
        Message
    }
}
