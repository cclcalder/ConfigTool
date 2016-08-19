using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.Controls.DynamicTab.ViewModels;
 

namespace Exceedra.Controls.DynamicTab.Controls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class DynamicTabControl : INotifyPropertyChanged
    {
        //private ObservableCollection<Record> _records;

        //private ObservableCollection<Record> Records
        //{
        //    get
        //    {
        //        return _records;
        //    }
        //    set
        //    {
        //        _records = value;
        //        PropertyChanged.Raise(this, "Records");

        //        if (Records != null && Records.Any())
        //            _tabControl.ItemsSource = Records[0].Properties;
        //    }
        //}

        public TabbedViewModel ItemDataSource
        {
            get { return (TabbedViewModel)GetValue(ItemDataSourceProperty); }
            set
            {

                    SetValue(ItemDataSourceProperty, value);

            }
        }


        #region DependencyProperties

        public static readonly DependencyProperty ItemDataSourceProperty =
                DependencyProperty.Register("ItemDataSource", typeof(TabbedViewModel),
                                               typeof(DynamicTabControl),
                                                new FrameworkPropertyMetadata()
                                                {
                                                    PropertyChangedCallback = OnDataChanged,
                                                    BindsTwoWayByDefault = true
                                                }
                                                       );

        #endregion

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DynamicTabControl)d).OnTrackerInstanceChanged(e);
        }

        protected virtual void OnTrackerInstanceChanged(DependencyPropertyChangedEventArgs e)
        {

            ItemDataSource.PropertyChanged += ViewModelPropertyChanged;


        }

        public DynamicTabControl()
        {
            InitializeComponent();

            //LoadingPanel.IsLoading = true;
            //LoadingPanel.Complete = false;

        }

        void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {           
            var vm = sender as TabbedViewModel;

            // If we have any property (tab) in the view model but none is selected in the tab view we will select the first one.
            // Happens after reloading the view model when the selected index was set to -1 (despite having tabs)
            //if (_tabControl.SelectedIndex == -1 && vm.Records.FirstOrDefault().Properties.Any())
            //    _tabControl.SelectedIndex = 0;

            //if (e.PropertyName.Equals("PanelMainMessage"))
            //{
            //    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            //      LoadingPanel.Message = _vm.PanelMainMessage
            //        ));
            //}
            //if (e.PropertyName.Equals("IsRunning"))
            //{
            //    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            //     LoadingPanel.IsLoading = _vm.IsRunning
            //       ));
            //}

        }



        public event PropertyChangedEventHandler PropertyChanged;
    }

}
