namespace WPF.Pages
{
    using System.Windows.Controls;
    using ViewModels;

    /// <summary>
    /// Interaction logic for PromotionView.xaml
    /// </summary>
    public partial class PromotionView : Page
    {
        # region Constructors

        public PromotionView() : this(null)
        {
        }

        public PromotionView(PromotionDataViewModel promotion)
        {
            InitializeComponent();
            DataContext = promotion;
        }

        # endregion

        #region Properties

        # endregion
    }
}