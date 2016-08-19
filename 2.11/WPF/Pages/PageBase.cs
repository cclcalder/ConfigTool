using System.Windows;
using System.Windows.Controls;

namespace WPF.Pages
{
    public class PageBase : Page
    {
        public PageBase()
        {
            Unloaded += Page_Unloaded;
        }

        public void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if(DataContext!=null)
                DataContext = null;            
        }
    }
} 
