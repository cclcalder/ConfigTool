using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Common.Utilities;
using Model.Entity;
using Model.Utilities;

namespace Model.DataAccess
{
    using System.Globalization;
using System.Security.Principal;
using System.Threading.Tasks;

    public static class LoginAccess
    {
        private const string Salt = "$2a$10$PwtD1e0SsFl79YAmJZT2le";

        public static User GetLoginUserID_Salt(string userName, string password)
        {
            string argument =
                "<Login><Username>{0}</Username><Password>{1}</Password></Login>".FormatWith(userName,
                    BCrypt.Net.BCrypt.HashPassword(password.ToUpperInvariant(), Salt).Substring(Salt.Length)); //UserSecurity.Encrypt(password));

            // -- Get the user
            var userNode = WebServiceProxy.Call(StoredProcedure.UserLogin, XElement.Parse(argument),DisplayErrors.No).Elements().FirstOrDefault();

            return new User(userNode);
        }


        public static User GetUser(string userID)
        {
            string argument = "<Login><User_Idx>{0}</User_Idx></Login>".FormatWith(userID);

            // -- Get the user
            var userNode = WebServiceProxy.Call(StoredProcedure.UserLogin, XElement.Parse(argument)).Elements().FirstOrDefault();

            return new User(userNode);
        }

     
        public static bool SaveUser()
        {
            string argument = @"<SaveUser>
                                <User_Idx>{0}</User_Idx>
                                <DefaultSalesOrganisation>{1}</DefaultSalesOrganisation>
                                <LanguageCode>{2}</LanguageCode>
                                <Accent>{3}</Accent>
                             </SaveUser>".FormatWith(User.CurrentUser.ID, User.CurrentUser.SalesOrganisationID, User.CurrentUser.LanguageCode, User.CurrentUser.Accent);

            // -- Get the user
            var userNode = WebServiceProxy.Call(StoredProcedure.SaveUser, XElement.Parse(argument)).Elements().FirstOrDefault();

            return true;
        }


  //      Expected input XML:
  //<Login>
  //  <User_Idx>1</User_Idx>
  //  <AppVersion>2.3.1.1111</AppVersion>
  //</Login>

  //Expected output XML:          
  //  <Results>
  //    <Outcome>Success</Outcome>
  //  </Results>

  //  <Results>
  //    <Outcome>Failed - And A Longer Message About What Is Missing</Outcome>
  //  </Results>

  //Usage:
  //  EXEC app.Procast_SP_LOGIN_CheckDependencies
  //  '
  //    <Login>
  //      <User_Idx>1</User_Idx>
  //      <AppVersion>2.3.1.1111</AppVersion>
  //    </Login>
  //  '

        public static string CheckDependencies(string version)
        {
            string argument = "<Login><User_Idx>{0}</User_Idx><AppVersion>{1}</AppVersion></Login>".FormatWith(User.CurrentUser.ID,version );
            var userNode = WebServiceProxy.Call(StoredProcedure.AppLoginCheckDependencies, XElement.Parse(argument)).Elements().FirstOrDefault();

            return "";
        }

        public static User GetActiveDirectoryUser()
        {
            var identity = WindowsIdentity.GetCurrent();
            if (identity == null) return null;
            var userName = identity.Name;

            string argument = "<Login><ADName>{0}</ADName></Login>".FormatWith(userName);

            // -- Get the user
            var userXml = WebServiceProxy.Call(StoredProcedure.ActiveDirectoryLogin, XElement.Parse(argument), DisplayErrors.No);
            var user = new User(userXml.Elements().FirstOrDefault());

            return user;
        }

        public static XElement GetLanguageSet(string code)
        {
            string argument = @"  <GetUserLanguage>
                                    <Language_Code>{0}</Language_Code>
                                </GetUserLanguage>
                                ".FormatWith(code);

            // -- Get the user
            var langNode = WebServiceProxy.Call(StoredProcedure.Login.UserLanguage, XElement.Parse(argument),DisplayErrors.No);

            return langNode;
        }

        public static System.Collections.Generic.IEnumerable<CultureInfo> GetLanguages()
        {
            string argument = "<GetUserLanguages></GetUserLanguages>";

            // -- Get the user
            var nodes = WebServiceProxy.Call(StoredProcedure.Login.GetLanguageList, XElement.Parse(argument), DisplayErrors.Yes).Elements();

            return nodes.Select(CultureXML.FromXml);
        }

