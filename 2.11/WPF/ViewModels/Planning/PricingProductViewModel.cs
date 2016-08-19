using Exceedra.Common.Utilities;

namespace ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Coder.WPF.UI;
    using Model;

    public class PricingProductViewModel : SearchableNode //ViewModelBase
    {
        private readonly List<PricingProductViewModel> _children;
        private readonly PricingProduct _product;


        public PricingProductViewModel(ISearchableTreeViewNodeEventsConsumer eventsConsumer,
                                       PricingProductViewModel parent, PricingProduct product)
            : base(eventsConsumer, false, parent)
        {
            _product = product;

            _children = new List<PricingProductViewModel>((from child in _product.Children
                                                           orderby child.DisplayName
                                                           select
                                                               new PricingProductViewModel(eventsConsumer, this, child))
                                                              .ToList<PricingProductViewModel>());
            IsSelected = product.IsSelected;
        }

        public override IEnumerable<SearchableNode> Children
        {
            get { return _children; }
        }

        public string Id
        {
            get { return _product.ID; }
        }

        public override string Title
        {
            get { return _product.DisplayName; }
            set { throw new NotImplementedException(); }
        }

        protected override void OnSelectedChanged()
        {
            _product.IsSelected = IsSelected;
            IsSelectedChanged.Raise(this);
        }

        public static event EventHandler IsSelectedChanged;
    }
}