using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Exceedra.Buttons;
using Exceedra.Controls;
using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.DynamicGrid.Converters;
using Exceedra.Test;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;
using Button = System.Windows.Controls.Button;
using ButtonBase = System.Windows.Controls.Primitives.ButtonBase;
using CheckBox = System.Windows.Controls.CheckBox;
using ComboBox = System.Windows.Controls.ComboBox;
using Control = System.Windows.Controls.Control;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Orientation = System.Windows.Controls.Orientation;
using TextBox = System.Windows.Controls.TextBox;
using TextBoxBase = System.Windows.Controls.Primitives.TextBoxBase;
using Coder.UI.WPF;
using System.Windows.Markup;
using System.Windows.Media;

namespace Exceedra.DynamicGrid.Models
{
    public class GridCellSelector : DataTemplateSelector
    {
        // The "columnIdx" argument has to be here because we're setting the bindings
        // for this cell in this constructor and if we don't specify the columnIdx
        // all the bindings will be set to the element of id "0" as the ColumnIdx property
        // won't be initialized on time
        // If confused check the implementation of the SetBindings() method.
        public GridCellSelector(int columnIdx)
        {
            ColumnIdx = columnIdx;
            SetBindings();
        }

        public int ColumnIdx { get; set; }
        public string ColumnCode { get; set; }
        public bool IsNum { get; set; }
        public ContextMenu ParentGridContextMenu { get; set; }

        #region handlers
       
        public RoutedEventHandler HyperlinkHandler { get; set; }

        public RoutedEventHandler NavigationHandler { get; set; }
        public RoutedEventHandler CheckBoxHandler { get; set; }
        public RoutedEventHandler LostFocusHandler { get; set; }
        public ToolTipEventHandler ToolTipOpeningHandler { get { return ToolTipHandler; }  }
        public RoutedEventHandler DeleteHandler { get; set; }
        public TextChangedEventHandler TextChangedHandler { get; set; }
        public SelectionChangedEventHandler DropdownHandler { get; set; }
        public KeyboardFocusChangedEventHandler TextFocusHandler { get; set; }

        #endregion

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null)
            {
                Record record = item as Record;

                if (record == null || record.Properties.Count <= ColumnIdx) return null;
                var cellType = record.Properties[ColumnIdx].ControlType.ToLowerInvariant();
                var isEditable = record.Properties[ColumnIdx].IsEditable;


                // The SetBindings() method assigns values to the bindings in this class.
                // Now, we'll be firing up this method just once in the default constructor
                // so that we can overwrite the bindings in different places of the app if we need it
                //SetBindings();
                switch (cellType)
                {
                    case "checkbox":
                        return GetCheckBoxTemplate();
                    case "dropdown":
                        return GetDropdownTemplate();
                    case "datepicker":
                        return GetDatePickerTemplate();
                    case "folderpicker":
                        return GetFileLocationTemplate();
                    case "navigation":
                        return GetNavigationLinkTemplate();
                    case "hyperlink":
                    case "externalhyperlink":
                        return GetHyperlinkTemplate();
                    case "labelwithcheckbox":
                        return GetLabelWithCheckBoxTemplate();
                    case "delete":
                        return GetDeleteTemplate();
                    case "treegridcell":
                        return isEditable ? GetEditableTreeGridCellTemplate() : GetTreeGridCellTemplate();
                    default: //Textbox
                        return isEditable ? GetTextBoxTemplate() : GetLabelTemplate();
                }
            }

