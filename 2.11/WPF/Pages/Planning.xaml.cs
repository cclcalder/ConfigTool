using System.Runtime.CompilerServices;
using Exceedra.Common.Utilities;
using Model.Annotations;

namespace WPF
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using Model;
    using global::ViewModels;

    /// <summary>
    /// Interaction logic for Insights.xaml
    /// </summary>
    public partial class Planning : INotifyPropertyChanged
    {
        public PlanningViewModel ViewModel { get; set; }

        public Planning()
        {
            InitializeComponent();
            //FilterCaretBtn.CaretSource = rowFilter;
            DataContext = ViewModel = PlanningViewModel.New();

            PropertyChanged.Raise(this, "ViewModel");

        }

        private void PredefinedDates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ManageDataPickersState();
        }

        private void ManageDataPickersState()
        {

            //if (cbPredefinedDates.SelectedItem != null && (cbPredefinedDates.SelectedItem as PlanningTimeRange).Name == "Custom")
            //{
            //    DateTime testDate = new DateTime(1900, 1, 1);

            //    PlanningTimeRange dateTimeRange = (cbPredefinedDates.SelectedItem as PlanningTimeRange);

            //    if (dateTimeRange.DateFrom != null && dateTimeRange.DateTo != null &&
            //        (DateTime.Parse(dateTimeRange.DateFrom) > testDate) && (DateTime.Parse(dateTimeRange.DateTo) > testDate))
            //    {
            //        ViewModel.DateFrom = DateTime.Parse(dateTimeRange.DateFrom);
            //        ViewModel.DateTo = DateTime.Parse(dateTimeRange.DateTo);
            //    }
            //    else
            //    {
            //        ViewModel.DateFrom = DateTime.Now.Subtract(new TimeSpan(30, 0, 0, 0));
            //        ViewModel.DateTo = DateTime.Now;
            //    }

            //    dtpEndDate.IsEnabled = true;
            //    dtpStartDate.IsEnabled = true;
            //}
            //else
            //{
            //    dtpEndDate.IsEnabled = false;
            //    dtpStartDate.IsEnabled = false;
            //}
        }


        private void DtpStartDate_OnLostFocus(object sender, RoutedEventArgs e)
        {
            //if (dtpStartDate.Text == "")
            //{
            //    dtpStartDate.Text = ViewModel.DateFrom.ToString();
            //}
        }


        private void DtpEndDate_OnLostFocus(object sender, RoutedEventArgs e)
        {
            //if (dtpEndDate.Text == "")
            //{
            //    dtpEndDate.Text = ViewModel.DateTo.ToString();
            //}
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if(PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}