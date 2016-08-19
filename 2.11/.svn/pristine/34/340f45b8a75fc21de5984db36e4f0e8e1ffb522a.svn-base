using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using WPF.ViewModels.Admin;

namespace WPF.Pages.Admin
{
    /// <summary>
    /// Interaction logic for Pattern1Control.xaml
    /// </summary>
    public partial class Pattern1Control : UserControl, INotifyPropertyChanged
    {

        private Pattern1ViewModel _viewModel;
        public Pattern1Control()
        {
            InitializeComponent();
            _viewModel = new Pattern1ViewModel();
            DataContext = _viewModel; 
            _viewModel.PropertyChanged += ViewModelPropertyChanged;
        }

        public static DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(Pattern1Control), new UIPropertyMetadata(false));

        public bool IsLoading
        {
            get
            {
                return (bool)GetValue(IsLoadingProperty);
            }

            set
            {
                SetValue(IsLoadingProperty, value);
            }
        }


        public static DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(Pattern1ViewModel), typeof(Pattern1Control), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = OnDataChanged,
                BindsTwoWayByDefault = true
            });

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
             ((Pattern1Control)d).OnTrackerInstanceChanged(e);
        }

        protected virtual void OnTrackerInstanceChanged(DependencyPropertyChangedEventArgs e)
        {
            DataContext = (Pattern1ViewModel)e.NewValue;
             
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DisplayRVM")
            {
              // Pattern1verticalControl.ItemDataSource = DataSource.DisplayRVM;
            }
        }
        public Pattern1ViewModel DataSource
        {
            get
            {
                return (Pattern1ViewModel)GetValue(DataSourceProperty);
            }

            set
            {
                SetValue(DataSourceProperty, value);
            }
        }


        public static DependencyProperty TextProperty = DependencyProperty.Register("Path", typeof(string), typeof(Pattern1Control), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Path
        {
            get
            {
                return GetValue(TextProperty) as string;
            }

            set
            {
                SetValue(TextProperty, value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region Tree Titles

        
        public static readonly DependencyProperty LeftTreeTitleProperty =
            DependencyProperty.Register("LeftTreeTitle", typeof(string),
            typeof(Pattern1Control),
            new FrameworkPropertyMetadata() { PropertyChangedCallback = OnLeftTreeTitleChanged, BindsTwoWayByDefault = true }
            );

        private static void OnLeftTreeTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Pattern1Control)d).OnLeftTreeTitleChange(e);
        }

        protected virtual void OnLeftTreeTitleChange(DependencyPropertyChangedEventArgs e)
        {
            if ((string)e.NewValue != null)
            {
                Pattern1LeftHandTree.TreeTitle = (string)e.NewValue;
                Pattern1LeftHandListTitle.Text = (string)e.NewValue;
            }
        }

        public string LeftTreeTitle
        {
            get { return (string)GetValue(LeftTreeTitleProperty); }

            set { SetValue(LeftTreeTitleProperty, value); }
        }

        #endregion
    }
}
