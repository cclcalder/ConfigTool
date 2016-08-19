namespace WPF.Pages
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using Model.DataAccess;
    using ViewModels.Pricing;
    using mshtml;
    using WPF.Properties;
    using System.Windows.Navigation;
    using System.Net;

    /// <summary>
    /// Interaction logic for Pricing.xaml
    /// </summary>
    public partial class Pricing : Page
    {
        public Pricing()
        {
            InitializeComponent();

            PricingViewModel pricingViewModel = PricingViewModel.New(tvProducts, new PricingAccess());
            DataContext = pricingViewModel;

            Unloaded += (o, e) => pricingViewModel.Dispose();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var link = sender as Hyperlink;
            if (link == null) return;

            var context = link.DataContext as ItemDetailViewModel;
            if (context == null) return;

            if (string.IsNullOrWhiteSpace(context.Url)) return;

            string url = context.Url.StartsWith("http") ? context.Url : "http://" + context.Url;

            PopupTitleTextBlock.Text = context.ProductName;
            PopupGrid.Visibility = Visibility.Visible;
            Dispatcher.BeginInvoke(new Action(() => ProductDetailFrame.Source = new Uri(url, UriKind.Absolute)));
        }

        private void ProductDetailFrame_ContentRendered(object sender, EventArgs e)
        {
            var browser = ProductDetailFrame.Content as WebBrowser;
            if (browser != null)
            {
                browser.Navigated -= browser_Navigated;
                browser.Navigated += browser_Navigated;
            }
        }

        private void ProductDetailFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            string temp = e.Exception.Message;
        }

        private void ProductDetailFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            var request = e.WebRequest as HttpWebRequest;

            if (request != null)
            {
                request.CookieContainer = new CookieContainer();
                request.AllowAutoRedirect = false;
            }
        }

        private void browser_Navigated(object sender, NavigationEventArgs e)
        {
            var browser = sender as WebBrowser;
            if (browser == null) return;

            HTMLInputTextElement txtPassword;
            HTMLInputButtonElement btnLogon;
            HTMLInputTextElement txtUser;
            HTMLInputButtonElement checkBox1;
            if (GetLogonElements(browser, out txtUser, out txtPassword, out btnLogon, out checkBox1))
            {
                txtUser.value = Settings.Default.ReportServerUser;
                txtPassword.value = Settings.Default.ReportServerPassword;
                checkBox1.setAttribute("checked", "checked");
                btnLogon.click();
            }
        }

        private static bool GetLogonElements(WebBrowser browser, out HTMLInputTextElement txtUser,
                                             out HTMLInputTextElement txtPassword, out HTMLInputButtonElement btnLogon,
                                             out HTMLInputButtonElement checkBox1)
        {
            txtUser = txtPassword = null;
            btnLogon = checkBox1 = null;

            if (browser == null) return false;
            var doc = browser.Document as IHTMLDocument3;
            if (doc == null) return false;
            txtUser = doc.getElementById("TxtUser") as HTMLInputTextElement;
            if (txtUser == null) return false;
            txtPassword = doc.getElementById("TxtPwd") as HTMLInputTextElement;
            if (txtPassword == null) return false;
            var form = doc.getElementById("Logon") as HTMLFormElement;
            if (form == null) return false;
            btnLogon = doc.getElementById("BtnLogon") as HTMLInputButtonElement;
            if (btnLogon == null) return false;
            checkBox1 = doc.getElementById("CheckBox1") as HTMLInputButtonElement;
            return true;
        }

        //private void ProductDetailFrame_ContentRendered(object sender, EventArgs e)
        //{
        //    var browser = ProductDetailFrame.Content as WebBrowser;
        //    if (browser != null)
        //    {
        //        browser.Navigated -= browser_Navigated;
        //        browser.Navigated += browser_Navigated;
        //    }
        //}

        //private void ProductDetailFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        //{
        //    string temp = e.Exception.Message;
        //}

        //private void ProductDetailFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        //{
        //    var request = e.WebRequest as HttpWebRequest;

        //    if (request != null)
        //    {
        //        request.CookieContainer = new CookieContainer();
        //        request.AllowAutoRedirect = false;
        //    }
        //}

        //private void browser_Navigated(object sender, NavigationEventArgs e)
        //{
        //    var browser = sender as WebBrowser;
        //    if (browser == null) return;

        //    HTMLInputTextElement txtPassword;
        //    HTMLInputButtonElement btnLogon;
        //    HTMLInputTextElement txtUser;
        //    HTMLInputButtonElement checkBox1;
        //    if (GetLogonElements(browser, out txtUser, out txtPassword, out btnLogon, out checkBox1))
        //    {
        //        txtUser.value = Settings.Default.ReportServerUser;
        //        txtPassword.value = Settings.Default.ReportServerPassword;
        //        checkBox1.setAttribute("checked", "checked");
        //        btnLogon.click();
        //    }
        //}

        //private static bool GetLogonElements(WebBrowser browser, out HTMLInputTextElement txtUser,
        //                                     out HTMLInputTextElement txtPassword, out HTMLInputButtonElement btnLogon,
        //                                     out HTMLInputButtonElement checkBox1)
        //{
        //    txtUser = txtPassword = null;
        //    btnLogon = checkBox1 = null;

        //    if (browser == null) return false;
        //    var doc = browser.Document as IHTMLDocument3;
        //    if (doc == null) return false;
        //    txtUser = doc.getElementById("TxtUser") as HTMLInputTextElement;
        //    if (txtUser == null) return false;
        //    txtPassword = doc.getElementById("TxtPwd") as HTMLInputTextElement;
        //    if (txtPassword == null) return false;
        //    var form = doc.getElementById("Logon") as HTMLFormElement;
        //    if (form == null) return false;
        //    btnLogon = doc.getElementById("BtnLogon") as HTMLInputButtonElement;
        //    if (btnLogon == null) return false;
        //    checkBox1 = doc.getElementById("CheckBox1") as HTMLInputButtonElement;
        //    return true;
        //}

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PopupGrid.Visibility = Visibility.Hidden;
        }

        private void PopupGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
        }
    }
}