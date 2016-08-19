namespace WPF.ViewModels.PromoTemplates
{
    using System.Collections.Generic;
    using Coder.WPF.UI;
    using Model.DataAccess;
    using WPF.ViewModels.PromoTemplates;
 

    public class PromotionTemplateViewModel : PromotionTemplateViewModelBase
    {
        public PromotionTemplateViewModel(ISearchableTreeViewNodeEventsConsumer eventsConsumer, string promotionId)
            : base(eventsConsumer, promotionId, new PromotionTemplateAccess())
        {
        }

        public PromotionTemplateViewModel()
            : base(new PromotionTemplateAccess())
        {
        }

        protected override IEnumerable<TemplatePageViewModel> ConstructPageList()
        {
            return new List<TemplatePageViewModel>
                {
                    //Dashboard,
                    CustomerPage,
                    DatesPage,
                    ProductsPage,
                    AttributesPage,
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