            return null;
        }

        public DataTemplate SelectTemplate(string controlType)
        {
            // The SetBindings() method assigns values to the bindings in this class.
            // Now, we'll be firing up this method just once in the default constructor
            // so that we can overwrite the bindings in different places of the app if we need it
            //SetBindings();
            switch (controlType)
            {
                case "checkbox":
                    return GetCheckBoxTemplate();
                case "dropdown":
                    return GetDropdownTemplate();
                case "datepicker":
                    return GetDatePickerTemplate();
                case "folderpicker":
                    return GetFileLocationTemplate();
                case "hyperlink":
                    return GetHyperlinkTemplate();
                case "labelwithcheckbox":
                    return GetLabelWithCheckBoxTemplate();
                case "GetHeaderLabelWithCheckBoxTemplate":
                    return GetHeaderLabelWithCheckBoxTemplate();
            }

            return null;
        }

        private DataTemplate GetTextBoxTemplate()
        {
            DataTemplate textBoxTemplate = new DataTemplate();

            FrameworkElementFactory textBoxCell = new FrameworkElementFactory(typeof(TextBox));

            textBoxCell.SetBinding(TextBox.TextProperty, ValueBinding);
            textBoxCell.SetValue(Control.BackgroundProperty, BackgroundBrushBinding);
            textBoxCell.SetValue(Control.ForegroundProperty, ForegroundBrushBinding);
            textBoxCell.SetValue(FrameworkElement.TagProperty, ColumnIdx);
            textBoxCell.AddHandler(UIElement.LostFocusEvent, LostFocusHandler);
            textBoxCell.AddHandler(Control.MouseDoubleClickEvent, new MouseButtonEventHandler(Target));
            textBoxCell.AddHandler(UIElement.GotKeyboardFocusEvent, TextFocusHandler);
            textBoxCell.AddHandler(TextBoxBase.TextChangedEvent, TextChangedHandler);

            textBoxTemplate.VisualTree = textBoxCell;

            return textBoxTemplate;
        }



        /* Special template for getting textblocks that mimic labels until they are in edit mode.
         * Required for the new planning/demand tree so it looks pretty.
         */
        private DataTemplate GetEditableTreeGridCellTemplate()
        {
            FrameworkElementFactory borderCell = new FrameworkElementFactory(typeof(Border));

            borderCell.SetValue(Border.BorderThicknessProperty, ToolTipVisibilityBinding);
            borderCell.SetBinding(Border.BorderBrushProperty, ToolTipColourBinding);
            borderCell.SetValue(Border.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);

            DataTemplate textBoxTemplate = new DataTemplate();

            //FrameworkElementFactory textBoxCell = new FrameworkElementFactory(typeof(TextBox));
            //textBoxCell.SetBinding(TextBox.TextAlignmentProperty, AlignmentBinding);
            //textBoxCell.SetBinding(TextBox.TextProperty, ValueBinding);
            //textBoxCell.SetValue(Control.BackgroundProperty, BackgroundBrushBinding);
            //textBoxCell.SetBinding(UIElement.IsEnabledProperty, IsEditableBinding);
            //textBoxCell.SetValue(FrameworkElement.ToolTipProperty, ToolTipElement);
            //textBoxCell.SetValue(Control.PaddingProperty, new Thickness(4, -3, -1, 1));
            //textBoxCell.SetValue(Control.BorderBrushProperty, new SolidColorBrush(Colors.CornflowerBlue));
            //textBoxCell.SetBinding(Control.BorderThicknessProperty, HasChangedBinding);
            //textBoxCell.SetValue(FrameworkElement.HeightProperty, 20.00);
            //textBoxCell.SetValue(FrameworkElement.ContextMenuProperty, ParentGridContextMenu);
            //textBoxCell.AddHandler(FrameworkElement.ToolTipOpeningEvent, ToolTipOpeningHandler);



            FrameworkElementFactory textBoxCell2 = new FrameworkElementFactory(typeof(EditableTextBlock));
            textBoxCell2.SetBinding(EditableTextBlock.AlignmentProperty, AlignmentBinding);
            textBoxCell2.SetBinding(EditableTextBlock.TextAlignmentProperty, AlignmentBinding);
            textBoxCell2.SetBinding(EditableTextBlock.TextProperty, ValueBinding);
            textBoxCell2.SetValue(EditableTextBlock.CellBackgroundProperty, BackgroundBrushBinding);
            textBoxCell2.SetBinding(EditableTextBlock.IsEditableProperty, IsEditableBinding);
            textBoxCell2.SetValue(EditableTextBlock.BorderBrushProperty, new SolidColorBrush(Colors.CornflowerBlue));
            textBoxCell2.SetBinding(EditableTextBlock.BorderThicknessProperty, HasChangedBinding);
            textBoxCell2.SetValue(FrameworkElement.ToolTipProperty, ToolTipElement);

            textBoxCell2.AddHandler(FrameworkElement.ToolTipOpeningEvent, ToolTipOpeningHandler);

            //gridCell.AppendChild(borderCell);
            borderCell.AppendChild(textBoxCell2);

            textBoxTemplate.VisualTree = borderCell;

            textBoxTemplate.Seal();

            return textBoxTemplate;
        }

        private DataTemplate GetTreeGridCellTemplate(bool seal = true)
        {
            DataTemplate labelTemplate = new DataTemplate();

            FrameworkElementFactory borderCell = new FrameworkElementFactory(typeof(Border));

            borderCell.SetValue(Border.BorderThicknessProperty, ToolTipVisibilityBinding);
            borderCell.SetBinding(Border.BackgroundProperty, BackgroundBrushBinding);
            borderCell.SetValue(Border.BorderBrushProperty, new SolidColorBrush(Colors.Red));

            FrameworkElementFactory labelCell = new FrameworkElementFactory(typeof(TextBlock));

            labelCell.SetBinding(TextBlock.TextProperty, ValueBinding);
            
            labelCell.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
            labelCell.SetValue(TextBlock.ForegroundProperty, ForegroundBrushBinding);
            labelCell.SetValue(FrameworkElement.ToolTipProperty, ToolTipElement);
            labelCell.SetBinding(FrameworkElement.HorizontalAlignmentProperty, AlignmentBinding);
            labelCell.AddHandler(FrameworkElement.ToolTipOpeningEvent, ToolTipOpeningHandler);
            //labelCell.SetValue(TextBlock.FontSizeProperty, 9.00);

            borderCell.AppendChild(labelCell);

            labelTemplate.VisualTree = borderCell;

            if(seal)
            labelTemplate.Seal();

            return labelTemplate;
        }
        

        private DataTemplate GetLabelTemplate()
        {
            DataTemplate labelTemplate = new DataTemplate();

            FrameworkElementFactory borderCell = new FrameworkElementFactory(typeof(Border));

            borderCell.SetValue(Border.BorderThicknessProperty, new Thickness(4, 1, 1, 1));
            borderCell.SetBinding(Border.BorderBrushProperty, BorderBrushBinding);
            borderCell.SetBinding(Border.BackgroundProperty, BackgroundBrushBinding);

            FrameworkElementFactory labelCell = new FrameworkElementFactory(typeof(TextBlock));

            labelCell.SetBinding(TextBlock.TextProperty, ValueBinding);
            labelCell.SetBinding(FrameworkElement.HorizontalAlignmentProperty, AlignmentBinding);
            labelCell.SetValue(TextBlock.ForegroundProperty, ForegroundBrushBinding);
            
            borderCell.AppendChild(labelCell);
            
            labelTemplate.VisualTree = borderCell;            

            return labelTemplate;
        }

        private DataTemplate GetCheckBoxTemplate()
        {
            DataTemplate checkBoxTemplate = new DataTemplate();

            FrameworkElementFactory checkBoxCell = new FrameworkElementFactory(typeof(CheckBox));

            checkBoxCell.SetValue(ToggleButton.IsCheckedProperty, ValueBinding);
            checkBoxCell.SetValue(FrameworkElement.TagProperty, ColumnIdx);
            checkBoxCell.AddHandler(ToggleButton.CheckedEvent, CheckBoxHandler);
            checkBoxCell.AddHandler(ToggleButton.UncheckedEvent, CheckBoxHandler);
            checkBoxCell.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);

            checkBoxTemplate.VisualTree = checkBoxCell;

            return checkBoxTemplate;
        }

        private DataTemplate GetHyperlinkTemplate()
        {
            DataTemplate hyperlinkTemplate = new DataTemplate();

            FrameworkElementFactory hyperlinkCell = new FrameworkElementFactory(typeof(Button));

            hyperlinkCell.SetValue(ContentControl.ContentProperty, ValueBinding);
            hyperlinkCell.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            hyperlinkCell.AddHandler(ButtonBase.ClickEvent, HyperlinkHandler);
            hyperlinkCell.SetValue(FrameworkElement.TagProperty, ColumnCode);
            hyperlinkCell.SetValue(FrameworkElement.MarginProperty, new Thickness(5d, 0d, 0d, 0d));
            hyperlinkCell.SetValue(FrameworkElement.StyleProperty, Application.Current.FindResource("link") as Style);

            hyperlinkTemplate.VisualTree = hyperlinkCell;

            return hyperlinkTemplate;
        }

        private DataTemplate GetNavigationLinkTemplate()
        {
            DataTemplate hyperlinkTemplate = new DataTemplate();

            FrameworkElementFactory hyperlinkCell = new FrameworkElementFactory(typeof(Button));

            hyperlinkCell.SetValue(ContentControl.ContentProperty, ValueBinding);
            hyperlinkCell.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            hyperlinkCell.AddHandler(ButtonBase.ClickEvent, NavigationHandler);
            hyperlinkCell.SetValue(FrameworkElement.TagProperty, ExternalDataBinding);
            hyperlinkCell.SetValue(FrameworkElement.MarginProperty, new Thickness(5d, 0d, 0d, 0d));
            hyperlinkCell.SetValue(FrameworkElement.StyleProperty, Application.Current.FindResource("link") as Style);

            hyperlinkTemplate.VisualTree = hyperlinkCell;

            return hyperlinkTemplate;
        }

        private DataTemplate GetDropdownTemplate()
        {
            DataTemplate dropdownTemplate = new DataTemplate();

            FrameworkElementFactory dropdownCell = new FrameworkElementFactory(typeof(ComboBox));

            dropdownCell.SetValue(ItemsControl.IsTextSearchEnabledProperty, true);
            dropdownCell.SetValue(ItemsControl.DisplayMemberPathProperty, "Item_Name");
            dropdownCell.SetValue(Selector.SelectedValuePathProperty, "Item_Idx");
            dropdownCell.SetValue(FrameworkElement.TagProperty, ColumnCode);
            dropdownCell.SetBinding(ItemsControl.ItemsSourceProperty, DropdownValuesBinding);
            dropdownCell.SetBinding(Selector.SelectedItemProperty, DropdownSelectedItemBinding);
            dropdownCell.AddHandler(Selector.SelectionChangedEvent, DropdownHandler);
            dropdownCell.SetValue(UIElement.IsEnabledProperty, IsEditableBinding);                        

            dropdownTemplate.VisualTree = dropdownCell;

            return dropdownTemplate;
        }

        private DataTemplate GetDatePickerTemplate()
        {
            DataTemplate dataPickerTemplate = new DataTemplate();

            FrameworkElementFactory datePickerCell = new FrameworkElementFactory(typeof(DatePicker));
            DateTimeConverter converter = new DateTimeConverter();

            ValueBinding = new Binding(string.Format("Properties[{0}].Value", ColumnIdx));
            ValueBinding.Mode = BindingMode.TwoWay;
            ValueBinding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            ValueBinding.BindsDirectlyToSource = true;
            ValueBinding.Converter = converter;

            datePickerCell.SetBinding(DatePicker.SelectedDateProperty, ValueBinding);
            datePickerCell.SetValue(UIElement.IsHitTestVisibleProperty, IsEditableBinding);

            dataPickerTemplate.VisualTree = datePickerCell;

            return dataPickerTemplate;
        }

        private DataTemplate GetFileLocationTemplate()
        {
            DataTemplate fileLocationTemplate = new DataTemplate();

            FrameworkElementFactory fileLocationCell = new FrameworkElementFactory(typeof(FileEntry));

            fileLocationCell.SetBinding(FileEntry.TextProperty, ValueBinding);

            fileLocationTemplate.VisualTree = fileLocationCell;

            return fileLocationTemplate;
        }

        private DataTemplate GetLabelWithCheckBoxTemplate()
        {
            DataTemplate labelWithCheckBoxTemplate = new DataTemplate();

            FrameworkElementFactory gridFactory = new FrameworkElementFactory(typeof(Grid));
            gridFactory.SetBinding(Grid.BackgroundProperty, BackgroundBrushBinding);
            var column1 = new FrameworkElementFactory(typeof(ColumnDefinition));
            column1.SetValue(ColumnDefinition.WidthProperty, new GridLength(20, GridUnitType.Pixel));
            var column2 = new FrameworkElementFactory(typeof(ColumnDefinition));
            column2.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Star));

            gridFactory.AppendChild(column1);
            gridFactory.AppendChild(column2);

            FrameworkElementFactory borderCell = GetTreeGridCellTemplate(false).VisualTree;            
            borderCell.SetValue(Grid.ColumnProperty, 1);

            //This is a bit messy. Ideally we would just get the checkbox from GetCheckBoxTemplate()
            //But this is the first implementation of a cell with two controls and hence two values.
            //Will want to make a dynamic cell builder if we get more requirements for mutilcontrol cells.
            FrameworkElementFactory checkBoxCell = new FrameworkElementFactory(typeof(CheckBox));
            checkBoxCell.SetValue(ToggleButton.IsCheckedProperty, Value2Binding);
            checkBoxCell.SetValue(UIElement.IsEnabledProperty, IsEditableBinding);
            checkBoxCell.AddHandler(ToggleButton.CheckedEvent, CheckBoxHandler);
            checkBoxCell.AddHandler(ToggleButton.UncheckedEvent, CheckBoxHandler);
            checkBoxCell.SetValue(Grid.ColumnProperty, 0);

            //FrameworkElementFactory spFactory = new FrameworkElementFactory(typeof(StackPanel));
            //spFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Right);
            //spFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            //spFactory.SetValue(Grid.ColumnProperty, 1);

            //spFactory.AppendChild(checkBoxCell);

            gridFactory.AppendChild(borderCell);
            gridFactory.AppendChild(checkBoxCell);

            labelWithCheckBoxTemplate.VisualTree = gridFactory;

            labelWithCheckBoxTemplate.Seal();

            return labelWithCheckBoxTemplate;
        }

        private DataTemplate GetDeleteTemplate()
        {
            DataTemplate deleteTemplate = new DataTemplate();

            FrameworkElementFactory deleteButton = new FrameworkElementFactory(typeof(CrossButton));
            deleteButton.AddHandler(ButtonBase.ClickEvent, DeleteHandler);
            deleteButton.SetValue(FrameworkElement.HeightProperty, Convert.ToDouble(12));
            deleteButton.SetValue(FrameworkElement.WidthProperty, Convert.ToDouble(12));

            deleteTemplate.VisualTree = deleteButton;

            return deleteTemplate;
        }

        private DataTemplate GetHeaderLabelWithCheckBoxTemplate()
        {
            DataTemplate labelWithCheckBoxTemplate = new DataTemplate();

            FrameworkElementFactory gridFactory = new FrameworkElementFactory(typeof(Grid));

            ValueBinding = new Binding(string.Format("DataItem.Content"));
            ValueBinding.Mode = BindingMode.TwoWay;
            ValueBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            ValueBinding.BindsDirectlyToSource = true;


            FrameworkElementFactory borderCell = new FrameworkElementFactory(typeof(Border));

            borderCell.SetValue(Border.BorderThicknessProperty, new Thickness(4, 1, 1, 1));
            borderCell.SetBinding(Border.BorderBrushProperty, BorderBrushBinding);
            borderCell.SetBinding(Border.BackgroundProperty, BackgroundBrushBinding);

            FrameworkElementFactory labelCell = new FrameworkElementFactory(typeof(TextBlock));

            labelCell.SetBinding(TextBlock.TextProperty, ValueBinding);
            labelCell.SetValue(TextBlock.ForegroundProperty, ForegroundBrushBinding);

            borderCell.AppendChild(labelCell);

            //This is a bit messy. Ideally we would just get the checkbox from GetCheckBoxTemplate()
            //But this is the first implementation of a cell with two controls and hence two values.
            //Will want to make a dynamic cell builder if we get more requirements for mutilcontrol cells.
            FrameworkElementFactory checkBoxCell = new FrameworkElementFactory(typeof(CheckBox));
            //checkBoxCell.SetValue(ToggleButton.IsCheckedProperty, Value2Binding);
            //checkBoxCell.SetValue(UIElement.IsEnabledProperty, IsEditableBinding);
            checkBoxCell.AddHandler(ToggleButton.CheckedEvent, CheckBoxHandler);
            checkBoxCell.AddHandler(ToggleButton.UncheckedEvent, CheckBoxHandler);

            FrameworkElementFactory spFactory = new FrameworkElementFactory(typeof(StackPanel));
            spFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Right);
            spFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            spFactory.SetValue(Grid.ColumnProperty, 1);

            spFactory.AppendChild(checkBoxCell);

            gridFactory.AppendChild(borderCell);
            gridFactory.AppendChild(spFactory);

            labelWithCheckBoxTemplate.VisualTree = gridFactory;

            return labelWithCheckBoxTemplate;
        }

        private static void Target(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
                textBox.SelectAll();
        }

        #region Construct Bindings

        public ListView ToolTipElement { get; set; }
        public Binding ToolTipVisibilityBinding { get; set; }
        public Binding HasChangedBinding { get; set; }
        public Binding ToolTipColourBinding { get; set; }
        public Binding ToolTipBinding { get; set; }
        public Binding ValueBinding { get; set; }

        public Binding ExternalDataBinding { get; set; }
        public Binding Value2Binding { get; set; }
        public Binding IsEditableBinding { get; set; }
        public Binding DropdownValuesBinding { get; set; }
        public Binding DropdownSelectedItemBinding { get; set; }
        public Binding BackgroundBrushBinding { get; set; }
        public Binding ForegroundBrushBinding { get; set; }
        public Binding BorderBrushBinding { get; set; }
        public Binding AlignmentBinding { get; set; }
        public Binding ContentStringFormat { get; set; }
        public string CellFormat { get; set; }

        private void SetBindings()
        {
            ValueBinding = new Binding(string.Format("Properties[{0}].Value", ColumnIdx));
            ValueBinding.Mode = BindingMode.TwoWay;
            ValueBinding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            ValueBinding.BindsDirectlyToSource = true;
 
            Value2Binding = new Binding(string.Format("Properties[{0}].Value2", ColumnIdx));
            Value2Binding.Mode = BindingMode.TwoWay;
            Value2Binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            Value2Binding.BindsDirectlyToSource = true;

            ExternalDataBinding = new Binding(string.Format("Properties[{0}].External_Data", ColumnIdx));
            ExternalDataBinding.Mode = BindingMode.OneWay;
            ExternalDataBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            ExternalDataBinding.BindsDirectlyToSource = true;

            ToolTipBinding = new Binding(string.Format("Properties[{0}].CommentList", ColumnIdx));
            ToolTipBinding.Mode = BindingMode.TwoWay;
            ToolTipBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            ToolTipBinding.BindsDirectlyToSource = true;

            ToolTipVisibilityBinding = new Binding(string.Format("Properties[{0}].HasComment", ColumnIdx));
            ToolTipVisibilityBinding.Converter = new BoolToThicknessConverter();
            ToolTipVisibilityBinding.Mode = BindingMode.TwoWay;
            ToolTipVisibilityBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            ToolTipVisibilityBinding.BindsDirectlyToSource = true;

            ToolTipColourBinding = new Binding(string.Format("Properties[{0}].CommentColour", ColumnIdx));

            ToolTipElement = GetListBoxTemplate();

            IsEditableBinding = new Binding(string.Format("Properties[{0}].IsEditable", ColumnIdx));
            IsEditableBinding.Mode = BindingMode.TwoWay;
            IsEditableBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            IsEditableBinding.BindsDirectlyToSource = true;

            DropdownValuesBinding = new Binding(string.Format("Properties[{0}].Values", ColumnIdx));
            DropdownValuesBinding.Mode = BindingMode.OneWay;
            DropdownValuesBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            DropdownValuesBinding.BindsDirectlyToSource = true;

            DropdownSelectedItemBinding = new Binding(string.Format("Properties[{0}].SelectedItem", ColumnIdx));
            DropdownSelectedItemBinding.Mode = BindingMode.TwoWay;
            DropdownSelectedItemBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            DropdownSelectedItemBinding.BindsDirectlyToSource = true;

            HasChangedBinding = new Binding(string.Format("Properties[{0}].HasChanged", ColumnIdx));
            HasChangedBinding.Converter = new BoolToThicknessConverter2();
            HasChangedBinding.Mode = BindingMode.TwoWay;
            HasChangedBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            HasChangedBinding.BindsDirectlyToSource = true;

            BackgroundBrushBinding = new Binding(string.Format("Properties[{0}].BackgroundColour", ColumnIdx));
            ForegroundBrushBinding = new Binding(string.Format("Properties[{0}].ForeColour", ColumnIdx));

            BorderBrushBinding = new Binding(string.Format("Properties[{0}].BorderColour", ColumnIdx));
            BorderBrushBinding.Mode = BindingMode.TwoWay;
            BorderBrushBinding.BindsDirectlyToSource = true;

            AlignmentBinding = new Binding(string.Format("Properties[{0}].Alignment", ColumnIdx));           
        }

        #endregion

        private ListView GetListBoxTemplate()
        {
            /* So loading some xaml directly from a resource (e.g. App.xaml) would cause all nodes cell in the same DataContext (i.e. a Record) to referece the same xaml!!!
             * This means that although it looked like each one should load its own AutoScrollingListView, they actually all used the same one, leading to them all point to the same Property binding.
             * To fix this I just wrote some ListView xaml, copied it into here and force each cell to get its own copy.
             */
            //THIS DOES NOT WORK, BUT LEFT AS A REFERENCE
            //var listbox = (AutoScrollingListView)Application.Current.FindResource("ListboxToolTipTemplate");
            //listbox.SetBinding(ItemsControl.ItemsSourceProperty, ToolTipBinding);
            //return listbox;

            var listview = (ListView)XamlReader.Parse("<ListView xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Height=\"auto\" MaxHeight=\"500\" MaxWidth=\"250\" BorderBrush=\"Transparent\" BorderThickness=\"0\" ><ListBox.ItemTemplate><DataTemplate><Grid ToolTip=\"{ Binding Value}\"><Grid.RowDefinitions><RowDefinition Height=\"Auto\" /><RowDefinition Height=\"Auto\" /></Grid.RowDefinitions><TextBlock Grid.Row=\"0\" Text=\"{ Binding Header}\" FontSize=\"11\" MaxWidth=\"240\" TextWrapping=\"Wrap\" FontWeight=\"Bold\" /><TextBlock Grid.Row=\"1\" Text=\"{ Binding Value}\" FontSize=\"12\" MaxWidth=\"240\" TextWrapping=\"Wrap\" /></Grid></DataTemplate></ListBox.ItemTemplate></ListView>");
            listview.SetBinding(ItemsControl.ItemsSourceProperty, ToolTipBinding);

            return listview;


        }

        private void ToolTipHandler(object sender, ToolTipEventArgs e)
        {
            if(sender.GetType() == typeof(TextBox))
            {
                e.Handled = !(((TextBox)sender).DataContext as Record).Properties[ColumnIdx].HasComment;
            }
            else if(sender.GetType() == typeof(TextBlock))
            {
                e.Handled = !(((TextBlock)sender).DataContext as Record).Properties[ColumnIdx].HasComment;
            }
            else if(sender.GetType() == typeof(TextBox))
            {
                e.Handled = !(((EditableTextBlock)sender).DataContext as Record).Properties[ColumnIdx].HasComment;
            }
        }
    }
}