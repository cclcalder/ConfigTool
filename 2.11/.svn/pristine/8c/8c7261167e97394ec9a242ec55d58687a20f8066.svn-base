using System.Collections.Generic;
using Model;
using Model.Utilities;
using WPF.ViewModels;

namespace WPF.Pages.LISTINGSMGMT
{
    /// <summary>
    /// Interaction logic for EditList.xaml
    /// </summary>
    public partial class EditList
    {
        public EditList()
        {
            InitializeComponent();

            var viewModel = EditListViewModel.New();
            DataContext = viewModel;

            viewModel.FiltersApplied += WebNavigatePost;
            WebNavigateGet("ListingsMgmt");
        }

        public EditList(bool reload)
        {
            InitializeComponent();

            DataContext = reload ? EditListViewModel.New(true) : EditListViewModel.New();
        }

        private void WebNavigateGet(string controller = null, string action = null, Dictionary<string, string> args = null)
        {
            Web.Get(WebBrowser, App.SiteData.BaseURL, controller, action, args);
        }

        private void WebNavigatePost(string controller = null, string action = null, Dictionary<string, string> args = null)
        {
            Web.Post(WebBrowser, App.SiteData.BaseURL, controller, action, args);
        }
    }
}
