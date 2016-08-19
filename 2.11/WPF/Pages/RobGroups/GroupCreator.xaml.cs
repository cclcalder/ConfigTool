using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Exceedra.Common.Utilities;
using Exceedra.Controls.DynamicGrid.Models;
using Model;
using Model.Annotations;
using Model.Entity.ROBs;
using Telerik.Windows.Controls;
using WPF.ViewModels.ROBGroups;
using System.Windows.Threading;

namespace WPF.Pages.RobGroups
{
    /// <summary>
    /// Interaction logic for GroupCreator.xaml
    /// </summary>
    public partial class GroupCreator : INotifyPropertyChanged
    {
        private GroupCreatorViewModel _viewModel;

        public GroupCreatorViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
            set
            {
                _viewModel = value;
                PropertyChanged.Raise(this, "ViewModel");
            }
        }
        public string AppTypeID;
        public GroupCreator(string appTypeIdx, string robGroupIdx = null)
        {
            InitializeComponent();

            DataContext = ViewModel = new GroupCreatorViewModel(appTypeIdx, robGroupIdx);
            AppTypeID = appTypeIdx;
            ROBGroupDynamicGrid.HyperLinkHandler = GenericLinkHandler;

            CustomerResizeTimer.Tick += ResizeCustomer;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ((sender as ComboBox).SelectedItem) as ImpactOption;

            ViewModel.UpdateImapctFormat(selectedItem);
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
                case "ROB_File_Location":

                    if (path != null)
                        App.OpenScanLocation(path.Value);
                    break;
            }
        }

        private static readonly DispatcherTimer CustomerResizeTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 250), IsEnabled = false };

        private void Page_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            CustomerResizeTimer.IsEnabled = true;
            CustomerResizeTimer.Stop();
            CustomerResizeTimer.Start();
        }

        private void ResizeCustomer(object sender, EventArgs e)
        {
            CustomerResizeTimer.IsEnabled = false;

            if (CustLevelStack.ActualWidth + CustStack.ActualWidth + 40 > CustGroupBox.ActualWidth)
            {
                CustStack.SetValue(Grid.RowProperty, 1);
                CustStack.SetValue(Grid.ColumnProperty, 0);
            }
            else
            {
                CustStack.SetValue(Grid.RowProperty, 0);
                CustStack.SetValue(Grid.ColumnProperty, 1);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
