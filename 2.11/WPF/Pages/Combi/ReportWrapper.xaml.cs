using System.Windows.Controls;


namespace WPF.Pages.Combi
{
    /// <summary>
    /// Interaction logic for ReportWrapper.xaml
    /// </summary>
    public partial class ReportWrapper : Page
    {
        public ReportWrapper()
        {
            InitializeComponent();

            Insights.Navigate(new Insights());
            Analytics.Navigate(new AnalyticsV2());
            Canvas.Navigate(new  Canvas.Canvas());
        }
    }
}
