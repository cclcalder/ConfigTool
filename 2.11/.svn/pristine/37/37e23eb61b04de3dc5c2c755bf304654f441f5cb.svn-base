//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Input;
//using Exceedra.Controls.Messages;
//using Microsoft.IdentityModel.Clients.ActiveDirectory;
//using Model;
//using Model.DataAccess;
//using ViewHelper;

namespace WPF.UserControls.Login.ViewModels
{
   public class AzureADViewModel
    {
       // #region help

       // //private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
       // //private static string tenant = ConfigurationManager.AppSettings["ida:Tenant"];
       // //private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
       // //Uri redirectUri = new Uri(ConfigurationManager.AppSettings["ida:RedirectUri"]);

       // //private static string authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);

       // ////
       // //// To authenticate to the To Do list service, the client needs to know the service's App ID URI.
       // //// To contact the To Do list service we need it's URL as well.
       // ////
       // //private static string todoListResourceId = ConfigurationManager.AppSettings["todo:TodoListResourceId"];
       // ////private static string todoListBaseAddress = ConfigurationManager.AppSettings["todo:TodoListBaseAddress"];
       // #endregion

       // public AzureADViewModel()
       //{

       //     //Register the TodoListClient app
       //     // from https://github.com/Azure-Samples/active-directory-dotnet-native-desktop
       //     //1 Sign in to the Azure management portal.
       //     //2 Click on Active Directory in the left hand nav.
       //     //3 Click the directory tenant where you wish to register the sample application.
       //     //4 Click the Applications tab.
       //     //5 In the drawer, click Add.
       //     //6 Click "Add an application my organization is developing".
       //     //7 Enter a friendly name for the application, for example "TodoListClient-DotNet", select "Native Client Application", and click next.
       //     //8 For the Redirect URI, enter http://TodoListClient. Click finish.
       //     //9 Click the Configure tab of the application.
       //     //10 Find the Client ID value and copy it aside, you will need this later when configuring your application.
       //     //11 In "Permissions to Other Applications", click "Add Application." Select "Other" in the "Show" dropdown, and click the upper check mark.Locate & click on the TodoListService, and click the bottom check mark to add the application.Select "Access TodoListService" from the "Delegated Permissions" dropdown, and save the configuration.


       // }

       // public User Login()
       //{

       //    if (!App.AzureADConfigData.IsActive)
       //    {
       //         CustomMessageBox.Show("App not configured for Azure Active Directory");
       //        return null;
       //    }
       //      string authority = String.Format(CultureInfo.InvariantCulture, App.AzureADConfigData.AADInstance, App.AzureADConfigData.Tenant);
       //     App.AuthContext = new AuthenticationContext(authority, new FileCache());
       //     AuthenticationResult result = null;
             
       //     try
       //     {
       //         if (App.AuthContext.TokenCache.Count != 0)
       //         {
       //             var r = App.AuthContext.TokenCache.ReadItems().First();
       //             return LoginAccess.LoginWithSession(r.DisplayableId, r.IdToken, r.TenantId);
       //         }
                
       //         result = App.AuthContext.AcquireToken(App.AzureADConfigData.ResourceId, App.AzureADConfigData.ClientId, new Uri(App.AzureADConfigData.RedirectUri), PromptBehavior.Always);

       //         //user is validated against AzureAD
       //         //get user context from our DB and set sessionToken using azureDB token
       //         return LoginAccess.LoginWithSession(result.UserInfo.DisplayableId, result.IdToken, result.TenantId);
                 
       //     }
       //     catch (AdalException ex)
       //     {
       //         if (ex.ErrorCode == "authentication_canceled")
       //         {
       //             CustomMessageBox.Show("Sign in was canceled by the user");
       //         }
       //         else
       //         {
       //             // An unexpected error occurred.
       //             string message = ex.Message;
       //             if (ex.InnerException != null)
       //             {
       //                 message += "Inner Exception : " + ex.InnerException.Message;
       //             }

       //             CustomMessageBox.Show(message);
       //         }

       //         return null;
       //     }
       // }
    }
}
