using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Exceedra.Controls.DynamicGrid.Controls;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;
using ComboBox = System.Windows.Controls.ComboBox;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Label = System.Windows.Controls.Label;
using TextBox = System.Windows.Controls.TextBox;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Shapes;
using Exceedra.Buttons;
using Exceedra.Common;
using Model;
using Exceedra.DynamicGrid.Models;
using ButtonBase = System.Windows.Controls.Primitives.ButtonBase;

namespace Exceedra.Controls.DynamicGrid.Models
{
   public static class GridColumns
    {

       public static DataGridTemplateColumn GetTextColumn(Binding bind, string name, RoutedEventHandler command, Binding tag = null, Binding borderBrush = null, Binding backgroundBrush = null, Binding forecolorBrush = null)
       {
           // Create The Column
           DataGridTemplateColumn tempColumn = new DataGridTemplateColumn();
           tempColumn.Header = name;
           // tempColumn.CellStyle = Application.Current.FindResource("exceedraStdCell") as Style;

           // Create the TextBlock
           FrameworkElementFactory textFactory = new FrameworkElementFactory(typeof(TextBox));
           textFactory.SetBinding(TextBox.TextProperty, bind);

           if (backgroundBrush != null)
           {
               textFactory.SetBinding(Border.BackgroundProperty, backgroundBrush);
           }

           if (forecolorBrush != null)
           {
               textFactory.SetBinding(TextBlock.ForegroundProperty, forecolorBrush);
           }

           if(tag!=null)
                textFactory.SetBinding(TextBox.TagProperty, tag);

           //textFactory.SetValue(TextBox.StyleProperty, Application.Current.FindResource("exceedraStdCell") as Style);  

           if (bind.Mode == BindingMode.TwoWay)
           {
               //only add a handler for editable text
               textFactory.AddHandler(TextBox.LostFocusEvent, command);
			    textFactory.AddHandler(TextBox.MouseDoubleClickEvent,  new  MouseButtonEventHandler(Target));                
           }
           DataTemplate textTemplate = new DataTemplate();
           textTemplate.VisualTree = textFactory;
           
 
            
           tempColumn.CellTemplate = textTemplate; 
           
           return tempColumn;
       }
       
       private static void Target(object sender, MouseButtonEventArgs mouseButtonEventArgs)
       {
           var textBox = sender as TextBox;
           if (textBox != null)
               textBox.SelectAll();
       }


       public static DataGridTemplateColumn GetHeaderOperationColumn(Binding bind, string name,
           RoutedEventHandler command, Binding tag = null, Binding visible = null)//Binding bind, string name, RoutedEventHandler command, RadComboBox comboBox, Binding tag = null, Binding visible = null)
       {
           // Create The Column
           DataGridTemplateColumn tempColumn = new DataGridTemplateColumn();
           tempColumn.Header = name;
    
           // Create the TextBlock
           FrameworkElementFactory operationFactory = new FrameworkElementFactory(typeof(HeaderOperationControl));
           operationFactory.SetBinding(HeaderOperationControl.TextProperty, bind);
 
           if (tag != null)
               operationFactory.SetBinding(HeaderOperationControl.TagDataProperty, tag);

           if (visible != null)
               operationFactory.SetBinding(HeaderOperationControl.VisibilityProperty, visible);
 
           //only add a handler for editable text
           operationFactory.AddHandler(HeaderOperationControl.ClickEvent, command);
           //}

           DataTemplate textTemplate = new DataTemplate();
           textTemplate.VisualTree = operationFactory;



           tempColumn.CellTemplate = textTemplate;

           return tempColumn;
       }


