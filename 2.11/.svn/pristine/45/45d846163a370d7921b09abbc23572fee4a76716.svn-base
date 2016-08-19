using Exceedra.Controls.DynamicGrid.Models;
using Model.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF.Navigation;
using WPF.ViewModels.Claims;

namespace WPF.Pages
{
    /// <summary>
    /// Interaction logic for ClaimPage.xaml
    /// </summary>
    public partial class ClaimPage
    {
        private ClaimEditorPageViewModel _viewModel;
        private string _claimId;
        public ClaimPage():this(null)
        {
        }

        public ClaimPage(string claimId)
        {
            _claimId = claimId;
            InitializeComponent();
            DataContext = _viewModel = new ClaimEditorPageViewModel(claimId);
            G3.Visibility = Visibility.Hidden;
            G2.HyperLinkHandler = GenericLinkHandler;

            G2.Header = App.CurrentLang.GetValue("Claims_G2", "Claims Events");
            G3.Header = App.CurrentLang.GetValue("Claims_G3", "Claims Products");
            UploadFile.Load("CLAIM$CLAIM",  claimId, App.Configuration.StorageDetails); 
            _viewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                var r = e;
            }
        }

        private void GenericLinkHandler(object sender, RoutedEventArgs e)
        {
            Record obj = ((FrameworkElement)sender).DataContext as Record;

            if (obj.Item_Idx != null)
            {
                if (e.OriginalSource.ToString().Contains("Select"))
                {
                    _viewModel.InitProductsGrid(obj.Item_Idx);

                    if (_viewModel.ClaimEditor_ProductGrid != null)
                        G3.Visibility = Visibility.Visible;
                }
                else
                {
                    var path = obj.Properties.SingleOrDefault(a => a.ColumnCode == "Event_Name");

                    if (path != null)
                        RedirectMe.Goto("event", obj.Item_Idx, path.Value);
                }
            }
        }

        private static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                var correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }

        //
        // SINGLE CLICK EDITING
        //
        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cell = sender as DataGridCell;

            // Perform only for Checkbox Column types
            if (cell.Column is DataGridCheckBoxColumn)
            {
                if (cell != null &&
                    !cell.IsEditing)
                {
                    if (!cell.IsFocused)
                    {
                        cell.Focus();
                    }
                    var dataGrid = FindVisualParent<DataGrid>(cell);
                    if (dataGrid != null)
                    {
                        if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
                        {
                            if (!cell.IsSelected)
                                cell.IsSelected = true;
                        }
                        else
                        {
                            var row = FindVisualParent<DataGridRow>(cell);
                            if (row != null && !row.IsSelected)
                            {
                                row.IsSelected = true;
                            }
                        }
                    }
                }
            }
        }

        private void EventsGrid_TargetUpdated(object sender, DataTransferEventArgs e)
        {

        }
    }
}
