using System.Globalization;
using System.Windows;

namespace WPF.PromoTemplates
{
    using System;
    using System.Windows.Controls;
    using ViewModels;

    /// <summary>
    /// Interaction logic for Dates.xaml
    /// </summary>
    public partial class TemplateDates : Page
    {
        public TemplateDates() : this(null)
        {
            InitializeComponent();
        }

        public TemplateDates(PromotionTemplateViewModelBase viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");
            InitializeComponent();
            DataContext = viewModel;
        }

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