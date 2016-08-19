namespace WPF.ViewModels
{
    using System.Collections.Generic;
    using Coder.WPF.UI;
    using Model.DataAccess;

    public class PromotionWizardViewModel : PromotionWizardViewModelBase
    {
        public PromotionWizardViewModel(ISearchableTreeViewNodeEventsConsumer eventsConsumer, string promotionId)
            : base(eventsConsumer, promotionId, new PromotionAccess())
        {
        }

        public PromotionWizardViewModel() : base(new PromotionAccess())
        {
        }

        protected override IEnumerable<WizardPageViewModel> ConstructPageList()
        {
            return new List<WizardPageViewModel>
                {
                    //Dashboard,
                    CustomerPage,
                    DatesPage,
                    ProductsPage,
                    AttributesPage,
                    VolumesPage,
                    FinancialsPage,
                    PAndLReviewPage
                };
        }

        public override bool IsSubCustomerActive
        {
            get { return App.Configuration.IsPromotionsSubCustomerActive; }
        }
    }
}