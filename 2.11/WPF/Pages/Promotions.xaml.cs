namespace WPF
{
    using ViewModels;

    /// <summary>
    /// Interaction logic for Promotions.xamlStatusList
    /// </summary>
    public partial class Promotions
    {
        public Promotions()
        {
            InitializeComponent();

            DataContext = PromotionMainViewModel.New();
        }

        public Promotions(bool reload)
        {
            InitializeComponent();

            DataContext = reload ? PromotionMainViewModel.New(true) : PromotionMainViewModel.New();
        }
    }
}