       public static DataGridTemplateColumn GetComboColumn(Binding selectedBinding, Binding items, string name, SelectionChangedEventHandler command, string columnCode, string displayMemberPath = "", Binding visible = null)
       {
           selectedBinding.Mode = BindingMode.TwoWay;
           // Create The Column
           DataGridTemplateColumn tempColumn = new DataGridTemplateColumn();
           tempColumn.Header = name;
 
           // Create the ComboBox
           Binding comboBind = items;
           comboBind.Mode = BindingMode.OneWay;

           FrameworkElementFactory comboFactory = new FrameworkElementFactory(typeof(ComboBox));
           comboFactory.SetValue(ComboBox.IsTextSearchEnabledProperty, true);
           comboFactory.SetValue(ComboBox.DisplayMemberPathProperty, displayMemberPath);
           comboFactory.SetValue(ComboBox.SelectedValuePathProperty, "Item_Idx");
           comboFactory.SetValue(ComboBox.TagProperty, columnCode);
           comboFactory.SetBinding(ComboBox.ItemsSourceProperty, items);
           comboFactory.SetBinding(ComboBox.SelectedItemProperty, selectedBinding);
           comboFactory.AddHandler(ComboBox.SelectionChangedEvent, command);
           if (visible != null) comboFactory.SetBinding(HeaderOperationControl.VisibilityProperty, visible);
 
           DataTemplate comboTemplate = new DataTemplate();
           comboTemplate.VisualTree = comboFactory;

           // Set the Templates to the Column
           tempColumn.CellTemplate = comboTemplate;
           // tempColumn.CellEditingTemplate = comboTemplate;

           return tempColumn;
       }

       //public static DataGridTemplateColumn GetComboColumn2Way(Binding bind, List<Option> items, string name, RoutedEventHandler command)
       //{
       //    // Create The Column
       //    DataGridTemplateColumn tempColumn = new DataGridTemplateColumn();
       //    tempColumn.Header = name;
          
       //    // Create the ComboBox
       //    Binding comboBind = bind;
       //    comboBind.Mode = BindingMode.TwoWay;

       //    FrameworkElementFactory comboFactory = new FrameworkElementFactory(typeof(ComboBox));
       //    comboFactory.SetValue(ComboBox.IsTextSearchEnabledProperty, true);
       //    comboFactory.SetValue(ComboBox.ItemsSourceProperty, items);
       //    comboFactory.SetBinding(ComboBox.SelectedItemProperty, comboBind);

       //    comboFactory.AddHandler(ComboBox.SelectionChangedEvent, command);
           
       //    DataTemplate comboTemplate = new DataTemplate();
       //    comboTemplate.VisualTree = comboFactory;

       //    // Set the Templates to the Column
       //    tempColumn.CellTemplate = comboTemplate;
           
       //    return tempColumn;
       //}

               
        public static ObservableCollection<Option> GetFromXML(string attr, string datasource)
        {

            var options = new List<Option>();

            if (attr != null)
            {
                //we have some Data input, get dataset from WS 
                //var argument = XElement.Parse(attr.Replace("&gt;", ">").Replace("&lt;", "<"));
                // Expected XML Input:
                //<GetItems>
                //  <User_Idx>?</User_Idx>
                //  <MenuItem_Idx>?</MenuItem_Idx>
                //  <SelectedItem_Idx>?</SelectedItem_Idx>
                //</GetItems> 

                var res = Model.DataAccess.@WebServiceProxy.Call(datasource, attr.ToString());
                //Expected XML Output:
                //<Results>
                //  <Item>
                //    <Item_Idx>?</Item_Idx>
                //    <Item_Name>?</Item_Name>
                //    <IsSelected>0 or 1</IsSelected>
                //  </Item>
                //</Results>
                options.AddRange(res.Elements("Item").Select(att => new Option
                {
                    Item_Idx = att.Element("Item_Idx").Value,
                    Item_Name = att.Element("Item_Name").Value,
                    IsSelected = (att.Element("IsSelected").MaybeValue() == "1" ? true : false)
                }));

                options.AddRange(res.Elements("Value").Select(att => new Option
                {
                    Item_Idx = att.Element("Item_Idx").Value,
                    Item_Name = att.Element("Item_Name").Value,
                    IsSelected = (att.Element("IsSelected").MaybeValue() == "1" ? true : false)
                }));

                options.AddRange(res.Elements("Dropdown").Select(att => new Option
                {
                    Item_Idx = att.Element("Item_Idx").Value,
                    Item_Name = att.Element("Item_Name").Value,
                    IsSelected = (att.Element("IsSelected").MaybeValue() == "1" ? true : false)
                }));
            }
            else
            {
                //check for static data
                return null;

            }


            return new ObservableCollection<Option>(options);
        }