        public static IList<SalesOrgData> GetSalesOrgs()
        {
            const string getSalesOrgTemplate = "<GetSalesOrg><UserID>{0}</UserID></GetSalesOrg>";
            var arguments = getSalesOrgTemplate.FormatWith(User.CurrentUser.ID);
          
            return WebServiceProxy.Call(StoredProcedure.GetSalesOrgs, XElement.Parse(arguments), DisplayErrors.Yes,true).Elements().Select(n => new SalesOrgData(n)).ToList();             
        }

        public static string ForgottenPasswordSavePassword(string oldPassword, string password)
        {
            const string passordTemplate = "<ChangePassword><UserID>{0}</UserID><OldPassword>{1}</OldPassword><Password>{2}</Password></ChangePassword>";
            var arguments = passordTemplate.FormatWith(User.CurrentUser.ID, UserSecurity.Encrypt(oldPassword), UserSecurity.Encrypt(password));

            return WebServiceProxy.Call(StoredProcedure.ForgottenPasswordSavePassword, XElement.Parse(arguments)).Element("Message").Value;
        }

        //public static XElement GetMailServerDetails()
        //{
        //    var res = WebServiceProxy.Call(StoredProcedure.MailServerInfo, new XElement("Dummy"), DisplayErrors.No);
        //    return res;
        //}

        public static XElement ForgottenPasswordGetDetails(string userName)
        {
            var arguments = string.Format("<SaveData><User_LoginName>{0}</User_LoginName></SaveData>", userName);

            return WebServiceProxy.Call(StoredProcedure.ForgottenPasswordGetDetails, XElement.Parse(arguments), DisplayErrors.No);
        }

        public static XElement GetPasswordPolicy(string userName)
        {
            var arguments = string.Format("<GetPasswordPolicy><User_LoginName>{0}</User_LoginName></GetPasswordPolicy>", userName);

            var res = WebServiceProxy.Call(StoredProcedure.GetPasswordPolicy, XElement.Parse(arguments), DisplayErrors.No);

            return res;
        }

        public static XElement SaveNewPassword(string userName, string resetCode, string newPassword)
        {
            var arguments =
                string.Format("<SaveData><User_LoginName>{0}</User_LoginName><User_ResetCode>{1}</User_ResetCode><NewPassword>{2}</NewPassword></SaveData>", userName, resetCode, newPassword);

            var res =  WebServiceProxy.Call(StoredProcedure.ForgottenPasswordSavePassword, XElement.Parse(arguments), DisplayErrors.No);

            return res;
        }

        public static User LoginWithSession(string sessionIdx)
        {
            string argument = string.Format(@"
                <Login>
                    <Use_Session_Idx>1</Use_Session_Idx>
                    <Session_Idx>{0}</Session_Idx>
                </Login>"
                , sessionIdx);

            var userNode = WebServiceProxy.Call(StoredProcedure.LoginWithSession, XElement.Parse(argument)).Elements().FirstOrDefault();

            return new User(userNode);
        }

        public static User LoginWithSession(string displayableId, string idToken, string tenantId)
        {
            string argument = string.Format(@"<Login>
                                    <User_Email>{0}</User_Email>
                                    <Session_Idx>{1}</Session_Idx>
                                    <Tenant_Idx>{2}</Tenant_Idx>
                                </Login>", displayableId, idToken, tenantId);

            //Get the user using email instead of ID and 3rd party session ID, plus AD tenant ID
             var userNode = WebServiceProxy.Call(StoredProcedure.LoginWithSession, XElement.Parse(argument)).Elements().FirstOrDefault();

            return new User(userNode);


            //return new User()
            //{
            //    Session = idToken,
            //    ID="11"
            //};

        }

        public static User LoginWithOAuth(string usersServiceID)
        {
            XElement argumentElement = new XElement("Login");
            argumentElement.Add(new XElement("User_SSOName", usersServiceID));

            var userNode = WebServiceProxy.Call(StoredProcedure.LoginWithSession, argumentElement).Elements().FirstOrDefault();

            return new User(userNode);
        }

        public static SSOSettings GetSSOSettings()
        {
            SSOSettings theseSSoSettings  = new SSOSettings(WebServiceProxy.Call(StoredProcedure.GetSSOSettings, new XElement("dummy")));

            return theseSSoSettings;
        }
    }

    public class CultureXML
    {
        public static CultureInfo FromXml(XElement xml)
        {
            return new CultureInfo(xml.Element("LanguageCode").Value);
        }
    }

    public class SSOSettings
    {
        public SSOSettings(XElement xml)
        {
            SSOLocation = xml.MaybeElement("SSO_Location").MaybeValue();
            CanUseTwitterLogin = xml.MaybeElement("CanUseTwitterLogin").MaybeValue() == "1";

        }

        public string SSOLocation;
        public bool CanUseTwitterLogin;

    }
}


