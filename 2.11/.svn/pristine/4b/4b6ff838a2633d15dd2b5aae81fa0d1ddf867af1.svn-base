//using Coder.WPF.UI;
using Model.Entity.Admin;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
 

namespace WPF.Pages.Admin
{
    /// <summary>
    /// Interaction logic for Pattern2Control.xaml
    /// </summary>
    public partial class Pattern2Control : UserControl, INotifyPropertyChanged
    {

   
      //  public ISearchableTreeViewNodeEventsConsumer rightTree;

        public Pattern2Control()
        {
            InitializeComponent();
            Loaded += Page_Loaded;
        }
        
        private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var X = e;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        //public void passDataToVM(string valueToPass)
        //{
            
        //}
        public static DependencyProperty IsLoadingProperty =
        DependencyProperty.Register("IsLoading", typeof(bool), typeof(Pattern2Control), new UIPropertyMetadata(false));

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

        public static DependencyProperty TextProperty = DependencyProperty.Register("Path", typeof(string), typeof(Pattern2Control), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

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

        public static DependencyProperty DataSourceProperty =
                    DependencyProperty.Register("DataSource", typeof(Pattern2ListVM), typeof(Pattern2Control), new FrameworkPropertyMetadata()
                    {
                        PropertyChangedCallback = OnDataChanged,
                        BindsTwoWayByDefault = true
                    });

        public static DependencyProperty CustomDepenencyProperty = DependencyProperty.Register("CustomDependency", typeof(Pattern2ListVM), typeof(Pattern2Control), null);

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           ((Pattern2Control)d).OnTrackerInstanceChanged(e);
        }

        protected virtual void OnTrackerInstanceChanged(DependencyPropertyChangedEventArgs e)
        {
            DataSource = (Pattern2ListVM)e.NewValue;

            if (DataSource != null)
                DataSource.PropertyChanged += ViewModelPropertyChanged;
        }


        public Pattern2ListVM DataSource
        {
            get
            {
                return (Pattern2ListVM)GetValue(DataSourceProperty);
            }

            set
            {
                SetValue(DataSourceProperty, value);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsFiltered { get; set; }

        public string FilterText { get; set; }

        #region Tree Titles

        public static readonly DependencyProperty LeftTreeTitleProperty =
    DependencyProperty.Register("LeftTreeTitle", typeof(string),
        typeof(Pattern2Control),
        new FrameworkPropertyMetadata() { PropertyChangedCallback = OnLeftTreeTitleChanged, BindsTwoWayByDefault = true }
        );

        private static void OnLeftTreeTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Pattern2Control)d).OnLeftTreeTitleChange(e);
        }

        protected virtual void OnLeftTreeTitleChange(DependencyPropertyChangedEventArgs e)
        {
            if ((string)e.NewValue != null)
            {
                LeftTree.TreeTitle = (string) e.NewValue;
            }
        }

        public string LeftTreeTitle
        {
            get { return (string)GetValue(LeftTreeTitleProperty); }

            set { SetValue(LeftTreeTitleProperty, value); }
        }

        public static readonly DependencyProperty RightTreeTitleProperty =
            DependencyProperty.Register("RightTreeTitle", typeof(string),
            typeof(Pattern2Control),
            new FrameworkPropertyMetadata() { PropertyChangedCallback = OnRightTreeTitleChanged, BindsTwoWayByDefault = true }
            );

        private static void OnRightTreeTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Pattern2Control)d).OnRightTreeTitleChange(e);
        }

        protected virtual void OnRightTreeTitleChange(DependencyPropertyChangedEventArgs e)
        {
            if ((string)e.NewValue != null)
            {
                RightTree.TreeTitle = (string)e.NewValue;
            }
        }

        public string RightTreeTitle
        {
            get { return (string)GetValue(RightTreeTitleProperty); }

            set { SetValue(RightTreeTitleProperty, value); }
        }

        #endregion


    }
}
