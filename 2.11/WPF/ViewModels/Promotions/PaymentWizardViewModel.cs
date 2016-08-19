namespace WPF.ViewModels
{
    using System.Collections.Generic;
    using Coder.WPF.UI;
    using Model.DataAccess;

    public class PaymentWizardViewModel : PromotionWizardViewModelBase
    {
        public PaymentWizardViewModel() : base(new PromotionAccess())
        {
        }

        public PaymentWizardViewModel(ISearchableTreeViewNodeEventsConsumer eventsConsumer, string promotionId)
            : base(eventsConsumer, promotionId, new PromotionAccess())
        {
        }

        protected override IEnumerable<WizardPageViewModel> ConstructPageList()
        {
            return new List<WizardPageViewModel>
                {
                    CustomerPage,
                    DatesPage,
                    ProductsPage,
                    FinancialsPage,
                    PAndLReviewPage
                };
        }

        public override bool IsSubCustomerActive
        {
            get { return App.Configuration.IsPaymentsSubCustomerActive; }
        }
    }
}