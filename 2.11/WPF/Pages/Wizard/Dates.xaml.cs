using System.Globalization;
using System.Windows;
using ViewModels;

namespace WPF.Wizard
{
    using System;
    using System.Windows.Controls;
    using ViewModels;

    /// <summary>
    /// Interaction logic for Dates.xaml
    /// </summary>
    public partial class Dates : Page
    {
        public Dates()
            : this(null)
        {
            InitializeComponent();
        }

        public Dates(PromotionWizardViewModelBase viewModel)
        {
            _viewModel = viewModel;

            if (_viewModel == null) throw new ArgumentNullException("viewModel");
            InitializeComponent();
            DataContext = _viewModel;
        }

        private PromotionWizardViewModelBase _viewModel;

        //private void btnNext_Click(object sender, RoutedEventArgs e)
        //{
        //    var page = new Products(DataContext as PromotionWizardViewModel);
        //    NavigationService.Navigate(page);
        //}

        private void DatePickerCellLostFocus(object sender, RoutedEventArgs e)
        {
            // if a user sets an empty date
            // it will be set back to the previous value

            DatePicker datePickerValue = (DatePicker)sender;
            if (string.IsNullOrEmpty(datePickerValue.Text))
                datePickerValue.Text = datePickerValue.DisplayDate.ToString(CultureInfo.CurrentCulture);
        }
    }
}