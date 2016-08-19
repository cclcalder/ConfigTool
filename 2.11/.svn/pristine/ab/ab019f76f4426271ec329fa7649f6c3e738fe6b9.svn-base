using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Exceedra.Common.Utilities;
using Exceedra.MultiSelectCombo.ViewModel;
using Model.Entity.Generic;
using Telerik.Windows.Controls;

namespace Exceedra.MultiSelectCombo.Controls
{
    /// <summary>
    /// Interaction logic for MultiSelectComboBox.xaml
    /// </summary>
    public partial class MultiSelectComboBox : INotifyPropertyChanged
    {
        public MultiSelectComboBox()
        {
            InitializeComponent();
        }

        public MultiSelectViewModel DataSource
        {
            get { return (MultiSelectViewModel)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(MultiSelectViewModel),
            typeof(MultiSelectComboBox),
            new FrameworkPropertyMetadata() { PropertyChangedCallback = OnDataChanged, BindsTwoWayByDefault = true }
            );

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MultiSelectComboBox)d).OnTrackerInstanceChanged(e);
        }

        protected virtual void OnTrackerInstanceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                var oldViewModel = e.OldValue as MultiSelectViewModel;
                oldViewModel.PropertyChanged -= RadComboBox_OnSelectionChanged;
            }

            if (e.NewValue != null && DataSource != null)
            {
                var newViewModel = e.NewValue as MultiSelectViewModel;
                newViewModel.PropertyChanged += RadComboBox_OnSelectionChanged;
            }
        }

        private void RadComboBox_OnSelectionChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItems")
                PropertyChanged.Raise(this, "SelectedIdx");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsSelectionEnabled
        {
            get { return (bool)GetValue(IsSelectionEnabledProperty); }
            set { SetValue(IsSelectionEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsSelectionEnabledProperty =
            DependencyProperty.Register("IsSelectionEnabled", typeof(bool),
            typeof(MultiSelectComboBox)
            );

        public event EventHandler SelectionChanged;

        public int SelectedIdx
        {
            get
            {
                if (RadComboBox.SelectedItem == null && DataSource != null && DataSource.Items != null && DataSource.Items.Any())
                    RadComboBox.SelectedItem = DataSource.Items[0];
                return 0;
            }
            set
            {
                var blah = value;
            }
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            if (SelectionChanged != null)
                SelectionChanged(sender, e);

            // refocus filter text box, so we can check a node and continue typing from keyboard
            var innerTextBoxes = RadComboBox.ChildrenOfType<TextBox>();
            var innerTextBox = innerTextBoxes.FirstOrDefault();
            if (innerTextBox != null) innerTextBox.Focus();
        }

        private void RadComboBox_OnDropdownClosed(object sender, EventArgs e)
        {
            RadComboBox.Text = DataSource.SelectedItemsText;

            if (string.IsNullOrEmpty(RadComboBox.Text))
                RadComboBox.SelectedItem = null;
        }

        private void RadComboBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            RadComboBox.Text = DataSource.SelectedItemsText;

            if (string.IsNullOrEmpty(RadComboBox.Text))
                RadComboBox.SelectedItem = null;
        }
    }
}
