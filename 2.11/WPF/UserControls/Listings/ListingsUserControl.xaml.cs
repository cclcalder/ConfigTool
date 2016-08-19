using System.Windows;

namespace WPF.UserControls.Listings
{
    public partial class ListingsUserControl
    {
        public ListingsUserControl()
        { 
            InitializeComponent();            
        }
         
        public ListingsViewModel ListingsSource
        {
            get { return (ListingsViewModel)GetValue(ListingsSourceProperty); }
            set { SetValue(ListingsSourceProperty, value); }
        }

        public static readonly DependencyProperty ListingsSourceProperty =
            DependencyProperty.Register("ListingsSource", typeof(ListingsViewModel),
            typeof(ListingsUserControl),
            new FrameworkPropertyMetadata { BindsTwoWayByDefault = true }
        );

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set { SetValue(ReadOnlyProperty, value); }
        }

        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool),
            typeof(ListingsUserControl),
            new FrameworkPropertyMetadata() { PropertyChangedCallback = OnReadOnlyChanged, BindsTwoWayByDefault = true }
            );

        private static void OnReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ListingsUserControl)d).OnReadOnlyTrackerInstanceChanged(e);
        }

        public bool AreGroupsVisible
        {
            get { return (bool)GetValue(AreGroupsVisibleProperty); }
            set { SetValue(AreGroupsVisibleProperty, value); }
        }

        public static readonly DependencyProperty AreGroupsVisibleProperty =
            DependencyProperty.Register("AreGroupsVisible", typeof(bool),
                typeof(ListingsUserControl),
                new UIPropertyMetadata(true));



        public bool CustomerSingleSelect
        {
            get { return (bool)GetValue(CustomerSingleSelectProperty); }
            set
            {
                SetValue(CustomerSingleSelectProperty, value);
                CustomersTree.IsSingleSelect = value;
            }
        }

        public static readonly DependencyProperty CustomerSingleSelectProperty =
            DependencyProperty.Register("CustomerSingleSelect", typeof(bool),
                typeof(ListingsUserControl),
                new UIPropertyMetadata(true));



        protected virtual void OnReadOnlyTrackerInstanceChanged(DependencyPropertyChangedEventArgs e)
        {
            CustomersTree.IsReadOnly = (bool) e.NewValue;
            ProductsTree.IsReadOnly = (bool)e.NewValue;
        }

    }
}
