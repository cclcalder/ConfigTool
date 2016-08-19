namespace WPF.Wizard
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Model;

    /// <summary>
    /// Interaction logic for WizardLeftMenu.xaml
    /// </summary>
    public partial class WizardLeftMenu : UserControl
    {
        public WizardLeftMenu()
        {
            InitializeComponent();
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstComments.SelectedItem != null)
                MessageBox.Show((lstComments.SelectedItem as PromotionComment).Value, "Comment details");
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}