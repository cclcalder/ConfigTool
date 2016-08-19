using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Exceedra.Web.Models;

namespace Exceedra.Web
{
    public partial class DummyLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //private string _address = "https://localhost:44300/api/SysConfig";
        //protected void submit_OnClick(object sender, EventArgs e)
        //{
        //    // Create an HttpClient instance 
        //        HttpClient client = new HttpClient(); 

        //        // Send a request asynchronously continue when complete 
        //        client.PostAsync(_address, new StringContent("<login><username>" + username.Text + "</username><password>" + password.Text +"</password></login>")).ContinueWith( 
        //            (requestTask) => 
        //            { 
        //                // Get HTTP response from completed task. 
        //                HttpResponseMessage response = requestTask.Result; 

        //               // Check that response was successful or throw exception 
        //                response.EnsureSuccessStatusCode(); 

        //                // Read response asynchronously as JsonValue
        //                response.Content.ReadAsAsync<GateWayData>().ContinueWith( 
        //                            (readTask) =>
        //                            {
        //                                var result = readTask.Result;
        //                                //Do something with the result                   
        //                            }); 
        //            }); 
        //}
    }
}