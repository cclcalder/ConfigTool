using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.DynamicGrid.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Telerik.Windows.Controls;
using WPF.Navigation;
using WPF.ViewModels.Claims;

namespace WPF.Pages
{
    /// <summary>
    /// Interaction logic for Claims.xaml
    /// </summary>
    public partial class Claims
    {
        public ClaimsViewModel ViewModel { get; set; }
        public Claims()
        {
            InitializeComponent();
            FilterCaretBtn.CaretSource = rowFilter;

            DataContext = ViewModel = ClaimsViewModel.New();

            DynamicEventsGrid.HyperLinkHandler = HyperLinkHandler;
            dynamicClaimsEntryGrid.DropDownChangedHandler = DropDownChangedHandler;

            Claims_Lower_Range.Text = ViewModel.ClaimsLowerValue;
            Claims_Upper_Range.Text = ViewModel.ClaimsUpperVlaue;

        }

        private void HyperLinkHandler(object sender, RoutedEventArgs e)
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

            switch (selectedColumn.ToLower())
            {
                case "event_name":

                    RedirectMe.Goto("event", record.Item_Idx);

                    break;
                default:
                    break;
            }

        }

        private void DropDownChangedHandler(object sender, SelectionChangedEventArgs e)
        {
            // we need the combobox  thats changed
            ComboBox comboBox = (ComboBox)sender;

            // need the option to set it as selected, whilst deselecting the others.
            foreach (Option o in comboBox.Items) o.IsSelected = false;
            Option selectedOption = (Option)comboBox.SelectedItem;

            if (selectedOption == null) return;

            // we also need the record that the control sits in - to allow us to find the dependent columns we may need to update
            var rec = comboBox.GetVisualParent<ContentPresenter>().Content as Record;

            // we also need the current column the control is in - we need the column header to use as the filter filter 
            var col = comboBox.Tag.ToString();
       
            //try
            //{
                selectedOption.IsSelected = true;
                // all data is passed through to viewmodel!
                ViewModel.UpDateManualEntryClaims(rec, col);
            //}
            //catch
            //{

            //}
        }


        private void ClaimsSelectAll_Clicked(object sender, RoutedEventArgs e)
        {
            ViewModel.SetAllCLaimschecked();

        }

        private void EventsSelectAll_Clicked(object sender, RoutedEventArgs e)
        {
            ViewModel.SetAllEventsChecked();
        }

        private void Claims_Lower_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.ClaimsLowerValue = Claims_Lower_Range.Text;

            Claims_Lower_Range.Text = ViewModel.ClaimsLowerValue;

        }

        private void Claims_Upper_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.ClaimsUpperVlaue = Claims_Upper_Range.Text;

            Claims_Upper_Range.Text = ViewModel.ClaimsUpperVlaue;

        }
        #region Filter Textboxes

        private const string FilterWatermark = "Filter...";

        private void FilterTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var filterTextBox = (RadWatermarkTextBox)sender;
            if (filterTextBox.Text.Equals(FilterWatermark))
            {
                filterTextBox.Clear();
                filterTextBox.Foreground = Brushes.Black;
            }
            else
            {
                filterTextBox.SelectAll();
            }
        }

        private void FilterTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var filterTextBox = (RadWatermarkTextBox)sender;

            if (string.IsNullOrWhiteSpace(filterTextBox.Text))
            {
                ViewModel.ClaimsDynamicGrid.Filter = filterTextBox.Text;
                filterTextBox.Foreground = Brushes.Gray;
            }
        }

        private void FilterTextBox_TextChanged(object sender, KeyEventArgs e)
        {
            var filterTextBox = (RadWatermarkTextBox)sender;

            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                ViewModel.ClaimsDynamicGrid.Filter = filterTextBox.Text;
                filterTextBox.Foreground = Brushes.Gray;
            }
        }

        #endregion


        #region Filte2r Textboxes
 
        private void FilterTextBox2_GotFocus(object sender, RoutedEventArgs e)
        {
            var filterTextBox = (RadWatermarkTextBox)sender;
            if (filterTextBox.Text.Equals(FilterWatermark))
            {
                filterTextBox.Clear();
                filterTextBox.Foreground = Brushes.Black;
            }
            else
            {
                filterTextBox.SelectAll();
            }
        }

        private void FilterTextBox2_LostFocus(object sender, RoutedEventArgs e)
        {
            var filterTextBox = (RadWatermarkTextBox)sender;

            if (string.IsNullOrWhiteSpace(filterTextBox.Text))
            {
                ViewModel.EventsDynamicGrid.Filter = filterTextBox.Text;
                filterTextBox.Foreground = Brushes.Gray;
            }
        }

        private void FilterTextBox2_TextChanged(object sender, KeyEventArgs e)
        {
            var filterTextBox = (RadWatermarkTextBox)sender;

            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                ViewModel.EventsDynamicGrid.Filter = filterTextBox.Text;
                filterTextBox.Foreground = Brushes.Gray;
            }
        }

        #endregion

    }
}