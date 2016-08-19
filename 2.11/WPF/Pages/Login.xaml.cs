using System.Linq;
using System.Windows.Media; 
using Exceedra.Common.Utilities;
using Telerik.Windows.Controls;
using WPF.Pages;
using WPF.UserControls.Login.ViewModels;
using WPF.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Exceedra.Common.Mvvm;
using Exceedra.Controls.Messages;
using Model;
using Model.DataAccess;
using Application = System.Windows.Application;
using Cursors = System.Windows.Input.Cursors;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Model.Utilities;

namespace WPF
{


    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : PageBase
    {
        public LoginViewModel ViewModel;
        private Manager Oauth;

        public Login()
        {
            Oauth = new Manager();
            Oauth["consumer_key"] = "iN4nNhFkVp9vhlg9vpNzTKjQu";
            Oauth["consumer_secret"] = "9EdHzfGmwyxMKf9ON6QrmCE2zKvnNKfc1JHzW5Acbu5goV0wm6";
            InitializeComponent();
            Loaded += Page_Loaded;
            ViewModel = new LoginViewModel();

            DataContext = ViewModel;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //string SSOSettings = LoginAccess.GetSSOSettings();
            //if (NavigationService != null && SSOSettings != null) NavigationService.Navigate(new Uri(SSOSettings));

            Application.Current.MainWindow.Title = "ESP - Login";
            if (!App.AutomaticLoginSuppressed)
            {
                Cursor = Cursors.Wait;
                LoginBorder.Visibility = Visibility.Hidden;

                // all user login services need to set RememberMe to be automatic 
                // so the app takes over on next login without calling the external services
                TryRememberedUser(); 

                Cursor = Cursors.Arrow;
            }

            OAuthBrowser.LoadCompleted += OAuthBrowserOnLoadCompleted;
        }

        private void OAuthBrowserOnLoadCompleted(object sender, NavigationEventArgs navigationEventArgs)
        {
            //scrape the page HTML
            dynamic doc = OAuthBrowser.Document;
            var htmlText = doc.documentElement.InnerHtml;

            var divMarker = "<div id=\"oauth_pin\">"; // the div for twitter's oauth pin
            if (((string) htmlText).Contains("oauth_pin"))
            {
                var index = htmlText.LastIndexOf(divMarker) + divMarker.Length;
                if (index != null)
                {
                    var snip = htmlText.Substring(index);
                    int codeFrom = snip.IndexOf("<code>") + "<code>".Length;
                    int codeTo = snip.IndexOf("</code>");
                    string pin = snip.Substring(codeFrom, codeTo - codeFrom);
                    if (pin != null)
                    {
                        var accessTokens = Oauth.AcquireAccessToken("https://api.twitter.com/oauth/access_token", "POST",
                            pin);
                        OAuthPresenter.Visibility = Visibility.Hidden;
                        int screenNameFrom = accessTokens.AllText.IndexOf("screen_name=") + "screen_name=".Length;
                        int screenNameTo = accessTokens.AllText.IndexOf("&x_auth_expires");
                        string returnedUserName = accessTokens.AllText.Substring(screenNameFrom, screenNameTo - screenNameFrom);

                        var user = LoginAccess.LoginWithOAuth(returnedUserName);

                        ContinueToApp(user);
                    }
                }
            }



        }

        private void TryLoginUser()
        {
            Cursor = Cursors.Wait;
 
            LoginBorder.Visibility = Visibility.Hidden;
           
            User user = null;
  
            if (!string.IsNullOrEmpty(txtUserName.Text) && txtPassword.Password.HasValue())
            {
                user = User.Login(txtUserName.Text, txtPassword.Password);

                if (user.LoginMessage != null)
                    MessageBoxShow(user.LoginMessage);

                RememberMe rem = RememberMe.Dont;
                if (chkRememberMe.IsChecked == true)
                {
                    rem = RememberMe.Name;
                }

                if (chkAutomatic.IsChecked == true)
                {
                    rem = RememberMe.Automatic;
                }

                user.RememberMe = rem;
                if (user == null || user.ID == null)
                {
                    CustomMessageBox.Show("Invalid username or password");
                    LoginBorder.Visibility = Visibility.Visible;                    
                }
                else
                {
                    ContinueToApp(user);
                }
                     
            }              
            Cursor = Cursors.Arrow;
        }

        private void TryRememberedUser()
        {
            User user;
            user = User.GetRememberedUser(App.VersionInfo);

            if (user != null)
            {
                LoginBorder.Visibility = Visibility.Collapsed;

                //set accent from user
                var convertFromString = ColorConverter.ConvertFromString(user.Accent);
                if (convertFromString != null)
                    Windows8Palette.Palette.AccentColor = (Color)convertFromString;

                //store culture in XML if user is set to auto login
                try
                {
                    if (!string.IsNullOrEmpty(user.LanguageCode)) // && (App.CurrentLang.LanguageCode != user.LanguageCode)
                    {
                        TranslationManager.Instance.CurrentLanguage = new CultureInfo(user.LanguageCode);
                        App.CurrentLang.LanguageCode = user.LanguageCode;
                    }
                }
                catch (Exception ex)
                {
                    
                }

                if (!string.IsNullOrEmpty(user.Accent) && user.Accent.Contains('#'))
                {
                    Windows8Palette.Palette.AccentColor = (Color)ColorConverter.ConvertFromString(user.Accent);
                }
                else
                {
                    user.Accent = Windows8Palette.Palette.AccentColor.ToString();
                }

                if (user.RememberMe == RememberMe.Automatic)
                {
                    chkAutomatic.IsChecked = true;
                    ContinueToApp(user);
                }
                else if (user.RememberMe == RememberMe.Name)
                {
                    if (user.Hash1 != null) txtUserName.Text = UserSecurity.Decrypt(user.Hash1);
                    chkRememberMe.IsChecked = true;
                    LoginBorder.Visibility = Visibility.Visible;
                    txtUserName.Focus();
                }
                else
                {
                    user = null;
                    LoginBorder.Visibility = Visibility.Visible;
                    txtUserName.Focus();
                }
            }
            else
            {
                LoginBorder.Visibility = Visibility.Visible;
                txtUserName.Focus();
            }
        }

        ////load login type from DB so app knows what sort of endpoint we are using for login
        //private void GetLoginTypeForUser()
        //{
        //    App.LoginType = "Standard";
        //    //LoginAccess.GetLoginTypeForUser(txtUserName.Text).ContinueWith(t =>
        //    //{
        //    //    App.LoginType = t.Result;
        //    //});
        //}

        #region "AD"

        private User GetActiveDirectoryUser()
        {
            var activeDirectoryUser = LoginAccess.GetActiveDirectoryUser();
            return activeDirectoryUser;
        }

        #endregion


        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            TryLoginUser();
        }

