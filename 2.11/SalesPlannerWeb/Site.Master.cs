using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using Website.Model;
//using Website.Model.DataAccess;

namespace SalesPlannerWeb
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Session["user"] != null)
            //{
            //    Model.User.CurrentUser = (Model.User)Session["user"];
            //}
            
            //p_login.Visible = (Model.User.CurrentUser != null);
        }
    }
}
