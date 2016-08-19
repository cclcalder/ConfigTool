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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class DropdownFilterControl : UserControl
    {
        public DropdownFilterControl()
        {
            InitializeComponent();
        }

        #region SelectedOption

        public static readonly DependencyProperty SelectedOptionProperty = DependencyProperty.Register(
            "SelectedOption", typeof (string), typeof (DropdownFilterControl), new PropertyMetadata(default(string)));

        public string SelectedOption
        {
            get { return (string) GetValue(SelectedOptionProperty); }
            set { SetValue(SelectedOptionProperty, value); }
        }

        #endregion

        #region ChangeSelection event

        public static readonly RoutedEvent ChangedSelectionEvent = EventManager.RegisterRoutedEvent(
            "ChangedSelection", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (DropdownFilterControl));

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ChangedSelectionEvent));
        }

        #endregion
    }
}