        private void ContinueToApp(User user)
        {
            User.CurrentUser = user;

            if (user.RememberMe != RememberMe.Dont)
            {
                if (txtUserName.Text != "")
                {
                    user.Hash1 =  UserSecurity.Encrypt(txtUserName.Text);
                }

                User.Remember(App.VersionInfo);

            }
            Cursor = Cursors.Arrow;

            if (App.LoadConfiguration() == false)
            {
                //if there is no sys config returned then kill all stored data and force user to login
                User.DeleteRememberUserFile();
                App.AutomaticLoginSuppressed = true;

                LoginBorder.Visibility = Visibility.Visible;
                txtUserName.Focus();
            }
            else
            {
                if (User.CurrentUser.LanguageCode != null)
                {
                    CultureInfo userCultureInfo = new CultureInfo(User.CurrentUser.LanguageCode);

                    App.SetDefaultCulture(userCultureInfo);
                    App.LoadLanguage(User.CurrentUser.LanguageCode);
                    
                    TranslationManager.Instance.CurrentLanguage = userCultureInfo;
                }

                if (NavigationService != null) NavigationService.Navigate(new MainPage());
            }

           // User.CurrentUser.Logging = (App.Configuration == null || App.Configuration.IsVerBoseLogging);
        }

        private void PagePreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (LoginBorder.Visibility != Visibility.Visible) return;
            if (e.Key == Key.Enter) TryLoginUser();
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            App.AutomaticLoginSuppressed = false;

            var activeDirectoryUser = GetActiveDirectoryUser();
            activeDirectoryUser.RememberMe = RememberMe.Automatic;

            if (activeDirectoryUser.LoginMessage != null)
                MessageBoxShow(activeDirectoryUser.LoginMessage);

            ContinueToApp(activeDirectoryUser);
        }

        private void Hyperlink2_OnClick(object sender, RoutedEventArgs e)
        {
            if (App.SiteData.UserPasswordReset)
            {
                PasswordModalPresenter.Visibility = Visibility.Visible;
            }
            else
            {
                CustomMessageBox.Show("Your account has not been configured to allow you cannot change your password here, please contact your IT team");
            }

        }

        private void ButtonBase2_OnClick(object sender, RoutedEventArgs e)
        {
            PasswordModalPresenter.Visibility = Visibility.Hidden;
        }

        #region "AzureDB"
        private void Azurelogin_OnClick(object sender, RoutedEventArgs e)
        {
            Azurelogin();
        }

        private void Azurelogin()
        {
            //var v = new AzureADViewModel();
            //var user = v.Login();

            //if (user != null)
            //{
            //    user.RememberMe = RememberMe.Automatic;
            //    ContinueToApp(user);
            //}
        }

        #endregion

        private void OAuthBrowser_OnNavigated(object sender, NavigationEventArgs e)
        {
            
        }

        private void OAuthBrowser_OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            
        }

        private void Twitterlogin_OnClick(object sender, RoutedEventArgs e)
        {
            OAuthPresenter.Visibility = Visibility.Visible;
         
            // the URL to obtain a temporary "request token"
            var rtUrl = "https://api.twitter.com/oauth/request_token";
            Oauth.AcquireRequestToken(rtUrl, "POST");
            var url = "https://api.twitter.com/oauth/authorize?oauth_token=" + Oauth["token"];

            OAuthBrowser.Navigate(url);          

        }

        protected static void MessageBoxShow(string message, string title = null,
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxImage image = MessageBoxImage.Information)
        {
            switch (image)
            {
                case MessageBoxImage.Error:
                    Messages.Instance.PutError(message);
                    break;
                case MessageBoxImage.Warning:
                    Messages.Instance.PutWarning(message);
                    break;
                default:
                    Messages.Instance.PutInfo(message);
                    break;
            }
        }
    }


}