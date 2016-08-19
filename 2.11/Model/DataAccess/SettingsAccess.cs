using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Common.Utilities;
using Model.Entity;
using Model.Entity.UserSettings;

namespace Model.DataAccess
{
    public class SettingsAccess
    {
        public XElement UserIdxElement = new XElement("User_Idx", User.CurrentUser.ID);
        public XElement SalesOrgIdxElement = new XElement("SalesOrg_Idx", User.CurrentUser.SalesOrganisationID);

        public XElement UserLanguageElement
        {
            get { return new XElement("User_Language", User.CurrentUser.LanguageCode); }
        }

        public Task<List<ScreenComboBoxItem>> GetScreens()
        {
            return GetComboboxItems(StoredProcedure.Settings.GetScreenList, "GetScreenList");
        }

        public void SaveStartScreen(ScreenComboBoxItem startScreen)
        {
            XElement arguments = new XElement("SaveStartScreen");
            arguments.Add(UserIdxElement);
            arguments.AddElement("StartScreen_Idx", startScreen.Idx);
            arguments.AddElement("IsRobScreen", startScreen.IsRobScreen);

            DisplayMessage(WebServiceProxy.Call(StoredProcedure.Settings.SaveStartScreen, arguments));
        }

        #region Comboboxes
        private Task<List<ScreenComboBoxItem>> GetComboboxItems(string proc, string rootTag)
        {
            XElement arguments = new XElement(rootTag);
            arguments.Add(UserIdxElement);
            arguments.Add(UserLanguageElement);

            return WebServiceProxy.CallAsync(proc, arguments).ContinueWith(t => GetComboboxItemsContinuation(t));
        }

        private List<ScreenComboBoxItem> GetComboboxItemsContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<ScreenComboBoxItem>() { new ScreenComboBoxItem(XElement.Parse("<Results><Name>No Items</Name><Idx>1</Idx><IsSelected>1</IsSelected></Results>")) };

            return task.Result.Elements().Select(n => new ScreenComboBoxItem(n)).ToList();
        }
        #endregion 

        #region Display Message

        private void DisplayMessage(XElement result)
        {
            bool success = result.ToString().ToLower().Contains("success");
            var mess = result.Value;
            MessageBox.Show(mess, (success ? "Success" : "Error"), MessageBoxButton.OK, (success ? MessageBoxImage.Information : MessageBoxImage.Error));
        }

        #endregion

        public XElement SaveNewPassword(string oldPassword, string password)
        {

            const string passordTemplate = "<SaveData><User_Idx>{0}</User_Idx><OldPassword>{1}</OldPassword><NewPassword>{2}</NewPassword></SaveData>";

            
            var arguments = passordTemplate.FormatWith(User.CurrentUser.ID, UserSecurity.BcryptEncrypt(oldPassword), UserSecurity.BcryptEncrypt(password));

            return WebServiceProxy.Call(StoredProcedure.ChangePassword, XElement.Parse(arguments), DisplayErrors.No);
        }

        public void SaveScreenOrder(List<Screen> screens)
        {
            XElement arguments = new XElement("SaveScreenOrder");
            arguments.Add(UserIdxElement);

            var root = new XElement("Screens");

            foreach (var screen in screens)
            {
                var s = new XElement("Screen");
                s.Add(new XElement("Key",screen.Key));
                s.Add(new XElement("SortOrder", screen.SortOrder));

                root.Add(s);
            }
            arguments.Add(root);

            DisplayMessage(WebServiceProxy.Call(StoredProcedure.Settings.SaveScreenOrder, arguments));
        }

        public Task<List<Procedure>> GetDbProcedures()
        {
            var xmlResponse = WebServiceProxy.CallAsync(StoredProcedure.Settings.GetProcedures, new XElement("CristinaRules"), DisplayErrors.No);
            var processedResult = xmlResponse.ContinueWith(t => GetDbProceduresContinuation(t));

            return processedResult;
        }

        private List<Procedure> GetDbProceduresContinuation(Task<XElement> task)
        {
            if (task.IsFaulted || task.IsCanceled) return new List<Procedure>();
            return task.Result.Elements("Procedure").Select(Procedure.FromXml).ToList();
        }
    }
}