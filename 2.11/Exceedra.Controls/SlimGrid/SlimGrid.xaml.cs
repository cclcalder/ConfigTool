using Coder.UI.WPF;
using Exceedra.Common.Utilities;
using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.DynamicGrid.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using Exceedra.Common;
using Telerik.Windows.Controls;

namespace Exceedra.SlimGrid
{
    /// <summary>
    /// Interaction logic for SlimGrid.xaml
    /// </summary>
    public partial class SlimGrid : UserControl, INotifyPropertyChanged
    {
        public SlimGrid()
        {
            InitializeComponent();

            grid.AddHandler(CommandManager.PreviewExecutedEvent, (ExecutedRoutedEventHandler)((sender, args) =>
            {
                if (args.Command == DataGrid.BeginEditCommand)
                {
                    DataGrid dataGrid = (DataGrid)sender;
                    DependencyObject focusScope = FocusManager.GetFocusScope(dataGrid);
                    FrameworkElement focusedElement = (FrameworkElement)FocusManager.GetFocusedElement(focusScope);
                    if ((string)focusedElement.Tag == null) return;
                    Controls.DynamicGrid.Models.Property model = ((Record)focusedElement.DataContext).GetProperty((string)focusedElement.Tag);
                    if (!model.IsEditable)
                    {
                        args.Handled = true;
                    }
                }
            }));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private IEnumerable<Record> _visibleRecords;
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

        public DataGridHeadersVisibility IsHeadersVisible
        {
            get { return (DataGridHeadersVisibility)GetValue(IsHeadersVisibleProperty); }
            set { SetValue(IsHeadersVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsHeadersVisibleProperty =
            DependencyProperty.Register("IsHeadersVisible", typeof(DataGridHeadersVisibility), typeof(SlimGrid));

        public bool CanUserSelectAllRow
        {
            get { return (bool)GetValue(CanUserSelectAllRowProperty); }
            set { SetValue(CanUserSelectAllRowProperty, value); }
        }

        public static readonly DependencyProperty CanUserSelectAllRowProperty =
            DependencyProperty.Register("CanUserSelectAllRow", typeof(bool), typeof(SlimGrid), new UIPropertyMetadata(true));

        public RecordViewModel DataSource
        {
            get { return (RecordViewModel)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(RecordViewModel),
                typeof(SlimGrid),
                new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true, PropertyChangedCallback = OnDataChanged }
                );

        //public double RowHeightDep
        //{
        //    get { return (double)GetValue(RowHeightDepProperty); }
        //    set { SetValue(RowHeightDepProperty, value); }
        //}

        //public static readonly DependencyProperty RowHeightDepProperty =
        //    DependencyProperty.Register("RowHeightDep", typeof(double),
        //        typeof(SlimGrid),
        //        new UIPropertyMetadata(20) 
        //        );

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SlimGrid)d).OnTrackerInstanceChanged(e);
        }

        protected virtual void OnTrackerInstanceChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((RecordViewModel)e.NewValue != null)
            {
                BuildColumns();

                VisibleRecords = DataSource.Records.Where(rec => rec.Item_IsDisplayed).ToList();

                MenuOptions.ItemsSource = new List<ContextMenuItem>(_visibleRecords.Select(r => new ContextMenuItem(r.Item_Idx, string.IsNullOrWhiteSpace(r.Item_Name) ? r.Item_Type : r.Item_Name, false)));

                AddCommentMenuItem.Visibility = DataSource.CanAddComments ? Visibility.Visible : Visibility.Collapsed;

                grid.ItemsSource = VisibleRecords;
            }
        }

        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register("RowHeight", typeof(double),
                typeof(SlimGrid),
                new UIPropertyMetadata(20.00));