       public static DataGridTemplateColumn GetPsuedoLinkColumn(Binding bind, string name, string path, RoutedEventHandler command, string columnCode)
       {
           // Create The Column
           DataGridTemplateColumn tempColumn = new DataGridTemplateColumn();
           tempColumn.Header = name;
           // tempColumn.CellStyle = Application.Current.FindResource("exceedraStdCell") as Style;
         
           // Create the Button
           Binding binding = bind;
           binding.Mode = BindingMode.OneWay;

           FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
           buttonFactory.SetValue(Button.ContentProperty,bind);
           buttonFactory.SetValue(Button.HorizontalAlignmentProperty, HorizontalAlignment.Left);
           buttonFactory.AddHandler(Button.ClickEvent,  command);
           buttonFactory.SetValue(Button.TagProperty, columnCode);
           buttonFactory.SetValue(Button.StyleProperty, Application.Current.FindResource("link") as Style);  

           DataTemplate buttonTemplate = new DataTemplate();
           buttonTemplate.VisualTree = buttonFactory;           

           // Set the Templates to the Column
        
           tempColumn.CellTemplate = buttonTemplate;

           return tempColumn;
       }

       public static DataGridTemplateColumn GetCheckBoxColumn(Binding bind, RoutedEventHandler command)
       {
           // Create The Column
           DataGridTemplateColumn tempColumn = new DataGridTemplateColumn();
           // tempColumn.CellStyle = Application.Current.FindResource("exceedraStdCell") as Style;
           // Create the Button
           Binding binding = bind;
         
           binding.Mode = BindingMode.TwoWay;
           binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
           binding.BindsDirectlyToSource = true;


           FrameworkElementFactory checkFactory = new FrameworkElementFactory(typeof(CheckBox));
           checkFactory.SetValue(CheckBox.IsCheckedProperty, bind);
           checkFactory.AddHandler(CheckBox.CheckedEvent, command);
           checkFactory.AddHandler(CheckBox.UncheckedEvent, command);
          // checkFactory.SetValue(Button.StyleProperty, Application.Current.FindResource("exceedraStdCell") as Style);  
            
           DataTemplate checkTemplate = new DataTemplate();
           checkTemplate.VisualTree = checkFactory;

           // Set the Templates to the Column

           tempColumn.CellTemplate = checkTemplate;

           return tempColumn;
       }

       public static DataGridTemplateColumn GetDetailsViewButton(Binding bind, Binding visibilityBinding, Binding isExpandedBinding, RoutedEventHandler command)
       {
           DataTrigger expandedTrigger = new DataTrigger();
           expandedTrigger.Binding = isExpandedBinding;
           expandedTrigger.Value = true;
           Setter expanded = new Setter(Button.ContentProperty, "-");
           expandedTrigger.Setters.Add(expanded);

           DataTrigger closedTrigger = new DataTrigger();
           closedTrigger.Binding = isExpandedBinding;
           closedTrigger.Value = false;
           Setter closed = new Setter(Button.ContentProperty, "+");
           
           closedTrigger.Setters.Add(closed);

           Style s = new Style();
           s.Triggers.Add(expandedTrigger);
           s.Triggers.Add(closedTrigger);

           // Create The Column
           DataGridTemplateColumn tempColumn = new DataGridTemplateColumn();

           // Create the Button
           Binding binding = bind; 
           binding.BindsDirectlyToSource = true;
            
           FrameworkElementFactory btnkFactory = new FrameworkElementFactory(typeof(Button));
         
           //btnkFactory.SetValue(Button.ContentProperty, "+");           
           btnkFactory.SetBinding(Button.BackgroundProperty, binding);
           btnkFactory.SetBinding(Button.VisibilityProperty, visibilityBinding);
           btnkFactory.SetValue(Button.BorderBrushProperty, new SolidColorBrush(Colors.Transparent));
           btnkFactory.SetValue(Button.FontWeightProperty, FontWeights.Bold);
           btnkFactory.SetValue(Button.FontSizeProperty, 12D );
           btnkFactory.SetValue(Button.StyleProperty, s);

           btnkFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
           btnkFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, System.Windows.VerticalAlignment.Stretch);

           btnkFactory.AddHandler(CheckBox.ClickEvent, command);

           FrameworkElementFactory grid = new FrameworkElementFactory(typeof(Grid));
           grid.AppendChild(btnkFactory); 
           grid.SetBinding(Grid.BackgroundProperty, binding);

           //btnkFactory.SetValue(Button.TemplateProperty, circleButtonTemplate);

           DataTemplate checkTemplate = new DataTemplate();
           checkTemplate.VisualTree = grid;
           
           // Set the Templates to the Column
            
           tempColumn.CellTemplate = checkTemplate;

           return tempColumn;
       }

