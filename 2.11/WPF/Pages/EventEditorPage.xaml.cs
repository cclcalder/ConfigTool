using Exceedra.Controls.DynamicGrid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
    /// Interaction logic for EventEditorPage.xaml
    /// </summary>
    public partial class EventEditorPage : Page
    {
        string _eventId;
        private EventEditorPageViewModel _viewModel;
        public EventEditorPage()
            : this(null)
        {

        }

        public EventEditorPage(string eventId)
        {
            _eventId = eventId;       
            InitializeComponent();
            DataContext = _viewModel = new EventEditorPageViewModel(_eventId);
            G1.HyperLinkHandler = GenericLinkHandler;

            G1.Header = App.CurrentLang.GetValue("ClaimEditor_G1", "Claims");
            G2.Header = App.CurrentLang.GetValue("ClaimEditor_G2", "Products");
            UploadFile.Load("CLAIM$EVENT", _eventId, App.Configuration.StorageDetails);
            //            dynamicEventsProductsGrid.Header = App.CurrentLang.GetValue("ClaimEditor_G2", "Products");
        }


        private void GenericLinkHandler(object sender, RoutedEventArgs e)
        {
            // find the control that has been clicked
            var control = e.OriginalSource as Button;

            // we also need the record (row) that the control sits in
            var record = ((FrameworkElement)sender).DataContext as Record;

            // we also need the current column the control is in - we need the column header to use as the filter filter 
            if (control == null) return;
            var selectedColumn = control.Tag.ToString();

            if (record == null) return;
            var path = record.Properties.SingleOrDefault(a => a.ColumnCode == selectedColumn);

            switch (selectedColumn)
            {

                case "Claim_Selection":
                    _viewModel.InitProductsGrid(record.Item_Idx);
                    G2.Visibility = (_viewModel.EventEditor_G2 != null && _viewModel.EventEditor_G2.Records !=null ? Visibility.Visible : Visibility.Hidden);

                    break;
                case "Claim_Scan_Location":

                    if (path != null)
                        App.OpenScanLocation(path.External_Data);
                    break;
                case "Claim_Line_Detail":


                    if (path != null)
                        RedirectMe.Goto("claim", record.Item_Idx, path.Value);
                    break;
                default:

                    break;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var url = _viewModel.ReportURL;

            if (!string.IsNullOrWhiteSpace(url))
            { 
                PopupGrid.Visibility = Visibility.Visible; 
            }

        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PopupGrid.Visibility = Visibility.Hidden;
        }

        private void Frame_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            string temp = e.Exception.Message;
        }

        private void Frame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            var request = e.WebRequest as HttpWebRequest;

            if (request != null)
            {
                request.CookieContainer = new CookieContainer();
                request.AllowAutoRedirect = false;
            }
        }

        private void EventAccrualGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            _viewModel.UpdateTotals();
        }

    }


}
