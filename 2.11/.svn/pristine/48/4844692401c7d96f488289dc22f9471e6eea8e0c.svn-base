using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Exceedra.Common;
using Model;
using Model.DataAccess;
using Model.DataAccess.Generic;
using WPF.UserControls.Trees.ViewModels;

namespace WPF.UserControls.Trees.DataAccess
{
    public class TreeAccess
    {
        public XElement UserIdxElement = new XElement("User_Idx", User.CurrentUser.ID);
        public XElement SalesOrgIdxElement = new XElement("SalesOrg_Idx", User.CurrentUser.SalesOrganisationID);

        public XElement GetSubCustomers(string promotionId, string userId)
        {
            string xml =
                string.Format(
                    @"  <TemplateGetCustomersSubLevel>
                            <User_Idx>{1}</User_Idx>
                            <Promo_Idx>{0}</Promo_Idx>                           
                          </TemplateGetCustomersSubLevel>",
                    promotionId, userId);

            var argument = XElement.Parse(xml);
            var nodes = WebServiceProxy.Call(StoredProcedure.Template.GetCustomersSubLevel, argument, DisplayErrors.No);
            return nodes;
        }

        public XElement GetSubCustomers(string promotionId, string userId, IEnumerable<string> customersIds)
        {
            string xml =
                string.Format(
                    @"  <TemplateGetCustomersSubLevel>
                            <User_Idx>{1}</User_Idx>
                            <Promo_Idx>{0}</Promo_Idx>                           
                          </TemplateGetCustomersSubLevel>",
                    promotionId, userId);

            var argument = XElement.Parse(xml);

            if (customersIds.Any())
            {
                XElement xCsvSelectedCustomers = new XElement("CSVSelectedCustomers");
                foreach (var nodeId in customersIds)
                    xCsvSelectedCustomers.Add(new XElement("Cust_Code", nodeId));
                argument.Add(xCsvSelectedCustomers);
            }

            var nodes = WebServiceProxy.Call(StoredProcedure.Template.GetCustomersSubLevel, argument, DisplayErrors.No);
            return nodes;
        }

        public XElement GetAdminRightTree(string menuItemIdx, List<string> selectedItems)
        {

            var arguments = new XElement("GetList");

            arguments.Add(UserIdxElement);
            arguments.AddElement("MenuItem_Idx", menuItemIdx);
            
            var selectedItemsElements = new XElement("SelectedItems");
            foreach (var item in selectedItems)
            {
                selectedItemsElements.Add(new XElement("Item_Idx", item));
            }
            arguments.Add(selectedItemsElements);

            var nodes = WebServiceProxy.Call(StoredProcedure.AdminPatternList.ApplySelection, arguments, DisplayErrors.No);
            return nodes;
        }

        public XElement GetClaimsStatuses(bool resetCache = false)
        {
            var cache = App.AppCache.GetItem("ClaimsStatuses");

            if (cache == null || resetCache)
            {
                XElement argument = new XElement("GetStatuses");
                argument.Add(UserIdxElement);
                
                var nodes = WebServiceProxy.Call(StoredProcedure.Claims.GetFilterStatuses, argument);
                App.AppCache.Upsert("ClaimsStatuses", nodes);
            }

            return (XElement) App.AppCache.GetItem("ClaimsStatuses").obj;
        }

    }
}