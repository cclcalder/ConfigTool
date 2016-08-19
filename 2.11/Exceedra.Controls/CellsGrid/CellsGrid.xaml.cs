using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Exceedra.CellsGrid
{
    /// <summary>
    /// Interaction logic for CellsGrid.xaml
    /// </summary>
    public partial class CellsGrid
    {
        #region ctor

        public CellsGrid()
        {
            InitializeComponent();
        }

        #endregion

        #region view model

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(CellsGridViewModel), typeof(CellsGrid), new PropertyMetadata());

        public CellsGridViewModel ViewModel
        {
            get { return (CellsGridViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        private void NavigationLinkHandler(object sender, RoutedEventArgs e)
        {
            var obj = ((FrameworkElement)sender).DataContext as NavigationViewModel;

            var path = obj.What;

            var extData = obj.Item_Idx;

            bool PopOutNavigation = (Keyboard.Modifiers & ModifierKeys.Control) > 0;

            var parentPage = VisualTreeHelper.GetParent(this);
            while (!(parentPage is Page))
            {
                parentPage = VisualTreeHelper.GetParent(parentPage);
            }

            //Get the linkHandler method used by the parent ROB List page
            var method = parentPage.GetType().GetMethod("NavigationlinkClicked");
            object[] param = { path, extData, PopOutNavigation };
            if (method != null)
                method.Invoke(parentPage, param);

        }

        #endregion

        #region NoDataMessage

        public static readonly DependencyProperty NoDataMessageProperty = DependencyProperty.Register(
            "NoDataMessage", typeof(string), typeof(CellsGrid), new PropertyMetadata(default(string)));

        public string NoDataMessage
        {
            get { return (string)GetValue(NoDataMessageProperty); }
            set { SetValue(NoDataMessageProperty, value); }
        }

        #endregion
    }
}
