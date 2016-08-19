using System; 
using System.Windows;
 

namespace Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //this.Content = new Login();

            try
            {
                wb.Source = new Uri(App2.xbap);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message, "Error");
            }
           
        
        }
    }
}
