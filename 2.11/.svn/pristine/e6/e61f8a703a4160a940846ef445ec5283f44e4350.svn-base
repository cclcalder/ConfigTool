using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Model.DataAccess.Storage
{
    public class StorageAccess
    {
        public Task<XElement> GetFiles(string idx, string currentScreen)
        {
            var args = new XElement("GetData");
            args.Add(new XElement("User_Idx", Model.User.CurrentUser.ID));
            args.Add(new XElement("SalesOrg_Idx", Model.User.CurrentUser.SalesOrganisationID));
            args.Add(new XElement("Item_Idx", idx));
            args.Add(new XElement("Screen_Code", currentScreen));
             
            return WebServiceProxy.CallAsync(StoredProcedure.Storage.GetFiles, args, DisplayErrors.Yes)
                .ContinueWith(t => GetFilesContinuation(t));
        }

        private XElement GetFilesContinuation(Task<XElement> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null) return null;
            var robs = task.Result;
            return robs;
        }

        public Task<XElement> SaveFile(string currentScreen, string path, string idx, long bytes)
        {
            var args = new XElement("SaveData");
            args.Add(new XElement("User_Idx", Model.User.CurrentUser.ID));
            args.Add(new XElement("SalesOrg_Idx", Model.User.CurrentUser.SalesOrganisationID));
            args.Add(new XElement("File_Name", path));
            args.Add(new XElement("Item_Idx", idx));
            args.Add(new XElement("Screen_Code", currentScreen));
            args.Add(new XElement("File_Size", bytes));
             
            return WebServiceProxy.CallAsync(StoredProcedure.Storage.SaveFile, args, DisplayErrors.Yes)
                .ContinueWith(t => GetFilesContinuation(t));
        }

        public Task<XElement> UpdateFileView(string path)
        {
            var args = new XElement("SaveData");
            args.Add(new XElement("User_Idx", Model.User.CurrentUser.ID));
            args.Add(new XElement("SalesOrg_Idx", Model.User.CurrentUser.SalesOrganisationID));
            args.Add(new XElement("File_Name", path));
            args.Add(new XElement("Update_ViewCount", "1"));

            return WebServiceProxy.CallAsync(StoredProcedure.Storage.SaveFile, args, DisplayErrors.Yes)
                .ContinueWith(t => GetFilesContinuation(t));
        }


    }
}
