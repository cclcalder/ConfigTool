using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Exceedra.Common;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using Model.Entity.Demand;
using Model.Entity.Generic;
using QFXModels;
using QFXShared.Entry;
using Model.Entity.Listings;
using System.Xml;

namespace Model.DataAccess
{
    public class DemandAccess
    {
        public static XElement UserIdxElement = new XElement("User_Idx", User.CurrentUser.ID);
        public XElement SalesOrgIdxElement = new XElement("SalesOrg_Idx", User.CurrentUser.SalesOrganisationID);

        public DemandAccess()
        {
            /* DO NOT REMOVE. This refereance is required to force WPF to include QFXSharp.dll in its published files */
            var n = new QFXSharp.NoEntry();
            var no = n.Nope;
        }


        #region Comboboxes
        private Task<List<ComboboxItem>> GetComboboxItems(string proc, DemandFilterObject inputs = null)
        {
            XElement arguments = DfoToXElement(inputs);
            
            return WebServiceProxy.CallAsync(proc, arguments).ContinueWith(t => GetComboboxItemsContinuation(t));
        }

        private List<ComboboxItem> GetComboboxItemsContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<ComboboxItem>() { new ComboboxItem(XElement.Parse("<Results><Name>No Items</Name><Idx>1</Idx><IsSelected>1</IsSelected></Results>")) };

            return task.Result.Elements().Select(n => new ComboboxItem(n)).ToList();
        }
        #endregion

        #region Dynamic Data
        private Task<XElement> GetDynamicData(DemandFilterObject inputs, string proc)
        {
            XElement arguments = DfoToXElement(inputs);

            return WebServiceProxy.CallAsync(proc, arguments).ContinueWith(t => GetDynamicDataContinuation(t));
        }

        private XElement GetDynamicDataContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new XElement("Results");

