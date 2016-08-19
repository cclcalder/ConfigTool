using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Exceedra.Common.Utilities;
using Model;
using Model.Annotations;
using WPF.ViewModels.EPOS;

namespace WPF.Pages.EPOS
{
    /// <summary>
    /// Interaction logic for ProductMatching.xaml
    /// </summary>
    public partial class Home : INotifyPropertyChanged
    {
        private GridLength _formerMenuInsightsWidth;
        // public ProductMatchingViewModel ViewModel { get; set; }
        public Home()
        {
            InitializeComponent();
            DataContext = new ProductMatchingViewModel();
            PropertyChanged.Raise(this, "ViewModel");

            Dispatcher.BeginInvoke(new Action(() =>
            {
                var root = VisualTreeHelper.GetChild(this.radTabControl, 0) as FrameworkElement;
                var headerElement = root.FindName("HeaderDockedElement") as UIElement;
                headerElement.Visibility = System.Windows.Visibility.Collapsed;
            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void insightsMenuBtnResize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MenuInsightsColumn.Width == new GridLength(0))
            {
                MenuInsightsColumn.Width = _formerMenuInsightsWidth;
                var uri = new Uri(App.SiteData.BaseURL + "/Images/caret/left.gif");
                var bitmap = new BitmapImage(uri);
                imgMenuInsightsResize.Source = bitmap;
            }
            else
            {
                _formerMenuInsightsWidth = MenuInsightsColumn.Width;
                MenuInsightsColumn.Width = new GridLength(0);
                var uri = new Uri(App.SiteData.BaseURL + "/Images/caret/right.gif");
                var bitmap = new BitmapImage(uri);
                imgMenuInsightsResize.Source = bitmap;
            }
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            radTabControl.SelectedIndex = 0;
            button1.Background = Brushes.LightGray;
            button2.Background = Brushes.White;
            button3.Background = Brushes.White;
        }

        private void Button3_OnClick(object sender, RoutedEventArgs e)
        {
            radTabControl.SelectedIndex = 1;
            button1.Background = Brushes.White;
            button3.Background = Brushes.LightGray;
            button2.Background = Brushes.White;
        }

        private void Button2_OnClick(object sender, RoutedEventArgs e)
        {
            radTabControl.SelectedIndex = 2;
            button1.Background = Brushes.White;
            button3.Background = Brushes.White;
            button2.Background = Brushes.LightGray;
        }
    }
}
