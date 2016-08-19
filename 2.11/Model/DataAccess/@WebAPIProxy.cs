using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Exceedra.Common.Logging;



namespace Model.DataAccess
{

    //public class UpliftResponse
    //{
    //    public Uplifts Uplifts { get; set; }

    //    public UpliftResponse()
    //    { }


    //    public UpliftResponse(XElement res)
    //    {

 

    //    }


    //}

    //public class Uplifts
    //{
    //    public string Promo_Idx { get; set; }
    //    public string Sku_Idx { get; set; }
    //    public List<UpliftMeasure> Measures { get; set; }
    //}

    //public class UpliftMeasure
    //{
    //    public string Code { get; set; }
    //    public string Value { get; set; }
    //}

    public class WebAPIProxy
    {

        public   static XElement Run(string endpoint, List<int> promoIDs)
       {
           using (var client = new HttpClient())
           {
               client.BaseAddress = new Uri(endpoint);
               client.DefaultRequestHeaders.Accept.Clear();
               client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                
               var breaks = endpoint.Split('/');
               var c = breaks.Length-1;
               var code = breaks[c] ?? "";
                
               //NOTE: the XML nodes MUST be in alphabetical order or it wont work
               //<UpliftRequest>
               //    <Promotions>
               //        <Promo_Idx>2</Promo_Idx>
               //        <Promo_Idx>1</Promo_Idx>
               //    </Promotions>	
               //    <RequestType>forecast</RequestType>
               //</UpliftRequest>

               var upr = new XElement("UpliftRequest");
               upr.Add(new XElement("Code", code));

               //upr.Add(new XElement("ConnectionString"));
               upr.Add(new XElement("Parameters"));

               var promos = new XElement("Promotions");
               foreach (var p in promoIDs)
               {
                   promos.Add(new XElement("Promo_Idx", p));
               }
               upr.Add(promos);
               upr.Add(new XElement("RequestType", "forecast"));

               var httpContent = new StringContent(upr.ToString(), Encoding.UTF8, "application/xml");

               if (User.CurrentUser != null && User.CurrentUser.Logging)
               {
                   StorageBase.LogMessageToFile("Service Call (Async)", "Call to DP API: " + endpoint, upr.ToString(),
                       (User.CurrentUser != null ? User.CurrentUser.ID : ""));
               }

               try
               {

                   var response = client.PostAsync("", httpContent).Result;
                   response.EnsureSuccessStatusCode();
                   if (response.IsSuccessStatusCode)
                   {
                       // by calling .Result you are performing a synchronous call
                       var responseContent = response.Content;

                       // by calling .Result you are synchronously reading the result
                       string responseString = responseContent.ReadAsStringAsync().Result;

                        if (User.CurrentUser != null && User.CurrentUser.Logging)
                        {
                            StorageBase.LogMessageToFile("Service Response", "Run", responseString,
                                (User.CurrentUser != null ? User.CurrentUser.ID : ""));
                        }
 
                    
                   Console.WriteLine(responseString);
                       return XElement.Parse(responseString);
                   }



               }
               catch (Exception ex)
               {
                   WebServiceProxy.LogException(DisplayErrors.No, ex);
                   return null;
               }

               return null;

           }
       }


    
    }
}
