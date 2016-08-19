namespace WPF.UserControls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for DropDownWithButtons.xaml
    /// </summary>
    public partial class DropDownWithButtons : UserControl
    {
        public static readonly DependencyProperty ProductComboStyleProperty =
            DependencyProperty.Register("ProductComboStyle", typeof (Style), typeof (DropDownWithButtons),
                                        new UIPropertyMetadata(null));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof (object), typeof (DropDownWithButtons),
                                        new UIPropertyMetadata(null, SelectedItemChanged));

       // private readonly string _displayMemberPropertyName;
        private object _lastSelectedItem;

        public DropDownWithButtons()
        {
            InitializeComponent();
            ItemsSourceChangedBehavior.SetItemsSourceChanged(ProductCombo, true);
            ProductCombo.AddHandler(ItemsSourceChangedBehavior.ItemsSourceChangedEvent,
                                    new RoutedEventHandler(ProductComboItemsSourceChanged));
            ProductCombo.SelectionChanged += ProductComboSelectionChanged;
        }

        public Style ProductComboStyle
        {
            get { return (Style) GetValue(ProductComboStyleProperty); }
            set { SetValue(ProductComboStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProductComboStyle.  This enables animation, styling, binding, etc...

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        private void ProductComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _lastSelectedItem = ProductCombo.SelectedItem ?? _lastSelectedItem;
        }

        private void ProductComboItemsSourceChanged(object sender, RoutedEventArgs e)
        {
            if (_lastSelectedItem == null)
            {
                ProductCombo.SelectedIndex = 0;
                return;
            }

            PropertyInfo property = _lastSelectedItem.GetType().GetProperty(ProductCombo.DisplayMemberPath);
            if (property == null) return;

            string lastSelectedDisplayText = (property.GetValue(_lastSelectedItem) ?? "").ToString();
            if (string.IsNullOrWhiteSpace(lastSelectedDisplayText))
                return;

            IEnumerable<object> q = from object item in ProductCombo.Items
                                    let itemDisplayMemberValue = property.GetValue(item)
                                    where itemDisplayMemberValue != null
                                    let itemDisplayText = itemDisplayMemberValue.ToString()
                                    where itemDisplayText == lastSelectedDisplayText
                                    select item;

            SelectedItem = ProductCombo.SelectedItem = q.FirstOrDefault();
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...

        private static void SelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DropDownWithButtons) d).SelectedItemChanged(e);
        }

        private void SelectedItemChanged(DependencyPropertyChangedEventArgs e)
        {
            _lastSelectedItem = e.NewValue ?? _lastSelectedItem;
            ProductCombo.SelectedItem = e.NewValue;
            Trace.WriteLine("SelectedItem changed from " + (e.OldValue ?? "NULL") + " to " + (e.NewValue ?? "NULL"));
        }
    }
}