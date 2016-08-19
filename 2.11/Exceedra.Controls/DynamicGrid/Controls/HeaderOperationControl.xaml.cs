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

namespace Exceedra.Controls.DynamicGrid.Controls
{
    /// <summary>
    /// Interaction logic for HeaderOperationControl.xaml
    /// </summary>
    public partial class HeaderOperationControl : UserControl
    {
        public HeaderOperationControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TextProperty =
       DependencyProperty.Register("Text", typeof(string),
                                      typeof(HeaderOperationControl),
                                      null
                                      );


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set
            {
                SetValue(TextProperty, value);
            }
        }
        
        public static readonly DependencyProperty TagDataProperty =
            DependencyProperty.Register("TagData", typeof(string),
                                    typeof(HeaderOperationControl),
                                    null
                                    );


        public string TagData
        {
            get { return (string)GetValue(TagDataProperty); }
            set
            {
                SetValue(TagDataProperty, value);
            }
        }

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HeaderOperationControl));

        // Provide CLR accessors for the event 
        public event RoutedEventHandler Click;
        //{
        //    add { AddHandler(ClickEvent, value); }
        //    remove { RemoveHandler(ClickEvent, value); }
        //}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ClickEvent));
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            Text = input.Text;

        }

  



    }
}
