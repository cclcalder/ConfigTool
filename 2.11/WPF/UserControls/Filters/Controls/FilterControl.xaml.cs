using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Model.Annotations;
using WPF.UserControls.Filters.ViewModels;

namespace WPF.UserControls.Filters.Controls
{
    /// <summary>
    /// Interaction logic for FilterControl.xaml
    /// </summary>
    public partial class FilterControl : INotifyPropertyChanged
    {
        //private FilterViewModel _viewModel;
        public FilterControl()
        {
            InitializeComponent();
          
        }

        #region Source Property

        public static readonly DependencyProperty FilterSourceProperty =
            DependencyProperty.Register("FilterSource", 
            typeof(FilterViewModel),
            typeof(FilterControl),
            new FrameworkPropertyMetadata { BindsTwoWayByDefault = true }
        );


        public FilterViewModel FilterSource
        {
            get { return (FilterViewModel)GetValue(FilterSourceProperty); }
            set { SetValue(FilterSourceProperty, value); }
        }

        #endregion

        public static readonly DependencyProperty SingleTreeTitleProperty =
            DependencyProperty.Register("SingleTreeTitle", typeof(string), typeof(FilterControl), new UIPropertyMetadata("Statuses"));

        public string SingleTreeTitle
        {
            get { return (string)GetValue(SingleTreeTitleProperty); }
            set { SetValue(SingleTreeTitleProperty, value); }
        }

        private string _otherGridTitle;
        public string OtherGridTitle
        {
            get { return _otherGridTitle ?? "Other"; }
            set
            {
                _otherGridTitle = value;
                OnPropertyChanged("OtherGridTitle");
            }
        }

        private string _expanderTitle;
        public string ExpanderTitle
        {
            get { return _expanderTitle ?? "Filters"; }
            set
            {
                _expanderTitle = value;
                OnPropertyChanged("ExpanderTitle");
            }
        }

        private Visibility _buttonsVisibility;
        public Visibility ButtonsVisibility
        {
            get { return _buttonsVisibility; }
            set
            {
                _buttonsVisibility = value;
                OnPropertyChanged("ButtonsVisibility");

                if (_buttonsVisibility == Visibility.Collapsed)
                    ButtonRow.Height = new GridLength(0);

                    //Grid.SetRowSpan(OtherProperties, 2);
            }
        }

        /// <summary>
        /// (Single Tree, Listings, Other) Listings requires 2 columns.
        /// </summary>
        public string Order
        {
            set
            {
                var items = value.Split(',');
                Tuple<int, int, int> v = new Tuple<int, int, int>(Convert.ToInt32(items[0]), Convert.ToInt32(items[1]), Convert.ToInt32(items[2]));
                Grid.SetColumn(SingleTree, v.Item1);
                Grid.SetColumn(Listings, v.Item2);
                Grid.SetColumn(PropertiesGrid, v.Item3);
                //Grid.SetColumn(Dates, v.Item3);

                Grid.SetRowSpan(SingleTree, (v.Item1 != 3) ? 2 : 1);
                Grid.SetRowSpan(Listings, (v.Item2 != 2) ? 3 : 2);
                Grid.SetRowSpan(PropertiesGrid, (v.Item3 != 3) ? 2 : 1);
                //Grid.SetRowSpan(Dates, (v.Item3 != 3) ? 2 : 1);
            }
        }

        public bool HideSingleTree
        {
            set
            {
                if (value)
                {
                    var column = Grid.GetColumn(SingleTree);
                    MainGrid.ColumnDefinitions[column].Width = new GridLength(0) ;

                    //Grid.SetColumn(CaretPanel, 0);
                    //Grid.SetColumnSpan(CaretPanel, 3);
                }
            }
        }

        public static readonly DependencyProperty CaretRowSourceProperty = DependencyProperty.Register("CaretRowSource", typeof(RowDefinition), typeof(FilterControl));

        public RowDefinition CaretRowSource
        {
            get { return (RowDefinition)GetValue(CaretRowSourceProperty); }
            set { SetValue(CaretRowSourceProperty, value); }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void EndDatePicker_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (EndDatePicker.Text == "")
            {
                EndDatePicker.Text = FilterSource.EndDate.ToString(); //TODO:Put VM here
            }
        }

        private void StartDatePicker_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (StartDatePicker.Text == "")
            {
                StartDatePicker.Text = FilterSource.StartDate.ToString(); //TODO:Put VM here
            }
        }

        public bool AreGroupsVisible
        {
            get { return (bool)GetValue(AreGroupsVisibleProperty); }
            set { SetValue(AreGroupsVisibleProperty, value); }
        }

        public static readonly DependencyProperty AreGroupsVisibleProperty =
            DependencyProperty.Register("AreGroupsVisible", typeof(bool),
                typeof(FilterControl),
                new PropertyMetadata(true, PropertyChangedCallback) );

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((FilterControl)d).AreGroupsVisibleTrackerInstanceChanged(e);
        }

        protected virtual void AreGroupsVisibleTrackerInstanceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
                //Grid.SetRow(Listings, 1);
                //Grid.SetRowSpan(Listings, Grid.GetRowSpan(Listings) - 1);
            }
        }

        private void DtpStartDate_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (dtpStartDate.Text == "")
            {
                dtpStartDate.Text = FilterSource.DateFrom.ToString(); 
            }
        }

        private void DtpEndDate_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (dtpEndDate.Text == "")
            {
                dtpEndDate.Text = FilterSource.DateTo.ToString();
            }
        }
    }
}