            return task.Result;
        }

        #endregion

        #region Save Forecast

        public bool SaveForecast(XElement args)
        {
            return MessageConverter.DisplayMessage(WebServiceProxy.Call(StoredProcedure.Demand.SaveForecast, args));
        }

        #endregion

        #region SaveSettings

        public void SaveSettings(bool excludeFromBulkForecast)
        {
            XElement arguments = new XElement("SaveSettings");
            arguments.Add(UserIdxElement);

            arguments.AddElement("Exclude", excludeFromBulkForecast ? "1" : "0");

            MessageConverter.DisplayMessage(WebServiceProxy.Call(StoredProcedure.Demand.SaveSettings, arguments));
        }

        #endregion

        #region LoadLibrary

        public static XElement LoadLibrary(string libraryIdx)
        {
            return DynamicDataAccess.GetDynamicData(StoredProcedure.Demand.GetSeasonals, XElement.Parse("<GetSeasonal><Seasonal_Idx>" + libraryIdx + "</Seasonal_Idx></GetSeasonal>"));
        }


        #endregion

        #region SaveSeasonalProfile

        public void SaveSeasonalProfile(XElement currentModel, XElement dataGrid, string profileIdx, string seasonalProfileName, DemandFilterObject dfo)
        {
            XElement arguments = new XElement("SaveSeasonalProfile");
            arguments.Add(UserIdxElement);

            arguments.AddElement("Profile_Idx", profileIdx);

            arguments.AddElement("Profile_Name", seasonalProfileName);

            arguments.AddElement("Current_Model", currentModel);

            arguments.AddElement("Grid", dataGrid);

            XElement customers = new XElement("Customers");
            foreach (var c in dfo.CustomerIdxs)
            {
                customers.AddElement("Idx", c);
            }
            arguments.Add(customers);

            XElement products = new XElement("Products");
            foreach (var p in dfo.ProductIdxs)
            {
                products.AddElement("Idx", p);
            }
            arguments.Add(products);
            
            arguments.AddElement("Trial_Idx", dfo.TrialIdx);
            
            MessageConverter.DisplayMessage(WebServiceProxy.Call(StoredProcedure.Demand.SaveSeasonalProfile, arguments));
        }

        #endregion

        #region WebService

        public GUIForecastResponseNew CalibrateCalculate(IEnumerable<ForecastParameter> currentModel, IEnumerable<ForecastParameter> parameters, string modelCode, string code, IEnumerable<Actual> actuals, IEnumerable<double> seasonals)
        {
            var entry = new GUIEntry();

            var request = new GUIRequest { config = GetConfig() };
            request.GUIForecastRequest = new GUIForecastRequest { Model_Code = modelCode, CurrentModel = currentModel, Parameters = parameters, Actuals = actuals, Seasonals = seasonals };            

            var response = entry.Post(code, request).NewForecastData;
            return response;


        }

        public static T Deserialize<T>(string xml, Type toType)
        {
            using (Stream stream = new MemoryStream())
            {
                byte[] data = Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                DataContractSerializer deserializer = new DataContractSerializer(toType);
                return (T)deserializer.ReadObject(stream);
            }
        }

        public static IEnumerable<double?> CalcSeasonals(IEnumerable<Actual> seasonalAndOutliers, string url)
        {
            var code = url.Split('/').Last();
            var entry = new GUIEntry();

            var request = new GUIRequest { config = GetConfig() };
            request.GUISeasonalRequest = new GUISeasonalRequest { SeasonalsWithOutliers = seasonalAndOutliers };

            var response = entry.Post(code, request).NewSeasonals.Seasonals;
            return response;
        }

        public static IEnumerable<double?> SeasonalsSmooNorm(IEnumerable<Actual> newSeasonals, string url)
        {
            var code = url.Split('/').Last();
            var entry = new GUIEntry();


            var request = new GUIRequest { config = GetConfig() }; ;
            request.GUISeasonalRequest = new GUISeasonalRequest { NewSeasonals = newSeasonals };

            var response = entry.Post(code, request).NewSeasonals.Seasonals;
            return response;
        }


        public XElement WebServicePost(string url, string postData)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                var httpContent = new StringContent(postData, Encoding.UTF8, "application/xml");

                var response = client.PostAsync(url, httpContent).Result;

                var responseContent = response.Content;
                var responseString = responseContent.ReadAsStringAsync().Result;

                if (!response.IsSuccessStatusCode)
                {
                    MessageConverter.DisplayMessage(XElement.Parse(responseString).Element("Message"));
                    return null;
                }

                return XElement.Parse(responseString);
            }
        }

        public List<ModelType> LoadNewModelTypes()
        {
            var entry = new GUIEntry();

            var code = "Models";

            var request = new GUIRequest();          

            var response = entry.Post(code, request).Xml;
            return XElement.Parse(response).Elements().Select(n => new ModelType(n)).ToList();
            
        }

        #endregion

        #region Config

        private static Config GetConfig()
        {
            var res = DynamicDataAccess.GetDynamicData(StoredProcedure.Demand.GetConfig, new XElement("NoInput"), false);

            var doc = ToXmlDocument(new XDocument(res));

            var config = QFXUtil.Config.read_config_from_xml(doc);
            
            return config;
            //return DynamicDataAccess.GetGenericItem<Config>(StoredProcedure.Demand.GetConfig, new XElement("<NoInput></NotInput>"), false);
        }

        public static XmlDocument ToXmlDocument(XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        #endregion

        #region VisibleListings

        //<Summary>
        //Tuple<List<custIdx>, List<skuIdxs>>
        //</Summary>
        public static UserSelectedDefaults GetVisibleListings(string trialForcastIdx)
        {
            var args = CommonXml.GetBaseArguments("GetData");
            args.AddElement("Forecast_Idx", trialForcastIdx);

            var result = DynamicDataAccess.GetGenericItem<UserSelectedDefaults>(StoredProcedure.Demand.GetCustSku, args);
            //var result = DynamicDataAccess.GetDynamicData("app.Procast_SP_DEMAND_GetCustSku", args);
            //var custs = result.Element("Customers").Elements().Select(e => e.Value).ToHashSet();
            //var skus = result.Element("Products").Elements().Select(e => e.Value).ToHashSet();

            return result;
        }

        #endregion

        public static XElement DfoToXElement(DemandFilterObject dfo, string rootNode = "GetData")
        {
            XElement arguments = new XElement(rootNode);
            arguments.Add(UserIdxElement);

            if (dfo == null) return arguments;

            arguments.Add(InputConverter.ToCustomers(dfo.CustomerIdxs));

            arguments.Add(InputConverter.ToProducts(dfo.ProductIdxs));

            if (dfo.DateRange != null)
            {
                arguments.AddElement("Start", dfo.DateRange.StartDate.ToString("yyyy-MM-dd"));
                arguments.AddElement("End", dfo.DateRange.EndDate.ToString("yyyy-MM-dd"));
            }
            arguments.AddElement("Trial_Idx", dfo.TrialIdx);
            arguments.AddElement("Tab_Idx", dfo.TabIdx);

            return arguments;
        }


    }
}