        public static DataGridTemplateColumn GetSelectRowButton(RoutedEventHandler command)
        {
            // Create The Column
            DataGridTemplateColumn tempColumn = new DataGridTemplateColumn();


            DataTemplate deleteTemplate = new DataTemplate();

            FrameworkElementFactory deleteButton = new FrameworkElementFactory(typeof(ArrowButton));
            deleteButton.AddHandler(ButtonBase.ClickEvent, command);
            //deleteButton.SetValue(FrameworkElement.HeightProperty, Convert.ToDouble(12));
            //deleteButton.SetValue(FrameworkElement.WidthProperty, Convert.ToDouble(12));

            deleteTemplate.VisualTree = deleteButton;

            tempColumn.CellTemplate = deleteTemplate;

            return tempColumn;
        }


        public static DataGridTemplateColumn GetDeleteButton(Binding bind, RoutedEventHandler command)
       {
           // Create the circle
           FrameworkElementFactory circle = new FrameworkElementFactory(typeof(Ellipse));
           circle.SetValue(Ellipse.FillProperty, Brushes.White);
           circle.SetValue(Ellipse.StrokeProperty, Brushes.Red);
           circle.SetValue(Ellipse.HeightProperty, Convert.ToDouble(20));
           circle.SetValue(Ellipse.WidthProperty, Convert.ToDouble(20));
           circle.SetValue(Ellipse.StrokeThicknessProperty, 1D);

           // Create the ContentPresenter to show the Button.Content
           FrameworkElementFactory presenter = new FrameworkElementFactory(typeof(ContentPresenter));
           presenter.SetValue(ContentPresenter.ContentProperty, new TemplateBindingExtension(Button.ContentProperty));
           presenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, System.Windows.HorizontalAlignment.Center);
           presenter.SetValue(ContentPresenter.VerticalAlignmentProperty, System.Windows.VerticalAlignment.Center);

           Binding binding = bind;
           binding.BindsDirectlyToSource = true;

           // Create the Grid to hold both of the elements
           FrameworkElementFactory grid = new FrameworkElementFactory(typeof(Grid));
           grid.AppendChild(circle);
           grid.AppendChild(presenter);
           grid.SetBinding(Grid.BackgroundProperty, binding);

           ControlTemplate circleButtonTemplate = new ControlTemplate(typeof(Button));
           // Set the Grid as the ControlTemplate.VisualTree
           circleButtonTemplate.VisualTree = grid;

           
            
           // Create The Column
           DataGridTemplateColumn tempColumn = new DataGridTemplateColumn();
           // tempColumn.CellStyle = Application.Current.FindResource("exceedraStdCell") as Style;
           // Create the Button 
           FrameworkElementFactory btnkFactory = new FrameworkElementFactory(typeof(Button));

           btnkFactory.SetValue(Button.ContentProperty, "X");
           
           btnkFactory.SetValue(Button.BorderBrushProperty, new SolidColorBrush(Colors.Transparent));
           btnkFactory.SetValue(Button.FontWeightProperty, FontWeights.Bold);
           btnkFactory.SetValue(Button.FontSizeProperty, 14D);
         //  btnkFactory.SetValue(ContentPresenter.RenderTransformProperty, new RotateTransform() { Angle = -45, CenterX = 10, CenterY = 10 });
           //btnkFactory.SetValue(Button.MarginProperty, new Thickness(0D, -5D, 0D, 0D));
           //btnkFactory.SetValue(Button.PaddingProperty, new Thickness(0D, -5D, 0D, 0D));
           //btnkFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, System.Windows.HorizontalAlignment.Stretch);
           //btnkFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, System.Windows.VerticalAlignment.Top);

           btnkFactory.AddHandler(CheckBox.ClickEvent, command);



            btnkFactory.SetValue(Button.TemplateProperty, circleButtonTemplate);

           DataTemplate checkTemplate = new DataTemplate();
           checkTemplate.VisualTree = btnkFactory;

           // Set the Templates to the Column

           tempColumn.CellTemplate = checkTemplate;