        private void grid_Selected(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(DataGridCell) && ((DataGridCell)e.OriginalSource).Column.GetType() == typeof(DataGridTextColumn))
            {
                // Starts the Edit on the row;
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);
            }
        }

        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = (DataGridCell)sender;
            if (cell != null && !cell.IsEditing && !cell.IsReadOnly)
            {
                if (!cell.IsFocused)
                {
                    cell.Focus();
                }
                if (grid.SelectionUnit != DataGridSelectionUnit.FullRow)
                {
                    if (!cell.IsSelected)
                    {
                        cell.IsSelected = true;
                        grid.BeginEdit();
                    }
                }
                else
                {
                    DataGridRow row = Common.Extensions.GetVisualParent<DataGridRow>(cell);
                    if (row != null && !row.IsSelected)
                    {
                        row.IsSelected = true;
                    }
                }
            }

        }

        private void BuildColumns()
        {
            if (grid.Columns.Count > 0) return;

            var propertyIndex = 0;

            var columns = DataSource.Records.First(r => r.Item_IsDisplayed).Properties;
            CommentColour = columns.First().CommentColour;

            var cellTemplate = (ControlTemplate)FindResource("DataGridCellTemplate");

            if (CanUserSelectAllRow)
            {
                var d = GridColumns.GetSelectRowButton(SelectRowHandler);
                d.Width = new DataGridLength(12, DataGridLengthUnitType.Pixel);
                d.MaxWidth = 12;
                d.CanUserSort = false;

                grid.Columns.Add(d);
            }

            foreach (var column in columns)
            {

                if (column.ControlType.ToLower() == "labelwithcheckbox")
                {
                    var newColumn = new DataGridTemplateColumn
                    {
                        Visibility = column.IsDisplayed ? Visibility.Visible : Visibility.Collapsed,
                        Header = column.HeaderText,
                        Width = column.Width.IsNumeric() ? column.Width.AsNumericInt() : 80,
                        CellTemplate = GetCheckBoxTemplate(propertyIndex),
                        CellStyle = new Style
                        {
                            Setters =
                                {
                                    new Setter
                                    {
                                        Property = BackgroundProperty,
                                        Value = new Binding("Properties[" + propertyIndex + "].BackgroundColour")
                                    },
                                },
                        }
                    };
                    grid.Columns.Add(newColumn);
                }
                else if (column.ControlType.ToLower() == "dropdown")
                {
                    if (RowHeight == 20) RowHeight = 25;

                    var newColumn = new DataGridTemplateColumn
                    {
                        Visibility = column.IsDisplayed ? Visibility.Visible : Visibility.Collapsed,
                        Header = column.HeaderText,
                        Width = column.Width.IsNumeric() ? column.Width.AsNumericInt() : 80,
                        CellTemplate = GetLabelTemplate(propertyIndex),
                        CellEditingTemplate = GetDropdownTemplate(propertyIndex),
                    };
                    grid.Columns.Add(newColumn);
                }
                else if (column.ControlType.ToLower() == "textbox")
                {
                    var newColumn = new DataGridTextColumn
                    {
                        Visibility = column.IsDisplayed ? Visibility.Visible : Visibility.Collapsed,
                        Header = column.HeaderText,
                        Binding = new Binding("Properties[" + propertyIndex + "].Value") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged },
                        Width = DataGridLength.Auto, //column.Width.IsNumeric() ? column.Width.AsNumericInt() : 80,
                        CellStyle = new Style
                        {
                            Setters =
                                {
                                    new Setter
                                    {
                                        Property = IsEnabledProperty,
                                        Value = false
                                    },
                                    new Setter
                                    {
                                        Property = BackgroundProperty,
                                        Value = new BrushConverter().ConvertFrom("#00000000") //new Binding("Properties[" + propertyIndex + "].BackgroundColour")
                                    },
                                    new Setter
                                    {
                                        Property = ForegroundProperty,
                                        Value = new BrushConverter().ConvertFrom("#FF000000")
                                    },
                                    new Setter
                                    {
                                        Property = TextBlock.TextAlignmentProperty,
                                        Value = new Binding("Properties[" + propertyIndex + "].Alignment")
                                    },
                                    new Setter
                                    {
                                        Property = BorderBrushProperty,
                                        Value = new Binding("Properties[" + propertyIndex + "].BorderColour")
                                    },
                                    new Setter
                                    {
                                        Property = BorderThicknessProperty,
                                        Value = new Thickness(3,1,1,1)
                                    },
                                    new Setter
                                    {
                                        Property = VerticalAlignmentProperty,
                                        Value = VerticalAlignment.Center
                                    },
                                    new Setter
                                    {
                                        Property = TagProperty,
                                        Value = column.ColumnCode
                                    },
                                    new Setter
                                    {
                                        Property = TemplateProperty,
                                        Value = cellTemplate
                                    },
                                },
                        }
                    };
                    grid.Columns.Add(newColumn);
                }
                else
                {
                    var newColumn = new DataGridTextColumn
                    {
                        Visibility = column.IsDisplayed ? Visibility.Visible : Visibility.Collapsed,
                        Header = column.HeaderText,
                        Binding = new Binding("Properties[" + propertyIndex + "].Value") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged },
                        Width = column.Width.IsNumeric() ? column.Width.AsNumericInt() : 80,
                        CellStyle = new Style
                        {
                            Setters =
                                {
                                    new Setter
                                    {
                                        Property = IsEnabledProperty,
                                        Value = true
                                    },
                                    new Setter
                                    {
                                        Property = BackgroundProperty,
                                        Value = new Binding("Properties[" + propertyIndex + "].BackgroundColour")
                                    },
                                    new Setter
                                    {
                                        Property = ForegroundProperty,
                                        Value = new BrushConverter().ConvertFrom("#FF000000")
                                    },
                                    /* This is a cheat. We have limited properties to set for the ContentTemplate and I needed one to set the comment indicator colour. 
                                    So I cheated and used borderbrush */
                                    //new Setter
                                    //{
                                    //    Property = BorderBrushProperty,
                                    //    Value = GetCommentColourBinding(propertyIndex)
                                    //},
                                    new Setter
                                    {
                                        Property = Extensions.TooltipColourProperty,
                                        Value = GetCommentColourBinding(propertyIndex)
                                    },
                                    new Setter
                                    {
                                        Property = Extensions.ParentTooltipColourProperty,
                                        Value = GetParentCommentColourBinding(propertyIndex)
                                    },
                                    new Setter
                                    {
                                        Property = TextBlock.TextAlignmentProperty,
                                        Value = new Binding("Properties[" + propertyIndex + "].Alignment")
                                    },
                                    new Setter
                                    {
                                        Property = ToolTipProperty,
                                        Value = GetListBoxTemplate(propertyIndex)
                                    },
                                    //new Setter
                                    //{
                                    //    Property = BorderThicknessProperty,
                                    //    Value = GetHasChangedBinding(propertyIndex)
                                    //},
                                    new Setter
                                    {
                                        Property = TagProperty,
                                        Value = column.ColumnCode
                                    },
                                    new Setter
                                    {
                                        Property = TemplateProperty,
                                        Value = cellTemplate
                                    },
                                    //new EventSetter
                                    //{
                                    //    Event = PreviewMouseLeftButtonDownEvent,
                                    //    Handler = new MouseButtonEventHandler(DataGridCell_PreviewMouseLeftButtonDown)
                                    //}
                                },
                        }
                    };
                    grid.Columns.Add(newColumn);
                }

                propertyIndex++;
            }
        }

        public Style GetComboboxElementStyle(int columnIdx)
        {
            var s = new Style(typeof(ComboBox))
            {
                Setters =
                {
                    new Setter
                    {
                        Property = ComboBox.ItemsSourceProperty,
                        Value = new Binding("Properties[" + columnIdx + "].Values") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged }
                    },
                    new Setter
                    {
                        Property = ComboBox.ItemContainerStyleProperty,
                        Value = new Style(typeof(ComboBoxItem))
                        {
                            Setters =
                            {
                                new Setter
                                {
                                    Property = ComboBoxItem.BackgroundProperty,
                                    Value = new Binding("Background") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged }
                                }
                            }
                        }
                    },
                    //new Setter
                    //{
                    //    Property = ComboBox.BackgroundProperty,
                    //    Value = new Binding("Properties[" + columnIdx + "].BorderColour") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged }
                    //}
                }
            };
            return s;
        }

        public string CommentColour { get; set; }

        private ListView GetListBoxTemplate(int columnIdx)
        {
            /* So loading some xaml directly from a resource (e.g. App.xaml) would cause all nodes cell in the same DataContext (i.e. a Record) to referece the same xaml!!!
             * This means that although it looked like each one should load its own AutoScrollingListView, they actually all used the same one, leading to them all point to the same Property binding.
             * To fix this I just wrote some ListView xaml, copied it into here and force each cell to get its own copy.
             */
            //THIS DOES NOT WORK, BUT LEFT AS A REFERENCE
            //var listbox = (AutoScrollingListView)Application.Current.FindResource("ListboxToolTipTemplate");
            //listbox.SetBinding(ItemsControl.ItemsSourceProperty, ToolTipBinding);
            //return listbox;

            var toolTipBinding = new Binding(string.Format("Properties[{0}].CommentList", columnIdx));
            toolTipBinding.Mode = BindingMode.TwoWay;
            toolTipBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            toolTipBinding.BindsDirectlyToSource = true;

            var listview = (ListView)XamlReader.Parse("<ListView xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Height=\"auto\" MaxHeight=\"500\" MaxWidth=\"250\" BorderBrush=\"Transparent\" BorderThickness=\"0\" ><ListBox.ItemTemplate><DataTemplate><Grid ToolTip=\"{ Binding Value}\"><Grid.RowDefinitions><RowDefinition Height=\"Auto\" /><RowDefinition Height=\"Auto\" /></Grid.RowDefinitions><TextBlock Grid.Row=\"0\" Text=\"{ Binding Header}\" FontSize=\"11\" MaxWidth=\"240\" TextWrapping=\"Wrap\" FontWeight=\"Bold\" /><TextBlock Grid.Row=\"1\" Text=\"{ Binding Value}\" FontSize=\"12\" MaxWidth=\"240\" TextWrapping=\"Wrap\" /></Grid></DataTemplate></ListBox.ItemTemplate></ListView>");
            listview.SetBinding(ItemsControl.ItemsSourceProperty, toolTipBinding);

            return listview;
        }


        private DataTemplate GetLabelTemplate(int columnIdx)
        {
            DataTemplate labelWithCheckBoxTemplate = new DataTemplate();

            FrameworkElementFactory labelCell = new FrameworkElementFactory(typeof(System.Windows.Controls.Label));
            labelCell.SetBinding(ContentProperty, new Binding("Properties[" + columnIdx + "].SelectedItem.Item_Name") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            labelCell.SetBinding(BackgroundProperty, new Binding("Properties[" + columnIdx + "].SelectedItem.Background") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            labelCell.SetValue(MarginProperty, new Thickness(0, -3, 0, 0));

            labelWithCheckBoxTemplate.VisualTree = labelCell;

            labelWithCheckBoxTemplate.Seal();

            return labelWithCheckBoxTemplate;
        }

        private DataTemplate GetCheckBoxTemplate(int columnIdx)
        {
            DataTemplate labelWithCheckBoxTemplate = new DataTemplate();

            FrameworkElementFactory checkBoxCell = new FrameworkElementFactory(typeof(CheckBox));
            checkBoxCell.SetBinding(ToggleButton.IsCheckedProperty, new Binding("Properties[" + columnIdx + "].Value2") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            checkBoxCell.SetBinding(ContentProperty, new Binding("Properties[" + columnIdx + "].Value") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            checkBoxCell.SetBinding(IsEnabledProperty, new Binding("Properties[" + columnIdx + "].IsEditable") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            //checkBoxCell.AddHandler(CheckBox.GotFocusEvent, focusDep);

            labelWithCheckBoxTemplate.VisualTree = checkBoxCell;

            labelWithCheckBoxTemplate.Seal();

            return labelWithCheckBoxTemplate;
        }

        private DataTemplate GetDropdownTemplate(int columnIdx)
        {
            DataTemplate dropdownTemplate = new DataTemplate();

            FrameworkElementFactory dropdown = new FrameworkElementFactory(typeof(ComboBox));
            dropdown.SetBinding(ComboBox.ItemsSourceProperty, new Binding("Properties[" + columnIdx + "].Values") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            dropdown.SetBinding(ComboBox.SelectedItemProperty, new Binding("Properties[" + columnIdx + "].SelectedItem") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            //dropdown.SetValue(ComboBox.BackgroundProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff3d3d")));
            //dropdown.SetValue(ComboBox.BorderBrushProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff3d3d")));
            dropdown.SetValue(FrameworkElement.TagProperty, columnIdx);
            //dropdown.AddHandler(ComboBox.MouseDownEvent, DropDownDependency);
            dropdown.SetValue(ComboBox.IsEnabledProperty, true);
            dropdown.SetValue(ComboBox.DisplayMemberPathProperty, "Item_Name");
            dropdown.SetValue(ComboBox.SelectedValuePathProperty, "Item_Idx");

            dropdownTemplate.VisualTree = dropdown;

            dropdownTemplate.Seal();

            return dropdownTemplate;
        }

        //private RoutedEventHandler focusDep
        //{
        //    get { return focusHandler; }
        //}

        //private void focusHandler(object sender, RoutedEventArgs e)
        //{
        //    //grid.CurrentCell = new DataGridCellInfo(grid.Items[0], grid.Columns[1]);
        //    grid.BeginEdit();
        //}

        //private MouseButtonEventHandler DropDownDependency
        //{
        //    get { return DropdownHandler; }
        //}

        //private void DropdownHandler(object sender, RoutedEventArgs e)
        //{
        //    var dropdown = ((FrameworkElement)sender);
        //    Record obj = dropdown.DataContext as Record;

        //    var columnIdx = (int)dropdown.Tag;

        //    int rowIdx = DataSource.Records.Where(r => r.Item_IsDisplayed).IndexOf(((FrameworkElement)sender).DataContext as Record);
        //    var row = grid.GetRow(rowIdx);

        //    grid.SelectedCells.Clear();

        //    var cell = grid.GetCell(row, columnIdx);
        //    grid.SelectedCells.Add(new DataGridCellInfo(cell));
        //}

        private Binding GetHasChangedBinding(int columnIdx)
        {
            var hasChangedBinding = new Binding(string.Format("Properties[{0}].HasChanged", columnIdx));
            hasChangedBinding.Converter = new BoolToThicknessConverter2();
            hasChangedBinding.Mode = BindingMode.TwoWay;
            hasChangedBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            hasChangedBinding.BindsDirectlyToSource = true;

            return hasChangedBinding;
        }

        private Binding GetCommentColourBinding(int columnIdx)
        {
            var hasChangedBinding = new Binding(string.Format("Properties[{0}].CommentColour", columnIdx));
            hasChangedBinding.Converter = new ChildTooltipColourToActualColour();
            hasChangedBinding.Mode = BindingMode.TwoWay;
            hasChangedBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            hasChangedBinding.BindsDirectlyToSource = true;

            return hasChangedBinding;
        }

        private Binding GetParentCommentColourBinding(int columnIdx)
        {
            var hasChangedBinding = new Binding(string.Format("Properties[{0}].CommentColour", columnIdx));
            hasChangedBinding.Converter = new ParentTooltipColourToActualColour();
            hasChangedBinding.Mode = BindingMode.TwoWay;
            hasChangedBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            hasChangedBinding.BindsDirectlyToSource = true;

            return hasChangedBinding;
        }

        private void SelectRowHandler(object sender, RoutedEventArgs e)
        {
            int rowIdx = DataSource.Records.Where(r => r.Item_IsDisplayed).IndexOf(((FrameworkElement)sender).DataContext as Record);

            var row = grid.GetRow(rowIdx);

            grid.SelectedCells.Clear();

            for (int i = 0; i < grid.Columns.Count; i++)
            {
                var cell = grid.GetCell(row, i);

                if (cell != null)
                    grid.SelectedCells.Add(new DataGridCellInfo(cell));
            }
        }

        #region Context Menu

        public void ShowHideMeasures_Click(object sender, EventArgs e)
        {
            var clickedItem = sender as MenuItem;

            if (clickedItem.Name == "rdnMeasure")
            {
                if (rdnSetValue.IsChecked)
                    rdnSetValue.IsChecked = false;
            }
            else if (rdnMeasure.IsChecked)
            {
                rdnMeasure.IsChecked = false;
            }

            MenuOptions.IsEnabled = rdnMeasure.IsChecked;
        }

        public void ContextMenuApply(object sender, RoutedEventArgs e)
        {
            /* If they are using an existing rows values */
            if (rdnMeasure.IsChecked)
            {
                var pairings = GetSelectedEditableCellsAndRecordsPairings();

                /* Get the record we want to grab values from */
                var name = ((ContextMenuItem)MenuOptions.SelectionBoxItem).DisplayName;
                var valuesFromRecord = DataSource.Records.First(r => r.Item_Type == name || r.Item_Name == name);

                var percChange = txtMassAmendValue.Text.Replace("%", "");

                if (percChange.IsNumeric())
                    foreach (var t in pairings)
                    {
                        if (valuesFromRecord.Properties[t.Item1].Value.IsNumeric())
                            t.Item2.Properties[t.Item1].Value = (valuesFromRecord.Properties[t.Item1].Value.AsNumeric() * (percChange.AsNumeric() + 100) / 100).ToString();
                    }
            }
            else if (rdnSetValue.IsChecked)
            {
                var pairings = GetSelectedEditableCellsAndRecordsPairings();

                foreach (var t in pairings)
                {
                    t.Item2.Properties[t.Item1].Value = txtMassAmendValue.Text;
                }

            }
        }

        /* WARNING: This may not work is we use hidden columns and the DisplayIdx may be outofsync with propertyIdx */
        /* For all the selected cells:
         * - get the ones generated by the GridCellSelector  
         * - Pair them by DisplayIdx and Record (row)
         * - Filter out the non-editable ones
         */
        private IEnumerable<Tuple<int, Record>> GetSelectedEditableCellsAndRecordsPairings()
        {
            var cellsToModify =
                grid.SelectedCells.Where(c => c.Column.GetType() == typeof(DataGridTextColumn))
                .Select(
                c =>
                {
                    return new Tuple<int, Record>(c.Column.DisplayIndex - 1, (Record)c.Item);
                }
                );

            cellsToModify = cellsToModify.Where(c => c.Item2.Properties[c.Item1].IsEditable);

            return cellsToModify;
        }

        private void btnAddCommentApply_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtComment.Text)) return;

            var pairings = GetSelectedEditableCellsAndRecordsPairings();

            foreach (var t in pairings)
            {
                t.Item2.Properties[t.Item1].AddComment(TxtComment.Text);
            }

            TxtComment.Text = "";
        }

        private void grid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (GetSelectedEditableCellsAndRecordsPairings().Any())
            {
                AddCommentMenuItem.Visibility = DataSource.CanAddComments ? Visibility.Visible : Visibility.Collapsed;
                ContextMenuSetValueTo.Visibility = Visibility.Visible;
            }
            else
            {
                AddCommentMenuItem.Visibility = Visibility.Collapsed;
                ContextMenuSetValueTo.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        /* Updating a Property's IsDisplayed in the Model does not immediately update the UI.
         * So after making changes to the model we can use this method to force an update.
         * I could not bind IsDisplayed to column visibility so had to do this instead.
         *
         * In the case where you are using a TreeGrid, hidden nodes (i.e. grids) will have a NULL DataSource,
         * So run this method first on the Root Node (which is always visible) and pass the result into the
         * second method for all the other nodes. 
         */
        public List<Tuple<string, Visibility>> ReassertVisibleColumns()
        {
            var propertyVisibilities = new List<Tuple<string, Visibility>>();

            grid.Columns.Do(c =>
            {
                var isVisible = c.Visibility == Visibility.Visible;
                var matchingProperty = DataSource.Records.First().Properties.FirstOrDefault(p => p.HeaderText == (string)c.Header);
                if (matchingProperty != null)
                {
                    var shouldBeVisible = matchingProperty.IsDisplayed;
                    if (isVisible != shouldBeVisible)
                        c.Visibility = shouldBeVisible ? Visibility.Visible : Visibility.Hidden;
                }
                propertyVisibilities.Add(new Tuple<string, Visibility>((string)c.Header, c.Visibility));
            });

            return propertyVisibilities;
        }

        public void ReassertVisibleColumns(List<Tuple<string, Visibility>> visibilities)
        {
            grid.Columns.Do(c =>
            {
                var matchingProperty = visibilities.FirstOrDefault(v => v.Item1 == (string)c.Header);
                if (matchingProperty != null)
                {
                    if (c.Visibility != matchingProperty.Item2)
                        c.Visibility = matchingProperty.Item2;
                }
            });
        }

    }

    public class Extensions
    {
        public static readonly DependencyProperty TooltipColourProperty =
            DependencyProperty.RegisterAttached("TooltipColour", typeof(string), typeof(Extensions), new PropertyMetadata(default(string)));

        public static void SetTooltipColour(UIElement element, string value)
        {
            element.SetValue(TooltipColourProperty, value);
        }

        public static string GeTooltipColour(UIElement element)
        {
            return (string)element.GetValue(TooltipColourProperty);
        }

        public static readonly DependencyProperty ParentTooltipColourProperty =
            DependencyProperty.RegisterAttached("ParentTooltipColour", typeof(string), typeof(Extensions), new PropertyMetadata(default(string)));

        public static void SetParentTooltipColour(UIElement element, string value)
        {
            element.SetValue(ParentTooltipColourProperty, value);
        }

        public static string GeParentTooltipColour(UIElement element)
        {
            return (string)element.GetValue(ParentTooltipColourProperty);
        }



    }

    public class ChildTooltipColourToActualColour : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                //IF we have the purple colour (indicating its a parent) then return transparent to the polygon pointing up
                return ((string)value == "#9966CC") ? "#00FFFFFF" : value;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class ParentTooltipColourToActualColour : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                //IF we have the purple colour (indicating its a parent) then return red to the polygon pointing down,
                //IF we have the red colour (indicating its a child) then return transparent to the poly pointing down.
                return ((string)value == "#9966CC") ? "#B20000" : (string)value == "#B20000" ? "#00FFFFFF" : value;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
