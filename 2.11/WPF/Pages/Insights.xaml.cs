using System.Net;
using mshtml;

namespace WPF
{
    using System;
    using System.Collections.Generic;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using Model;
    using Properties;
    using global::ViewModels;


    /// <summary>
    /// Interaction logic for Insights.xaml
    /// </summary>
    public partial class Insights : Page
    {
        private InsightsViewModel _vm;
        public Insights()
        {
            InitializeComponent();
            _vm = new InsightsViewModel();
            DataContext = _vm;
            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Accordion.SelectedItem = (DataContext as InsightsViewModel).GroupWithSelection;
        }

        private void btnResize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (leftCol.Width == new GridLength(0))
            {
                leftCol.Width = new GridLength(300);
                var uri = new Uri(App.SiteData.BaseURL + "/Images/caret/left.gif");
                var bitmap = new BitmapImage(uri);
                btnResize.Source = bitmap;
            }
            else
            {
                leftCol.Width = new GridLength(0);
                var uri = new Uri(App.SiteData.BaseURL + "/Images/caret/right.gif");
                var bitmap = new BitmapImage(uri);
                btnResize.Source = bitmap;
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var clickedReport = (sender as TextBlock).DataContext as Report;
            (DataContext as InsightsViewModel).CurrentReport = clickedReport;
        }

        private void frame1_ContentRendered(object sender, EventArgs e)
        {
            var browser = frame1.Content as WebBrowser;
            if (browser != null)
            {
                browser.Navigated -= browser_Navigated;
                browser.Navigated += browser_Navigated;
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

        private readonly HashSet<ListBox> _listBoxes = new HashSet<ListBox>();

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0) return;
            var currentBox = sender as ListBox;
            if (currentBox == null) return;

            _listBoxes.Add(currentBox);

            foreach (var box in _listBoxes)
            {
                if (!ReferenceEquals(box, currentBox))
                {
                    if (box.SelectedItem != null)
                    {
                        box.SelectedItem = null;
                    }
                }
            }
        }

        private void frame1_NavigationFailed_1(object sender, NavigationFailedEventArgs e)
        {
            string temp = e.Exception.Message;
        }

        private void frame1_Navigating_1(object sender, NavigatingCancelEventArgs e)
        {
            var request = e.WebRequest as HttpWebRequest;

            if (request != null)
            {
                request.CookieContainer = new CookieContainer();
                request.AllowAutoRedirect = false;
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var c = e.OriginalSource as Report;

            Report obj = ((FrameworkElement)sender).DataContext as Report;

            _vm.CurrentReport = obj;
        }
    }
}