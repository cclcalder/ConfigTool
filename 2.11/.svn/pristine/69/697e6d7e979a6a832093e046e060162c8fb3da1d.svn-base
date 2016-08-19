using Exceedra.Common.Utilities;
using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.DynamicGrid.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace Exceedra.TelerikGrid.Controls
{
    /// <summary>
    /// Interaction logic for TelerikGrid.xaml
    /// </summary>
    public partial class TelerikGridControl : UserControl, INotifyPropertyChanged
    {
        public TelerikGridControl()
        {
            InitializeComponent();
        }

        public RecordViewModel DataSource
        {
            get { return (RecordViewModel)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(RecordViewModel),
                typeof(TelerikGridControl),
                new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true, PropertyChangedCallback = OnDataChanged }
                );

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TelerikGridControl)d).OnTrackerInstanceChanged(e);
        }

        protected virtual void OnTrackerInstanceChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((RecordViewModel)e.NewValue != null)
            {
                BuildColumns();

                VisibleRecords = DataSource.Records.Where(rec => rec.Item_IsDisplayed).ToList();

                Grid.ItemsSource = VisibleRecords;
            }
        }

        private IEnumerable<Record> _visibleRecords;

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<Record> VisibleRecords
        {
            get
            {

                if (DataSource != null && DataSource.Records != null)
                {                    
                    return _visibleRecords;
                }

                return null;
            }
            set
            {
                _visibleRecords = value;
                PropertyChanged.Raise(this, "VisibleRecords");
            }
        }

        private void BuildColumns()
        {
            var columns = DataSource.Records.First(r => r.Item_IsDisplayed).Properties;
            var propertyIndex = 0;
            Grid.Columns.Clear();
            foreach (var column in columns)
            {
                GridViewDataColumn templateColumn = new GridViewDataColumn
                {
                    Header = column.HeaderText,
                    Width = new GridViewLength(80.00, GridViewLengthUnitType.Pixel),
                    CellTemplateSelector = new GridCellSelector(propertyIndex)
                    {
                        CellFormat = column.StringFormat,
                        ColumnCode = column.ColumnCode,
                        CheckBoxHandler = CheckboxHandler
                    }
                };

                Grid.Columns.Add(templateColumn);

                propertyIndex++;
            }
        }

        private void CheckboxHandler(object sender, RoutedEventArgs e)
        {
            //IsSelectedHandler(sender, e);

            Record obj = ((FrameworkElement)sender).DataContext as Record;
            if (obj == null) return;

            var columnIdx = Convert.ToInt32(((CheckBox)sender).Tag);

            var thisCell = obj.Properties[columnIdx];

            var checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                if (thisCell.Value == checkBox.IsChecked.ToString())
                {
                    e.Handled = true;
                    return;
                }


                //BindingExpression bindingExpression = checkBox.GetBindingExpression(CheckBox.IsCheckedProperty);
                //bindingExpression.UpdateSource();

            }

        }

    }
}
