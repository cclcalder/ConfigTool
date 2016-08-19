using System;
using System.Windows;
using System.Windows.Controls;
using WPF.ViewModels.NPD;

namespace WPF.Pages.NPD
{
    /// <summary>
    /// Interaction logic for NPDPageV2.xaml
    /// </summary>
    public partial class NPDPageV2
    {
        public NPDViewModel ViewModel { get; set; }

        public NPDPageV2(string npdIdx = null)
        {
            InitializeComponent();

            DataContext = ViewModel = new NPDViewModel(npdIdx);

            ComponentsGrid.DeleteHandler = ViewModel.Delete;

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            //ComponentTreeView.TreeSource = ViewModel.ComponentTree;
            //ComponentTreeView.TreeTitle = "Products";
            //ForecaseSkuTreeView.TreeSource = ViewModel.ForecastTree;
            //ForecaseSkuTreeView.TreeTitle = "Products";
        }

        public void DecimalsOnlyTextChanged(object sender, EventArgs args)
        {
            TextBox txtbx = (sender as TextBox);
            String candidateText = txtbx.Text;
            if (String.IsNullOrEmpty(candidateText)) return;
            try
            {
                Convert.ToDecimal(candidateText);
            }
            catch (Exception)
            {
                String allButTheLast = candidateText.Substring(0, candidateText.Length - 1);
                txtbx.Text = allButTheLast;
                txtbx.Select(txtbx.Text.Length, 0);
            }
        }

        public void IntegersOnlyTextChanged(object sender, EventArgs args)
        {
            TextBox txtbx = (sender as TextBox);
            String candidateText = txtbx.Text;
            if (String.IsNullOrEmpty(candidateText)) return;
            try
            {
                Convert.ToInt32(candidateText);
            }
            catch (Exception)
            {
                String allButTheLast = candidateText.Substring(0, candidateText.Length - 1);
                txtbx.Text = allButTheLast;
                txtbx.Select(txtbx.Text.Length, 0);
            }
        }
    }

}