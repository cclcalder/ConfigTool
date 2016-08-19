using System;
using System.Collections.Generic;
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
using Exceedra.Controls.DynamicRow.Controls;
using Exceedra.Controls.DynamicRow.Models;
using Exceedra.Controls.DynamicRow.ViewModels;
 

namespace Exceedra.Controls.DynamicGrid.Controls
{
    /// <summary>
    /// Interaction logic for SingleRow.xaml
    /// </summary>
    public partial class SingleRow : UserControl
    {
        private RowProperty _viewModel;
        public SingleRow()
        {
            InitializeComponent();

            //cb.Visibility = Visibility.Hidden;
            //cmb.Visibility = Visibility.Hidden;
            //txt.Visibility = Visibility.Hidden;
            //ccb.Visibility = Visibility.Hidden;
            //hyp.Visibility = Visibility.Hidden;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            //if (CurrentProperty.ControlType.ToLower() == "multiselectdropdown")
            //    RadComboBox.SelectedItem = CurrentProperty.Values.First();
        }


        public static readonly RoutedEvent SelectedChangeRowEvent =
       EventManager.RegisterRoutedEvent("SelectedChangeRow", RoutingStrategy.Bubble,
       typeof(RoutedEventHandler), typeof(RowProperty));

        // .NET wrapper
        public event RoutedEventHandler SelectedChangeRow
        {
            add { AddHandler(SelectedChangeRowEvent, value); }
            remove { RemoveHandler(SelectedChangeRowEvent, value); }
        }
      
        public RowProperty CurrentProperty
        {
            get { 
                return (RowProperty)GetValue(MyPropertyProperty); 
            }
            set { 
                SetValue(MyPropertyProperty, value);
                DataContext =  _viewModel = CurrentProperty;
                
            }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("CurrentProperty", typeof(RowProperty), typeof(SingleRow), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = OnDataChanged 
            });

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        void RaiseTapEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(SelectedChangeRowEvent);
            RaiseEvent(newEventArgs);
        }
        private void Ccb_OnSelected(object sender, RoutedEventArgs e)
        {
            RaiseTapEvent();
        }

        //private void Txt_OnTextChanged(object sender, TextChangedEventArgs e)
        //{
        //    TextBox cb = (TextBox)sender;
        //    var obj = ((FrameworkElement)sender).DataContext as RowProperty;

        //    obj.Value = cb.Text;
        //}

        private void UIElement_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox cb = (TextBox)sender;
                var obj = ((FrameworkElement)sender).DataContext as RowProperty;

                obj.Value = cb.Text;

                var drg = FindVisualParent<DynamicRowControl>(this);

                if (drg != null)
                {
                    var items = drg.ItemDataSource as RowViewModel;
                    items.CalulateRecordColumns();
                }
            }
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox cb = (TextBox)sender;
            var obj = ((FrameworkElement)sender).DataContext as RowProperty;

            obj.Value = cb.Text;

            var drg = FindVisualParent<DynamicRowControl>(this);

            if (drg != null)
            {
                var items = drg.ItemDataSource as RowViewModel;
                items.CalulateRecordColumns();
            }
        }


        private static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                var correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }

    }
}
