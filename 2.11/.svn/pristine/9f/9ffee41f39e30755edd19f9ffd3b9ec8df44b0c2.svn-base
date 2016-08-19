using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entity;
using System.Xml.Linq;
using Model.Entity.Admin;

namespace Model.DataAccess.Admin
{
    public class MenuItemAccessor
    {
        public string WebProxyReturned = null;        

        public const string GetMenuItemString = @"<{0}><User_Idx>{1}</User_Idx></{0}>";

        public Task<IList<MenuItemList>> GetMenuList()
        {
            //const string getMenuHierarchy = "GetMenuHierarchy";
            const string getMenuItems = "GetMenuItems";
            string arguments = GetMenuItemString.FormatWith(getMenuItems, User.CurrentUser.ID);

            return WebServiceProxy.CallAsync(StoredProcedure.AdminPatternList.GetMenuItemList, XElement.Parse(arguments))
                .ContinueWith(t => GetMenuListContinuation(t));
        }

        public IList<MenuItemList> GetMenuListContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
            {
                return new List<MenuItemList>();
            }


            WebProxyReturned = task.ToString();

            return task.Result.Elements().Select(n => new MenuItemList(n)).ToList();
        }
    }
}