           return tempColumn;
       }

       public static DataGridTemplateColumn GetBlankColumn()
       {
           // Create The Column
           DataGridTemplateColumn tempColumn = new DataGridTemplateColumn();
 
           FrameworkElementFactory textFactory = new FrameworkElementFactory(typeof(Label));

          
           DataTemplate textTemplate = new DataTemplate();
           textTemplate.VisualTree = textFactory;
            
           tempColumn.CellTemplate = textTemplate;

           return tempColumn;
       }


       public static DataGridTemplateColumn GetLabelColumn(Binding controlBinding, string name, Binding borderBrush = null, Binding tag = null, Binding backgroundBrush = null, Binding forecolorBrush = null, Binding bg =null)
       {
           // Create The Column
           DataGridTemplateColumn tempColumn = new DataGridTemplateColumn();
           tempColumn.Header = name;
           // tempColumn.CellStyle = Application.Current.FindResource("exceedraStdCell") as Style;


           FrameworkElementFactory spOuterFactory =
                    new FrameworkElementFactory(typeof(Border));

           if (borderBrush != null)
           { 
               spOuterFactory.SetBinding(Border.BorderBrushProperty, borderBrush);
               spOuterFactory.SetValue(Border.BorderThicknessProperty, new Thickness(4, 1, 1, 1));               
           }        


           // Create the TextBlock
           FrameworkElementFactory textFactory = new FrameworkElementFactory(typeof(TextBlock));
           textFactory.SetBinding(TextBlock.TextProperty, controlBinding);
           textFactory.SetValue(TextBlock.PaddingProperty, new Thickness(2, 0, 0, 0));

           if (backgroundBrush != null)
           {
               spOuterFactory.SetBinding(Border.BackgroundProperty, backgroundBrush);              
           }


           if (forecolorBrush != null)
           {
               textFactory.SetBinding(TextBlock.ForegroundProperty, forecolorBrush);                
           }

           if (tag != null)
               textFactory.SetBinding(TextBlock.TagProperty, tag);
            
           spOuterFactory.AppendChild(textFactory);

           FrameworkElementFactory grid = new FrameworkElementFactory(typeof(Grid));
           grid.AppendChild(spOuterFactory);

           if (bg != null)
           {
               Binding binding = bg;
               binding.BindsDirectlyToSource = true;
               grid.SetBinding(Grid.BackgroundProperty, binding);
           }
         
           DataTemplate textTemplate = new DataTemplate();
           textTemplate.VisualTree = grid;
            
           tempColumn.CellTemplate = textTemplate;

           return tempColumn;
       }

       public static DataGridTemplateColumn GetColumnDate(Binding bind)
       {
           DataGridTemplateColumn tempColumn = new DataGridTemplateColumn();

           Binding binding = bind;

           binding.Mode = BindingMode.TwoWay;
           binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
           binding.BindsDirectlyToSource = true;

           FrameworkElementFactory checkFactory = new FrameworkElementFactory(typeof(DatePicker));
           checkFactory.SetBinding(DatePicker.SelectedDateProperty, bind);

           DataTemplate datePickerTemplate = new DataTemplate();
           datePickerTemplate.VisualTree = checkFactory;

           // Set the Templates to the Column

           tempColumn.CellTemplate = datePickerTemplate;

           return tempColumn;
       }

       public static DataGridTemplateColumn GetFileLocation(Binding bind)
       {
           DataGridTemplateColumn tempColumn = new DataGridTemplateColumn();

           Binding binding = bind;

           binding.Mode = BindingMode.TwoWay;
           binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
           binding.BindsDirectlyToSource = true;

           FrameworkElementFactory checkFactory = new FrameworkElementFactory(typeof(FileEntry));
           checkFactory.SetBinding(FileEntry.TextProperty, bind);

           DataTemplate fileLocationTemplate = new DataTemplate();
           fileLocationTemplate.VisualTree = checkFactory;

           tempColumn.CellTemplate = fileLocationTemplate;

           return tempColumn;
       }

    }


   //public static class TelerikGridColumns
   //{

   //    public static GridViewDataColumn GetTextColumn(Binding bind, string name, RoutedEventHandler command, Binding tag = null, Binding borderBrush = null, Binding backgroundBrush = null, Binding forecolorBrush = null)
   //    {
   //        // Create The Column
   //        var tempColumn = new GridViewDataColumn();
   //        tempColumn.Header = name;
   //        // tempColumn.CellStyle = Application.Current.FindResource("exceedraStdCell") as Style;

   //        // Create the TextBlock
   //        FrameworkElementFactory textFactory = new FrameworkElementFactory(typeof(TextBox));
   //        textFactory.SetBinding(TextBox.TextProperty, bind);

   //        if (backgroundBrush != null)
   //        {
   //            textFactory.SetBinding(Border.BackgroundProperty, backgroundBrush);
   //        }

   //        if (forecolorBrush != null)
   //        {
   //            textFactory.SetBinding(TextBlock.ForegroundProperty, forecolorBrush);
   //        }

   //        if (tag != null)
   //            textFactory.SetBinding(TextBox.TagProperty, tag);

   //        //textFactory.SetValue(TextBox.StyleProperty, Application.Current.FindResource("exceedraStdCell") as Style);  

   //        if (bind.Mode == BindingMode.TwoWay)
   //        {
   //            //only add a handler for editable text
   //            textFactory.AddHandler(TextBox.LostFocusEvent, command);
   //        }
   //        DataTemplate textTemplate = new DataTemplate();
   //        textTemplate.VisualTree = textFactory;



   //        tempColumn.CellTemplate = textTemplate;

   //        return tempColumn;
   //    }

   //    public static GridViewDataColumn GetHeaderOperationColumn(Binding bind, string name, RoutedEventHandler command, Binding tag = null, Binding visible = null)
   //    {
   //        // Create The Column
   //        var tempColumn = new GridViewDataColumn();
   //        tempColumn.Header = name;
   //        // tempColumn.CellStyle = Application.Current.FindResource("exceedraStdCell") as Style;

   //        // Create the TextBlock
   //        FrameworkElementFactory operationFactory = new FrameworkElementFactory(typeof(HeaderOperationControl));
   //        operationFactory.SetBinding(HeaderOperationControl.TextProperty, bind);

   //        if (tag != null)
   //            operationFactory.SetBinding(HeaderOperationControl.TagDataProperty, tag);

   //        if (visible != null)
   //            operationFactory.SetBinding(HeaderOperationControl.VisibilityProperty, visible);

   //        //textFactory.SetValue(TextBox.StyleProperty, Application.Current.FindResource("exceedraStdCell") as Style);  

   //        //if (controlBinding.Mode == BindingMode.TwoWay)
   //        //{
   //        //only add a handler for editable text
   //        operationFactory.AddHandler(HeaderOperationControl.ClickEvent, command);
   //        //}

   //        DataTemplate textTemplate = new DataTemplate();
   //        textTemplate.VisualTree = operationFactory;



   //        tempColumn.CellTemplate = textTemplate;

   //        return tempColumn;
   //    }


   //    public static GridViewDataColumn GetComboColumn(Binding selectedBinding, Binding items, string name, SelectionChangedEventHandler command, string columnCode)
   //    {
   //        selectedBinding.Mode = BindingMode.TwoWay;
   //        // Create The Column
   //        var tempColumn = new GridViewDataColumn();
   //        tempColumn.Header = name;

   //        // tempColumn.CellStyle = Application.Current.FindResource("exceedraStdCell") as Style;
   //        // controlBinding.Mode = BindingMode.OneWay;

   //        //// Create the TextBlock
   //        //FrameworkElementFactory textFactory = new FrameworkElementFactory(typeof(TextBlock));
   //        //textFactory.SetBinding(TextBlock.TextProperty, controlBinding);
   //        //DataTemplate textTemplate = new DataTemplate();
   //        //textTemplate.VisualTree = textFactory;

   //        // Create the ComboBox
   //        Binding comboBind = items;
   //        comboBind.Mode = BindingMode.OneWay;

   //        FrameworkElementFactory comboFactory = new FrameworkElementFactory(typeof(ComboBox));
   //        comboFactory.SetValue(ComboBox.IsTextSearchEnabledProperty, true);
   //        comboFactory.SetValue(ComboBox.DisplayMemberPathProperty, "Item_Name");
   //        comboFactory.SetValue(ComboBox.SelectedValuePathProperty, "Item_Idx");
   //        comboFactory.SetValue(ComboBox.TagProperty, columnCode);
   //        comboFactory.SetBinding(ComboBox.ItemsSourceProperty, items);
   //        comboFactory.SetBinding(ComboBox.SelectedItemProperty, selectedBinding);
   //        comboFactory.AddHandler(ComboBox.SelectionChangedEvent, command);

   //        // comboFactory.SetValue(ComboBox.StyleProperty, Application.Current.FindResource("exceedraStdCell") as Style);  

   //        DataTemplate comboTemplate = new DataTemplate();
   //        comboTemplate.VisualTree = comboFactory;

   //        // Set the Templates to the Column
   //        tempColumn.CellTemplate = comboTemplate;
   //        // tempColumn.CellEditingTemplate = comboTemplate;

   //        return tempColumn;
   //    }

  

   //    public static ObservableCollection<Option> GetFromXML(string attr, string datasource)
   //    {

   //        var options = new List<Option>();

   //        if (attr != null)
   //        {
   //            //we have some Data input, get dataset from WS 
   //            //var argument = XElement.Parse(attr.Replace("&gt;", ">").Replace("&lt;", "<"));
   //            // Expected XML Input:
   //            //<GetItems>
   //            //  <User_Idx>?</User_Idx>
   //            //  <MenuItem_Idx>?</MenuItem_Idx>
   //            //  <SelectedItem_Idx>?</SelectedItem_Idx>
   //            //</GetItems> 

   //            var res = Model.DataAccess.@WebServiceProxy.Call(datasource, attr.ToString());
   //            //Expected XML Output:
   //            //<Results>
   //            //  <Item>
   //            //    <Item_Idx>?</Item_Idx>
   //            //    <Item_Name>?</Item_Name>
   //            //    <IsSelected>0 or 1</IsSelected>
   //            //  </Item>
   //            //</Results>
   //            options.AddRange(res.Elements("Item").Select(att => new Option
   //            {
   //                Item_Idx = att.Element("Item_Idx").Value,
   //                Item_Name = att.Element("Item_Name").Value,
   //                IsSelected = (att.Element("IsSelected").MaybeValue() == "1" ? true : false)
   //            }));

   //            options.AddRange(res.Elements("Value").Select(att => new Option
   //            {
   //                Item_Idx = att.Element("Item_Idx").Value,
   //                Item_Name = att.Element("Item_Name").Value,
   //                IsSelected = (att.Element("IsSelected").MaybeValue() == "1" ? true : false)
   //            }));

   //            options.AddRange(res.Elements("Dropdown").Select(att => new Option
   //            {
   //                Item_Idx = att.Element("Item_Idx").Value,
   //                Item_Name = att.Element("Item_Name").Value,
   //                IsSelected = (att.Element("IsSelected").MaybeValue() == "1" ? true : false)
   //            }));
   //        }
   //        else
   //        {
   //            //check for static data
   //            return null;

   //        }


   //        return new ObservableCollection<Option>(options);
   //    }


   //    public static GridViewDataColumn GetPsuedoLinkColumn(Binding bind, string name, string path, RoutedEventHandler command, string columnCode)
   //    {
   //        // Create The Column
   //        GridViewDataColumn tempColumn = new GridViewDataColumn();
   //        tempColumn.Header = name;
   //        // tempColumn.CellStyle = Application.Current.FindResource("exceedraStdCell") as Style;

   //        // Create the Button
   //        Binding binding = bind;
   //        binding.Mode = BindingMode.OneWay;

   //        FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
   //        buttonFactory.SetValue(Button.ContentProperty, bind);
   //        buttonFactory.SetValue(Button.HorizontalAlignmentProperty, HorizontalAlignment.Left);
   //        buttonFactory.AddHandler(Button.ClickEvent, command);
   //        buttonFactory.SetValue(Button.TagProperty, columnCode);
   //        buttonFactory.SetValue(Button.StyleProperty, Application.Current.FindResource("link") as Style);

   //        DataTemplate buttonTemplate = new DataTemplate();
   //        buttonTemplate.VisualTree = buttonFactory;

   //        // Set the Templates to the Column

   //        tempColumn.CellTemplate = buttonTemplate;

   //        return tempColumn;
   //    }

   //    public static GridViewDataColumn GetCheckBoxColumn(Binding bind, RoutedEventHandler command)
   //    {
   //        // Create The Column
   //        GridViewDataColumn tempColumn = new GridViewDataColumn();
   //        // tempColumn.CellStyle = Application.Current.FindResource("exceedraStdCell") as Style;
   //        // Create the Button
   //        Binding binding = bind;

   //        binding.Mode = BindingMode.TwoWay;
   //        binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
   //        binding.BindsDirectlyToSource = true;


   //        FrameworkElementFactory checkFactory = new FrameworkElementFactory(typeof(CheckBox));
   //        checkFactory.SetValue(CheckBox.IsCheckedProperty, bind);
   //        checkFactory.AddHandler(CheckBox.CheckedEvent, command);
   //        checkFactory.AddHandler(CheckBox.UncheckedEvent, command);
   //        // checkFactory.SetValue(Button.StyleProperty, Application.Current.FindResource("exceedraStdCell") as Style);  

   //        DataTemplate checkTemplate = new DataTemplate();
   //        checkTemplate.VisualTree = checkFactory;

   //        // Set the Templates to the Column

   //        tempColumn.CellTemplate = checkTemplate;

   //        return tempColumn;
   //    }


   //    public static GridViewDataColumn GetBlankColumn()
   //    {
   //        // Create The Column
   //        GridViewDataColumn tempColumn = new GridViewDataColumn();

   //        FrameworkElementFactory textFactory = new FrameworkElementFactory(typeof(Label));


   //        DataTemplate textTemplate = new DataTemplate();
   //        textTemplate.VisualTree = textFactory;

   //        tempColumn.CellTemplate = textTemplate;

   //        return tempColumn;
   //    }


   //    public static GridViewDataColumn GetLabelColumn(Binding controlBinding, string name, Binding borderBrush = null, Binding tag = null, Binding backgroundBrush = null, Binding forecolorBrush = null)
   //    {
   //        // Create The Column
   //        GridViewDataColumn tempColumn = new GridViewDataColumn();
   //        tempColumn.Header = name;
   //        // tempColumn.CellStyle = Application.Current.FindResource("exceedraStdCell") as Style;


   //        FrameworkElementFactory spOuterFactory =
   //                 new FrameworkElementFactory(typeof(Border));

   //        if (borderBrush != null)
   //        {
   //            spOuterFactory.SetBinding(Border.BorderBrushProperty, borderBrush);
   //            spOuterFactory.SetValue(Border.BorderThicknessProperty, new Thickness(4, 1, 1, 1));
   //        }


   //        // Create the TextBlock
   //        FrameworkElementFactory textFactory = new FrameworkElementFactory(typeof(TextBlock));
   //        textFactory.SetBinding(TextBlock.TextProperty, controlBinding);
   //        textFactory.SetValue(TextBlock.PaddingProperty, new Thickness(2, 0, 0, 0));

   //        if (backgroundBrush != null)
   //        {
   //            spOuterFactory.SetBinding(Border.BackgroundProperty, backgroundBrush);
   //        }


   //        if (forecolorBrush != null)
   //        {
   //            textFactory.SetBinding(TextBlock.ForegroundProperty, forecolorBrush);
   //        }

   //        if (tag != null)
   //            textFactory.SetBinding(TextBlock.TagProperty, tag);

   //        spOuterFactory.AppendChild(textFactory);

   //        DataTemplate textTemplate = new DataTemplate();
   //        textTemplate.VisualTree = spOuterFactory;


   //        tempColumn.CellTemplate = textTemplate;

   //        return tempColumn;
   //    }

   //    public static GridViewDataColumn GetColumnDate(Binding bind)
   //    {
   //        GridViewDataColumn tempColumn = new GridViewDataColumn();

   //        Binding binding = bind;

   //        binding.Mode = BindingMode.TwoWay;
   //        binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
   //        binding.BindsDirectlyToSource = true;

   //        FrameworkElementFactory checkFactory = new FrameworkElementFactory(typeof(DatePicker));
   //        checkFactory.SetBinding(DatePicker.SelectedDateProperty, bind);

   //        DataTemplate datePickerTemplate = new DataTemplate();
   //        datePickerTemplate.VisualTree = checkFactory;

   //        // Set the Templates to the Column

   //        tempColumn.CellTemplate = datePickerTemplate;

   //        return tempColumn;
   //    }

   //    public static GridViewDataColumn GetFileLocation(Binding bind)
   //    {
   //        GridViewDataColumn tempColumn = new GridViewDataColumn();

   //        Binding binding = bind;

   //        binding.Mode = BindingMode.TwoWay;
   //        binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
   //        binding.BindsDirectlyToSource = true;

   //        FrameworkElementFactory checkFactory = new FrameworkElementFactory(typeof(FileEntry));
   //        checkFactory.SetBinding(FileEntry.TextProperty, bind);

   //        DataTemplate fileLocationTemplate = new DataTemplate();
   //        fileLocationTemplate.VisualTree = checkFactory;

   //        tempColumn.CellTemplate = fileLocationTemplate;

   //        return tempColumn;
   //    }

   //}

}
