using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Exceedra.SingleSelectCombo.ViewModel;
using Model.Entity.Generic;
using Telerik.Windows.Controls;

namespace Exceedra.SingleSelectCombo.Controls
{
    /// <summary>
    /// Interaction logic for SingleSelectComboBox.xaml
    /// </summary>
    public partial class SingleSelectComboBox : INotifyPropertyChanged
    {
        public SingleSelectComboBox()
        {
            InitializeComponent();
        }

        public SingleSelectViewModel DataSource
        {
            get { return (SingleSelectViewModel)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(SingleSelectViewModel), typeof(SingleSelectComboBox), new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true });

        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(SingleSelectComboBox), new UIPropertyMetadata(true));


        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems.Count == 0 || !((ComboboxItem)e.AddedItems[0]).IsEnabled) && (e.RemovedItems.Count != 0 && ((ComboboxItem)e.RemovedItems[0]).IsEnabled))
            {
                if(DataSource == null)
                    ThisComboBox.SelectedItem = e.RemovedItems[0];
                else
                    ThisComboBox.SelectedItem = DataSource.SelectedItem;

            }
        }

        public static readonly DependencyProperty AreButtonsVisibleProperty =
            DependencyProperty.Register("AreButtonsVisible", typeof(bool), typeof(SingleSelectComboBox), new UIPropertyMetadata(false));


        public bool AreButtonsVisible
        {
            get { return (bool)GetValue(AreButtonsVisibleProperty); }
            set { SetValue(AreButtonsVisibleProperty, value); }
        }
        

        public event PropertyChangedEventHandler PropertyChanged;

    }

    public class StringLenghtFilteringBehavior : ComboBoxFilteringBehavior
    {
        public override List<int> FindMatchingIndexes(string text)
        {
            var list = ComboBox.Items.OfType<ComboboxItem>().Where(i => i.IsEnabled && i.Name.ToLower().Contains(text.ToLower())).Select(i => ComboBox.Items.IndexOf(i)).ToList();
            return list;
        }
    }
}
